using System;

// Token: 0x020000F0 RID: 240
public class Action_Passport : ItemAction
{
	// Token: 0x06000846 RID: 2118 RVA: 0x0002DA88 File Offset: 0x0002BC88
	public override void RunAction()
	{
		PassportManager.instance.ToggleOpen();
	}
}
