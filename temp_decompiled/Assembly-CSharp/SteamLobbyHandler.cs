using System;
using System.Collections;
using System.Linq;
using System.Text;
using Photon.Pun;
using Steamworks;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.Core.Serizalization;
using Zorro.UI.Modal;

// Token: 0x0200008D RID: 141
public class SteamLobbyHandler : GameService
{
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x0600059A RID: 1434 RVA: 0x0001FF60 File Offset: 0x0001E160
	// (set) Token: 0x0600059B RID: 1435 RVA: 0x0001FF67 File Offset: 0x0001E167
	private static bool HasCheckedSteamLaunchCommand { get; set; }

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x0600059C RID: 1436 RVA: 0x0001FF6F File Offset: 0x0001E16F
	public string CurrentLobbyID
	{
		get
		{
			return this.m_currentLobby.ToString();
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x0600059D RID: 1437 RVA: 0x0001FF82 File Offset: 0x0001E182
	public CSteamID CurrentLobbySteamID
	{
		get
		{
			return this.m_currentLobby;
		}
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x0001FF8C File Offset: 0x0001E18C
	public SteamLobbyHandler()
	{
		Debug.Log("Steam Lobby Handler initialized");
		Callback<LobbyCreated_t>.Create(new Callback<LobbyCreated_t>.DispatchDelegate(this.OnLobbyCreated));
		Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(this.OnLobbyEnter));
		Callback<LobbyDataUpdate_t>.Create(new Callback<LobbyDataUpdate_t>.DispatchDelegate(this.OnLobbyDataUpdate));
		Callback<LobbyChatMsg_t>.Create(new Callback<LobbyChatMsg_t>.DispatchDelegate(this.OnLobbyChat));
		this.m_currentLobby = CSteamID.Nil;
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x00020008 File Offset: 0x0001E208
	public bool TryConsumeLaunchCommandInvite(out CSteamID invite)
	{
		invite = default(CSteamID);
		if (SteamLobbyHandler.HasCheckedSteamLaunchCommand)
		{
			return false;
		}
		SteamLobbyHandler.HasCheckedSteamLaunchCommand = true;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs.Length < 2)
		{
			return false;
		}
		for (int i = 0; i < commandLineArgs.Length - 1; i++)
		{
			ulong num;
			if (!(commandLineArgs[i].ToLower() != "+connect_lobby") && ulong.TryParse(commandLineArgs[i + 1], ref num) && num > 0UL)
			{
				invite = new CSteamID(num);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x00020080 File Offset: 0x0001E280
	public void CheckForSteamInviteAndConnect()
	{
		if (SteamLobbyHandler.HasCheckedSteamLaunchCommand)
		{
			return;
		}
		SteamLobbyHandler.HasCheckedSteamLaunchCommand = true;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs.Length < 2)
		{
			return;
		}
		for (int i = 0; i < commandLineArgs.Length - 1; i++)
		{
			ulong num;
			if (!(commandLineArgs[i].ToLower() != "+connect_lobby") && ulong.TryParse(commandLineArgs[i + 1], ref num) && num > 0UL)
			{
				Debug.Log(string.Format("Steam invite detected! Attempting to connect to lobby {0}", num));
				this.RequestLobbyData(new CSteamID(num));
				return;
			}
		}
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x00020104 File Offset: 0x0001E304
	private void OnLobbyEnter(LobbyEnter_t param)
	{
		if (this.m_isHosting)
		{
			this.m_isHosting = false;
			return;
		}
		if (param.m_EChatRoomEnterResponse != 1U)
		{
			this.m_currentLobby = CSteamID.Nil;
			return;
		}
		this.m_currentLobby = new CSteamID(param.m_ulSteamIDLobby);
		Debug.Log("Entered Steam Lobby: " + this.m_currentLobby.ToString());
		string lobbyData = SteamMatchmaking.GetLobbyData(this.m_currentLobby, "PhotonRegion");
		string lobbyData2 = SteamMatchmaking.GetLobbyData(this.m_currentLobby, "CurrentScene");
		if (!string.IsNullOrEmpty(lobbyData))
		{
			this.tryingToFetchLobbyDataAttempts = Optionable<int>.None;
			this.m_currentlyWaitingForRoomID = Optionable<ValueTuple<CSteamID, string, string>>.Some(new ValueTuple<CSteamID, string, string>(this.m_currentLobby, lobbyData2, lobbyData));
			return;
		}
		if (this.tryingToFetchLobbyDataAttempts.IsNone)
		{
			this.tryingToFetchLobbyDataAttempts = Optionable<int>.Some(1);
		}
		else
		{
			this.tryingToFetchLobbyDataAttempts = Optionable<int>.Some(this.tryingToFetchLobbyDataAttempts.Value + 1);
		}
		Debug.LogError(string.Format("Failed to get lobby region, attempts: {0}", this.tryingToFetchLobbyDataAttempts.Value));
		if (this.tryingToFetchLobbyDataAttempts.Value < 5)
		{
			this.LeaveLobby();
			this.TryJoinLobby(new CSteamID(param.m_ulSteamIDLobby));
			return;
		}
		Debug.LogError("Failed to fetch steam lobby");
		this.LeaveLobby();
		Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTJOINLOBBY_TITLE", true), LocalizedText.GetText("MODAL_INVALIDLOBBY_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
		{
			new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
		}), null);
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x00020285 File Offset: 0x0001E485
	public override void Update()
	{
		base.Update();
		if (this.m_currentlyWaitingForRoomID.IsSome && this.m_currentlyRequestingRoomID.IsNone)
		{
			this.RequestPhotonRoomID();
		}
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x000202B0 File Offset: 0x0001E4B0
	private void RequestPhotonRoomID()
	{
		if (this.m_currentLobby == CSteamID.Nil)
		{
			Debug.LogError("Not in a lobby");
			return;
		}
		this.m_currentlyRequestingRoomID = Optionable<CSteamID>.Some(this.m_currentLobby);
		BinarySerializer binarySerializer = new BinarySerializer(256, 2);
		binarySerializer.WriteByte(1);
		byte[] array = NativeArrayExtensions.ToByteArray(binarySerializer.buffer);
		binarySerializer.Dispose();
		if (!SteamMatchmaking.SendLobbyChatMsg(this.m_currentLobby, array, array.Length))
		{
			this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
			Debug.LogError("Failed to request Room ID");
			return;
		}
		Debug.Log("Requested Room ID");
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x00020340 File Offset: 0x0001E540
	private void SendRoomID()
	{
		if (this.m_currentLobby == CSteamID.Nil)
		{
			Debug.LogError("Not in a lobby");
			return;
		}
		BinarySerializer binarySerializer = new BinarySerializer(256, 2);
		binarySerializer.WriteByte(2);
		binarySerializer.WriteString(PhotonNetwork.CurrentRoom.Name, Encoding.ASCII);
		byte[] array = NativeArrayExtensions.ToByteArray(binarySerializer.buffer);
		binarySerializer.Dispose();
		if (!SteamMatchmaking.SendLobbyChatMsg(this.m_currentLobby, array, array.Length))
		{
			this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
			Debug.LogError("Failed to send Room ID...");
			return;
		}
		Debug.Log("Lobby has been requested. Sending " + PhotonNetwork.CurrentRoom.Name);
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x000203E4 File Offset: 0x0001E5E4
	private void OnLobbyChat(LobbyChatMsg_t param)
	{
		if (param.m_ulSteamIDLobby != this.m_currentLobby.m_SteamID)
		{
			Debug.LogError(string.Format("Received Chat Message from another lobby: {0}", param.m_ulSteamIDLobby));
			return;
		}
		if (param.m_ulSteamIDUser == SteamUser.GetSteamID().m_SteamID)
		{
			Debug.Log("Ignoring local chat message");
			return;
		}
		byte[] array = new byte[1024];
		CSteamID csteamID;
		EChatEntryType echatEntryType;
		if (SteamMatchmaking.GetLobbyChatEntry(this.m_currentLobby, (int)param.m_iChatID, ref csteamID, array, array.Length, ref echatEntryType) <= 0)
		{
			Debug.LogError("Failed to get chat message, no bytes written");
			return;
		}
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(ArrayExtensions.ToNativeArray<byte>(array, 2));
		SteamLobbyHandler.MessageType messageType = (SteamLobbyHandler.MessageType)binaryDeserializer.ReadByte();
		if (messageType == SteamLobbyHandler.MessageType.INVALID)
		{
			Debug.LogError("Invalid message type");
		}
		else
		{
			this.HandleMessage(messageType, binaryDeserializer, new CSteamID(param.m_ulSteamIDLobby));
		}
		binaryDeserializer.Dispose();
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x000204B0 File Offset: 0x0001E6B0
	private void HandleMessage(SteamLobbyHandler.MessageType messageType, BinaryDeserializer deserializer, CSteamID lobbyID)
	{
		if (messageType == SteamLobbyHandler.MessageType.RequestRoomID)
		{
			if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
			{
				this.SendRoomID();
				return;
			}
		}
		else if (messageType == SteamLobbyHandler.MessageType.RoomID)
		{
			if (this.m_currentlyRequestingRoomID.IsNone)
			{
				Debug.LogError("Not requesting room id, ignoring...");
				return;
			}
			if (this.m_currentlyRequestingRoomID.IsSome && this.m_currentlyRequestingRoomID.Value != lobbyID)
			{
				Debug.LogError("Got room id for wrong lobby");
				return;
			}
			string text = deserializer.ReadString(Encoding.ASCII);
			ValueTuple<CSteamID, string, string> value = this.m_currentlyWaitingForRoomID.Value;
			string text2 = value.Item2;
			string item = value.Item3;
			this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
			this.m_currentlyWaitingForRoomID = Optionable<ValueTuple<CSteamID, string, string>>.None;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "Airport";
				Debug.LogError("Failed to get scene to load, defaulting to airport");
			}
			JoinSpecificRoomState joinSpecificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false);
			joinSpecificRoomState.RoomName = text;
			joinSpecificRoomState.RegionToJoin = item;
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(text2, false, true, 3f) });
		}
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x000205BC File Offset: 0x0001E7BC
	internal void RequestLobbyData(CSteamID lobbyId)
	{
		if (SteamMatchmaking.RequestLobbyData(lobbyId))
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.Some(lobbyId);
			return;
		}
		Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTFINDLOBBY_TITLE", true), LocalizedText.GetText("MODAL_FAILEDTOFETCH_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
		{
			new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
		}), null);
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00020620 File Offset: 0x0001E820
	private void OnLobbyDataUpdate(LobbyDataUpdate_t param)
	{
		if (param.m_bSuccess != 1)
		{
			Debug.LogError("Failed to fetch lobby data");
			Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTFINDLOBBY_TITLE", true), LocalizedText.GetText("MODAL_CANTFINDLOBBY_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
			{
				new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
			}), null);
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.None;
			return;
		}
		if (!this.m_currentlyFetchingGameVersion.IsSome || this.m_currentlyFetchingGameVersion.Value.m_SteamID != param.m_ulSteamIDLobby)
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.None;
			return;
		}
		string lobbyData = SteamMatchmaking.GetLobbyData(this.m_currentlyFetchingGameVersion.Value, "PeakVersion");
		if (lobbyData == new BuildVersion(Application.version, "???").ToMatchmaking())
		{
			if (PhotonNetwork.InRoom)
			{
				Debug.LogError("Not joining invite because your already in a room...");
				return;
			}
			this.JoinLobby(this.m_currentlyFetchingGameVersion.Value);
		}
		else
		{
			Debug.LogError("Game version mismatch: " + lobbyData);
			string text = LocalizedText.GetText("MODAL_MISMATCH_BODY", true).Replace("#", lobbyData).Replace("&", new BuildVersion(Application.version, "???").ToMatchmaking());
			Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_MISMATCH_TITLE", true), text), new ModalButtonsOption(new ModalButtonsOption.Option[]
			{
				new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
			}), null);
		}
		if (this.m_currentlyFetchingGameVersion.IsSome)
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.None;
		}
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x000207AD File Offset: 0x0001E9AD
	private void JoinLobby(CSteamID lobbyID)
	{
		this.LeaveLobby();
		Debug.Log(string.Format("Joining lobby: {0}", lobbyID));
		SteamMatchmaking.JoinLobby(lobbyID);
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x000207D4 File Offset: 0x0001E9D4
	public void TryJoinLobby(CSteamID lobbyID)
	{
		if (SteamMatchmaking.RequestLobbyData(lobbyID))
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.Some(lobbyID);
			return;
		}
		Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTJOINLOBBY_TITLE", true), LocalizedText.GetText("MODAL_FAILEDTOFETCH_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
		{
			new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
		}), null);
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x00020838 File Offset: 0x0001EA38
	private string GenerateRoomName()
	{
		if (!Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "Player1"))
		{
			return Guid.NewGuid().ToString();
		}
		return Environment.MachineName;
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x00020870 File Offset: 0x0001EA70
	private void OnLobbyCreated(LobbyCreated_t param)
	{
		this.m_isHosting = true;
		if (param.m_eResult != 1)
		{
			Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTCREATELOBBY_TITLE", true), string.Format("{0}", param.m_eResult)), new ModalButtonsOption(new ModalButtonsOption.Option[]
			{
				new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
			}), null);
			return;
		}
		Debug.Log(string.Format("Lobby Created: {0}", param.m_ulSteamIDLobby));
		this.m_currentLobby = new CSteamID(param.m_ulSteamIDLobby);
		if (!SteamMatchmaking.SetLobbyData(this.m_currentLobby, "PeakVersion", new BuildVersion(Application.version, "???").ToMatchmaking()))
		{
			Debug.LogError("Failed to assign game version to lobby");
		}
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<HostState>(false).RoomName = this.GenerateRoomName();
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f) });
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x0002097C File Offset: 0x0001EB7C
	public void SetLobbyData()
	{
		if (this.m_currentLobby == CSteamID.Nil)
		{
			Debug.LogError("Failed to set lobby data, no lobby joined...");
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			Debug.LogError("Failed to set Lobby data. not in a photon room");
			return;
		}
		if (SteamMatchmaking.SetLobbyData(this.m_currentLobby, "PhotonRegion", PhotonNetwork.CloudRegion))
		{
			Debug.Log("Set Photon Region to steam lobby data: " + PhotonNetwork.CloudRegion);
		}
		else
		{
			Debug.LogError("Failed to set lobby data, returned not okay...");
		}
		string name = SceneManager.GetActiveScene().name;
		if (SteamMatchmaking.SetLobbyData(this.m_currentLobby, "CurrentScene", name))
		{
			Debug.Log("Set current scene to: " + name);
			return;
		}
		Debug.LogError("Failed to set lobby data, returned not okay...");
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x00020A2C File Offset: 0x0001EC2C
	public void LeaveLobby()
	{
		this.m_currentlyWaitingForRoomID = Optionable<ValueTuple<CSteamID, string, string>>.None;
		this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
		if (this.m_currentLobby != CSteamID.Nil)
		{
			string text = "Leaving current lobby: ";
			CSteamID currentLobby = this.m_currentLobby;
			Debug.Log(text + currentLobby.ToString());
			SteamMatchmaking.LeaveLobby(this.m_currentLobby);
			this.m_currentLobby = CSteamID.Nil;
			return;
		}
		Debug.Log("Can't leave current lobby because not in a lobby");
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x00020AA5 File Offset: 0x0001ECA5
	public bool InSteamLobby()
	{
		return this.m_currentLobby != CSteamID.Nil;
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x00020AB7 File Offset: 0x0001ECB7
	public bool InSteamLobby(out CSteamID lobbyID)
	{
		lobbyID = this.m_currentLobby;
		return this.m_currentLobby != CSteamID.Nil;
	}

	// Token: 0x040005B7 RID: 1463
	private const string PHOTON_REGION_KEY = "PhotonRegion";

	// Token: 0x040005B8 RID: 1464
	private const string GAME_VERSION_KEY = "PeakVersion";

	// Token: 0x040005B9 RID: 1465
	private const string CURRENT_SCENE_KEY = "CurrentScene";

	// Token: 0x040005BA RID: 1466
	private bool m_isHosting;

	// Token: 0x040005BB RID: 1467
	private CSteamID m_currentLobby;

	// Token: 0x040005BC RID: 1468
	private Optionable<CSteamID> m_currentlyFetchingGameVersion;

	// Token: 0x040005BD RID: 1469
	private Optionable<CSteamID> m_currentlyRequestingRoomID;

	// Token: 0x040005BE RID: 1470
	private Optionable<ValueTuple<CSteamID, string, string>> m_currentlyWaitingForRoomID;

	// Token: 0x040005C0 RID: 1472
	private Optionable<int> tryingToFetchLobbyDataAttempts = Optionable<int>.None;

	// Token: 0x02000422 RID: 1058
	public enum MessageType : byte
	{
		// Token: 0x040017C3 RID: 6083
		INVALID,
		// Token: 0x040017C4 RID: 6084
		RequestRoomID,
		// Token: 0x040017C5 RID: 6085
		RoomID
	}
}
