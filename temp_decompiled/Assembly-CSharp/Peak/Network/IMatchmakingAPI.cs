using System;

namespace Peak.Network
{
	// Token: 0x020003C3 RID: 963
	public interface IMatchmakingAPI : IMatchmakingEvents
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060018B9 RID: 6329
		bool InLobby { get; }

		// Token: 0x060018BA RID: 6330
		void CreateLobby(LobbyTypeSetting.LobbyType lobbyType);

		// Token: 0x060018BB RID: 6331
		void LeaveLobby();

		// Token: 0x060018BC RID: 6332
		bool PlayerIsInLobby(string playerId);

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060018BD RID: 6333
		bool HasPendingInvite { get; }

		// Token: 0x060018BE RID: 6334
		void ConsumePendingJoin();

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060018BF RID: 6335
		string LobbyId { get; }

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x060018C0 RID: 6336
		// (remove) Token: 0x060018C1 RID: 6337
		event Action<short, string> JoinLobbyFailed;
	}
}
