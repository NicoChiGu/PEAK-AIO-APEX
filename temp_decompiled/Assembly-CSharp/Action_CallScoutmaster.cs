using System;

// Token: 0x020000DD RID: 221
public class Action_CallScoutmaster : ItemAction
{
	// Token: 0x06000814 RID: 2068 RVA: 0x0002D1F0 File Offset: 0x0002B3F0
	public override void RunAction()
	{
		Scoutmaster scoutmaster;
		if (Scoutmaster.GetPrimaryScoutmaster(out scoutmaster))
		{
			scoutmaster.SetCurrentTarget(this.item.holderCharacter, this.forcedChaseTime);
		}
	}

	// Token: 0x040007D6 RID: 2006
	public float forcedChaseTime;
}
