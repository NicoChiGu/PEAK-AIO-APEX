using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000103 RID: 259
[RequireComponent(typeof(PhotonView))]
public class Bonkable : MonoBehaviour
{
	// Token: 0x06000886 RID: 2182 RVA: 0x0002E760 File Offset: 0x0002C960
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x0002E76E File Offset: 0x0002C96E
	private void OnEnable()
	{
		GlobalEvents.OnItemThrown = (Action<Item>)Delegate.Combine(GlobalEvents.OnItemThrown, new Action<Item>(this.TestItemThrown));
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x0002E790 File Offset: 0x0002C990
	private void OnDisable()
	{
		GlobalEvents.OnItemThrown = (Action<Item>)Delegate.Remove(GlobalEvents.OnItemThrown, new Action<Item>(this.TestItemThrown));
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0002E7B2 File Offset: 0x0002C9B2
	private void TestItemThrown(Item thrownItem)
	{
		if (thrownItem == this.item)
		{
			this.thrown = true;
		}
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x0002E7CC File Offset: 0x0002C9CC
	public bool VelocityAboveThreshold(Collision coll)
	{
		float magnitude = coll.relativeVelocity.magnitude;
		if (this.thrown && this.decreaseMinVelocityIfRecentlyThrown)
		{
			return magnitude >= this.minBonkVelocityThrown;
		}
		return magnitude >= this.minBonkVelocity;
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x0002E814 File Offset: 0x0002CA14
	private void OnCollisionEnter(Collision coll)
	{
		if (!this.item.photonView.IsMine)
		{
			return;
		}
		if ((HelperFunctions.terrainMapMask & (1 << coll.transform.gameObject.layer)) != 0)
		{
			this.thrown = false;
		}
		if (this.item.itemState == ItemState.Ground && this.item.rig && this.VelocityAboveThreshold(coll))
		{
			this.Bonk(coll);
		}
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x0002E88C File Offset: 0x0002CA8C
	private void Bonk(Collision coll)
	{
		Character componentInParent = coll.gameObject.GetComponentInParent<Character>();
		if (componentInParent && Time.time > this.lastBonkedTime + this.bonkCooldown)
		{
			componentInParent.Fall(this.ragdollTime, 0f);
			for (int i = 0; i < this.bonk.Length; i++)
			{
				this.bonk[i].Play(base.transform.position);
			}
			this.lastBonkedTime = Time.time;
			if (this.bonkMassMultiplier)
			{
				componentInParent.AddForceAtPosition(-coll.relativeVelocity.normalized * this.bonkForce * this.item.GetComponent<ItemScaleSyncer>().currentScale, coll.contacts[0].point, this.bonkRange);
				return;
			}
			componentInParent.AddForceAtPosition(-coll.relativeVelocity.normalized * this.bonkForce, coll.contacts[0].point, this.bonkRange);
		}
	}

	// Token: 0x04000824 RID: 2084
	private Item item;

	// Token: 0x04000825 RID: 2085
	public float minBonkVelocity = 5f;

	// Token: 0x04000826 RID: 2086
	public bool decreaseMinVelocityIfRecentlyThrown;

	// Token: 0x04000827 RID: 2087
	public float minBonkVelocityThrown = 5f;

	// Token: 0x04000828 RID: 2088
	public float ragdollTime = 1f;

	// Token: 0x04000829 RID: 2089
	public float bonkForce = 1000f;

	// Token: 0x0400082A RID: 2090
	public float bonkRange = 3f;

	// Token: 0x0400082B RID: 2091
	public SFX_Instance[] bonk;

	// Token: 0x0400082C RID: 2092
	public float lastBonkedTime;

	// Token: 0x0400082D RID: 2093
	private float bonkCooldown = 1f;

	// Token: 0x0400082E RID: 2094
	private bool thrown;

	// Token: 0x0400082F RID: 2095
	public bool bonkMassMultiplier;
}
