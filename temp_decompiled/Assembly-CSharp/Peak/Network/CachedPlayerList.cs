using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Peak.Network
{
	// Token: 0x020003BC RID: 956
	public struct CachedPlayerList : IInRoomCallbacks, IDisposable
	{
		// Token: 0x17000166 RID: 358
		// (get) Token: 0x0600188D RID: 6285 RVA: 0x0007C6C1 File Offset: 0x0007A8C1
		private bool ShouldRefreshCache
		{
			get
			{
				return this._frameCacheLastDirtied - this._frameCacheLastUpdated > 1 || this._playerList.Length != PhotonNetwork.CurrentRoom.PlayerCount;
			}
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x0007C6EC File Offset: 0x0007A8EC
		public Player[] Get()
		{
			if (!this._isInitialized)
			{
				PhotonNetwork.AddCallbackTarget(this);
				this._isInitialized = true;
				this.RefreshCache();
			}
			if (!PhotonNetwork.InRoom)
			{
				Debug.LogWarning("Attempting to fetch player list when not in a room...");
				return CachedPlayerList.s_EmptyList;
			}
			if (this.ShouldRefreshCache || this._playerList.Length != PhotonNetwork.CurrentRoom.PlayerCount)
			{
				this.RefreshCache();
			}
			return this._playerList;
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x0007C760 File Offset: 0x0007A960
		public Player GetNetworkPlayerByActorId(int actorId)
		{
			foreach (Player player in this.Get())
			{
				if (player.ActorNumber == actorId)
				{
					return player;
				}
			}
			return null;
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x0007C792 File Offset: 0x0007A992
		public string GetUserID(int actorId)
		{
			Player networkPlayerByActorId = this.GetNetworkPlayerByActorId(actorId);
			return ((networkPlayerByActorId != null) ? networkPlayerByActorId.UserId : null) ?? string.Empty;
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x0007C7B0 File Offset: 0x0007A9B0
		private void RefreshCache()
		{
			this._playerList = PhotonNetwork.PlayerList;
			this._frameCacheLastUpdated = Time.frameCount;
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x0007C7C8 File Offset: 0x0007A9C8
		private void MarkDirty()
		{
			this._frameCacheLastDirtied = Time.frameCount;
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x0007C7D5 File Offset: 0x0007A9D5
		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			this.MarkDirty();
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x0007C7DD File Offset: 0x0007A9DD
		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			this.MarkDirty();
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0007C7E5 File Offset: 0x0007A9E5
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			this.MarkDirty();
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x0007C7ED File Offset: 0x0007A9ED
		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			this.MarkDirty();
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x0007C7F5 File Offset: 0x0007A9F5
		public void OnMasterClientSwitched(Player newMasterClient)
		{
			this.MarkDirty();
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x0007C7FD File Offset: 0x0007A9FD
		public void Dispose()
		{
			if (this._isInitialized)
			{
				PhotonNetwork.RemoveCallbackTarget(this);
			}
		}

		// Token: 0x040016A3 RID: 5795
		private bool _isInitialized;

		// Token: 0x040016A4 RID: 5796
		private int _frameCacheLastDirtied;

		// Token: 0x040016A5 RID: 5797
		private int _frameCacheLastUpdated;

		// Token: 0x040016A6 RID: 5798
		private static Player[] s_EmptyList = new Player[0];

		// Token: 0x040016A7 RID: 5799
		private Player[] _playerList;
	}
}
