using System;
using System.Collections;
using System.Linq;
using System.Text;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core.CLI;
using Zorro.UI.Modal;

// Token: 0x02000149 RID: 329
public class NetworkConnector : MonoBehaviourPunCallbacks
{
	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000A9A RID: 2714 RVA: 0x00038C08 File Offset: 0x00036E08
	private static bool UsingEditorBootstrapping
	{
		get
		{
			return Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "Client") || (Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "Player1") && !PhotonNetwork.IsConnected) || NetworkConnector.CurrentConnectionState is DefaultConnectionState;
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000A9B RID: 2715 RVA: 0x00038C42 File Offset: 0x00036E42
	public static ConnectionState CurrentConnectionState
	{
		get
		{
			return GameHandler.GetService<ConnectionService>().StateMachine.CurrentState;
		}
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x00038C53 File Offset: 0x00036E53
	public static void ChangeConnectionState<T>() where T : ConnectionState
	{
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<T>(false);
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000A9D RID: 2717 RVA: 0x00038C66 File Offset: 0x00036E66
	// (set) Token: 0x06000A9E RID: 2718 RVA: 0x00038C6D File Offset: 0x00036E6D
	public static bool IsInitialized { get; private set; }

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000A9F RID: 2719 RVA: 0x00038C75 File Offset: 0x00036E75
	public static bool ReadyToHostOrJoinRoom
	{
		get
		{
			return NetworkConnector.IsInitialized && PhotonNetwork.NetworkClientState == 15 && !(NetworkConnector.CurrentConnectionState is DefaultConnectionState);
		}
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x00038C9C File Offset: 0x00036E9C
	private void Start()
	{
		ConnectionState currentState = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState;
		Debug.Log("Network Connector is starting in scene: " + SceneManager.GetActiveScene().name + ". \n" + string.Format("State is currently {0}.", currentState.GetType()));
		if (currentState is InRoomState)
		{
			foreach (Player player in PlayerHandler.GetAllPlayers())
			{
				player.hasClosedEndScreen = false;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				GameHandler.GetService<SteamLobbyHandler>().SetLobbyData();
			}
			if (this.keepSettingLobbyDataCoroutine == null)
			{
				this.keepSettingLobbyDataCoroutine = base.StartCoroutine(this.KeepSettingLobbyData());
			}
			NetworkConnector.IsInitialized = true;
			return;
		}
		NetworkConnector.IsInitialized = true;
		if (NetworkConnector.ReadyToHostOrJoinRoom)
		{
			NetworkConnector.HandleConnectionState(NetworkConnector.CurrentConnectionState);
			return;
		}
		if (PhotonNetwork.OfflineMode)
		{
			NetworkingUtilities.JoinDummyRoom();
			return;
		}
		if (!NetworkConnector.UsingEditorBootstrapping)
		{
			Debug.LogError("NetworkConnector is initialized, but it can't connect to a room! Something is wrong. Dumping Networking state.", this);
			NetworkConnector.PrintNetworkStates();
		}
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00038DA0 File Offset: 0x00036FA0
	private void OnDestroy()
	{
		NetworkConnector.IsInitialized = false;
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x00038DA8 File Offset: 0x00036FA8
	private static void HandleConnectionState(ConnectionState state)
	{
		HostState hostState = state as HostState;
		if (hostState != null)
		{
			RoomOptions roomOptions = NetworkingUtilities.HostRoomOptions();
			PhotonNetwork.CreateRoom(hostState.RoomName, roomOptions, null, null);
			return;
		}
		JoinSpecificRoomState joinSpecificRoomState = state as JoinSpecificRoomState;
		if (joinSpecificRoomState == null)
		{
			Debug.LogWarning(string.Format("No logic for handling state {0}", NetworkConnector.CurrentConnectionState.GetType()));
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			"$Connecting to specific region: ",
			joinSpecificRoomState.RegionToJoin,
			" with app ID ",
			PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime,
			". Is currently connected to: ",
			PhotonNetwork.CloudRegion
		}));
		if (PhotonNetwork.CloudRegion != joinSpecificRoomState.RegionToJoin && !string.IsNullOrEmpty(joinSpecificRoomState.RegionToJoin))
		{
			Debug.Log("Changing regions from " + PhotonNetwork.CloudRegion + " and reconnecting to " + joinSpecificRoomState.RegionToJoin);
			PhotonNetwork.Disconnect();
			PhotonNetwork.ConnectToRegion(joinSpecificRoomState.RegionToJoin);
			return;
		}
		PhotonNetwork.JoinRoom(joinSpecificRoomState.RoomName, null);
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00038EA4 File Offset: 0x000370A4
	private IEnumerator KeepSettingLobbyData()
	{
		while (PhotonNetwork.InRoom)
		{
			if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby() && PhotonNetwork.IsMasterClient)
			{
				GameHandler.GetService<SteamLobbyHandler>().SetLobbyData();
				Debug.Log("IS master, is updating lobby data");
			}
			yield return new WaitForSecondsRealtime(100f);
		}
		yield break;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x00038EAC File Offset: 0x000370AC
	public override void OnConnectedToMaster()
	{
		if (!NetworkConnector.UsingEditorBootstrapping)
		{
			Debug.LogWarning("Reconnect detected! Dumping network state and then attempting to recover current connection state.");
			NetworkConnector.PrintNetworkStates();
		}
		NetworkConnector.HandleConnectionState(NetworkConnector.CurrentConnectionState);
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00038ED0 File Offset: 0x000370D0
	[ConsoleCommand(true)]
	public static void PrintNetworkStates()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<color=#" + ColorUtility.ToHtmlStringRGBA(Color.magenta) + ">[[[ DUMPING NETWORK STATES ]]]</color>");
		stringBuilder.AppendLine(string.Format("Services || Connection: {0} ", NetworkConnector.CurrentConnectionState.GetType()) + "| SteamLobby.LobbyID: " + NetCode.Matchmaking.LobbyId);
		stringBuilder.AppendLine(string.Format("NetworkConnector || Initialized: {0} | Ready: {1}", NetworkConnector.IsInitialized, NetworkConnector.ReadyToHostOrJoinRoom));
		stringBuilder.Append(string.Format("PhotonNetwork || Connected: {0} | Ready: {1} ", PhotonNetwork.IsConnected, PhotonNetwork.IsConnectedAndReady) + "| ClientState: " + Enum.GetName(typeof(ClientState), PhotonNetwork.NetworkClientState));
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00038FA8 File Offset: 0x000371A8
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		SceneManager.LoadScene(NetworkConnector.rejoinScene);
		NetworkConnector.rejoinScene = "Title";
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00038FC4 File Offset: 0x000371C4
	public override void OnCreatedRoom()
	{
		base.OnCreatedRoom();
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<InRoomState>(false);
		if (!NetCode.Matchmaking.InLobby)
		{
			return;
		}
		SteamLobbyHandler service = GameHandler.GetService<SteamLobbyHandler>();
		if (PhotonNetwork.IsMasterClient)
		{
			service.SetLobbyData();
		}
		if (this.keepSettingLobbyDataCoroutine == null)
		{
			this.keepSettingLobbyDataCoroutine = base.StartCoroutine(this.KeepSettingLobbyData());
		}
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00039022 File Offset: 0x00037222
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		base.OnCreateRoomFailed(returnCode, message);
		Debug.LogError(string.Format("Failed to create Photon Room, code: {0}, message: {1}", returnCode, message));
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00039044 File Offset: 0x00037244
	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);
		if (PhotonNetwork.OfflineMode)
		{
			return;
		}
		if (cause == 17)
		{
			return;
		}
		Debug.LogError(string.Format("Disconnected from Photon Server: {0}", cause));
		NetworkConnector.ChangeConnectionState<DefaultConnectionState>();
		HeaderModalOption headerModalOption = new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_DISCONNECTEDPHOTON_TITLE", true), LocalizedText.GetText("MODAL_DISCONNECTEDPHOTON_BODY", true).Replace("#", cause.ToString()));
		ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[1];
		array[0] = new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), delegate
		{
			SceneManager.LoadScene("Title");
		});
		Modal.OpenModal(headerModalOption, new ModalButtonsOption(array), null);
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x000390F8 File Offset: 0x000372F8
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		if (Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "Client") && message != "KICKED")
		{
			JoinSpecificRoomState joinSpecificRoomState = NetworkConnector.CurrentConnectionState as JoinSpecificRoomState;
			if (joinSpecificRoomState != null)
			{
				Debug.Log("Failed to join " + joinSpecificRoomState.RoomName + ". Attempting again");
				return;
			}
			Debug.Log("Failed to join room and not in the right state!");
		}
		base.OnJoinRoomFailed(returnCode, message);
		if (message == "KICKED")
		{
			NetworkConnector.ChangeConnectionState<KickedState>();
			return;
		}
		Debug.LogError(string.Format("Failed to join Photon Room, code: {0}, message: {1}", returnCode, message));
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<DefaultConnectionState>(false);
		HeaderModalOption headerModalOption = new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_FAILEDPHOTON_TITLE", true), string.Format("[{0}] {1}", returnCode, message));
		ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[1];
		array[0] = new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), delegate
		{
			SceneManager.LoadScene("Title");
		});
		Modal.OpenModal(headerModalOption, new ModalButtonsOption(array), null);
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x000391FC File Offset: 0x000373FC
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		base.OnJoinRandomFailed(returnCode, message);
		Debug.LogError(string.Format("Failed to join Random Photon Room, code: {0}, message: {1}", returnCode, message));
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0003921C File Offset: 0x0003741C
	public override void OnJoinedRoom()
	{
		if (Character.localCharacter != null)
		{
			Debug.Log(string.Format("On Joined Photon Room. UserId:{0}, rejoined: {1}", Character.localCharacter.photonView.Owner.UserId, Character.localCharacter.photonView.Owner.HasRejoined));
		}
		else
		{
			Debug.Log("On Joined Photon Room. No Character");
		}
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<InRoomState>(false);
	}

	// Token: 0x040009F3 RID: 2547
	private Coroutine keepSettingLobbyDataCoroutine;

	// Token: 0x040009F5 RID: 2549
	private static string rejoinScene = "Title";
}
