using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000E7 RID: 231
public class Action_IncrementSteamStat : ItemAction
{
	// Token: 0x06000833 RID: 2099 RVA: 0x0002D728 File Offset: 0x0002B928
	public override void RunAction()
	{
		int num = Singleton<AchievementManager>.Instance.IncrementSteamStat(this.statType, this.incrementAmount);
		Debug.Log(string.Format("New value of {0} is {1}", this.statType, num));
	}

	// Token: 0x040007E3 RID: 2019
	public STEAMSTATTYPE statType;

	// Token: 0x040007E4 RID: 2020
	public int incrementAmount;
}
