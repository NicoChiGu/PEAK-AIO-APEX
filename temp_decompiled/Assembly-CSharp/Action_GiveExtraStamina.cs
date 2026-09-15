using System;

// Token: 0x020000E4 RID: 228
public class Action_GiveExtraStamina : ItemAction
{
	// Token: 0x06000827 RID: 2087 RVA: 0x0002D54A File Offset: 0x0002B74A
	public override void RunAction()
	{
		base.character.AddExtraStamina(this.amount);
	}

	// Token: 0x040007DE RID: 2014
	public float amount;
}
