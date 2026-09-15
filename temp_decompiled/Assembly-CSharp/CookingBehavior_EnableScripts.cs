using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class CookingBehavior_EnableScripts : AdditionalCookingBehavior
{
	// Token: 0x06000423 RID: 1059 RVA: 0x0001A338 File Offset: 0x00018538
	protected override void TriggerBehaviour(int cookedAmount)
	{
		foreach (MonoBehaviour monoBehaviour in this.scriptsToEnable)
		{
			if (monoBehaviour != null)
			{
				monoBehaviour.enabled = true;
			}
		}
	}

	// Token: 0x040004AC RID: 1196
	public MonoBehaviour[] scriptsToEnable;
}
