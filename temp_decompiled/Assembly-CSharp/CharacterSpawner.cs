using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Peak.Dev;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000144 RID: 324
public class CharacterSpawner : MonoBehaviourPunCallbacks
{
	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000A6B RID: 2667 RVA: 0x000374D1 File Offset: 0x000356D1
	private bool CanSpawnCharacters
	{
		get
		{
			return !Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "NoCharacter") && !this.hasSpawnedPlayer;
		}
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x000374EF File Offset: 0x000356EF
	private SpawnPoint GetSpawnPoint(int actorNumber)
	{
		return SpawnPoint.GetSpawnPoint(actorNumber);
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x000374F8 File Offset: 0x000356F8
	private void Update()
	{
		if (!NetCode.Session.InRoom)
		{
			return;
		}
		if (NetCode.Session.IsHost)
		{
			this.<Update>g__HostUpdate|10_0();
		}
		else
		{
			CharacterSpawner.<Update>g__ClientUpdate|10_1();
		}
		foreach (Player player in this._playerList.Get())
		{
			Character character;
			if (!PlayerHandler.TryGetCharacter(player.ActorNumber, out character) && (!this.attemptedSpawns.ContainsKey(player.ActorNumber) || Time.realtimeSinceStartup - this.attemptedSpawns[player.ActorNumber] >= 2f))
			{
				this.attemptedSpawns[player.ActorNumber] = Time.realtimeSinceStartup;
				if (!player.IsLocal)
				{
					ReconnectData reconnectData;
					if (ReconnectHandler.TryGetReconnectData(player, out reconnectData) && GameHandler.IsOnIsland)
					{
						Debug.Log(string.Format("Reconnect data found for {0} ({1})! ", player.UserId, player.ActorNumber) + "Sending: " + Pretty.Print(reconnectData));
						bool flag = CharacterSpawner.ScoutsWereRevivedAtCurrentBaseCamp && reconnectData.lastRevivedSegment < MapHandler.BaseCampScoutStatue.SegmentNumber;
						base.photonView.RPC("RPC_ReconnectingPlayerSpawn", player, new object[]
						{
							MapHandler.CurrentSegmentNumber,
							reconnectData,
							flag,
							Singleton<MapHandler>.Instance.LastRevivedSegment
						});
					}
					else
					{
						Debug.Log(string.Format("No reconnect data found for {0} ({1}). Requesting fresh spawn.", player.UserId, player.ActorNumber));
						bool flag2 = false;
						if (GameHandler.IsOnIsland)
						{
							RespawnChest baseCampScoutStatue = MapHandler.BaseCampScoutStatue;
							flag2 = !MapHandler.CurrentBaseCampIsFogged && baseCampScoutStatue != null && MapHandler.BaseCampHasRevived;
						}
						base.photonView.RPC("RPC_NewPlayerSpawn", player, new object[]
						{
							GameHandler.IsOnIsland ? MapHandler.CurrentSegmentNumber : Segment.Beach,
							flag2,
							GameHandler.IsOnIsland ? Singleton<MapHandler>.Instance.LastRevivedSegment : int.MinValue
						});
					}
				}
			}
		}
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x00037718 File Offset: 0x00035918
	private void SpawnHostCharacter()
	{
		SpawnPoint spawnPoint = this.GetSpawnPoint(NetCode.Session.SeatNumber);
		Debug.Log("Attempting to spawn host's local character -- " + NetCode.Session.NickName + " " + string.Format("[Actor #{0}] (that's me)", NetCode.Session.SeatNumber));
		if (GameHandler.IsOnIsland)
		{
			if (!spawnPoint.startPassedOut)
			{
				Debug.LogWarning("Spawn point " + spawnPoint.name + " does NOT say to spawn passed out. Is it broken?", spawnPoint);
			}
			this.SpawnMyPlayerCharacter(spawnPoint.startPassedOut ? CharacterSpawner.SpawnFlavor.PassedOut : CharacterSpawner.SpawnFlavor.Instant, null, default(Vector3));
			return;
		}
		this.SpawnSelfInAirport();
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x000377BC File Offset: 0x000359BC
	private Character SpawnMyPlayerCharacter(CharacterSpawner.SpawnFlavor spawnFlavor = CharacterSpawner.SpawnFlavor.Instant, Transform spawnOverride = null, Vector3 spawnOffset = default(Vector3))
	{
		SceneSwitchingStatus sceneSwitchingStatus;
		if (GameHandler.TryGetStatus<SceneSwitchingStatus>(out sceneSwitchingStatus))
		{
			GameHandler.ClearStatus<SceneSwitchingStatus>();
		}
		Character character = Character.localCharacter;
		if (this.CanSpawnCharacters)
		{
			this.hasSpawnedPlayer = true;
			Transform transform = spawnOverride ?? this.GetSpawnPoint(NetCode.Session.SeatNumber).transform;
			Vector3 vector = transform.position + spawnOffset;
			Debug.Log(string.Format("Spawning myself ({0} [{1}]) at {2}! (Flavor: {3})", new object[]
			{
				NetCode.Session.NickName,
				NetCode.Session.SeatNumber,
				vector,
				spawnFlavor
			}));
			character = PhotonNetwork.Instantiate("Character", vector, transform.rotation, 0, null).GetComponent<Character>();
			character.data.spawnPoint = transform;
			if (spawnFlavor == CharacterSpawner.SpawnFlavor.PassedOut)
			{
				character.StartPassedOutOnTheBeach();
			}
			if (spawnFlavor == CharacterSpawner.SpawnFlavor.Poof)
			{
				character.PlayPoofVFX(vector);
			}
			if (spawnFlavor != CharacterSpawner.SpawnFlavor.Lobby)
			{
				Singleton<MountainProgressHandler>.Instance.CheckProgress(false);
			}
		}
		else if (!this.hasSpawnedPlayer)
		{
			Debug.Log("Skipping character spawn because of MPPM tags");
		}
		if (Player.localPlayer == null)
		{
			Debug.Log("Spawning local player object.");
			PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0, null);
		}
		else
		{
			Debug.LogWarning("Local player (" + Player.localPlayer.name + ") already exists?? That seems bad.", Player.localPlayer);
		}
		return character;
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x00037910 File Offset: 0x00035B10
	private static void KillImmediately(Character self)
	{
		Debug.Log("Killing " + self.name + " now that they're spawned.");
		self.photonView.RPC("RPCA_Die", 0, new object[] { Character.localCharacter.Center });
		Character.localCharacter.data.deathTimer = 5f;
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x00037974 File Offset: 0x00035B74
	private static void DeathOnArrival()
	{
		if (Character.localCharacter == null)
		{
			PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Combine(PlayerHandler.CharacterRegistered, new Action<Character>(CharacterSpawner.<DeathOnArrival>g__WaitThenDie|14_0));
			return;
		}
		CharacterSpawner.KillImmediately(Character.localCharacter);
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000A72 RID: 2674 RVA: 0x000379AE File Offset: 0x00035BAE
	private static Vector3 CurrentRevivePoint
	{
		get
		{
			return MapHandler.CurrentBaseCampSpawnPoint.position;
		}
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x000379BC File Offset: 0x00035BBC
	private void ReviveBeforeSpawn(ReconnectData reconnectData)
	{
		reconnectData.dead = false;
		reconnectData.deathTimer = 0f;
		reconnectData.fullyPassedOut = false;
		int num = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length;
		for (int i = 0; i < num; i++)
		{
			CharacterAfflictions.STATUSTYPE statustype = (CharacterAfflictions.STATUSTYPE)i;
			if (statustype != CharacterAfflictions.STATUSTYPE.Weight && statustype != CharacterAfflictions.STATUSTYPE.Crab && statustype != CharacterAfflictions.STATUSTYPE.Curse)
			{
				reconnectData.currentStatuses[i] = 0f;
			}
		}
		Character character = this.SpawnSelfAtSpecificPosition(CharacterSpawner.SpawnFlavor.Poof, CharacterSpawner.CurrentRevivePoint + CharacterSpawner.RandomBaseCampOffset);
		CharacterSpawner.PushReconnectData(character, reconnectData);
		Character.ApplyPostReviveStatus(character.refs.afflictions);
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000A74 RID: 2676 RVA: 0x00037A4C File Offset: 0x00035C4C
	private static Vector3 RandomBaseCampOffset
	{
		get
		{
			Vector2 vector = 2f * Random.insideUnitCircle.normalized;
			return new Vector3(vector.x, 4f, vector.y);
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x06000A75 RID: 2677 RVA: 0x00037A88 File Offset: 0x00035C88
	private static bool ScoutsWereRevivedAtCurrentBaseCamp
	{
		get
		{
			switch (Singleton<MapHandler>.Instance.GetCurrentSegment())
			{
			case Segment.Beach:
			case Segment.Tropics:
			case Segment.Alpine:
			case Segment.Caldera:
				return MapHandler.BaseCampHasRevived;
			case Segment.TheKiln:
				return !Singleton<LavaRising>.Instance.started && MapHandler.BaseCampHasRevived;
			case Segment.Peak:
				return false;
			default:
				Debug.LogError("uh oh! We added more Segments to our game without telling me how to handle them");
				return false;
			}
		}
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00037AE8 File Offset: 0x00035CE8
	private void SpawnSelfInAirport()
	{
		Debug.Log("Spawning local character in airport.");
		this.SpawnMyPlayerCharacter(CharacterSpawner.SpawnFlavor.Lobby, null, default(Vector3));
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x00037B14 File Offset: 0x00035D14
	private void SpawnSelfOnShore()
	{
		Debug.Log("Spawning fresh on the shore. Game hasn't started");
		this.SpawnMyPlayerCharacter(CharacterSpawner.SpawnFlavor.PassedOut, null, default(Vector3));
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00037B40 File Offset: 0x00035D40
	private void SpawnDeadAtBaseCamp(bool andRevive)
	{
		CharacterSpawner.<>c__DisplayClass24_0 CS$<>8__locals1 = new CharacterSpawner.<>c__DisplayClass24_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.andRevive = andRevive;
		base.StartCoroutine(CS$<>8__locals1.<SpawnDeadAtBaseCamp>g__LateConnectRoutine|0());
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x00037B70 File Offset: 0x00035D70
	private Character SpawnSelfAtSpecificPosition(CharacterSpawner.SpawnFlavor spawnFlavor, Vector3 position)
	{
		Vector3 vector = position - SpawnPoint.LocalSpawnPoint.transform.position;
		return this.SpawnMyPlayerCharacter(spawnFlavor, SpawnPoint.LocalSpawnPoint.transform, vector);
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x00037BA8 File Offset: 0x00035DA8
	private static void PushReconnectData(Character self, ReconnectData data)
	{
		Debug.Log("Restoring state from ReconnectData: " + Pretty.Print(data));
		self.refs.afflictions.ApplyReconnectData(data);
		self.player.photonView.RPC("SyncInventoryRPC", 0, new object[]
		{
			IBinarySerializable.ToManagedArray<InventorySyncData>(data.inventorySyncData),
			true
		});
		if (data.inventorySyncData.tempSlot.ItemID != 65535)
		{
			self.refs.items.EquipSlot(Optionable<byte>.Some(250));
		}
		if (data.dead)
		{
			self.SetDeadAfterReconnect(data.position);
		}
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x00037C58 File Offset: 0x00035E58
	[PunRPC]
	public void RPC_ReconnectingPlayerSpawn(Segment currentMapStage, ReconnectData reconnectData, bool canRevive, int lastReviveSegment, PhotonMessageInfo info)
	{
		CharacterSpawner.<>c__DisplayClass27_0 CS$<>8__locals1 = new CharacterSpawner.<>c__DisplayClass27_0();
		CS$<>8__locals1.currentMapStage = currentMapStage;
		CS$<>8__locals1.lastReviveSegment = lastReviveSegment;
		CS$<>8__locals1.reconnectData = reconnectData;
		CS$<>8__locals1.canRevive = canRevive;
		CS$<>8__locals1.<>4__this = this;
		if (info.Sender.ActorNumber != NetCode.Session.HostId)
		{
			return;
		}
		if (NetCode.Session.IsHost || !GameHandler.IsOnIsland || !CS$<>8__locals1.reconnectData.isValid)
		{
			Debug.LogError("What da heck! This shouldn't happen.");
		}
		if (this.hasSpawnedPlayer)
		{
			Debug.LogWarning("Ignoring spawn request because we already spawned this player.");
			return;
		}
		base.StartCoroutine(CS$<>8__locals1.<RPC_ReconnectingPlayerSpawn>g__ReconnectRoutine|0());
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x00037CF2 File Offset: 0x00035EF2
	private static IEnumerator WaitUntilCharacterInitialized()
	{
		while (Character.localCharacter == null)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x00037CFC File Offset: 0x00035EFC
	[PunRPC]
	public void RPC_NewPlayerSpawn(Segment currentMapStage, bool canRevive, int lastReviveSegment, PhotonMessageInfo info)
	{
		CharacterSpawner.<>c__DisplayClass29_0 CS$<>8__locals1 = new CharacterSpawner.<>c__DisplayClass29_0();
		CS$<>8__locals1.currentMapStage = currentMapStage;
		CS$<>8__locals1.lastReviveSegment = lastReviveSegment;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.canRevive = canRevive;
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (NetCode.Session.IsHost)
		{
			Debug.LogError("What da heck! I should have already spawned myself and not gone through this code path.");
		}
		if (this.hasSpawnedPlayer)
		{
			Debug.LogWarning("Ignoring spawn request because we already spawned this player.");
			return;
		}
		base.StartCoroutine(CS$<>8__locals1.<RPC_NewPlayerSpawn>g__NewPlayerRoutine|0());
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00037D70 File Offset: 0x00035F70
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		GameHandler.GetService<RichPresenceService>().Dirty();
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x00037D83 File Offset: 0x00035F83
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		GameHandler.GetService<RichPresenceService>().Dirty();
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x00037DAC File Offset: 0x00035FAC
	[CompilerGenerated]
	private void <Update>g__HostUpdate|10_0()
	{
		Character character;
		if (!PlayerHandler.TryGetCharacter(NetCode.Session.SeatNumber, out character))
		{
			this.SpawnHostCharacter();
			return;
		}
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x00037DD3 File Offset: 0x00035FD3
	[CompilerGenerated]
	internal static void <Update>g__ClientUpdate|10_1()
	{
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x00037DD5 File Offset: 0x00035FD5
	[CompilerGenerated]
	internal static void <DeathOnArrival>g__WaitThenDie|14_0(Character registeredCharacter)
	{
		if (!registeredCharacter.photonView.IsMine)
		{
			return;
		}
		Debug.Log("Player was registered and needs to die!");
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Remove(PlayerHandler.CharacterRegistered, new Action<Character>(CharacterSpawner.<DeathOnArrival>g__WaitThenDie|14_0));
		CharacterSpawner.KillImmediately(registeredCharacter);
	}

	// Token: 0x040009D5 RID: 2517
	private CachedPlayerList _playerList;

	// Token: 0x040009D6 RID: 2518
	private const float SpawnAttemptCooldown = 2f;

	// Token: 0x040009D7 RID: 2519
	public Item[] itemsToSpawnWith;

	// Token: 0x040009D8 RID: 2520
	private float _timeLastLocalSpawnStarted;

	// Token: 0x040009D9 RID: 2521
	private bool hasSpawnedPlayer;

	// Token: 0x040009DA RID: 2522
	private Dictionary<int, float> attemptedSpawns = new Dictionary<int, float>();

	// Token: 0x0200046A RID: 1130
	public enum SpawnFlavor
	{
		// Token: 0x04001911 RID: 6417
		Instant,
		// Token: 0x04001912 RID: 6418
		PassedOut,
		// Token: 0x04001913 RID: 6419
		Poof,
		// Token: 0x04001914 RID: 6420
		Lobby
	}
}
