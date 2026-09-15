using System;

// Token: 0x02000166 RID: 358
internal class NoRichPresence : IRichPresence
{
	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x06000B60 RID: 2912 RVA: 0x0003C90E File Offset: 0x0003AB0E
	public RichPresenceState State
	{
		get
		{
			return RichPresenceState.Status_MainMenu;
		}
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0003C911 File Offset: 0x0003AB11
	public void SetState(RichPresenceState state)
	{
	}
}
