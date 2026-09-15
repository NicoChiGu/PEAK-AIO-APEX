using System;

namespace Peak.Network
{
	// Token: 0x020003C9 RID: 969
	public class PlayerSessionData
	{
		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06001902 RID: 6402 RVA: 0x0007D051 File Offset: 0x0007B251
		public string Id { get; }

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06001903 RID: 6403 RVA: 0x0007D059 File Offset: 0x0007B259
		// (set) Token: 0x06001904 RID: 6404 RVA: 0x0007D061 File Offset: 0x0007B261
		public int ActorNumber { get; internal set; }

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06001905 RID: 6405 RVA: 0x0007D06A File Offset: 0x0007B26A
		// (set) Token: 0x06001906 RID: 6406 RVA: 0x0007D072 File Offset: 0x0007B272
		public bool IsBanned { get; internal set; }

		// Token: 0x06001907 RID: 6407 RVA: 0x0007D07B File Offset: 0x0007B27B
		public PlayerSessionData(string id, int actorNumber)
		{
			this.Id = id;
			this.ActorNumber = actorNumber;
		}
	}
}
