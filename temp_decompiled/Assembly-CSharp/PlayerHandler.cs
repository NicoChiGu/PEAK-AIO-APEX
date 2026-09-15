using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200015B RID: 347
public class PlayerHandler : GameService, IDisposable
{
	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06000B1C RID: 2844 RVA: 0x0003B62A File Offset: 0x0003982A
	[Obsolete("Moved to static CharacterRegistered event. This will be deleted.")]
	public Action<Character> OnCharacterRegistered
	{
		get
		{
			return PlayerHandler.CharacterRegistered;
		}
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0003B634 File Offset: 0x00039834
	public PlayerHandler()
	{
		NetCode.Session.NetworkEventReceived += new Action<INetworkEventData>(this.OnNetworkEvent);
	}

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x06000B1E RID: 2846 RVA: 0x0003B689 File Offset: 0x00039889
	protected static PlayerHandler Instance
	{
		get
		{
			return GameHandler.GetService<PlayerHandler>();
		}
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0003B690 File Offset: 0x00039890
	public static List<Character> GetAllPlayerCharacters()
	{
		if (PlayerHandler.s_FrameCharactersLastCached == Time.frameCount)
		{
			return PlayerHandler.s_CharacterCache;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, Character> keyValuePair in PlayerHandler.Instance.m_playerCharacterLookup)
		{
			Player player;
			if (!PhotonNetwork.TryGetPlayer(keyValuePair.Key, ref player))
			{
				list.Add(keyValuePair.Key);
			}
			else if (player.IsInactive)
			{
				list.Add(keyValuePair.Key);
			}
			else if (keyValuePair.Value == null)
			{
				Debug.LogError(string.Format("{0}({1}) doesn't have a Character any more! ", player.NickName, player.ActorNumber) + "How'd they do that???");
				list.Add(keyValuePair.Key);
			}
		}
		foreach (int num in list)
		{
			PlayerHandler.Instance.m_playerCharacterLookup.Remove(num);
			Debug.Log(string.Format("Removing character for id {0} from list..", num));
		}
		PlayerHandler.s_CharacterCache = Enumerable.ToList<Character>(PlayerHandler.Instance.m_playerCharacterLookup.Values);
		PlayerHandler.s_FrameCharactersLastCached = Time.frameCount;
		return PlayerHandler.s_CharacterCache;
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0003B804 File Offset: 0x00039A04
	public static void RegisterPlayer(Player player)
	{
		PlayerHandler.Instance.RegisterPlayerImpl(player);
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0003B814 File Offset: 0x00039A14
	private void RegisterPlayerImpl(Player player)
	{
		PhotonView component = player.GetComponent<PhotonView>();
		int actorNumber = component.Owner.ActorNumber;
		Debug.Log(string.Format("Registering Player object for {0} : {1}", component.Owner.NickName, actorNumber));
		if (this.m_playerLookup.ContainsKey(actorNumber))
		{
			Player player2 = this.m_playerLookup[actorNumber];
			if (player2 == null)
			{
				Debug.LogWarning(string.Format("Found a null entry already in player lookup for actor #{0}. ", actorNumber) + "Cleaning it up.");
			}
			else
			{
				Debug.LogError("Already found player " + player2.name + " already in the lookup table. This shouldn't be possible! Replacing with " + player.name);
			}
			this.m_playerLookup.Remove(actorNumber);
		}
		this.m_playerLookup.Add(actorNumber, player);
		string userId = component.Owner.UserId;
		if (this.m_playerSessionData.ContainsKey(userId))
		{
			int actorNumber2 = this.m_playerSessionData[userId].ActorNumber;
			Debug.LogWarning(string.Format("Stale entry detected. Updating actor {0} to {1}", actorNumber2, actorNumber) + "\n (key: " + component.Owner.UserId + ")");
			this.m_playerSessionData[component.Owner.UserId].ActorNumber = actorNumber;
			return;
		}
		this.m_playerSessionData.Add(userId, new PlayerSessionData(userId, actorNumber));
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0003B966 File Offset: 0x00039B66
	private void OnNetworkEvent(INetworkEventData eventData)
	{
		if (eventData.EventCode == 18)
		{
			Debug.LogWarning("Kick event received!");
		}
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0003B97C File Offset: 0x00039B7C
	public static void RegisterCharacter(Character character)
	{
		PhotonView component = character.GetComponent<PhotonView>();
		if (PlayerHandler.Instance.m_playerCharacterLookup.ContainsKey(component.Owner.ActorNumber))
		{
			Debug.Log(string.Format("Overwriting character for {0}", component.Owner.ActorNumber));
			Character character2 = PlayerHandler.Instance.m_playerCharacterLookup[component.Owner.ActorNumber];
			if (character2 != null)
			{
				character2.gameObject.SetActive(false);
				Debug.LogError("Disabled Old Player....");
			}
			PlayerHandler.Instance.m_playerCharacterLookup.Remove(component.Owner.ActorNumber);
		}
		PlayerHandler.Instance.m_playerCharacterLookup.Add(component.Owner.ActorNumber, character);
		Debug.Log(string.Format("Registering Character object for {0} : {1}", component.Owner.NickName, component.Owner.ActorNumber));
		Action<Character> characterRegistered = PlayerHandler.CharacterRegistered;
		if (characterRegistered == null)
		{
			return;
		}
		characterRegistered.Invoke(character);
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x0003BA75 File Offset: 0x00039C75
	public static void Kick(int actorNumber)
	{
		if (!NetCode.Session.IsHost)
		{
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
			Debug.LogError("Kick button should not show up for a non-host!");
			return;
		}
		Player.localPlayer.StartCoroutine(PlayerHandler.KickRoutine(actorNumber));
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x0003BAAA File Offset: 0x00039CAA
	private static IEnumerator KickRoutine(int actorNumber)
	{
		if (!NetCode.Session.IsHost)
		{
			yield break;
		}
		Player player = PlayerHandler.GetPlayer(actorNumber);
		string playerId = player.GetUserId();
		PlayerHandler.Instance.m_playerSessionData[playerId].IsBanned = true;
		player.photonView.RPC("RPC_GetKicked", player.photonView.Owner, Array.Empty<object>());
		yield return new WaitForSeconds(0.25f);
		NetCode.Session.Kick(playerId);
		yield break;
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x0003BAB9 File Offset: 0x00039CB9
	public static Player GetPlayer(Player photonPlayer)
	{
		return CollectionExtensions.GetValueOrDefault<int, Player>(PlayerHandler.Instance.m_playerLookup, photonPlayer.ActorNumber);
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x0003BAD0 File Offset: 0x00039CD0
	public static Player GetPlayer(int actorNumber)
	{
		PlayerHandler instance = PlayerHandler.Instance;
		if (instance == null)
		{
			return null;
		}
		return CollectionExtensions.GetValueOrDefault<int, Player>(instance.m_playerLookup, actorNumber);
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x0003BAE8 File Offset: 0x00039CE8
	public static string GetUserId(int actorNumber)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber, false);
		if (player == null)
		{
			return null;
		}
		return player.UserId;
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0003BB04 File Offset: 0x00039D04
	public static Player GetPlayer(string userID)
	{
		PlayerSessionData playerSessionData = null;
		PlayerHandler instance = PlayerHandler.Instance;
		if (instance == null || !instance.m_playerSessionData.TryGetValue(userID, ref playerSessionData))
		{
			return null;
		}
		return CollectionExtensions.GetValueOrDefault<int, Player>(PlayerHandler.Instance.m_playerLookup, playerSessionData.ActorNumber);
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0003BB48 File Offset: 0x00039D48
	public static bool IsBanned(string id)
	{
		PlayerSessionData playerSessionData;
		return PlayerHandler.Instance.m_playerSessionData.TryGetValue(id, ref playerSessionData) && playerSessionData.IsBanned;
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x0003BB71 File Offset: 0x00039D71
	public static bool TryGetPlayer(int actorNumber, out Player player)
	{
		player = PlayerHandler.GetPlayer(actorNumber);
		return player != null;
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x0003BB83 File Offset: 0x00039D83
	public static Character GetPlayerCharacter(Player photonPlayer)
	{
		return CollectionExtensions.GetValueOrDefault<int, Character>(PlayerHandler.Instance.m_playerCharacterLookup, photonPlayer.ActorNumber);
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x0003BB9A File Offset: 0x00039D9A
	public static bool HasHadPlayerCharacter(Player photonPlayer)
	{
		return PlayerHandler.Instance.m_playerCharacterLookup.ContainsKey(photonPlayer.ActorNumber);
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x0003BBB4 File Offset: 0x00039DB4
	public static byte AssignMixerGroup(Character character)
	{
		for (byte b = 0; b < 4; b += 1)
		{
			if (!PlayerHandler.Instance.m_assignedVoiceGroups.ContainsKey(b) || !ClassExtensionsMethods.UnityObjectExists<Character>(PlayerHandler.Instance.m_assignedVoiceGroups[b]))
			{
				PlayerHandler.Instance.m_assignedVoiceGroups[b] = character;
				return b;
			}
		}
		return byte.MaxValue;
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x0003BC0F File Offset: 0x00039E0F
	public void Dispose()
	{
		Debug.Log("Disposing PlayerHandler");
		this._playerList.Dispose();
		PlayerHandler.CharacterRegistered = null;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0003BC2C File Offset: 0x00039E2C
	public static IEnumerable<Player> GetAllPlayers()
	{
		if (!PhotonNetwork.InRoom)
		{
			yield break;
		}
		Player[] array = ((PlayerHandler.Instance == null) ? PhotonNetwork.PlayerList : PlayerHandler.Instance._playerList.Get());
		foreach (Player player in array)
		{
			Player player2;
			if (!player.IsInactive && PlayerHandler.TryGetPlayer(player.ActorNumber, out player2))
			{
				yield return player2;
			}
		}
		Player[] array2 = null;
		yield break;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0003BC38 File Offset: 0x00039E38
	public static bool TryGetCharacter(int actorID, out Character character)
	{
		Player player;
		if (!PlayerHandler.TryGetPlayer(actorID, out player))
		{
			character = null;
			return false;
		}
		character = player.character;
		if (character == null)
		{
			Debug.LogWarning(string.Format("Null character entry found for {0} ({1})! ", player.name, actorID) + "Won't be cleaned up until the player registry is updated.");
			return false;
		}
		return true;
	}

	// Token: 0x04000A66 RID: 2662
	[Obsolete("This data is stored in m_playerSessionData now")]
	private Dictionary<string, int> m_userIdToActorNumber;

	// Token: 0x04000A67 RID: 2663
	private CachedPlayerList _playerList;

	// Token: 0x04000A68 RID: 2664
	private Dictionary<int, Player> m_playerLookup = new Dictionary<int, Player>();

	// Token: 0x04000A69 RID: 2665
	private Dictionary<string, PlayerSessionData> m_playerSessionData = new Dictionary<string, PlayerSessionData>();

	// Token: 0x04000A6A RID: 2666
	private Dictionary<int, Character> m_playerCharacterLookup = new Dictionary<int, Character>();

	// Token: 0x04000A6B RID: 2667
	private Dictionary<byte, Character> m_assignedVoiceGroups = new Dictionary<byte, Character>();

	// Token: 0x04000A6C RID: 2668
	public static Action<Character> CharacterRegistered;

	// Token: 0x04000A6D RID: 2669
	private static List<Character> s_CharacterCache;

	// Token: 0x04000A6E RID: 2670
	private static int s_FrameCharactersLastCached;
}
