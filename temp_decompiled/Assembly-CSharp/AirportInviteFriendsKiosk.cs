using System;
using Steamworks;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class AirportInviteFriendsKiosk : MonoBehaviour, IInteractible
{
	// Token: 0x06000435 RID: 1077 RVA: 0x0001A539 File Offset: 0x00018739
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000436 RID: 1078 RVA: 0x0001A53C File Offset: 0x0001873C
	// (set) Token: 0x06000437 RID: 1079 RVA: 0x0001A56A File Offset: 0x0001876A
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = base.GetComponentsInChildren<MeshRenderer>();
				MonoBehaviour.print(this._mr.Length);
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x0001A573 File Offset: 0x00018773
	public void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x0001A580 File Offset: 0x00018780
	public void Interact(Character interactor)
	{
		CSteamID csteamID;
		if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out csteamID))
		{
			Debug.Log("Open Invite Friends UI...");
			SteamFriends.ActivateGameOverlayInviteDialog(csteamID);
		}
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x0001A5AC File Offset: 0x000187AC
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

	// Token: 0x0600043B RID: 1083 RVA: 0x0001A60C File Offset: 0x0001880C
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].SetPropertyBlock(this.mpb);
			}
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x0001A65C File Offset: 0x0001885C
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x0001A669 File Offset: 0x00018869
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x0001A671 File Offset: 0x00018871
	public string GetInteractionText()
	{
		return LocalizedText.GetText("INVITEFRIENDS", true);
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x0001A67E File Offset: 0x0001887E
	public string GetName()
	{
		return LocalizedText.GetText("INVITEKIOSK", true);
	}

	// Token: 0x040004BB RID: 1211
	private MaterialPropertyBlock mpb;

	// Token: 0x040004BC RID: 1212
	private MeshRenderer[] _mr;
}
