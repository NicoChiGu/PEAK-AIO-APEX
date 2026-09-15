using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000A6 RID: 166
public class Capybara : MonoBehaviour
{
	// Token: 0x06000646 RID: 1606 RVA: 0x00023FAF File Offset: 0x000221AF
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.serenadeDistance);
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00023FD1 File Offset: 0x000221D1
	private void OnEnable()
	{
		GlobalEvents.OnBugleTooted = (Action<Item>)Delegate.Combine(GlobalEvents.OnBugleTooted, new Action<Item>(this.TestBugleTooted));
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00023FF3 File Offset: 0x000221F3
	private void OnDisable()
	{
		GlobalEvents.OnBugleTooted = (Action<Item>)Delegate.Remove(GlobalEvents.OnBugleTooted, new Action<Item>(this.TestBugleTooted));
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00024018 File Offset: 0x00022218
	private void TestBugleTooted(Item bugle)
	{
		if (Vector3.Distance(base.transform.position, bugle.transform.position) < this.serenadeDistance && bugle.holderCharacter && bugle.holderCharacter.IsLocal)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AnimalSerenadingBadge);
		}
	}

	// Token: 0x04000653 RID: 1619
	public float serenadeDistance;
}
