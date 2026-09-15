using System;
using System.Linq;
using Unity.Multiplayer.Playmode;

namespace Peak.Network
{
	// Token: 0x020003BD RID: 957
	public static class NetCode
	{
		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600189A RID: 6298 RVA: 0x0007C824 File Offset: 0x0007AA24
		private static bool SteamIsEnabled
		{
			get
			{
				return !Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "NoSteam");
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600189B RID: 6299 RVA: 0x0007C838 File Offset: 0x0007AA38
		public static ISessionAPI Session { get; } = new PhotonShim();

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600189C RID: 6300 RVA: 0x0007C83F File Offset: 0x0007AA3F
		public static IMatchmakingAPI Matchmaking { get; }

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x0600189D RID: 6301 RVA: 0x0007C846 File Offset: 0x0007AA46
		public static ISessionEvents ConnectionEvents
		{
			get
			{
				return NetCode.Session;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x0600189E RID: 6302 RVA: 0x0007C84D File Offset: 0x0007AA4D
		public static INetworkRoomEvents RoomEvents { get; }

		// Token: 0x0600189F RID: 6303 RVA: 0x0007C854 File Offset: 0x0007AA54
		// Note: this type is marked as 'beforefieldinit'.
		static NetCode()
		{
			IMatchmakingAPI matchmakingAPI2;
			if (!NetCode.SteamIsEnabled)
			{
				IMatchmakingAPI matchmakingAPI = new NoMatchmaking();
				matchmakingAPI2 = matchmakingAPI;
			}
			else
			{
				IMatchmakingAPI matchmakingAPI = new SteamLobbyAPI();
				matchmakingAPI2 = matchmakingAPI;
			}
			NetCode.Matchmaking = matchmakingAPI2;
			NetCode.RoomEvents = new PhotonRoomEvents();
		}
	}
}
