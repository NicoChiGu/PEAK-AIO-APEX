using System;

// Token: 0x020000EF RID: 239
public class Action_Parasol : ItemAction
{
	// Token: 0x06000844 RID: 2116 RVA: 0x0002DA73 File Offset: 0x0002BC73
	public override void RunAction()
	{
		this.parasol.ToggleOpen();
	}

	// Token: 0x040007F2 RID: 2034
	public Parasol parasol;
}
