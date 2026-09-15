using System;
using Photon.Pun;
using Steamworks;

// Token: 0x02000167 RID: 359
internal class SteamRichPresence : IRichPresence
{
	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x06000B63 RID: 2915 RVA: 0x0003C91B File Offset: 0x0003AB1B
	public RichPresenceState State
	{
		get
		{
			return this.m_currentState;
		}
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x0003C924 File Offset: 0x0003AB24
	public void SetState(RichPresenceState state)
	{
		this.m_currentState = state;
		int num = 1;
		int num2 = 4;
		if (state == RichPresenceState.Status_MainMenu)
		{
			SteamFriends.ClearRichPresence();
		}
		else if (PhotonNetwork.OfflineMode)
		{
			num = 1;
			num2 = 1;
		}
		else if (PhotonNetwork.InRoom)
		{
			num = PhotonNetwork.PlayerList.Length;
			SteamFriends.SetRichPresence("steam_player_group", PhotonNetwork.CurrentRoom.Name);
			num2 = PhotonNetwork.CurrentRoom.MaxPlayers;
		}
		SteamFriends.SetRichPresence("steam_display", "#" + state.ToString());
		SteamFriends.SetRichPresence("players", num.ToString());
		SteamFriends.SetRichPresence("steam_player_group_size", num.ToString());
		SteamFriends.SetRichPresence("max_players", num2.ToString());
	}

	// Token: 0x04000A85 RID: 2693
	private RichPresenceState m_currentState;
}
