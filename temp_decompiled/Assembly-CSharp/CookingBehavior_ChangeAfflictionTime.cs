using System;

// Token: 0x02000053 RID: 83
public class CookingBehavior_ChangeAfflictionTime : AdditionalCookingBehavior
{
	// Token: 0x06000433 RID: 1075 RVA: 0x0001A512 File Offset: 0x00018712
	protected override void TriggerBehaviour(int cookedAmount)
	{
		this.action.affliction.totalTime += this.change;
	}

	// Token: 0x040004B9 RID: 1209
	public Action_ApplyAffliction action;

	// Token: 0x040004BA RID: 1210
	public float change;
}
