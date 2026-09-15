using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000234 RID: 564
[RequireComponent(typeof(PhotonView))]
public class CloudFungus : MonoBehaviour
{
	// Token: 0x060010BC RID: 4284 RVA: 0x000542F5 File Offset: 0x000524F5
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x00054304 File Offset: 0x00052504
	private void Update()
	{
		if (this.item.itemState != ItemState.Ground || !this.item.photonView.IsMine)
		{
			base.enabled = false;
			return;
		}
		if (this.item.itemState == ItemState.Ground && this.item.rig.linearVelocity.y < this.yVelocityNeeded && this.timeAlive > this.timeAliveNeeded)
		{
			this.Break();
		}
		this.timeAlive += Time.deltaTime;
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x00054388 File Offset: 0x00052588
	public void Break()
	{
		if (this.alreadyBroke)
		{
			return;
		}
		this.alreadyBroke = true;
		string text = "0_Items/" + this.instantiateOnBreak.name;
		Quaternion quaternion = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
		PhotonNetwork.Instantiate(text, base.transform.position, quaternion, 0, null);
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x04000EF2 RID: 3826
	private Item item;

	// Token: 0x04000EF3 RID: 3827
	public GameObject instantiateOnBreak;

	// Token: 0x04000EF4 RID: 3828
	public float timeAliveNeeded = 0.5f;

	// Token: 0x04000EF5 RID: 3829
	public float yVelocityNeeded = -1f;

	// Token: 0x04000EF6 RID: 3830
	private float timeAlive;

	// Token: 0x04000EF7 RID: 3831
	private bool alreadyBroke;
}
