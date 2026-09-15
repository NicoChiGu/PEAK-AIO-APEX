using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000A9 RID: 169
public class GiantTreeAchievementZone : MonoBehaviour
{
	// Token: 0x06000659 RID: 1625 RVA: 0x00024319 File Offset: 0x00022519
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Character") && other.GetComponentInParent<Character>().IsLocal)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.ArboristBadge);
		}
	}
}
