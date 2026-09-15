using System;

// Token: 0x0200004E RID: 78
public class CookingBehavior_Wreck : AdditionalCookingBehavior
{
	// Token: 0x06000429 RID: 1065 RVA: 0x0001A3F2 File Offset: 0x000185F2
	protected override void TriggerBehaviour(int cookedAmount)
	{
		this.itemCooking.Wreck();
	}
}
