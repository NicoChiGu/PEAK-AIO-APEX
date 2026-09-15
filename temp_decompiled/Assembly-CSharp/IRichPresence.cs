using System;

// Token: 0x02000165 RID: 357
internal interface IRichPresence
{
	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x06000B5E RID: 2910
	RichPresenceState State { get; }

	// Token: 0x06000B5F RID: 2911
	void SetState(RichPresenceState state);
}
