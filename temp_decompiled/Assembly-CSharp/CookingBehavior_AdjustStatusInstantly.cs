using System;

// Token: 0x0200004F RID: 79
public class CookingBehavior_AdjustStatusInstantly : AdditionalCookingBehavior
{
	// Token: 0x0600042B RID: 1067 RVA: 0x0001A408 File Offset: 0x00018608
	protected override void TriggerBehaviour(int cookedAmount)
	{
		if (this.itemCooking.item.holderCharacter)
		{
			this.itemCooking.item.holderCharacter.refs.afflictions.AdjustStatus(this.statusType, this.amount, false);
		}
	}

	// Token: 0x040004B1 RID: 1201
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x040004B2 RID: 1202
	public float amount;
}
