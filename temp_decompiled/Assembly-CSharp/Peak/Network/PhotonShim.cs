using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

namespace Peak.Network
{
	// Token: 0x020003C6 RID: 966
	public class PhotonShim : PhotonCallbackTarget, ISessionAPI, ISessionEvents, IConnectionCallbacks
	{
		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060018DB RID: 6363 RVA: 0x0007CBFD File Offset: 0x0007ADFD
		public bool IsHost
		{
			get
			{
				return PhotonNetwork.IsMasterClient;
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060018DC RID: 6364 RVA: 0x0007CC04 File Offset: 0x0007AE04
		public bool IsConnectedAndReady
		{
			get
			{
				return PhotonNetwork.IsConnectedAndReady;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060018DD RID: 6365 RVA: 0x0007CC0B File Offset: 0x0007AE0B
		public int HostId
		{
			get
			{
				return PhotonNetwork.MasterClient.ActorNumber;
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060018DE RID: 6366 RVA: 0x0007CC17 File Offset: 0x0007AE17
		public int SeatNumber
		{
			get
			{
				return PhotonNetwork.LocalPlayer.ActorNumber;
			}
		}

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x060018DF RID: 6367 RVA: 0x0007CC24 File Offset: 0x0007AE24
		// (remove) Token: 0x060018E0 RID: 6368 RVA: 0x0007CC5C File Offset: 0x0007AE5C
		public event Action ConnectedAndReady;

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060018E1 RID: 6369 RVA: 0x0007CC94 File Offset: 0x0007AE94
		// (remove) Token: 0x060018E2 RID: 6370 RVA: 0x0007CCCC File Offset: 0x0007AECC
		public event Action<INetworkEventData> NetworkEventReceived;

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060018E3 RID: 6371 RVA: 0x0007CD01 File Offset: 0x0007AF01
		// (set) Token: 0x060018E4 RID: 6372 RVA: 0x0007CD0D File Offset: 0x0007AF0D
		public string NickName
		{
			get
			{
				return PhotonNetwork.NetworkingClient.NickName;
			}
			set
			{
				PhotonNetwork.NetworkingClient.NickName = value;
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060018E5 RID: 6373 RVA: 0x0007CD1A File Offset: 0x0007AF1A
		// (set) Token: 0x060018E6 RID: 6374 RVA: 0x0007CD2F File Offset: 0x0007AF2F
		public AuthenticationValues AuthValues
		{
			get
			{
				if (PhotonNetwork.NetworkingClient == null)
				{
					return null;
				}
				return PhotonNetwork.NetworkingClient.AuthValues;
			}
			set
			{
				if (PhotonNetwork.NetworkingClient != null)
				{
					PhotonNetwork.NetworkingClient.AuthValues = value;
				}
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060018E7 RID: 6375 RVA: 0x0007CD43 File Offset: 0x0007AF43
		public bool InRoom
		{
			get
			{
				return PhotonNetwork.NetworkClientState == 9;
			}
		}

		// Token: 0x060018E8 RID: 6376 RVA: 0x0007CD4E File Offset: 0x0007AF4E
		public PhotonShim()
		{
			PhotonNetwork.NetworkingClient.EventReceived += new Action<EventData>(this.RaiseGenericEvent);
		}

		// Token: 0x060018E9 RID: 6377 RVA: 0x0007CD6C File Offset: 0x0007AF6C
		public void InitializeNetcodeBackend()
		{
			BuildVersion buildVersion;
			buildVersion..ctor(Application.version, "???");
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.GameVersion = buildVersion.ToString();
			PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = buildVersion.ToMatchmaking();
			PhotonNetwork.ConnectUsingSettings();
			Debug.Log("Photon Start" + PhotonNetwork.NetworkClientState.ToString() + " using app version: " + buildVersion.ToMatchmaking());
		}

		// Token: 0x060018EA RID: 6378 RVA: 0x0007CDEC File Offset: 0x0007AFEC
		public override void Dispose()
		{
			base.Dispose();
			this.ConnectedAndReady = null;
			PhotonNetwork.NetworkingClient.EventReceived -= new Action<EventData>(this.RaiseGenericEvent);
		}

		// Token: 0x060018EB RID: 6379 RVA: 0x0007CE11 File Offset: 0x0007B011
		public void OnConnectedToMaster()
		{
			Action connectedAndReady = this.ConnectedAndReady;
			if (connectedAndReady == null)
			{
				return;
			}
			connectedAndReady.Invoke();
		}

		// Token: 0x060018EC RID: 6380 RVA: 0x0007CE23 File Offset: 0x0007B023
		public void OnConnected()
		{
		}

		// Token: 0x060018ED RID: 6381 RVA: 0x0007CE25 File Offset: 0x0007B025
		public void OnDisconnected(DisconnectCause cause)
		{
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x0007CE27 File Offset: 0x0007B027
		public void OnRegionListReceived(RegionHandler regionHandler)
		{
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x0007CE29 File Offset: 0x0007B029
		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x0007CE2B File Offset: 0x0007B02B
		public void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x0007CE2D File Offset: 0x0007B02D
		public void Kick(string userId)
		{
			if (!NetCode.Session.IsHost)
			{
				Debug.LogWarning("Someone who is not MasterClient is clicking the Kick button (why do they have a kick button?)");
				return;
			}
			PhotonNetwork.RaiseEvent(18, userId, RaiseEventOptions.Default, SendOptions.SendReliable);
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x0007CE59 File Offset: 0x0007B059
		private void RaiseGenericEvent(EventData eventData)
		{
			Action<INetworkEventData> networkEventReceived = this.NetworkEventReceived;
			if (networkEventReceived == null)
			{
				return;
			}
			networkEventReceived.Invoke(new PhotonShim.NetworkEventData(eventData));
		}

		// Token: 0x02000539 RID: 1337
		public class NetworkEventData : INetworkEventData
		{
			// Token: 0x1700027D RID: 637
			// (get) Token: 0x06001DF1 RID: 7665 RVA: 0x00088E04 File Offset: 0x00087004
			public int EventCode
			{
				get
				{
					return (int)this._eventData.Code;
				}
			}

			// Token: 0x06001DF2 RID: 7666 RVA: 0x00088E11 File Offset: 0x00087011
			internal NetworkEventData(EventData eventData)
			{
				this._eventData = eventData;
			}

			// Token: 0x04001BF9 RID: 7161
			private EventData _eventData;
		}
	}
}
