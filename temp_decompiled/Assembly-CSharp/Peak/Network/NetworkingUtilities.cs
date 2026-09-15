using System;
using System.Linq;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using Zorro.Core;

namespace Peak.Network
{
	// Token: 0x020003C4 RID: 964
	public static class NetworkingUtilities
	{
		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060018C2 RID: 6338 RVA: 0x0007C899 File Offset: 0x0007AA99
		public static int MAX_PLAYERS
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x0007C89C File Offset: 0x0007AA9C
		public static string Sanitize(string text)
		{
			return NetworkingUtilities._richText.Replace(text, string.Empty);
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x0007C8AE File Offset: 0x0007AAAE
		public static string GetUsername()
		{
			return NetworkingUtilities.Sanitize(SteamFriends.GetPersonaName());
		}

		// Token: 0x060018C5 RID: 6341 RVA: 0x0007C8BC File Offset: 0x0007AABC
		private static string GetBestUserID()
		{
			string text;
			if (!Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "NoSteam") && SteamAPI.IsSteamRunning() && SteamUser.BLoggedOn())
			{
				text = SteamUser.GetSteamID().m_SteamID.ToString();
			}
			else if (PlayerPrefs.HasKey("UserID"))
			{
				text = PlayerPrefs.GetString("UserID");
			}
			else
			{
				text = Guid.NewGuid().ToString();
				PlayerPrefs.SetString("UserID", text);
			}
			return text;
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x0007C936 File Offset: 0x0007AB36
		public static AuthenticationValues LoadUserID()
		{
			return new AuthenticationValues
			{
				AuthType = 255,
				UserId = NetworkingUtilities.GetBestUserID()
			};
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060018C7 RID: 6343 RVA: 0x0007C953 File Offset: 0x0007AB53
		public static bool IsConnectedToNetwork
		{
			get
			{
				return PhotonNetwork.IsConnected;
			}
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x0007C95A File Offset: 0x0007AB5A
		public static RoomOptions HostRoomOptions()
		{
			return new RoomOptions
			{
				IsVisible = false,
				MaxPlayers = NetworkingUtilities.MAX_PLAYERS,
				PublishUserId = true
			};
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x0007C97C File Offset: 0x0007AB7C
		public static void JoinDummyRoom()
		{
			if (!PhotonNetwork.OfflineMode)
			{
				Debug.LogWarning("Oy! Attempting to start a dummy room when not in offline mode. That's inappropriate");
			}
			PhotonNetwork.NickName = NetworkingUtilities.GetUsername();
			PhotonNetwork.AuthValues = NetworkingUtilities.LoadUserID();
			RoomOptions roomOptions = NetworkingUtilities.HostRoomOptions();
			PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), roomOptions, null, null);
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x0007C9D0 File Offset: 0x0007ABD0
		public static bool HasAuthority(this MonoBehaviourPunCallbacks self)
		{
			return self.photonView.IsMine || PhotonNetwork.IsMasterClient;
		}

		// Token: 0x060018CB RID: 6347 RVA: 0x0007C9E8 File Offset: 0x0007ABE8
		public static bool ConnectToNetwork()
		{
			if (PhotonNetwork.IsConnected)
			{
				Debug.LogWarning("Attempted to connect to Photon even though we're already connected? Doing nothing.");
				return false;
			}
			PhotonNetwork.SerializationRate = 30;
			PhotonNetwork.SendRate = 30;
			BuildVersion buildVersion;
			buildVersion..ctor(Application.version, "???");
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.GameVersion = buildVersion.ToString();
			PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = buildVersion.ToMatchmaking();
			PhotonNetwork.NickName = NetworkingUtilities.GetUsername();
			PhotonNetwork.AuthValues = NetworkingUtilities.LoadUserID();
			Debug.Log("Photon Start" + PhotonNetwork.NetworkClientState.ToString() + " using app version: " + buildVersion.ToMatchmaking());
			return PhotonNetwork.ConnectUsingSettings();
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x0007CA9C File Offset: 0x0007AC9C
		public static string GetUserId(this Player self)
		{
			return self.photonView.Owner.UserId;
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x0007CAAE File Offset: 0x0007ACAE
		public static int GetActorNumber(this Player self)
		{
			return self.photonView.OwnerActorNr;
		}

		// Token: 0x040016AB RID: 5803
		private static readonly Regex _richText = new Regex("<[^>]*>");
	}
}
