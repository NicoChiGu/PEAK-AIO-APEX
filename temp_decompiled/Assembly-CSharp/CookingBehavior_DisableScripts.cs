using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class CookingBehavior_DisableScripts : AdditionalCookingBehavior
{
	// Token: 0x06000421 RID: 1057 RVA: 0x0001A2F8 File Offset: 0x000184F8
	protected override void TriggerBehaviour(int cookedAmount)
	{
		foreach (MonoBehaviour monoBehaviour in this.scriptsToDisable)
		{
			if (monoBehaviour != null)
			{
				monoBehaviour.enabled = false;
			}
		}
	}

	// Token: 0x040004AB RID: 1195
	public MonoBehaviour[] scriptsToDisable;
}
