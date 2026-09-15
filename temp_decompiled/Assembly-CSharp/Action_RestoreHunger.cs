using System;

// Token: 0x020000F5 RID: 245
public class Action_RestoreHunger : ItemAction
{
	// Token: 0x06000853 RID: 2131 RVA: 0x0002DC93 File Offset: 0x0002BE93
	public override void RunAction()
	{
		base.character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, this.restorationAmount, false, false);
	}

	// Token: 0x040007FF RID: 2047
	public float restorationAmount;
}
