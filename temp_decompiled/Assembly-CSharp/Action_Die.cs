using System;

// Token: 0x020000E2 RID: 226
public class Action_Die : ItemAction
{
	// Token: 0x06000823 RID: 2083 RVA: 0x0002D509 File Offset: 0x0002B709
	public override void RunAction()
	{
		if (base.character)
		{
			base.character.Invoke("DieInstantly", 0.02f);
		}
	}
}
