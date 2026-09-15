using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000349 RID: 841
public class SyncedAnimation : MonoBehaviour
{
	// Token: 0x06001593 RID: 5523 RVA: 0x0006F303 File Offset: 0x0006D503
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.anim = base.GetComponent<Animator>();
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x0006F320 File Offset: 0x0006D520
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.syncCounter += Time.deltaTime;
			if (this.syncCounter > 5f)
			{
				this.view.RPC("RPCA_SyncAnim", 0, new object[] { this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f });
				this.syncCounter = 0f;
			}
		}
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x0006F398 File Offset: 0x0006D598
	[PunRPC]
	public void RPCA_SyncAnim(float syncTime)
	{
		this.anim.Play(this.anim.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, syncTime);
	}

	// Token: 0x04001469 RID: 5225
	private PhotonView view;

	// Token: 0x0400146A RID: 5226
	private Animator anim;

	// Token: 0x0400146B RID: 5227
	private float syncCounter;
}
