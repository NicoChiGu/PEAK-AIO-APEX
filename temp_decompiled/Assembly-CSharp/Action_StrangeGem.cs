using System;

// Token: 0x020000F8 RID: 248
public class Action_StrangeGem : ItemAction
{
	// Token: 0x0600085E RID: 2142 RVA: 0x0002DF43 File Offset: 0x0002C143
	public override void RunAction()
	{
		Action_StrangeGem.gemActive = !Action_StrangeGem.gemActive;
		GlobalEvents.TriggerGemActivated(Action_StrangeGem.gemActive);
	}

	// Token: 0x04000805 RID: 2053
	public static bool gemActive;
}
