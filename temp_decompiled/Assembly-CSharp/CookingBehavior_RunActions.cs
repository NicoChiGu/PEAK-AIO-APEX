using System;

// Token: 0x02000050 RID: 80
public class CookingBehavior_RunActions : AdditionalCookingBehavior
{
	// Token: 0x0600042D RID: 1069 RVA: 0x0001A460 File Offset: 0x00018660
	protected override void TriggerBehaviour(int cookedAmount)
	{
		ItemAction[] array = this.actionsToRun;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RunAction();
		}
	}

	// Token: 0x040004B3 RID: 1203
	public ItemAction[] actionsToRun;
}
