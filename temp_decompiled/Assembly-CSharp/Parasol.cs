using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002B3 RID: 691
[RequireComponent(typeof(PhotonView))]
public class Parasol : MonoBehaviour
{
	// Token: 0x060012E4 RID: 4836 RVA: 0x000601A8 File Offset: 0x0005E3A8
	internal void ToggleOpen()
	{
		if (this.item.photonView.IsMine)
		{
			this.item.photonView.RPC("ToggleOpenRPC", 0, new object[] { !this.isOpen });
		}
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x000601F4 File Offset: 0x0005E3F4
	private void FixedUpdate()
	{
		if (this.item.holderCharacter && !this.item.holderCharacter.data.isGrounded && this.isOpen)
		{
			this.item.holderCharacter.refs.movement.ApplyParasolDrag(this.extraYDrag, this.extraXZDrag, false);
		}
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x00060259 File Offset: 0x0005E459
	private void OnDisable()
	{
		if (this.isOpen)
		{
			this.OnClose();
		}
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x00060269 File Offset: 0x0005E469
	private void OnClose()
	{
		if (this.item.holderCharacter)
		{
			this.item.holderCharacter.data.sinceGrounded = this.sinceGroundedOnClose;
		}
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x00060298 File Offset: 0x0005E498
	[PunRPC]
	private void ToggleOpenRPC(bool open)
	{
		this.isOpen = open;
		this.anim.SetBool("Open", open);
		if (!this.isOpen)
		{
			this.OnClose();
		}
	}

	// Token: 0x0400118B RID: 4491
	public Item item;

	// Token: 0x0400118C RID: 4492
	public float extraYDrag = 0.8f;

	// Token: 0x0400118D RID: 4493
	public float extraXZDrag = 0.8f;

	// Token: 0x0400118E RID: 4494
	public float sinceGroundedOnClose = 2f;

	// Token: 0x0400118F RID: 4495
	public GameObject openParasol;

	// Token: 0x04001190 RID: 4496
	public GameObject closedParasol;

	// Token: 0x04001191 RID: 4497
	public Animator anim;

	// Token: 0x04001192 RID: 4498
	public bool isOpen;
}
