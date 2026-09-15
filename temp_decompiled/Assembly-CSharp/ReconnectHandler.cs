using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Peak.Dev;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using Zorro.Core;

// Token: 0x02000163 RID: 355
public class ReconnectHandler : Singleton<ReconnectHandler>, IInRoomCallbacks
{
	// Token: 0x170000CF RID: 207
	// (get) Token: 0x06000B45 RID: 2885 RVA: 0x0003C11C File Offset: 0x0003A31C
	internal IReadOnlyDictionary<string, ReconnectHandler.ReconnectDataRecord> Records
	{
		get
		{
			return this._reconnectRecords;
		}
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x0003C124 File Offset: 0x0003A324
	protected override void Awake()
	{
		base.Awake();
		this.photonView = base.GetComponent<PhotonView>();
		this._cachedPlayers = default(CachedPlayerList);
		this._reconnectRecords = new Dictionary<string, ReconnectHandler.ReconnectDataRecord>();
		this._observedActors = new HashSet<int>();
		PhotonNetwork.AddCallbackTarget(this);
		SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded);
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0003C17C File Offset: 0x0003A37C
	private void OnSceneLoaded(Scene arg0, LoadSceneMode _)
	{
		if (!GameHandler.IsOnIsland)
		{
			Debug.Log("Non-gameplay scene detected. Clearing out old reconnectdata.");
			this.Clear();
		}
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0003C195 File Offset: 0x0003A395
	private void Start()
	{
		this.Clear();
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x0003C19D File Offset: 0x0003A39D
	private void Clear()
	{
		this._observedActors.Clear();
		this._reconnectRecords.Clear();
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0003C1B5 File Offset: 0x0003A3B5
	public override void OnDestroy()
	{
		base.OnDestroy();
		PhotonNetwork.RemoveCallbackTarget(this);
		this._cachedPlayers.Dispose();
		SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded);
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x0003C1E0 File Offset: 0x0003A3E0
	private void Update()
	{
		Character character;
		if (!PhotonNetwork.InRoom || !PlayerHandler.TryGetCharacter(PhotonNetwork.LocalPlayer.ActorNumber, out character))
		{
			return;
		}
		foreach (Player player in PlayerHandler.GetAllPlayers())
		{
			bool flag = this._observedActors.Contains(player.GetActorNumber());
			Player player2;
			bool flag2 = Singleton<MapHandler>.Instance != null && PlayerHandler.TryGetPlayer(player.GetActorNumber(), out player2);
			if (!flag && flag2)
			{
				this.RegisterPlayer(player);
			}
			else if (flag)
			{
				ReconnectHandler.ReconnectDataRecord reconnectDataRecord = this._reconnectRecords[player.GetUserId()];
				if (PhotonNetwork.ServerTimestamp - reconnectDataRecord.Timestamp < 1000)
				{
					this.UpdateReconnectData(player.character);
				}
			}
		}
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x0003C2B8 File Offset: 0x0003A4B8
	private void RegisterPlayer(Player player)
	{
		string userId = player.GetUserId();
		ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
		if (this._reconnectRecords.TryGetValue(userId, ref reconnectDataRecord))
		{
			Debug.Log("Already have reconnect record for " + player.name + " so no need to make a new one now:\n" + Pretty.Print(reconnectDataRecord));
		}
		else
		{
			ReconnectHandler.ReconnectDataRecord reconnectDataRecord2 = new ReconnectHandler.ReconnectDataRecord(userId, ReconnectData.CreateFromCharacter(player.character));
			if (!reconnectDataRecord2.Data.isValid)
			{
				Debug.LogWarning("Uh oh! Initial reconnectdata was not valid! This will cause problems if not overwritten.\n" + Pretty.Print(reconnectDataRecord2.Data));
			}
			this._reconnectRecords.Add(userId, reconnectDataRecord2);
		}
		player.character.data.CharacterStateUpdated += delegate
		{
			this.UpdateReconnectData(player.character);
		};
		player.character.refs.afflictions.StatusesUpdated += new Action<Character>(this.UpdateReconnectData);
		Player player2 = player;
		player2.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Combine(player2.itemsChangedAction, delegate(ItemSlot[] _)
		{
			this.UpdateReconnectData(player.character);
		});
		Debug.Log(string.Concat(new string[]
		{
			"Registered ",
			player.name,
			". Most recent data: ",
			this._reconnectRecords[userId].ToPrettyString(),
			"\nKey: ",
			userId
		}));
		this._observedActors.Add(player.GetActorNumber());
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0003C44C File Offset: 0x0003A64C
	public void UpdateReconnectData(Character character)
	{
		ReconnectHandler.ReconnectDataRecord reconnectDataRecord = new ReconnectHandler.ReconnectDataRecord(character.player.GetUserId(), ReconnectData.CreateFromCharacter(character));
		if (!reconnectDataRecord.Data.isValid)
		{
			return;
		}
		this._reconnectRecords[character.player.GetUserId()] = reconnectDataRecord;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0003C498 File Offset: 0x0003A698
	public void UpdateFromRevive(Character character, Vector3 revivePosition)
	{
		ReconnectData reconnectData = ReconnectData.CreateFromCharacter(character);
		if (reconnectData.dead || reconnectData.isValid || reconnectData.fullyPassedOut)
		{
			Debug.LogError("Post-revive reconnectdata came out malformed!:\n " + Pretty.Print(reconnectData));
		}
		reconnectData.position = revivePosition;
		reconnectData.isValid = true;
		string userId = character.player.GetUserId();
		this._reconnectRecords[userId] = new ReconnectHandler.ReconnectDataRecord(userId, reconnectData);
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x0003C510 File Offset: 0x0003A710
	[PunRPC]
	public void RefreshReconnectDataTable(string key, ReconnectData data)
	{
		if (Ext.IsNullOrEmpty(key))
		{
			Debug.LogError("Received ReconnectData with no key. Can't do anything!");
			return;
		}
		if (this._reconnectRecords.ContainsKey(key))
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"Already have reconnect data for ",
				key,
				": ",
				Pretty.Print(this._reconnectRecords[key].Data),
				"\n Discarding new data: ",
				Pretty.Print(this._reconnectRecords[key].Data)
			}));
			return;
		}
		if (key == PhotonNetwork.LocalPlayer.UserId)
		{
			Debug.Log("Ignoring reconnect data for " + key + " because that's me (and I'll get it when my character spawns).\n" + Pretty.Print(data));
			return;
		}
		Player player = PlayerHandler.GetPlayer(key);
		string text;
		if (player == null)
		{
			text = null;
		}
		else
		{
			Character character = player.character;
			text = ((character != null) ? character.characterName : null);
		}
		string text2 = text;
		text2 = (Ext.IsNullOrEmpty(text2) ? "UNKNOWN" : text2);
		Debug.Log(string.Concat(new string[]
		{
			"ReconnectData for ",
			text2,
			": ",
			Pretty.Print(data),
			"\nSetting for ID ",
			key,
			"."
		}));
		this._reconnectRecords.Add(key, new ReconnectHandler.ReconnectDataRecord(key, data));
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x0003C664 File Offset: 0x0003A864
	private void PrintReconnectDataIfExists(string msgPrefix, string id)
	{
		ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
		if (this._reconnectRecords.TryGetValue(id, ref reconnectDataRecord))
		{
			Debug.Log(msgPrefix + Pretty.Print(reconnectDataRecord));
			return;
		}
		Debug.Log(msgPrefix + "No reconnect record found.");
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x0003C6A8 File Offset: 0x0003A8A8
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (PlayerHandler.IsBanned(newPlayer.UserId))
		{
			Debug.LogWarning("Banned player just joined room!!");
		}
		this.PrintReconnectDataIfExists(string.Format("Player {0} ({1}) just entered.\n", newPlayer.UserId, newPlayer.ActorNumber), newPlayer.UserId);
		this.BroadcastMissingPlayerDataIfHost(newPlayer);
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0003C6FA File Offset: 0x0003A8FA
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.PrintReconnectDataIfExists(string.Format("Player {0} ({1}) just left.\n", otherPlayer.UserId, otherPlayer.ActorNumber), otherPlayer.UserId);
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0003C724 File Offset: 0x0003A924
	private void BroadcastMissingPlayerDataIfHost(Player newPlayer)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		string[] currentPlayerIds = Enumerable.ToArray<string>(Enumerable.Select<Player, string>(PlayerHandler.GetAllPlayers(), (Player p) => p.GetUserId()));
		IEnumerable<KeyValuePair<string, ReconnectHandler.ReconnectDataRecord>> reconnectRecords = this._reconnectRecords;
		Func<KeyValuePair<string, ReconnectHandler.ReconnectDataRecord>, bool> <>9__1;
		Func<KeyValuePair<string, ReconnectHandler.ReconnectDataRecord>, bool> func;
		if ((func = <>9__1) == null)
		{
			func = (<>9__1 = (KeyValuePair<string, ReconnectHandler.ReconnectDataRecord> record) => !Enumerable.Contains<string>(currentPlayerIds, record.Key) && newPlayer.UserId != record.Key);
		}
		foreach (KeyValuePair<string, ReconnectHandler.ReconnectDataRecord> keyValuePair in Enumerable.Where<KeyValuePair<string, ReconnectHandler.ReconnectDataRecord>>(reconnectRecords, func))
		{
			string text;
			ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
			keyValuePair.Deconstruct(ref text, ref reconnectDataRecord);
			string text2 = text;
			ReconnectHandler.ReconnectDataRecord reconnectDataRecord2 = reconnectDataRecord;
			Debug.Log("Sending " + text2 + ": " + Pretty.Print(reconnectDataRecord2.Data));
			this.photonView.RPC("RefreshReconnectDataTable", newPlayer, new object[] { text2, reconnectDataRecord2.Data });
		}
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x0003C840 File Offset: 0x0003AA40
	public static bool TryGetReconnectData(Player player, out ReconnectData data)
	{
		data = ((Singleton<ReconnectHandler>.Instance == null) ? ReconnectData.Invalid : Singleton<ReconnectHandler>.Instance.GetReconnectData(player));
		return data.isValid;
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x0003C870 File Offset: 0x0003AA70
	public ReconnectData GetReconnectData(Player player)
	{
		ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
		if (!this._reconnectRecords.TryGetValue(player.UserId, ref reconnectDataRecord))
		{
			return ReconnectData.Invalid;
		}
		return reconnectDataRecord.Data;
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x0003C89E File Offset: 0x0003AA9E
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x0003C8A0 File Offset: 0x0003AAA0
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x0003C8A2 File Offset: 0x0003AAA2
	public void OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x04000A7D RID: 2685
	private const int k_UpdatePeriodInMilliseconds = 1000;

	// Token: 0x04000A7E RID: 2686
	private CachedPlayerList _cachedPlayers;

	// Token: 0x04000A7F RID: 2687
	private Dictionary<string, ReconnectHandler.ReconnectDataRecord> _reconnectRecords;

	// Token: 0x04000A80 RID: 2688
	private HashSet<int> _observedActors;

	// Token: 0x04000A81 RID: 2689
	private PhotonView photonView;

	// Token: 0x0200047B RID: 1147
	internal struct ReconnectDataRecord : IPrettyPrintable
	{
		// Token: 0x06001B6B RID: 7019 RVA: 0x00082718 File Offset: 0x00080918
		public string ToPrettyString()
		{
			return Pretty.Print(this.Data) + string.Format("\n(Time: {0} | ID: {1})", this.Timestamp, this.UserID);
		}

		// Token: 0x06001B6C RID: 7020 RVA: 0x0008274A File Offset: 0x0008094A
		internal ReconnectDataRecord(string userId, ReconnectData data)
		{
			this.Timestamp = PhotonNetwork.ServerTimestamp;
			this.UserID = userId;
			this.Data = data;
		}

		// Token: 0x0400194B RID: 6475
		internal readonly int Timestamp;

		// Token: 0x0400194C RID: 6476
		internal readonly string UserID;

		// Token: 0x0400194D RID: 6477
		internal readonly ReconnectData Data;
	}
}
