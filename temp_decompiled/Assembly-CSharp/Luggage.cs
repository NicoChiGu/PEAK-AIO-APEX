using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class Luggage : Spawner, IInteractibleConstant, IInteractible
{
	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000922 RID: 2338 RVA: 0x00030A99 File Offset: 0x0002EC99
	// (set) Token: 0x06000923 RID: 2339 RVA: 0x00030AB5 File Offset: 0x0002ECB5
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = base.GetComponentsInChildren<MeshRenderer>();
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x00030ABE File Offset: 0x0002ECBE
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.anim = base.GetComponent<Animator>();
		this.mpb = new MaterialPropertyBlock();
		Luggage.ALL_LUGGAGE.Add(this);
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00030AEE File Offset: 0x0002ECEE
	public virtual void Interact(Character interactor)
	{
		this.anim.Play("Luggage_Unclasp");
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x00030B00 File Offset: 0x0002ED00
	[PunRPC]
	protected void OpenLuggageRPC(bool spawnItems)
	{
		if (this.state == Luggage.LuggageState.Closed)
		{
			this.anim.Play("Luggage_Open");
			Luggage.ALL_LUGGAGE.Remove(this);
			this.state = Luggage.LuggageState.Open;
			if (spawnItems)
			{
				base.StartCoroutine(this.<OpenLuggageRPC>g__SpawnItemRoutine|14_0());
			}
		}
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00030B3D File Offset: 0x0002ED3D
	private void OnDestroy()
	{
		if (Luggage.ALL_LUGGAGE.Contains(this))
		{
			Luggage.ALL_LUGGAGE.Remove(this);
		}
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x00030B58 File Offset: 0x0002ED58
	public Vector3 Center()
	{
		return HelperFunctions.GetTotalBounds(this.meshRenderers).center;
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00030B78 File Offset: 0x0002ED78
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x00030B80 File Offset: 0x0002ED80
	public virtual string GetInteractionText()
	{
		return LocalizedText.GetText("OPEN", true);
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x00030B8D File Offset: 0x0002ED8D
	public string GetName()
	{
		return LocalizedText.GetText(this.displayName, true);
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x00030B9B File Offset: 0x0002ED9B
	public bool IsInteractible(Character interactor)
	{
		return this.state == Luggage.LuggageState.Closed;
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x00030BA8 File Offset: 0x0002EDA8
	public void HoverEnter()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				if (this.meshRenderers[i] != null)
				{
					this.meshRenderers[i].SetPropertyBlock(this.mpb);
				}
			}
		}
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00030C08 File Offset: 0x0002EE08
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				if (this.meshRenderers[i] != null)
				{
					this.meshRenderers[i].SetPropertyBlock(this.mpb);
				}
			}
		}
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x00030C68 File Offset: 0x0002EE68
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x00030C6A File Offset: 0x0002EE6A
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.state == Luggage.LuggageState.Closed;
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00030C75 File Offset: 0x0002EE75
	public float GetInteractTime(Character interactor)
	{
		return this.timeToOpen;
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00030C80 File Offset: 0x0002EE80
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (newPlayer.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && PhotonNetwork.IsMasterClient && this.state == Luggage.LuggageState.Open)
		{
			this.photonView.RPC("OpenLuggageRPC", 0, new object[] { false });
		}
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00030CD6 File Offset: 0x0002EED6
	public virtual void Interact_CastFinished(Character interactor)
	{
		if (this.state == Luggage.LuggageState.Closed)
		{
			this.photonView.RPC("OpenLuggageRPC", 0, new object[] { true });
			GlobalEvents.TriggerLuggageOpened(this, interactor);
		}
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00030D07 File Offset: 0x0002EF07
	public void CancelCast(Character interactor)
	{
		this.anim.SetTrigger("Reclasp");
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000935 RID: 2357 RVA: 0x00030D19 File Offset: 0x0002EF19
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00030D1C File Offset: 0x0002EF1C
	protected override void OffsetSpawn(Item item)
	{
		if (item.offsetLuggageSpawn && !(this is RespawnChest))
		{
			item.transform.position += base.transform.rotation * item.offsetLuggagePosition;
			item.transform.rotation *= Quaternion.Euler(item.offsetLuggageRotation);
		}
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00030D9A File Offset: 0x0002EF9A
	[CompilerGenerated]
	private IEnumerator <OpenLuggageRPC>g__SpawnItemRoutine|14_0()
	{
		yield return new WaitForSeconds(0.1f);
		this.SpawnItems(this.GetSpawnSpots());
		yield break;
	}

	// Token: 0x0400088F RID: 2191
	public string displayName;

	// Token: 0x04000890 RID: 2192
	private Animator anim;

	// Token: 0x04000891 RID: 2193
	[SerializeField]
	protected Luggage.LuggageState state;

	// Token: 0x04000892 RID: 2194
	private PhotonView photonView;

	// Token: 0x04000893 RID: 2195
	public float timeToOpen;

	// Token: 0x04000894 RID: 2196
	private MaterialPropertyBlock mpb;

	// Token: 0x04000895 RID: 2197
	public static List<Luggage> ALL_LUGGAGE = new List<Luggage>();

	// Token: 0x04000896 RID: 2198
	private MeshRenderer[] _mr;

	// Token: 0x02000451 RID: 1105
	public enum LuggageState
	{
		// Token: 0x04001897 RID: 6295
		Closed,
		// Token: 0x04001898 RID: 6296
		Open
	}
}
