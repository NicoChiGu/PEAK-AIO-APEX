using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class ScoutCannon : MonoBehaviour
{
	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000C0D RID: 3085 RVA: 0x0004055D File Offset: 0x0003E75D
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x00040560 File Offset: 0x0003E760
	private void FixedUpdate()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		float num = 1f;
		float num2 = num;
		Character character = null;
		this.characters.Clear();
		this.characters.AddRange(Character.AllCharacters);
		this.characters.AddRange(Character.AllBotCharacters);
		foreach (Character character2 in this.characters)
		{
			float num3 = Vector3.Distance(character2.Center, this.entry.position);
			if (num3 < num2)
			{
				num2 = num3;
				character = character2;
			}
			if (num3 < num)
			{
				if (character2 == this.target)
				{
					if (character2.data.sinceJump < 0.5f)
					{
						continue;
					}
					List<Bodypart> partList = character2.refs.ragdoll.partList;
					if (character2.isBot && character2.data.fallSeconds == 0f)
					{
						character2.Fall(2f, 0f);
					}
					using (List<Bodypart>.Enumerator enumerator2 = partList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Bodypart bodypart = enumerator2.Current;
							BodypartType partType = bodypart.partType;
							if (partType != BodypartType.Hand_L && partType != BodypartType.Hand_R && partType != BodypartType.Elbow_L && partType != BodypartType.Elbow_R && partType != BodypartType.Arm_L && partType != BodypartType.Arm_R)
							{
								Vector3 vector = bodypart.transform.position - this.tube.position;
								vector = Vector3.Project(vector, this.tube.forward);
								Vector3 vector2 = this.tube.position + vector - bodypart.transform.position;
								vector2 = Vector3.ClampMagnitude(vector2, 1f);
								bodypart.Rig.AddForce(vector2 * this.pullForce, 5);
								if (this.entry.InverseTransformPoint(bodypart.transform.position).z < 0f && HelperFunctions.LineCheck(this.entry.position, bodypart.transform.position, HelperFunctions.LayerType.Map, 0f, 1).transform)
								{
									vector2 = this.entry.forward;
									bodypart.Rig.AddForce(vector2 * this.pullForce, 5);
								}
							}
						}
						continue;
					}
				}
				if (HelperFunctions.LineCheck(this.tube.position, character2.Center, HelperFunctions.LayerType.Map, 0f, 1).transform == null)
				{
					Vector3 vector3 = this.tube.position - character2.Center;
					vector3.Normalize();
					character2.AddForce(-vector3 * this.pushForce, 1f, 1f);
				}
			}
		}
		if (this.target != character)
		{
			if (character == null)
			{
				this.view.RPC("RPCA_SetTarget", 0, new object[] { -1 });
				return;
			}
			this.view.RPC("RPCA_SetTarget", 0, new object[] { character.refs.view.ViewID });
		}
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x000408EC File Offset: 0x0003EAEC
	[PunRPC]
	private void RPCA_SetTarget(int setTargetID)
	{
		this.targetID = setTargetID;
		if (this.targetID == -1)
		{
			this.target = null;
			return;
		}
		this.target = PhotonNetwork.GetPhotonView(this.targetID).GetComponent<Character>();
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x0004091C File Offset: 0x0003EB1C
	private void Awake()
	{
		this.tube = base.transform.Find("Cannon");
		this.entry = this.tube.Find("Entry");
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06000C11 RID: 3089 RVA: 0x00040956 File Offset: 0x0003EB56
	// (set) Token: 0x06000C12 RID: 3090 RVA: 0x00040977 File Offset: 0x0003EB77
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = this.tube.GetComponentsInChildren<MeshRenderer>();
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x00040980 File Offset: 0x0003EB80
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x00040982 File Offset: 0x0003EB82
	public void Light()
	{
		this.view.RPC("RPCA_Light", 0, Array.Empty<object>());
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x0004099A File Offset: 0x0003EB9A
	[PunRPC]
	public void RPCA_Light()
	{
		base.StartCoroutine(this.<RPCA_Light>g__LightRoutine|30_0());
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x000409A9 File Offset: 0x0003EBA9
	private void FireTargets()
	{
		this.LaunchPlayers();
		this.LaunchItems();
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x000409B8 File Offset: 0x0003EBB8
	private void LaunchPlayers()
	{
		List<Character> list = new List<Character>();
		if (this.target)
		{
			list.Add(this.target);
		}
		foreach (Character character in Character.AllCharacters)
		{
			if (Vector3.Distance(character.Center, this.entry.position) <= 0.75f && !(character == this.target))
			{
				list.Add(character);
			}
		}
		foreach (Character character2 in Character.AllBotCharacters)
		{
			if (Vector3.Distance(character2.Center, this.entry.position) <= 0.75f && !(character2 == this.target))
			{
				list.Add(character2);
			}
		}
		foreach (Character character3 in list)
		{
			this.view.RPC("RPCA_LaunchTarget", 0, new object[] { character3.refs.view.ViewID });
		}
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00040B28 File Offset: 0x0003ED28
	private void LaunchItems()
	{
		Collider[] array = Physics.OverlapSphere(this.tube.position, 1f, HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysical));
		List<Item> list = new List<Item>();
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Item componentInParent = array2[i].GetComponentInParent<Item>();
			if (componentInParent && componentInParent.itemState == ItemState.Ground && !HelperFunctions.LineCheck(this.tube.position, componentInParent.Center(), HelperFunctions.LayerType.Map, 0f, 1).transform && !list.Contains(componentInParent))
			{
				list.Add(componentInParent);
			}
		}
		foreach (Item item in list)
		{
			this.view.RPC("RPCA_LaunchItem", 0, new object[] { item.photonView.ViewID });
		}
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x00040C28 File Offset: 0x0003EE28
	[PunRPC]
	public void RPCA_LaunchItem(int targetID)
	{
		PhotonView photonView = PhotonNetwork.GetPhotonView(targetID);
		if (photonView == null)
		{
			return;
		}
		Item component = photonView.GetComponent<Item>();
		if (component == null)
		{
			return;
		}
		if (component is Backpack)
		{
			component.rig.AddForce(this.tube.forward * this.backpackLaunchForce, 2);
			return;
		}
		component.rig.AddForce(this.tube.forward * this.itemLaunchForce, 2);
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x00040CA4 File Offset: 0x0003EEA4
	[PunRPC]
	public void RPCA_LaunchTarget(int targetID)
	{
		ScoutCannon.<>c__DisplayClass35_0 CS$<>8__locals1 = new ScoutCannon.<>c__DisplayClass35_0();
		CS$<>8__locals1.<>4__this = this;
		PhotonView photonView = PhotonNetwork.GetPhotonView(targetID);
		if (photonView == null)
		{
			return;
		}
		CS$<>8__locals1.t = photonView.GetComponent<Character>();
		if (CS$<>8__locals1.t == null)
		{
			return;
		}
		CS$<>8__locals1.t.data.launchedByCannon = true;
		CS$<>8__locals1.t.RPCA_Fall(this.fallFor);
		CS$<>8__locals1.t.AddForce(this.tube.forward * this.launchForce, 1f, 1f);
		base.StartCoroutine(CS$<>8__locals1.<RPCA_LaunchTarget>g__ILaunch|0());
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x00040DB6 File Offset: 0x0003EFB6
	[CompilerGenerated]
	private IEnumerator <RPCA_Light>g__LightRoutine|30_0()
	{
		this.lit = true;
		this.litParticle.Play();
		this.anim.Play("Light");
		yield return new WaitForSeconds(this.fireTime);
		this.anim.Play("Fire");
		this.fireParticle.Play();
		this.fireSFX.SetActive(true);
		if (this.view.IsMine)
		{
			this.FireTargets();
		}
		yield return new WaitForSeconds(this.fallFor);
		this.lit = false;
		yield break;
	}

	// Token: 0x04000B2C RID: 2860
	public float launchForce = 500f;

	// Token: 0x04000B2D RID: 2861
	public float itemLaunchForce = 500f;

	// Token: 0x04000B2E RID: 2862
	public float backpackLaunchForce = 5000f;

	// Token: 0x04000B2F RID: 2863
	public float fallFor = 1f;

	// Token: 0x04000B30 RID: 2864
	public float pullForce = 10f;

	// Token: 0x04000B31 RID: 2865
	public float pushForce = 10f;

	// Token: 0x04000B32 RID: 2866
	public bool lit;

	// Token: 0x04000B33 RID: 2867
	public float fireTime = 3f;

	// Token: 0x04000B34 RID: 2868
	public ParticleSystem litParticle;

	// Token: 0x04000B35 RID: 2869
	public ParticleSystem fireParticle;

	// Token: 0x04000B36 RID: 2870
	public GameObject fireSFX;

	// Token: 0x04000B37 RID: 2871
	public Animator anim;

	// Token: 0x04000B38 RID: 2872
	private MaterialPropertyBlock mpb;

	// Token: 0x04000B39 RID: 2873
	private PhotonView view;

	// Token: 0x04000B3A RID: 2874
	private Transform tube;

	// Token: 0x04000B3B RID: 2875
	private Transform entry;

	// Token: 0x04000B3C RID: 2876
	private Character target;

	// Token: 0x04000B3D RID: 2877
	private int targetID = -1;

	// Token: 0x04000B3E RID: 2878
	private List<Character> characters = new List<Character>();

	// Token: 0x04000B3F RID: 2879
	private MeshRenderer[] _mr;
}
