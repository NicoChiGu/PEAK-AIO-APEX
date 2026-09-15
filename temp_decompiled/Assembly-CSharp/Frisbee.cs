using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000B9 RID: 185
public class Frisbee : MonoBehaviour
{
	// Token: 0x060006C7 RID: 1735 RVA: 0x00026B20 File Offset: 0x00024D20
	private void OnEnable()
	{
		GlobalEvents.OnItemThrown = (Action<Item>)Delegate.Combine(GlobalEvents.OnItemThrown, new Action<Item>(this.OnItemThrown));
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00026B70 File Offset: 0x00024D70
	private void OnDisable()
	{
		GlobalEvents.OnItemThrown = (Action<Item>)Delegate.Remove(GlobalEvents.OnItemThrown, new Action<Item>(this.OnItemThrown));
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x00026BBD File Offset: 0x00024DBD
	private void OnItemThrown(Item obj)
	{
		if (obj == this.item)
		{
			this.startedThrowPosition = obj.Center();
			this.throwValidForAchievement = true;
		}
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00026BE0 File Offset: 0x00024DE0
	private void FixedUpdate()
	{
		if (this.item.holderCharacter == null)
		{
			float num = Mathf.InverseLerp(0f, this.velocityForLift, this.item.rig.linearVelocity.sqrMagnitude);
			Vector3 up = base.transform.up;
			float num2 = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
			this.item.rig.AddForce(up * num2 * this.liftForce * num);
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00026C78 File Offset: 0x00024E78
	private void OnCollisionEnter(Collision collision)
	{
		this.item.rig.linearVelocity *= 0.5f;
		if (this.throwValidForAchievement)
		{
			int layer = collision.gameObject.layer;
			if ((HelperFunctions.terrainMapMask & (1 << layer)) != 0)
			{
				this.throwValidForAchievement = false;
				return;
			}
			if ((LayerMask.GetMask(new string[] { "Water" }) & (1 << layer)) != 0)
			{
				this.throwValidForAchievement = false;
			}
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x060006CC RID: 1740 RVA: 0x00026CF7 File Offset: 0x00024EF7
	private float throwDistance
	{
		get
		{
			return Vector3.Distance(this.startedThrowPosition, this.item.Center()) * CharacterStats.unitsToMeters;
		}
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x00026D18 File Offset: 0x00024F18
	private void TestRequestedItem(Item requestedItem, Character character)
	{
		if (!this.throwValidForAchievement)
		{
			return;
		}
		if (character.IsLocal && requestedItem == this.item)
		{
			Debug.Log("Frisbee grabbed at distance " + this.throwDistance.ToString());
			if (this.throwDistance >= 100f)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.UltimateBadge);
			}
		}
	}

	// Token: 0x040006D8 RID: 1752
	public Item item;

	// Token: 0x040006D9 RID: 1753
	public float liftForce = 10f;

	// Token: 0x040006DA RID: 1754
	public float velocityForLift = 10f;

	// Token: 0x040006DB RID: 1755
	private Vector3 startedThrowPosition;

	// Token: 0x040006DC RID: 1756
	private bool throwValidForAchievement;
}
