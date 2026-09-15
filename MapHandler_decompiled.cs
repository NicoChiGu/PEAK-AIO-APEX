using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Peak.Network;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.PhotonUtility;

// Token: 0x02000139 RID: 313
public class MapHandler : Singleton<MapHandler>
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060009F2 RID: 2546 RVA: 0x00034C1C File Offset: 0x00032E1C
	// (set) Token: 0x060009F3 RID: 2547 RVA: 0x00034C24 File Offset: 0x00032E24
	public int LastRevivedSegment
	{
		get
		{
			return this._lastRevivedSegment;
		}
		private set
		{
			Debug.Log(string.Format("Revive segment updated to {0}", value));
			this._lastRevivedSegment = value;
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x060009F4 RID: 2548 RVA: 0x00034C42 File Offset: 0x00032E42
	public static bool Exists
	{
		get
		{
			return Singleton<MapHandler>.Instance != null;
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x060009F5 RID: 2549 RVA: 0x00034C4F File Offset: 0x00032E4F
	public static MapHandler.MapSegment CurrentMapSegment
	{
		get
		{
			return Singleton<MapHandler>.Instance.segments[Singleton<MapHandler>.Instance.currentSegment];
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x060009F6 RID: 2550 RVA: 0x00034C68 File Offset: 0x00032E68
	public static bool CurrentBaseCampIsFogged
	{
		get
		{
			if (Singleton<MapHandler>.Instance.currentSegment < 4)
			{
				string text;
				return OrbFogHandler.IsFoggingCurrentSegment && !MapHandler.CurrentCampfire.EveryoneInRange(out text, 50f);
			}
			return Singleton<LavaRising>.Instance.started;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x060009F7 RID: 2551 RVA: 0x00034CAC File Offset: 0x00032EAC
	private static bool PreviousSegmentIsStillBaseCamp
	{
		get
		{
			string text;
			return Singleton<MapHandler>.Instance.currentSegment == 4 || (Singleton<MapHandler>.Instance.LastRevivedSegment != Singleton<MapHandler>.Instance.currentSegment && !OrbFogHandler.IsFoggingCurrentSegment && !MapHandler.CurrentCampfire.EveryoneInRange(out text, 50f));
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x060009F8 RID: 2552 RVA: 0x00034CFB File Offset: 0x00032EFB
	public static RespawnChest BaseCampScoutStatue
	{
		get
		{
			if (!MapHandler.PreviousSegmentIsStillBaseCamp)
			{
				return MapHandler.CurrentScoutStatue;
			}
			return MapHandler.PreviousScoutStatue;
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x060009F9 RID: 2553 RVA: 0x00034D0F File Offset: 0x00032F0F
	public static bool BaseCampHasRevived
	{
		get
		{
			if (!MapHandler.PreviousSegmentIsStillBaseCamp)
			{
				return Singleton<MapHandler>.Instance.currentSegment == Singleton<MapHandler>.Instance.LastRevivedSegment;
			}
			return Singleton<MapHandler>.Instance.LastRevivedSegment == Singleton<MapHandler>.Instance.currentSegment - 1;
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x060009FA RID: 2554 RVA: 0x00034D47 File Offset: 0x00032F47
	public static Transform CurrentBaseCampSpawnPoint
	{
		get
		{
			if (!MapHandler.PreviousSegmentIsStillBaseCamp)
			{
				return MapHandler.CurrentCampfire.transform;
			}
			if (Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.Beach)
			{
				return MapHandler.PreviousCampfire.transform;
			}
			return SpawnPoint.LocalSpawnPoint.transform;
		}
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x00034D7C File Offset: 0x00032F7C
	private static GameObject GetCampfireRoot(int segmentIndex)
	{
		if (segmentIndex >= 0)
		{
			return Singleton<MapHandler>.Instance.segments[segmentIndex].segmentCampfire;
		}
		return null;
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x060009FC RID: 2556 RVA: 0x00034D95 File Offset: 0x00032F95
	public static Campfire PreviousCampfire
	{
		get
		{
			if (Singleton<MapHandler>.Instance.currentSegment != 0)
			{
				return MapHandler.GetCampfireRoot(Singleton<MapHandler>.Instance.currentSegment - 1).GetComponentInChildren<Campfire>(true);
			}
			return null;
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x060009FD RID: 2557 RVA: 0x00034DBC File Offset: 0x00032FBC
	public static RespawnChest PreviousScoutStatue
	{
		get
		{
			if (Singleton<MapHandler>.Instance.currentSegment == 0)
			{
				return null;
			}
			if (Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.TheKiln)
			{
				return MapHandler.GetCampfireRoot(Singleton<MapHandler>.Instance.currentSegment - 1).GetComponentInChildren<RespawnChest>();
			}
			return Singleton<MapHandler>.Instance.segments[Singleton<MapHandler>.Instance.currentSegment - 1].segmentParent.GetComponentInChildren<RespawnChest>(true);
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x060009FE RID: 2558 RVA: 0x00034E1D File Offset: 0x0003301D
	public static Campfire CurrentCampfire
	{
		get
		{
			return MapHandler.GetCampfireRoot(Singleton<MapHandler>.Instance.currentSegment).GetComponentInChildren<Campfire>();
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x060009FF RID: 2559 RVA: 0x00034E34 File Offset: 0x00033034
	public static RespawnChest CurrentScoutStatue
	{
		get
		{
			if (Singleton<MapHandler>.Instance.GetCurrentSegment() > Segment.Caldera)
			{
				return null;
			}
			if (Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.Caldera)
			{
				return MapHandler.GetCampfireRoot(Singleton<MapHandler>.Instance.currentSegment).GetComponentInChildren<RespawnChest>();
			}
			return Singleton<MapHandler>.Instance.segments[Singleton<MapHandler>.Instance.currentSegment].segmentParent.GetComponentInChildren<RespawnChest>();
		}
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x00034E91 File Offset: 0x00033091
	protected override void Awake()
	{
		base.Awake();
		this.debugCommandHandle = CustomCommands<CustomCommandType>.RegisterListener<SyncMapHandlerDebugCommandPackage>(new Action<SyncMapHandlerDebugCommandPackage>(this.OnPackageHandle));
		if (Application.isEditor)
		{
			this.DetectBiomes();
		}
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x00034EBD File Offset: 0x000330BD
	public override void OnDestroy()
	{
		base.OnDestroy();
		CustomCommands<CustomCommandType>.UnregisterListener(this.debugCommandHandle);
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x00034ED0 File Offset: 0x000330D0
	private IEnumerator Start()
	{
		yield return null;
		MapHandler.CurrentScoutStatue.SegmentNumber = Segment.Beach;
		MapHandler.CurrentScoutStatue.ReviveUsed += delegate(RespawnChest statue)
		{
			this.LastRevivedSegment = (int)statue.SegmentNumber;
		};
		for (int i = 1; i < this.segments.Length; i++)
		{
			this.segments[i].segmentParent.SetActive(false);
			if (this.segments[i].segmentCampfire != null)
			{
				this.segments[i].segmentCampfire.SetActive(false);
			}
			Debug.Log(string.Format("Disabling segment: {0} with parent: {1}", i, this.segments[i].segmentParent.name));
		}
		this.segments[0].wallNext.SetActive(true);
		yield break;
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00034EE0 File Offset: 0x000330E0
	internal void DetectBiomes()
	{
		this.biomes.Clear();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			for (int j = 0; j < child.childCount; j++)
			{
				Biome biome;
				if (child.GetChild(j).gameObject.activeInHierarchy && child.GetChild(j).TryGetComponent<Biome>(ref biome))
				{
					this.biomes.Add(biome.biomeType);
				}
			}
		}
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x00034F60 File Offset: 0x00033160
	private void Update()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && !this.hasSpawnedInitialSpawners)
		{
			ISpawner[] componentsInChildren = this.segments[0].segmentParent.GetComponentsInChildren<ISpawner>();
			ISpawner[] componentsInChildren2 = this.segments[0].segmentCampfire.GetComponentsInChildren<ISpawner>();
			ISpawner[] componentsInChildren3 = this.globalParent.GetComponentsInChildren<ISpawner>();
			this.hasSpawnedInitialSpawners = true;
			foreach (ISpawner spawner in componentsInChildren)
			{
				this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments.AddRange(spawner.TrySpawnItems());
			}
			ISpawner[] array = componentsInChildren2;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TrySpawnItems();
			}
			array = componentsInChildren3;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TrySpawnItems();
			}
		}
		else if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
		{
			this.hasSpawnedInitialSpawners = true;
		}
		bool flag = false;
		Player[] array2 = Enumerable.ToArray<Player>(PlayerHandler.GetAllPlayers());
		int num = array2.Length;
		if (array2.Length > 4)
		{
			num = array2.Length / 2;
		}
		else if (array2.Length == 4)
		{
			num = 3;
		}
		else if (array2.Length == 3)
		{
			num = 2;
		}
		if (Enumerable.Count<Player>(array2, (Player player) => player.hasClosedEndScreen) >= num)
		{
			flag = true;
		}
		EndScreenStatus endScreenStatus;
		if (flag && array2.Length != 0 && !GameHandler.TryGetStatus<EndScreenStatus>(out endScreenStatus) && !this.hasEnded)
		{
			bool flag2 = Character.localCharacter.refs.stats.won || Character.localCharacter.refs.stats.somebodyElseWon;
			this.hasEnded = true;
			if (flag2)
			{
				GameHandler.AddStatus<EndScreenStatus>(new EndScreenStatus());
				Singleton<PeakHandler>.Instance.EndScreenComplete();
			}
			else
			{
				Debug.LogError("Everyone has closed end screen.. Loading airport");
				Singleton<GameOverHandler>.Instance.LoadAirport();
			}
		}
		bool flag3 = false;
		Player[] array3 = array2;
		for (int i = 0; i < array3.Length; i++)
		{
			if (array3[i].doneWithCutscene)
			{
				flag3 = true;
				break;
			}
		}
		if (flag3 && array2.Length != 0 && !this.hasCutsceneEnded)
		{
			this.hasCutsceneEnded = true;
			Debug.Log("Everyone is done with cutscene, loading airport");
			GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 1f) });
		}
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x000351A4 File Offset: 0x000333A4
	public void GoToSegment(Segment s)
	{
		MapHandler.<>c__DisplayClass47_0 CS$<>8__locals1 = new MapHandler.<>c__DisplayClass47_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.s = s;
		if ((int)CS$<>8__locals1.s <= this.currentSegment)
		{
			Debug.LogError(string.Format("Trying to transition to segment already passed: {0}", CS$<>8__locals1.s));
			return;
		}
		Debug.Log(string.Format("Going to segment: {0}", CS$<>8__locals1.s));
		base.StartCoroutine(CS$<>8__locals1.<GoToSegment>g__ShowNextSegmentCoroutine|0());
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x00035218 File Offset: 0x00033418
	[ConsoleCommand]
	public static void JumpToSegment(Segment segment)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			MapHandler.JumpToSegmentLogic(segment, Enumerable.ToHashSet<int>(Enumerable.Select<Player, int>(PlayerHandler.GetAllPlayers(), (Player player) => player.photonView.Owner.ActorNumber)), true, true);
			return;
		}
		GameUtils.instance.photonView.RPC("RPC_WarpToSegment", 2, new object[] { (int)segment });
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00035288 File Offset: 0x00033488
	public static void SetSegmentOnSpawn(Segment segment, int lastRevivedSegment)
	{
		Singleton<MapHandler>.Instance.LastRevivedSegment = lastRevivedSegment;
		if (segment < Segment.TheKiln && (Singleton<OrbFogHandler>.Instance == null || Singleton<OrbFogHandler>.Instance.currentID != (int)segment))
		{
			Debug.LogError("Uh oh! The fog handler needs to be initialized before we spawn or the spawn will be broken!!!");
		}
		if (segment == Segment.Beach)
		{
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		hashSet.Add(NetCode.Session.SeatNumber);
		MapHandler.JumpToSegmentLogic(segment, hashSet, false, false);
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x000352EC File Offset: 0x000334EC
	private static void JumpToSegmentLogic(Segment segment, HashSet<int> playersToTeleport, bool sendToEveryone, bool updateFog = false)
	{
		Debug.Log(string.Format("Jumping to beginning of segment: {0}", segment));
		foreach (MapHandler.MapSegment mapSegment in Singleton<MapHandler>.Instance.segments)
		{
			mapSegment.segmentParent.SetActive(false);
			if (mapSegment.segmentCampfire)
			{
				mapSegment.segmentCampfire.SetActive(false);
			}
			if (mapSegment.wallNext)
			{
				mapSegment.wallNext.gameObject.SetActive(false);
			}
			if (mapSegment.wallPrevious)
			{
				mapSegment.wallPrevious.gameObject.SetActive(false);
			}
		}
		Singleton<MapHandler>.Instance.currentSegment = (int)segment;
		int num = Singleton<MapHandler>.Instance.currentSegment;
		if (segment == Segment.Peak)
		{
			num--;
		}
		MapHandler.MapSegment mapSegment2 = Singleton<MapHandler>.Instance.segments[num];
		mapSegment2.segmentParent.SetActive(true);
		if (mapSegment2.segmentCampfire)
		{
			mapSegment2.segmentCampfire.SetActive(true);
		}
		if (mapSegment2.wallNext)
		{
			mapSegment2.wallNext.gameObject.SetActive(true);
		}
		if (mapSegment2.wallPrevious)
		{
			mapSegment2.wallPrevious.gameObject.SetActive(true);
		}
		Vector3 vector = mapSegment2.reconnectSpawnPos.position;
		if (segment == Segment.Peak)
		{
			vector = Singleton<MapHandler>.Instance.respawnThePeak.position;
		}
		if (num > 0)
		{
			MapHandler.MapSegment mapSegment3 = Singleton<MapHandler>.Instance.segments[num - 1];
			if (mapSegment3.segmentCampfire != null)
			{
				mapSegment3.segmentCampfire.SetActive(true);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log(string.Format("Spawning items in {0}. Parent: {1}", segment, mapSegment2.segmentParent.gameObject.name));
			ISpawner[] componentsInChildren = mapSegment2.segmentParent.GetComponentsInChildren<ISpawner>();
			int num2 = 0;
			foreach (ISpawner spawner in componentsInChildren)
			{
				string text = "Spawning...";
				string text2 = num2.ToString();
				string text3 = " ";
				Type type = spawner.GetType();
				Debug.Log(text + text2 + text3 + ((type != null) ? type.ToString() : null));
				spawner.TrySpawnItems();
				num2++;
			}
			if (mapSegment2.segmentCampfire)
			{
				Debug.Log("Spawning items in " + mapSegment2.segmentCampfire.gameObject.name);
				ISpawner[] array2 = mapSegment2.segmentCampfire.GetComponentsInChildren<ISpawner>();
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].TrySpawnItems();
				}
			}
			else
			{
				Debug.Log("NO CAMPFIRE SEGMENT");
			}
		}
		if (updateFog)
		{
			Singleton<OrbFogHandler>.Instance.SetFogOrigin(num);
		}
		if (mapSegment2.dayNightProfile != null)
		{
			DayNightManager.instance.BlendProfiles(mapSegment2.dayNightProfile);
		}
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log(string.Format("Teleporting all players to {0} campfire..", segment));
			foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
			{
				if (playersToTeleport.Contains(character.photonView.Owner.ActorNumber))
				{
					character.photonView.RPC("WarpPlayerRPC", 0, new object[] { vector, false });
				}
			}
		}
		if (sendToEveryone)
		{
			CustomCommands<CustomCommandType>.SendPackage(new SyncMapHandlerDebugCommandPackage(segment, Array.Empty<int>()), 0);
		}
		if (MapHandler.CurrentScoutStatue != null)
		{
			MapHandler.CurrentScoutStatue.SegmentNumber = MapHandler.CurrentSegmentNumber;
			MapHandler.CurrentScoutStatue.ReviveUsed += delegate(RespawnChest statue)
			{
				Singleton<MapHandler>.Instance.LastRevivedSegment = (int)statue.SegmentNumber;
			};
		}
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x00035698 File Offset: 0x00033898
	private void OnPackageHandle(SyncMapHandlerDebugCommandPackage p)
	{
		MapHandler.JumpToSegmentLogic(p.Segment, Enumerable.ToHashSet<int>(p.PlayerToTeleport), false, false);
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000A0A RID: 2570 RVA: 0x000356B2 File Offset: 0x000338B2
	public static Segment CurrentSegmentNumber
	{
		get
		{
			return (Segment)Singleton<MapHandler>.Instance.currentSegment;
		}
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x000356BF File Offset: 0x000338BF
	public Segment GetCurrentSegment()
	{
		return (Segment)this.currentSegment;
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x000356C8 File Offset: 0x000338C8
	public Biome.BiomeType GetCurrentBiome()
	{
		return this.segments[this.currentSegment].biome;
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x000356DC File Offset: 0x000338DC
	public bool BiomeIsPresent(Biome.BiomeType biomeType)
	{
		return this.biomes.Contains(biomeType);
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x000356EC File Offset: 0x000338EC
	public MapHandler.MapSegment GetVariantSegmentFromBiome(Biome.BiomeType biome)
	{
		for (int i = 0; i < this.variantSegments.Length; i++)
		{
			if (this.variantSegments[i].biome == biome)
			{
				return this.variantSegments[i];
			}
		}
		Debug.LogError("COULDNT FIND SEGMENT FROM BIOME. RETURNING SHORE SEGMENT");
		return this.segments[0];
	}

	// Token: 0x04000962 RID: 2402
	private const float k_CampSafetyRadius = 50f;

	// Token: 0x04000963 RID: 2403
	public Transform globalParent;

	// Token: 0x04000964 RID: 2404
	private int _lastRevivedSegment = int.MinValue;

	// Token: 0x04000965 RID: 2405
	public MapHandler.MapSegment[] segments;

	// Token: 0x04000966 RID: 2406
	public MapHandler.MapSegment[] variantSegments;

	// Token: 0x04000967 RID: 2407
	public Transform respawnTheKiln;

	// Token: 0x04000968 RID: 2408
	public Transform respawnThePeak;

	// Token: 0x04000969 RID: 2409
	public LavaRising lavaRising;

	// Token: 0x0400096A RID: 2410
	[SerializeField]
	private int currentSegment;

	// Token: 0x0400096B RID: 2411
	private bool hasSpawnedInitialSpawners;

	// Token: 0x0400096C RID: 2412
	private ListenerHandle debugCommandHandle;

	// Token: 0x0400096D RID: 2413
	private bool hasEnded;

	// Token: 0x0400096E RID: 2414
	private bool hasCutsceneEnded;

	// Token: 0x0400096F RID: 2415
	private List<PhotonView> viewsToDestoryIfNotAlreadyWhenSwitchingSegments = new List<PhotonView>();

	// Token: 0x04000970 RID: 2416
	public List<Biome.BiomeType> biomes = new List<Biome.BiomeType>();

	// Token: 0x02000462 RID: 1122
	[Serializable]
	public class MapSegment
	{
		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06001B11 RID: 6929 RVA: 0x00081CB1 File Offset: 0x0007FEB1
		public Biome.BiomeType biome
		{
			get
			{
				if (this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome))
				{
					return Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).biome;
				}
				return this._biome;
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06001B12 RID: 6930 RVA: 0x00081CE9 File Offset: 0x0007FEE9
		public GameObject segmentParent
		{
			get
			{
				if (this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome))
				{
					return Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).segmentParent;
				}
				return this._segmentParent;
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06001B13 RID: 6931 RVA: 0x00081D21 File Offset: 0x0007FF21
		public GameObject segmentCampfire
		{
			get
			{
				if (this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome))
				{
					return Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).segmentCampfire;
				}
				return this._segmentCampfire;
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06001B14 RID: 6932 RVA: 0x00081D59 File Offset: 0x0007FF59
		public DayNightProfile dayNightProfile
		{
			get
			{
				if (this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome))
				{
					return Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).dayNightProfile;
				}
				return this._dayNightProfile;
			}
		}

		// Token: 0x040018F0 RID: 6384
		[SerializeField]
		private Biome.BiomeType _biome;

		// Token: 0x040018F1 RID: 6385
		[SerializeField]
		private GameObject _segmentParent;

		// Token: 0x040018F2 RID: 6386
		[SerializeField]
		private GameObject _segmentCampfire;

		// Token: 0x040018F3 RID: 6387
		public GameObject wallNext;

		// Token: 0x040018F4 RID: 6388
		public GameObject wallPrevious;

		// Token: 0x040018F5 RID: 6389
		public Transform reconnectSpawnPos;

		// Token: 0x040018F6 RID: 6390
		[SerializeField]
		private DayNightProfile _dayNightProfile;

		// Token: 0x040018F7 RID: 6391
		public bool hasVariant;

		// Token: 0x040018F8 RID: 6392
		public Biome.BiomeType variantBiome;

		// Token: 0x040018F9 RID: 6393
		public bool isVariant;
	}
}
