using System;

// Token: 0x02000052 RID: 82
public class CookingBehavior_ModifyBugleWobble : AdditionalCookingBehavior
{
	// Token: 0x06000431 RID: 1073 RVA: 0x0001A4D0 File Offset: 0x000186D0
	protected override void TriggerBehaviour(int cookedAmount)
	{
		int num = 0;
		while (num < cookedAmount && num <= this.maxCooking)
		{
			this.source.pitchWobble += this.changePerCooking;
			num++;
		}
	}

	// Token: 0x040004B6 RID: 1206
	public float changePerCooking;

	// Token: 0x040004B7 RID: 1207
	public int maxCooking;

	// Token: 0x040004B8 RID: 1208
	public BugleSFX source;
}
