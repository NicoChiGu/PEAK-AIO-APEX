using System;
using Sirenix.Utilities;
using Unity.Multiplayer.Playmode;
using UnityEngine;

namespace Peak.Network
{
	// Token: 0x020003C5 RID: 965
	public class NoMatchmaking : IMatchmakingAPI, IMatchmakingEvents
	{
		// Token: 0x060018CF RID: 6351 RVA: 0x0007CACC File Offset: 0x0007ACCC
		public NoMatchmaking()
		{
			Debug.LogWarning("Initializing without a matchmaking API. Will be unable to receive game invites.");
		}

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x060018D0 RID: 6352 RVA: 0x0007CAE0 File Offset: 0x0007ACE0
		// (remove) Token: 0x060018D1 RID: 6353 RVA: 0x0007CB18 File Offset: 0x0007AD18
		public event Action InviteReceived;

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060018D2 RID: 6354 RVA: 0x0007CB4D File Offset: 0x0007AD4D
		public string LobbyId
		{
			get
			{
				return "Disabled";
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060018D3 RID: 6355 RVA: 0x0007CB54 File Offset: 0x0007AD54
		public bool InLobby
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060018D4 RID: 6356 RVA: 0x0007CB57 File Offset: 0x0007AD57
		public void CreateLobby(LobbyTypeSetting.LobbyType lobbyType)
		{
			Debug.LogWarning("Not actually creating a lobby.");
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x0007CB63 File Offset: 0x0007AD63
		public void LeaveLobby()
		{
		}

		// Token: 0x060018D6 RID: 6358 RVA: 0x0007CB65 File Offset: 0x0007AD65
		public bool PlayerIsInLobby(string playerId)
		{
			Debug.Log("There is no lobby");
			return !LinqExtensions.IsNullOrEmpty<string>(CurrentPlayer.ReadOnlyTags());
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060018D7 RID: 6359 RVA: 0x0007CB7E File Offset: 0x0007AD7E
		public bool HasPendingInvite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060018D8 RID: 6360 RVA: 0x0007CB81 File Offset: 0x0007AD81
		public void ConsumePendingJoin()
		{
			Debug.LogError("Can't consume pending join! We don't have any matchmaking initialized.");
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x060018D9 RID: 6361 RVA: 0x0007CB90 File Offset: 0x0007AD90
		// (remove) Token: 0x060018DA RID: 6362 RVA: 0x0007CBC8 File Offset: 0x0007ADC8
		public event Action<short, string> JoinLobbyFailed;
	}
}
