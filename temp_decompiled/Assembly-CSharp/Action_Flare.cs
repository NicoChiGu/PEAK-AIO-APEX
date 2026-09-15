using System;

// Token: 0x020000E3 RID: 227
public class Action_Flare : ItemAction
{
	// Token: 0x06000825 RID: 2085 RVA: 0x0002D535 File Offset: 0x0002B735
	public override void RunAction()
	{
		this.flare.LightFlare();
	}

	// Token: 0x040007DD RID: 2013
	public Flare flare;
}
