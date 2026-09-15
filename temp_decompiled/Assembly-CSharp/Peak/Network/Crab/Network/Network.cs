using System;
using Crab.Network;

namespace Peak.Network.Crab.Network
{
	// Token: 0x020003CC RID: 972
	[Obsolete("Moved to Peak.Network.NetCode")]
	public static class Network
	{
		// Token: 0x1700018B RID: 395
		// (get) Token: 0x0600191E RID: 6430 RVA: 0x0007D339 File Offset: 0x0007B539
		[Obsolete("Moved to Peak.Network.NetCode.Session")]
		public static IRelayAPI Relay
		{
			get
			{
				return NetCode.Session as IRelayAPI;
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x0600191F RID: 6431 RVA: 0x0007D345 File Offset: 0x0007B545
		[Obsolete("Moved to Peak.Network.NetCode")]
		public static IMatchmakingAPI Matchmaking
		{
			get
			{
				return NetCode.Matchmaking;
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06001920 RID: 6432 RVA: 0x0007D34C File Offset: 0x0007B54C
		[Obsolete("Moved to Peak.Network.NetCode")]
		public static IRelayEvents ConnectionEvents
		{
			get
			{
				return Network.Relay;
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06001921 RID: 6433 RVA: 0x0007D353 File Offset: 0x0007B553
		[Obsolete("Moved to Peak.Network.NetCode")]
		public static INetworkRoomEvents RoomEvents
		{
			get
			{
				return NetCode.RoomEvents;
			}
		}
	}
}
