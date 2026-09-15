using System;
using System.Collections.Generic;
using Photon.Realtime;
using Steamworks;
using UnityEngine;

namespace Peak.Network
{
	// Token: 0x020003CB RID: 971
	public class SteamLobbyAPI : IMatchmakingAPI, IMatchmakingEvents, IMatchmakingCallbacks
	{
		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06001909 RID: 6409 RVA: 0x0007D09C File Offset: 0x0007B29C
		// (remove) Token: 0x0600190A RID: 6410 RVA: 0x0007D0D4 File Offset: 0x0007B2D4
		public event Action InviteReceived;

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x0600190B RID: 6411 RVA: 0x0007D109 File Offset: 0x0007B309
		public string LobbyId
		{
			get
			{
				SteamLobbyHandler lobbyHandler = SteamLobbyAPI.LobbyHandler;
				return ((lobbyHandler != null) ? lobbyHandler.CurrentLobbyID : null) ?? "None";
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x0600190C RID: 6412 RVA: 0x0007D125 File Offset: 0x0007B325
		public bool InLobby
		{
			get
			{
				SteamLobbyHandler lobbyHandler = SteamLobbyAPI.LobbyHandler;
				return lobbyHandler != null && lobbyHandler.InSteamLobby();
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x0600190D RID: 6413 RVA: 0x0007D137 File Offset: 0x0007B337
		private static SteamLobbyHandler LobbyHandler
		{
			get
			{
				return GameHandler.GetService<SteamLobbyHandler>();
			}
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x0007D13E File Offset: 0x0007B33E
		public SteamLobbyAPI()
		{
			Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnLobbyJoinRequested));
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x0007D158 File Offset: 0x0007B358
		public void CreateLobby(LobbyTypeSetting.LobbyType lobbyType)
		{
			SteamMatchmaking.CreateLobby((lobbyType == LobbyTypeSetting.LobbyType.Friends) ? 1 : 0, NetworkingUtilities.MAX_PLAYERS);
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x0007D16C File Offset: 0x0007B36C
		public void LeaveLobby()
		{
			SteamLobbyAPI.LobbyHandler.LeaveLobby();
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06001911 RID: 6417 RVA: 0x0007D178 File Offset: 0x0007B378
		public bool HasPendingInvite
		{
			get
			{
				if (this._hasPendingJoinRequest)
				{
					return true;
				}
				if (!GameHandler.Initialized)
				{
					return false;
				}
				if (SteamLobbyAPI.LobbyHandler.TryConsumeLaunchCommandInvite(out SteamLobbyAPI._pendingLobbyJoinID))
				{
					this._hasPendingJoinRequest = true;
				}
				return this._hasPendingJoinRequest;
			}
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x0007D1AC File Offset: 0x0007B3AC
		public bool PlayerIsInLobby(string playerId)
		{
			ulong num;
			if (!ulong.TryParse(playerId, ref num))
			{
				Debug.LogWarning("Player " + playerId + " is not a valid Steam ID. Something is fishy!");
				return false;
			}
			CSteamID currentLobbySteamID = SteamLobbyAPI.LobbyHandler.CurrentLobbySteamID;
			CSteamID csteamID;
			csteamID..ctor(num);
			int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(currentLobbySteamID);
			for (int i = 0; i < numLobbyMembers; i++)
			{
				if (csteamID == SteamMatchmaking.GetLobbyMemberByIndex(currentLobbySteamID, i))
				{
					return true;
				}
			}
			Debug.LogWarning("Didn't find " + playerId + " in this steam lobby. Something is fishy!");
			return false;
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x0007D22C File Offset: 0x0007B42C
		public void ConsumePendingJoin()
		{
			if (!this.HasPendingInvite)
			{
				Debug.LogWarning("Attempted to consume join request but we have none pending.");
				return;
			}
			this._hasPendingJoinRequest = false;
			SteamLobbyAPI.LobbyHandler.RequestLobbyData(SteamLobbyAPI._pendingLobbyJoinID);
		}

		// Token: 0x06001914 RID: 6420 RVA: 0x0007D258 File Offset: 0x0007B458
		private void OnLobbyJoinRequested(GameLobbyJoinRequested_t param)
		{
			Debug.Log(string.Format("On Lobby Join Requested: {0} by {1}", param.m_steamIDLobby, param.m_steamIDFriend));
			this._hasPendingJoinRequest = true;
			SteamLobbyAPI._pendingLobbyJoinID = param.m_steamIDLobby;
			Action inviteReceived = this.InviteReceived;
			if (inviteReceived == null)
			{
				return;
			}
			inviteReceived.Invoke();
		}

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06001915 RID: 6421 RVA: 0x0007D2AC File Offset: 0x0007B4AC
		// (remove) Token: 0x06001916 RID: 6422 RVA: 0x0007D2E4 File Offset: 0x0007B4E4
		public event Action<short, string> JoinLobbyFailed;

		// Token: 0x06001917 RID: 6423 RVA: 0x0007D319 File Offset: 0x0007B519
		public void OnJoinRoomFailed(short returnCode, string message)
		{
			Action<short, string> joinLobbyFailed = this.JoinLobbyFailed;
			if (joinLobbyFailed == null)
			{
				return;
			}
			joinLobbyFailed.Invoke(returnCode, message);
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x0007D32D File Offset: 0x0007B52D
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x0007D32F File Offset: 0x0007B52F
		public void OnCreatedRoom()
		{
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x0007D331 File Offset: 0x0007B531
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x0007D333 File Offset: 0x0007B533
		public void OnJoinedRoom()
		{
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x0007D335 File Offset: 0x0007B535
		public void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x0007D337 File Offset: 0x0007B537
		public void OnLeftRoom()
		{
		}

		// Token: 0x040016B6 RID: 5814
		private static CSteamID _pendingLobbyJoinID;

		// Token: 0x040016B7 RID: 5815
		private bool _hasPendingJoinRequest;
	}
}
