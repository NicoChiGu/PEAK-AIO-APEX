using System;

// Token: 0x0200004C RID: 76
public class CookingBehavior_Explode : AdditionalCookingBehavior
{
	// Token: 0x06000425 RID: 1061 RVA: 0x0001A376 File Offset: 0x00018576
	protected override void TriggerBehaviour(int cookedAmount)
	{
		if (this.dontRunIfOutOfFuel && this.itemCooking.item.GetData<FloatItemData>(DataEntryKey.Fuel).Value <= 0f)
		{
			return;
		}
		this.itemCooking.Explode();
	}

	// Token: 0x040004AD RID: 1197
	public bool dontRunIfOutOfFuel;
}
