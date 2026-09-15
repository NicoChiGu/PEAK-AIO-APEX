using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000330 RID: 816
[RequireComponent(typeof(PhotonView))]
public class ShelfShroom : MonoBehaviour
{
	// Token: 0x0600150E RID: 5390 RVA: 0x0006B798 File Offset: 0x00069998
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x0006B7A8 File Offset: 0x000699A8
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.item.photonView.IsMine)
		{
			return;
		}
		if (this.item.itemState == ItemState.Ground && this.breakOnCollision && this.item.rig && collision.relativeVelocity.magnitude > this.minBreakVelocity)
		{
			this.Break(collision);
		}
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x0006B80C File Offset: 0x00069A0C
	public void Break(Collision coll)
	{
		if (this.alreadyBroke)
		{
			return;
		}
		this.alreadyBroke = true;
		string text = "0_Items/" + this.instantiateOnBreak.name;
		Quaternion quaternion = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
		if (this.stickToNormal)
		{
			quaternion = Quaternion.LookRotation(Vector3.forward, coll.contacts[0].normal);
		}
		PhotonNetwork.Instantiate(text, coll.contacts[0].point, quaternion, 0, null);
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x04001393 RID: 5011
	private Item item;

	// Token: 0x04001394 RID: 5012
	public bool breakOnCollision;

	// Token: 0x04001395 RID: 5013
	public float minBreakVelocity;

	// Token: 0x04001396 RID: 5014
	public GameObject instantiateOnBreak;

	// Token: 0x04001397 RID: 5015
	public Transform instantiatePoint;

	// Token: 0x04001398 RID: 5016
	public bool stickToNormal;

	// Token: 0x04001399 RID: 5017
	private bool alreadyBroke;
}
