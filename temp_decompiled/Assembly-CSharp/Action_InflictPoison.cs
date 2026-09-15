using System;
using Peak.Afflictions;

// Token: 0x020000E8 RID: 232
public class Action_InflictPoison : ItemAction
{
	// Token: 0x06000835 RID: 2101 RVA: 0x0002D774 File Offset: 0x0002B974
	public override void RunAction()
	{
		base.character.refs.afflictions.AddAffliction(new Affliction_PoisonOverTime(this.inflictionTime, this.delay, this.poisonPerSecond), false);
	}

	// Token: 0x040007E5 RID: 2021
	public float inflictionTime;

	// Token: 0x040007E6 RID: 2022
	public float poisonPerSecond;

	// Token: 0x040007E7 RID: 2023
	public float delay;
}
