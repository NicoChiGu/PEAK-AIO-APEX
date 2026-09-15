using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001F7 RID: 503
public class Action_BookOfBonesAnim : MonoBehaviour
{
	// Token: 0x06000F2C RID: 3884 RVA: 0x0004A3AC File Offset: 0x000485AC
	private void Update()
	{
		if (this.item.holderCharacter && !this.item.holderCharacter.IsLocal)
		{
			return;
		}
		if (this.usingOnSkeleton)
		{
			return;
		}
		if ((this.item.isUsingPrimary || this.item.isUsingSecondary) && !this.usingLocal)
		{
			this.Open(false);
			return;
		}
		if (!this.item.isUsingPrimary && !this.item.isUsingSecondary && this.usingLocal)
		{
			this.Close();
		}
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x0004A439 File Offset: 0x00048639
	public void Open(bool usingOnSkeleton)
	{
		this.usingOnSkeleton = usingOnSkeleton;
		this.item.photonView.RPC("BookOfBonesAnimRPC", 0, new object[] { true });
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x0004A467 File Offset: 0x00048667
	public void Close()
	{
		this.usingOnSkeleton = false;
		this.item.photonView.RPC("BookOfBonesAnimRPC", 0, new object[] { false });
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x0004A495 File Offset: 0x00048695
	[PunRPC]
	private void BookOfBonesAnimRPC(bool isUsing)
	{
		this.anim.SetBool("Using", isUsing);
		this.usingLocal = isUsing;
	}

	// Token: 0x04000D35 RID: 3381
	public Animator anim;

	// Token: 0x04000D36 RID: 3382
	public Item item;

	// Token: 0x04000D37 RID: 3383
	private bool usingLocal;

	// Token: 0x04000D38 RID: 3384
	private bool usingOnSkeleton;
}
