using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000104 RID: 260
[RequireComponent(typeof(PhotonView))]
public class Breakable : MonoBehaviour
{
	// Token: 0x0600088E RID: 2190 RVA: 0x0002E9F9 File Offset: 0x0002CBF9
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
		this.rig = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0002EA14 File Offset: 0x0002CC14
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

	// Token: 0x06000890 RID: 2192 RVA: 0x0002EA78 File Offset: 0x0002CC78
	private void FixedUpdate()
	{
		if (this.rig == null || this.rig.isKinematic)
		{
			return;
		}
		this.lastVelocity = this.rig.linearVelocity;
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x0002EAA8 File Offset: 0x0002CCA8
	public virtual void Break(Collision coll)
	{
		if (this.alreadyBroke)
		{
			return;
		}
		this.alreadyBroke = true;
		for (int i = 0; i < this.breakSFX.Count; i++)
		{
			this.breakSFX[i].Play(base.transform.position);
		}
		if (this.ragdollCharacterOnBreak)
		{
			Character componentInParent = coll.transform.GetComponentInParent<Character>();
			if (componentInParent)
			{
				Rigidbody componentInParent2 = coll.transform.GetComponentInParent<Rigidbody>();
				Vector3 vector = this.lastVelocity.normalized * this.pushForce;
				componentInParent.AddForceToBodyPart(componentInParent2, vector * this.pushForce, vector * this.wholeBodyPushForce);
				componentInParent.Fall(2f, 15f);
			}
		}
		if (this.alternateChance > 0f && Random.value < this.alternateChance)
		{
			this.instantiateOnBreak = this.alternateInstantiateOnBreak;
			this.spawnsItemsKinematic = false;
		}
		for (int j = 0; j < this.instantiateOnBreak.Count; j++)
		{
			Item component = PhotonNetwork.Instantiate("0_Items/" + this.instantiateOnBreak[j].name, this.instantiatePoints[j].position, this.instantiatePoints[j].rotation, 0, null).GetComponent<Item>();
			if (component)
			{
				IntItemData intItemData;
				if (this.item.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
				{
					component.photonView.RPC("SetCookedAmountRPC", 0, new object[] { intItemData.Value });
				}
				if (this.spawnsItemsKinematic)
				{
					component.rig.isKinematic = true;
					component.transform.position = coll.contacts[0].point;
					component.transform.up = coll.contacts[0].normal;
				}
				else
				{
					component.rig.linearVelocity = this.item.rig.linearVelocity;
					component.rig.angularVelocity = this.item.rig.angularVelocity;
				}
				if (this.playAnimationOnInstantiatedObject)
				{
					Animator componentInChildren = component.GetComponentInChildren<Animator>();
					if (componentInChildren)
					{
						componentInChildren.Play(this.animString, 0, 0f);
					}
				}
			}
		}
		if (this.instantiateNonItemOnBreak.Count > 0)
		{
			this.item.photonView.RPC("RPC_NonItemBreak", 0, Array.Empty<object>());
		}
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x0002ED30 File Offset: 0x0002CF30
	[PunRPC]
	private void RPC_NonItemBreak()
	{
		for (int i = 0; i < this.instantiateNonItemOnBreak.Count; i++)
		{
			Rigidbody component = Object.Instantiate<GameObject>(this.instantiateNonItemOnBreak[i], base.transform.position, base.transform.rotation).GetComponent<Rigidbody>();
			if (component)
			{
				component.linearVelocity = this.item.rig.linearVelocity;
				component.angularVelocity = this.item.rig.angularVelocity;
			}
		}
	}

	// Token: 0x04000830 RID: 2096
	private Item item;

	// Token: 0x04000831 RID: 2097
	public bool breakOnCollision;

	// Token: 0x04000832 RID: 2098
	public float minBreakVelocity;

	// Token: 0x04000833 RID: 2099
	public List<GameObject> instantiateOnBreak;

	// Token: 0x04000834 RID: 2100
	public float alternateChance;

	// Token: 0x04000835 RID: 2101
	public List<GameObject> alternateInstantiateOnBreak;

	// Token: 0x04000836 RID: 2102
	public List<SFX_Instance> breakSFX;

	// Token: 0x04000837 RID: 2103
	public List<GameObject> instantiateNonItemOnBreak;

	// Token: 0x04000838 RID: 2104
	public List<Transform> instantiatePoints;

	// Token: 0x04000839 RID: 2105
	public bool spawnsItemsKinematic;

	// Token: 0x0400083A RID: 2106
	public bool playAnimationOnInstantiatedObject;

	// Token: 0x0400083B RID: 2107
	public string animString;

	// Token: 0x0400083C RID: 2108
	public bool ragdollCharacterOnBreak;

	// Token: 0x0400083D RID: 2109
	private Rigidbody rig;

	// Token: 0x0400083E RID: 2110
	private bool alreadyBroke;

	// Token: 0x0400083F RID: 2111
	private Vector3 lastVelocity;

	// Token: 0x04000840 RID: 2112
	public float pushForce = 2f;

	// Token: 0x04000841 RID: 2113
	public float wholeBodyPushForce = 1f;
}
