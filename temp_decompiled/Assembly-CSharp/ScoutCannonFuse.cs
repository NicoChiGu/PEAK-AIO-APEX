using System;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class ScoutCannonFuse : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x06000C23 RID: 3107 RVA: 0x00040EFC File Offset: 0x0003F0FC
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06000C24 RID: 3108 RVA: 0x00040EFF File Offset: 0x0003F0FF
	// (set) Token: 0x06000C25 RID: 3109 RVA: 0x00040F1B File Offset: 0x0003F11B
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

	// Token: 0x06000C26 RID: 3110 RVA: 0x00040F24 File Offset: 0x0003F124
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x00040F26 File Offset: 0x0003F126
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x00040F33 File Offset: 0x0003F133
	public string GetInteractionText()
	{
		return LocalizedText.GetText("LIGHT", true);
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x00040F40 File Offset: 0x0003F140
	public float GetInteractTime(Character interactor)
	{
		return 1f;
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x00040F47 File Offset: 0x0003F147
	public string GetName()
	{
		return LocalizedText.GetText("SCOUTCANNONFUSE", true);
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x00040F54 File Offset: 0x0003F154
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x00040F5C File Offset: 0x0003F15C
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

	// Token: 0x06000C2D RID: 3117 RVA: 0x00040FBC File Offset: 0x0003F1BC
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

	// Token: 0x06000C2E RID: 3118 RVA: 0x0004101C File Offset: 0x0003F21C
	public void Interact(Character interactor)
	{
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0004101E File Offset: 0x0003F21E
	public void Interact_CastFinished(Character interactor)
	{
		this.scoutCannon.Light();
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x0004102B File Offset: 0x0003F22B
	public bool IsConstantlyInteractable(Character interactor)
	{
		return !this.scoutCannon.lit;
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0004103B File Offset: 0x0003F23B
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x0004103E File Offset: 0x0003F23E
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x04000B42 RID: 2882
	public ScoutCannon scoutCannon;

	// Token: 0x04000B43 RID: 2883
	private MaterialPropertyBlock mpb;

	// Token: 0x04000B44 RID: 2884
	private MeshRenderer[] _mr;
}
