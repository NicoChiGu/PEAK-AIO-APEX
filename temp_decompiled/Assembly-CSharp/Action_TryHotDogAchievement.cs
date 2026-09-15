using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000FC RID: 252
public class Action_TryHotDogAchievement : ItemAction
{
	// Token: 0x06000867 RID: 2151 RVA: 0x0002E064 File Offset: 0x0002C264
	public override void RunAction()
	{
		Action_TryHotDogAchievement.hotDogEatTimes.Add(Time.time);
		while (Action_TryHotDogAchievement.hotDogEatTimes[0] + 5f < Time.time)
		{
			Action_TryHotDogAchievement.hotDogEatTimes.RemoveAt(0);
		}
		if (Action_TryHotDogAchievement.hotDogEatTimes.Count >= 3)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.CompetitiveEatingBadge);
		}
	}

	// Token: 0x0400080C RID: 2060
	public static List<float> hotDogEatTimes = new List<float>();
}
