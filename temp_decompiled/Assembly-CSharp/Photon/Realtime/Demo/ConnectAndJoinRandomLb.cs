using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Realtime.Demo
{
	// Token: 0x02000397 RID: 919
	public class ConnectAndJoinRandomLb : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
	{
		// Token: 0x060017DB RID: 6107 RVA: 0x000791B4 File Offset: 0x000773B4
		public void Start()
		{
			this.lbc = new LoadBalancingClient(0);
			this.lbc.AddCallbackTarget(this);
			if (!this.lbc.ConnectUsingSettings(this.appSettings))
			{
				Debug.LogError("Error while connecting");
			}
			this.ch = base.gameObject.GetComponent<ConnectionHandler>();
			if (this.ch != null)
			{
				this.ch.Client = this.lbc;
				this.ch.StartFallbackSendAckThread();
			}
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x00079234 File Offset: 0x00077434
		public void Update()
		{
			LoadBalancingClient loadBalancingClient = this.lbc;
			if (loadBalancingClient != null)
			{
				loadBalancingClient.Service();
				Text stateUiText = this.StateUiText;
				string text = loadBalancingClient.State.ToString();
				if (stateUiText != null && !stateUiText.text.Equals(text))
				{
					stateUiText.text = "State: " + text;
				}
			}
		}

		// Token: 0x060017DD RID: 6109 RVA: 0x00079295 File Offset: 0x00077495
		public void OnConnected()
		{
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x00079297 File Offset: 0x00077497
		public void OnConnectedToMaster()
		{
			Debug.Log("OnConnectedToMaster");
			this.lbc.OpJoinRandomRoom(null);
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x000792B0 File Offset: 0x000774B0
		public void OnDisconnected(DisconnectCause cause)
		{
			Debug.Log("OnDisconnected(" + cause.ToString() + ")");
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x000792D3 File Offset: 0x000774D3
		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x000792D5 File Offset: 0x000774D5
		public void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x000792D7 File Offset: 0x000774D7
		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			Debug.Log("OnRegionListReceived");
			regionHandler.PingMinimumOfRegions(new Action<RegionHandler>(this.OnRegionPingCompleted), null);
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x000792F7 File Offset: 0x000774F7
		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x000792F9 File Offset: 0x000774F9
		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x000792FB File Offset: 0x000774FB
		public void OnJoinedLobby()
		{
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x000792FD File Offset: 0x000774FD
		public void OnLeftLobby()
		{
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x000792FF File Offset: 0x000774FF
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x00079301 File Offset: 0x00077501
		public void OnCreatedRoom()
		{
		}

		// Token: 0x060017E9 RID: 6121 RVA: 0x00079303 File Offset: 0x00077503
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060017EA RID: 6122 RVA: 0x00079305 File Offset: 0x00077505
		public void OnJoinedRoom()
		{
			Debug.Log("OnJoinedRoom");
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x00079311 File Offset: 0x00077511
		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060017EC RID: 6124 RVA: 0x00079313 File Offset: 0x00077513
		public void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.Log("OnJoinRandomFailed");
			this.lbc.OpCreateRoom(new EnterRoomParams());
		}

		// Token: 0x060017ED RID: 6125 RVA: 0x00079330 File Offset: 0x00077530
		public void OnLeftRoom()
		{
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x00079334 File Offset: 0x00077534
		private void OnRegionPingCompleted(RegionHandler regionHandler)
		{
			string text = "OnRegionPingCompleted ";
			Region bestRegion = regionHandler.BestRegion;
			Debug.Log(text + ((bestRegion != null) ? bestRegion.ToString() : null));
			Debug.Log("RegionPingSummary: " + regionHandler.SummaryToCache);
			this.lbc.ConnectToRegionMaster(regionHandler.BestRegion.Code);
		}

		// Token: 0x04001636 RID: 5686
		[SerializeField]
		private AppSettings appSettings = new AppSettings();

		// Token: 0x04001637 RID: 5687
		private LoadBalancingClient lbc;

		// Token: 0x04001638 RID: 5688
		private ConnectionHandler ch;

		// Token: 0x04001639 RID: 5689
		public Text StateUiText;
	}
}
