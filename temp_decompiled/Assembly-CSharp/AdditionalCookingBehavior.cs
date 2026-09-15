using System;

// Token: 0x02000049 RID: 73
[Serializable]
public abstract class AdditionalCookingBehavior
{
	// Token: 0x0600041E RID: 1054 RVA: 0x0001A2B6 File Offset: 0x000184B6
	public void Cook(ItemCooking sourceScript, int cookedAmount)
	{
		this.itemCooking = sourceScript;
		if (this.onlyOnce && this.triggered)
		{
			return;
		}
		if (cookedAmount >= this.cookedAmountToTrigger)
		{
			this.TriggerBehaviour(cookedAmount);
			this.triggered = true;
		}
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x0001A2E7 File Offset: 0x000184E7
	protected virtual void TriggerBehaviour(int cookedAmount)
	{
	}

	// Token: 0x040004A7 RID: 1191
	public int cookedAmountToTrigger = 1;

	// Token: 0x040004A8 RID: 1192
	internal ItemCooking itemCooking;

	// Token: 0x040004A9 RID: 1193
	public bool onlyOnce;

	// Token: 0x040004AA RID: 1194
	private bool triggered;
}
