using System;

// Token: 0x020000EC RID: 236
public class Action_MoraleBoost : ItemAction
{
	// Token: 0x0600083E RID: 2110 RVA: 0x0002D9E0 File Offset: 0x0002BBE0
	public override void RunAction()
	{
		MoraleBoost.SpawnMoraleBoost(base.transform.position, this.boostRadius, this.baselineStaminaBoost, this.staminaBoostPerAdditionalScout, true, 1);
	}

	// Token: 0x040007ED RID: 2029
	public float boostRadius;

	// Token: 0x040007EE RID: 2030
	public float baselineStaminaBoost;

	// Token: 0x040007EF RID: 2031
	public float staminaBoostPerAdditionalScout;
}
