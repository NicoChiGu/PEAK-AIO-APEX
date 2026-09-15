using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Peak.Afflictions;
using Steamworks;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000045 RID: 69
[ConsoleClassCustomizer("Achievements")]
public class AchievementManager : Singleton<AchievementManager>
{
	// Token: 0x060003D6 RID: 982 RVA: 0x00018C29 File Offset: 0x00016E29
	public void DebugGetAchievement()
	{
		this.ThrowAchievement(this.debugAchievement);
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00018C38 File Offset: 0x00016E38
	public void DebugGetAllAchievements()
	{
		foreach (object obj in Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			ACHIEVEMENTTYPE achievementtype = (ACHIEVEMENTTYPE)obj;
			this.ThrowAchievement(achievementtype);
		}
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x060003D8 RID: 984 RVA: 0x00018C9C File Offset: 0x00016E9C
	// (set) Token: 0x060003D9 RID: 985 RVA: 0x00018CAD File Offset: 0x00016EAD
	public bool gotStats
	{
		get
		{
			return this._gotStats && SteamManager.Initialized;
		}
		private set
		{
			this._gotStats = value;
		}
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00018CB8 File Offset: 0x00016EB8
	public void InitRunBasedValues()
	{
		Debug.Log("Init Run Based Values");
		this.runBasedValueData = new SerializableRunBasedValues();
		this.runBasedValueData.PrimeExistingAchievements();
		List<AchievementManager.RunBasedAchievementData> list = new List<AchievementManager.RunBasedAchievementData>();
		list.Add(new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.KnotTyingBadge, RUNBASEDVALUETYPE.RopePlaced, 100));
		list.Add(new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.ClutchBadge, RUNBASEDVALUETYPE.ScoutsResurrected, 3));
		list.Add(new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.PlundererBadge, RUNBASEDVALUETYPE.LuggageOpened, 15));
		list.Add(new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.FirstAidBadge, RUNBASEDVALUETYPE.FriendsHealedAmount, 20));
		this.runBasedAchievements = list;
		List<AchievementManager.SteamStatBasedAchievementData> list2 = new List<AchievementManager.SteamStatBasedAchievementData>();
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.ForestryBadge, STEAMSTATTYPE.GotBadge_Forestry, 1));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.TreadLightlyBadge, STEAMSTATTYPE.GotBadge_TreadLightly, 1));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.WebSecurityBadge, STEAMSTATTYPE.GotBadge_WebSecurity, 1));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.UndeadEncounterBadge, STEAMSTATTYPE.GotBadge_UndeadEncounter, 1));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.AdvancedMycologyBadge, STEAMSTATTYPE.GotBadge_AdvancedMycology, 1));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.DisasterResponseBadge, STEAMSTATTYPE.GotBadge_DisasterResponse, 1));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.CalciumIntakeBadge, STEAMSTATTYPE.DamageBlockedByMilk, 100));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.CompetitiveEatingBadge, STEAMSTATTYPE.GotBadge_CompetitiveEating, 1));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.AppliedEsotericaBadge, STEAMSTATTYPE.GotBadge_AppliedEsoterica, 1));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.MycoacrobaticsBadge, STEAMSTATTYPE.GotBadge_Mycoacrobatics, 1));
		list2.Add(new AchievementManager.SteamStatBasedAchievementData(ACHIEVEMENTTYPE.CryptogastronomyBadge, STEAMSTATTYPE.GotBadge_Cryptogastronomy, 1));
		this.steamStatBasedAchievements = list2;
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00018DEB File Offset: 0x00016FEB
	[ConsoleCommand]
	public static void ClearAchievements()
	{
		Singleton<AchievementManager>.Instance.ResetAllUserStats();
	}

	// Token: 0x060003DC RID: 988 RVA: 0x00018DF7 File Offset: 0x00016FF7
	[ContextMenu("RESET ALL DATA")]
	private void ResetAllUserStats()
	{
		SteamUserStats.ResetAllStats(true);
		this.StoreUserStats();
		this.InitRunBasedValues();
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00018E0C File Offset: 0x0001700C
	private void Start()
	{
		if (Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "NoSteam"))
		{
			return;
		}
		base.StartCoroutine(this.GetUserStatsRoutine());
		this.InitRunBasedValues();
		this.SubscribeToEvents();
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00018E39 File Offset: 0x00017039
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.UnsubscribeFromEvents();
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00018E47 File Offset: 0x00017047
	private IEnumerator GetUserStatsRoutine()
	{
		while (SteamManager.Instance == null)
		{
			Debug.Log("Waiting for steam manager");
			yield return null;
		}
		while (!SteamManager.Initialized)
		{
			yield return null;
		}
		while (!this.gotStats)
		{
			SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
			yield return new WaitForSeconds(2f);
		}
		yield break;
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00018E56 File Offset: 0x00017056
	private void StoreUserStats()
	{
		base.StartCoroutine(this.StoreUserStatsRoutine());
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00018E65 File Offset: 0x00017065
	private IEnumerator StoreUserStatsRoutine()
	{
		while (!SteamManager.Initialized)
		{
			yield return null;
		}
		SteamUserStats.StoreStats();
		yield break;
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00018E70 File Offset: 0x00017070
	public int GetMaxAscent()
	{
		if (this.useDebugAscent)
		{
			return this.debugAscent;
		}
		int num;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out num))
		{
			return num;
		}
		return 0;
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00018EA0 File Offset: 0x000170A0
	public bool AllBaseAchievementsUnlocked()
	{
		for (int i = 1; i <= 32; i++)
		{
			if (!Singleton<AchievementManager>.Instance.IsAchievementUnlocked((ACHIEVEMENTTYPE)i))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00018ECC File Offset: 0x000170CC
	public bool IsAchievementUnlocked(ACHIEVEMENTTYPE achievementType)
	{
		if (!SteamManager.Initialized)
		{
			return false;
		}
		bool flag;
		if (this.CheckSteamStatLinkedAchievementUnlocked(achievementType, out flag))
		{
			return flag;
		}
		bool flag2;
		SteamUserStats.GetAchievement(achievementType.ToString(), ref flag2);
		return flag2;
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x00018F08 File Offset: 0x00017108
	private bool TryGetSteamStatLinkedAchievement(ACHIEVEMENTTYPE achievementType, out AchievementManager.SteamStatBasedAchievementData result)
	{
		result = null;
		if (!SteamManager.Initialized)
		{
			return false;
		}
		foreach (AchievementManager.SteamStatBasedAchievementData steamStatBasedAchievementData in this.steamStatBasedAchievements)
		{
			if (steamStatBasedAchievementData.achievementType == achievementType)
			{
				result = steamStatBasedAchievementData;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x00018F74 File Offset: 0x00017174
	public bool CheckSteamStatLinkedAchievementUnlocked(ACHIEVEMENTTYPE achievementType, out bool isUnlocked)
	{
		isUnlocked = false;
		AchievementManager.SteamStatBasedAchievementData steamStatBasedAchievementData;
		if (this.TryGetSteamStatLinkedAchievement(achievementType, out steamStatBasedAchievementData))
		{
			isUnlocked = steamStatBasedAchievementData.IsAchieved();
			return true;
		}
		return false;
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00018F9C File Offset: 0x0001719C
	public bool TryThrowStatLinkedAchievement(ACHIEVEMENTTYPE achievementType)
	{
		AchievementManager.SteamStatBasedAchievementData steamStatBasedAchievementData;
		if (this.TryGetSteamStatLinkedAchievement(achievementType, out steamStatBasedAchievementData))
		{
			this.SetSteamStat(steamStatBasedAchievementData.statType, steamStatBasedAchievementData.requiredValue);
			return true;
		}
		return false;
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x00018FCC File Offset: 0x000171CC
	private void CheckRunBasedAchievement(RUNBASEDVALUETYPE type)
	{
		foreach (AchievementManager.RunBasedAchievementData runBasedAchievementData in this.runBasedAchievements)
		{
			if (runBasedAchievementData.valueType == type && runBasedAchievementData.IsAchieved())
			{
				this.ThrowAchievement(runBasedAchievementData.achievementType);
			}
		}
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x00019038 File Offset: 0x00017238
	private void CheckNewAchievements()
	{
		foreach (object obj in Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			ACHIEVEMENTTYPE achievementtype = (ACHIEVEMENTTYPE)obj;
			if (!this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Contains(achievementtype) && this.IsAchievementUnlocked(achievementtype))
			{
				Debug.Log("EARNED ACHIEVEMENT: " + achievementtype.ToString());
				if (!this.runBasedValueData.achievementsEarnedThisRun.Contains(achievementtype))
				{
					this.runBasedValueData.achievementsEarnedThisRun.Add(achievementtype);
				}
				this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Add(achievementtype);
				GlobalEvents.OnAchievementThrown.Invoke(achievementtype);
			}
		}
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x00019114 File Offset: 0x00017314
	public void SetSteamStat(STEAMSTATTYPE steamStatType, int value)
	{
		if (!SteamManager.Initialized || !Singleton<AchievementManager>.Instance.gotStats)
		{
			return;
		}
		SteamUserStats.SetStat(steamStatType.ToString(), value);
		this.StoreUserStats();
		this.CheckNewAchievements();
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x0001914A File Offset: 0x0001734A
	public bool GetSteamStatInt(STEAMSTATTYPE steamStatType, out int value)
	{
		if (!SteamManager.Initialized)
		{
			value = -1;
			return false;
		}
		return SteamUserStats.GetStat(steamStatType.ToString(), ref value);
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x0001916C File Offset: 0x0001736C
	public int GetNextPage()
	{
		if (!SteamManager.Initialized)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			int num2;
			SteamUserStats.GetStat("ReadGuidebookPage_" + i.ToString(), ref num2);
			if (num2 != 1)
			{
				return num;
			}
			num++;
		}
		return num;
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x000191B8 File Offset: 0x000173B8
	public int GetTotalPagesSeen()
	{
		if (!SteamManager.Initialized)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			int num2;
			SteamUserStats.GetStat("ReadGuidebookPage_" + i.ToString(), ref num2);
			if (num2 == 1)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00019200 File Offset: 0x00017400
	public bool SeenGuidebookPage(int index)
	{
		int num;
		SteamUserStats.GetStat("ReadGuidebookPage_" + index.ToString(), ref num);
		return num == 1;
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x0001922C File Offset: 0x0001742C
	public void TriggerSeenGuidebookPage(int index)
	{
		SteamUserStats.SetStat("ReadGuidebookPage_" + index.ToString(), 1);
		this.StoreUserStats();
		int totalPagesSeen = this.GetTotalPagesSeen();
		Debug.Log("Total pages seen: " + totalPagesSeen.ToString());
		this.SetSteamStat(STEAMSTATTYPE.TotalPagesRead, totalPagesSeen);
		if (totalPagesSeen >= 8)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.BookwormBadge);
		}
		this.StoreUserStats();
		Debug.Log("Saw page " + index.ToString());
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x000192A8 File Offset: 0x000174A8
	public int IncrementSteamStat(STEAMSTATTYPE steamStatType, int value)
	{
		int num;
		try
		{
			if (!SteamManager.Initialized)
			{
				num = 0;
			}
			else
			{
				int num2;
				SteamUserStats.GetStat(steamStatType.ToString(), ref num2);
				num2 += value;
				SteamUserStats.SetStat(steamStatType.ToString(), num2);
				this.StoreUserStats();
				this.CheckNewAchievements();
				num = num2;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
			num = 0;
		}
		return num;
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x00019318 File Offset: 0x00017518
	[ConsoleCommand]
	internal static void Grant(ACHIEVEMENTTYPE type)
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(type);
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x00019325 File Offset: 0x00017525
	[ConsoleCommand]
	internal static void GiveAscentLevel(int level)
	{
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, level);
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00019334 File Offset: 0x00017534
	internal void ThrowAchievement(ACHIEVEMENTTYPE type)
	{
		try
		{
			if (!SteamManager.Initialized)
			{
				Debug.LogError(string.Format("Tried to throw achievement of type {0} before Steam Manager was initialized. This probably resulted in the achievement not being granted.", type));
			}
			else
			{
				if (!this.IsAchievementUnlocked(type) && !this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Contains(type))
				{
					Debug.Log("Throwing achievement: " + type.ToString());
					if (!this.TryThrowStatLinkedAchievement(type))
					{
						if (!this.runBasedValueData.achievementsEarnedThisRun.Contains(type))
						{
							this.runBasedValueData.achievementsEarnedThisRun.Add(type);
						}
						SteamUserStats.SetAchievement(type.ToString());
						GlobalEvents.OnAchievementThrown.Invoke(type);
						this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Add(type);
					}
				}
				this.StoreUserStats();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00019418 File Offset: 0x00017618
	public void SetRunBasedInt(RUNBASEDVALUETYPE type, int value)
	{
		this.runBasedValueData.runBasedInts[type] = value;
		this.CheckRunBasedAchievement(type);
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x00019434 File Offset: 0x00017634
	public int GetRunBasedInt(RUNBASEDVALUETYPE type)
	{
		if (!this.runBasedValueData.runBasedInts.ContainsKey(type))
		{
			this.SetRunBasedInt(type, 0);
		}
		try
		{
			return this.runBasedValueData.runBasedInts[type];
		}
		catch
		{
			Debug.LogError(string.Format("Tried to retrieve run based int {0} that is not an int.", type));
		}
		return 0;
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x0001949C File Offset: 0x0001769C
	public void AddToRunBasedInt(RUNBASEDVALUETYPE type, int valueToAdd)
	{
		int runBasedInt = this.GetRunBasedInt(type);
		this.SetRunBasedInt(type, runBasedInt + valueToAdd);
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x000194BB File Offset: 0x000176BB
	public void SetRunBasedFloat(RUNBASEDVALUETYPE type, float value)
	{
		this.runBasedValueData.runBasedFloats[type] = value;
		this.CheckRunBasedAchievement(type);
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x000194D8 File Offset: 0x000176D8
	public float GetRunBasedFloat(RUNBASEDVALUETYPE type)
	{
		if (!this.runBasedValueData.runBasedFloats.ContainsKey(type))
		{
			this.SetRunBasedFloat(type, 0f);
		}
		try
		{
			return Convert.ToSingle(this.runBasedValueData.runBasedFloats[type]);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("Tried to retrieve run based float {0} that is not a float.\n{1}", type, ex.ToString()));
		}
		return 0f;
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00019554 File Offset: 0x00017754
	public void AddToRunBasedFloat(RUNBASEDVALUETYPE type, float valueToAdd)
	{
		float runBasedFloat = this.GetRunBasedFloat(type);
		this.SetRunBasedFloat(type, runBasedFloat + valueToAdd);
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x00019574 File Offset: 0x00017774
	private void SubscribeToEvents()
	{
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
		GlobalEvents.OnLocalStatusIncremented = (Action<Character, CharacterAfflictions.STATUSTYPE, bool>)Delegate.Combine(GlobalEvents.OnLocalStatusIncremented, new Action<Character, CharacterAfflictions.STATUSTYPE, bool>(this.TestStatusIncremented));
		GlobalEvents.OnRespawnChestOpened = (Action<RespawnChest, Character>)Delegate.Combine(GlobalEvents.OnRespawnChestOpened, new Action<RespawnChest, Character>(this.TestRespawnChestOpened));
		GlobalEvents.OnLuggageOpened = (Action<Luggage, Character>)Delegate.Combine(GlobalEvents.OnLuggageOpened, new Action<Luggage, Character>(this.TestLuggageOpened));
		GlobalEvents.OnLocalCharacterWonRun = (Action)Delegate.Combine(GlobalEvents.OnLocalCharacterWonRun, new Action(this.TestWonRun));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Combine(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		GlobalEvents.OnSomeoneWonRun = (Action)Delegate.Combine(GlobalEvents.OnSomeoneWonRun, new Action(this.TestSomeoneWonRun));
		GlobalEvents.OnRunEnded = (Action)Delegate.Combine(GlobalEvents.OnRunEnded, new Action(this.TestRunEnded));
		GlobalEvents.OnCharacterDied = (Action<Character>)Delegate.Combine(GlobalEvents.OnCharacterDied, new Action<Character>(this.TestPlayerDied));
		Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.OnUserStatsRecieved));
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x000196D4 File Offset: 0x000178D4
	private void UnsubscribeFromEvents()
	{
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
		GlobalEvents.OnLocalStatusIncremented = (Action<Character, CharacterAfflictions.STATUSTYPE, bool>)Delegate.Remove(GlobalEvents.OnLocalStatusIncremented, new Action<Character, CharacterAfflictions.STATUSTYPE, bool>(this.TestStatusIncremented));
		GlobalEvents.OnRespawnChestOpened = (Action<RespawnChest, Character>)Delegate.Remove(GlobalEvents.OnRespawnChestOpened, new Action<RespawnChest, Character>(this.TestRespawnChestOpened));
		GlobalEvents.OnLuggageOpened = (Action<Luggage, Character>)Delegate.Remove(GlobalEvents.OnLuggageOpened, new Action<Luggage, Character>(this.TestLuggageOpened));
		GlobalEvents.OnLocalCharacterWonRun = (Action)Delegate.Remove(GlobalEvents.OnLocalCharacterWonRun, new Action(this.TestWonRun));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Remove(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		GlobalEvents.OnSomeoneWonRun = (Action)Delegate.Remove(GlobalEvents.OnSomeoneWonRun, new Action(this.TestSomeoneWonRun));
		GlobalEvents.OnRunEnded = (Action)Delegate.Remove(GlobalEvents.OnRunEnded, new Action(this.TestRunEnded));
		GlobalEvents.OnCharacterDied = (Action<Character>)Delegate.Remove(GlobalEvents.OnCharacterDied, new Action<Character>(this.TestPlayerDied));
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00019821 File Offset: 0x00017A21
	private void OnUserStatsRecieved(UserStatsReceived_t result)
	{
		if (result.m_eResult != 2)
		{
			if (!this.gotStats)
			{
				this.InitRunBasedValues();
			}
			this.gotStats = true;
		}
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x00019841 File Offset: 0x00017A41
	private void TestRequestedItem(Item item, Character character)
	{
		if (character.IsLocal && item.itemTags.HasFlag(Item.ItemTags.Mystical))
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.EsotericaBadge);
		}
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x0001986C File Offset: 0x00017A6C
	private void TestItemConsumed(Item item, Character character)
	{
		if (character.IsLocal)
		{
			if (item.itemTags.HasFlag(Item.ItemTags.Berry))
			{
				this.AddToRunBasedFruitsEaten(item.itemID);
				if (item.itemTags.HasFlag(Item.ItemTags.Mushroom))
				{
					this.AddToShroomBerriesEaten(item.itemID);
				}
			}
			if (item.itemTags.HasFlag(Item.ItemTags.Bird) && !Character.localCharacter.data.cannibalismPermitted)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.ResourcefulnessBadge);
			}
			if (item.itemTags.HasFlag(Item.ItemTags.PackagedFood))
			{
				this.AddToRunBasedInt(RUNBASEDVALUETYPE.PackagedFoodEaten, 1);
			}
			if (item.itemTags.HasFlag(Item.ItemTags.Mushroom) && !item.itemTags.HasFlag(Item.ItemTags.Berry) && item.GetComponent<Action_InflictPoison>() == null)
			{
				this.AddToNonToxicMushroomsEaten(item.itemID);
			}
			if (item.itemTags.HasFlag(Item.ItemTags.GourmandRequirement) && item.GetData<IntItemData>(DataEntryKey.CookedAmount).Value >= 1)
			{
				this.AddToGourmandRequirementsEaten(item.itemID);
			}
		}
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x000199A0 File Offset: 0x00017BA0
	private void TestStatusIncremented(Character character, CharacterAfflictions.STATUSTYPE statusType, bool changeWasPositive)
	{
		if (!changeWasPositive && this.GetRunBasedInt(RUNBASEDVALUETYPE.BitByZombie) > 0)
		{
			Debug.Log("Was previously bit");
			bool flag = character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Spores) >= 0.025f;
			Affliction affliction;
			bool flag2 = character.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.ZombieBite, out affliction);
			Debug.Log(string.Format("Has spores: {0}, hasAffliction: {1}", flag, flag2));
			if (!flag && !flag2)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.UndeadEncounterBadge);
			}
		}
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00019A20 File Offset: 0x00017C20
	private void TestRespawnChestOpened(RespawnChest chest, Character opener)
	{
		if (opener.IsLocal)
		{
			foreach (Character character in Character.AllCharacters)
			{
				if (character.data.dead || character.data.fullyPassedOut)
				{
					this.AddToRunBasedInt(RUNBASEDVALUETYPE.ScoutsResurrected, 1);
				}
			}
		}
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00019A98 File Offset: 0x00017C98
	private void TestLuggageOpened(Luggage luggage, Character opener)
	{
		if (opener.IsLocal)
		{
			this.AddToRunBasedInt(RUNBASEDVALUETYPE.LuggageOpened, 1);
		}
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00019AAC File Offset: 0x00017CAC
	public void TestTimeAchievements()
	{
		int num = Mathf.FloorToInt(RunManager.Instance.timeSinceRunStarted);
		if ((float)num <= 3600f)
		{
			Debug.Log("Sub one hour!!");
			this.ThrowAchievement(ACHIEVEMENTTYPE.SpeedClimberBadge);
		}
		int num2;
		this.GetSteamStatInt(STEAMSTATTYPE.BestTime, out num2);
		if (num < num2)
		{
			this.SetSteamStat(STEAMSTATTYPE.BestTime, num);
		}
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00019AFC File Offset: 0x00017CFC
	private void TestRunEnded()
	{
		this.TestAscentAchievements(-1);
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00019B05 File Offset: 0x00017D05
	public void TestCoolCucumberAchievement()
	{
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxHeatTakenInMesa) <= 0.1f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.CoolCucumberBadge);
		}
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00019B1E File Offset: 0x00017D1E
	public void TestTreadLightlyAchievement()
	{
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxSporesTakenInRoots) <= 0.25f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.TreadLightlyBadge);
		}
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00019B37 File Offset: 0x00017D37
	public void TestBundledUpAchievement()
	{
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxColdTakenInAlpine) <= 0.2f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.BundledUpBadge);
		}
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00019B50 File Offset: 0x00017D50
	private void TestWonRun()
	{
		this.ThrowAchievement(ACHIEVEMENTTYPE.PeakBadge);
		this.IncrementSteamStat(STEAMSTATTYPE.TimesPeaked, 1);
		if (Character.AllCharacters.Count == 1)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.LoneWolfBadge);
		}
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken) == 0f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.BalloonBadge);
		}
		if (this.GetRunBasedInt(RUNBASEDVALUETYPE.PackagedFoodEaten) == 0)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.NaturalistBadge);
		}
		if (this.GetRunBasedInt(RUNBASEDVALUETYPE.TimesPassedOut) == 0)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.SurvivalistBadge);
		}
		if (this.GetRunBasedInt(RUNBASEDVALUETYPE.PermanentItemsPlaced) == 0)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.LeaveNoTraceBadge);
		}
		if (this.HasBingBong(Character.localCharacter))
		{
			GameUtils.instance.ThrowBingBongAchievement();
		}
		if (this.runBasedValueData.gourmandRequirementsEaten.Count >= 4)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.GourmandBadge);
		}
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00019C00 File Offset: 0x00017E00
	private bool HasBingBong(Character character)
	{
		if (character.data.currentItem && character.data.currentItem.itemTags.HasFlag(Item.ItemTags.BingBong))
		{
			return true;
		}
		foreach (ItemSlot itemSlot in character.player.itemSlots)
		{
			if (itemSlot != null && itemSlot.prefab != null && itemSlot.prefab.itemTags.HasFlag(Item.ItemTags.BingBong))
			{
				return true;
			}
		}
		BackpackData backpackData;
		if (!character.player.backpackSlot.IsEmpty() && character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			foreach (ItemSlot itemSlot2 in backpackData.itemSlots)
			{
				if (itemSlot2 != null && itemSlot2.prefab != null && itemSlot2.prefab.itemTags.HasFlag(Item.ItemTags.BingBong))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00019D0E File Offset: 0x00017F0E
	private void TestCharacterPassedOut(Character character)
	{
		if (character.IsLocal)
		{
			this.AddToRunBasedInt(RUNBASEDVALUETYPE.TimesPassedOut, 1);
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00019D20 File Offset: 0x00017F20
	private void TestSomeoneWonRun()
	{
		if (Character.localCharacter.refs.stats.lost)
		{
			Debug.Log("YOU TRIED");
			this.ThrowAchievement(ACHIEVEMENTTYPE.TriedYourBestBadge);
		}
		this.TryCompleteAscent();
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00019D50 File Offset: 0x00017F50
	private void TryCompleteAscent()
	{
		int i;
		if (this.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out i))
		{
			if (Ascents.currentAscent >= i)
			{
				while (i <= Ascents.currentAscent)
				{
					Debug.Log("Completed Ascent: " + i.ToString());
					if (!this.runBasedValueData.completedAscentsThisRun.Contains(i))
					{
						this.runBasedValueData.completedAscentsThisRun.Add(i);
					}
					i++;
				}
				this.SetSteamStat(STEAMSTATTYPE.MaxAscent, Ascents.currentAscent + 1);
			}
			this.TestAscentAchievements(i);
		}
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x00019DD0 File Offset: 0x00017FD0
	private void AddToRunBasedFruitsEaten(ushort value)
	{
		if (!this.runBasedValueData.runBasedFruitsEaten.Contains(value))
		{
			this.runBasedValueData.runBasedFruitsEaten.Add(value);
			if (this.runBasedValueData.runBasedFruitsEaten.Count >= 5)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.ForagingBadge);
			}
		}
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00019E1C File Offset: 0x0001801C
	private void AddToShroomBerriesEaten(ushort value)
	{
		if (!this.runBasedValueData.shroomBerriesEaten.Contains(value))
		{
			this.runBasedValueData.shroomBerriesEaten.Add(value);
			if (this.runBasedValueData.shroomBerriesEaten.Count >= 5)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.AdvancedMycologyBadge);
			}
		}
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00019E68 File Offset: 0x00018068
	private void AddToNonToxicMushroomsEaten(ushort value)
	{
		Debug.Log("Local player ate non toxic mushroom: " + value.ToString());
		if (!this.runBasedValueData.nonToxicMushroomsEaten.Contains(value))
		{
			this.runBasedValueData.nonToxicMushroomsEaten.Add(value);
			if (this.runBasedValueData.nonToxicMushroomsEaten.Count >= 4)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.MycologyBadge);
			}
		}
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00019ECA File Offset: 0x000180CA
	private void AddToGourmandRequirementsEaten(ushort value)
	{
		if (!this.runBasedValueData.gourmandRequirementsEaten.Contains(value))
		{
			Debug.Log("ATE GOURMAND REQUIREMENT: " + value.ToString());
			this.runBasedValueData.gourmandRequirementsEaten.Add(value);
		}
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00019F08 File Offset: 0x00018108
	internal void RecordMaxHeight(int meters)
	{
		if (meters < 25)
		{
			return;
		}
		int runBasedInt = this.GetRunBasedInt(RUNBASEDVALUETYPE.MaxHeightReached);
		if (meters >= runBasedInt + 5)
		{
			this.IncrementSteamStat(STEAMSTATTYPE.HeightClimbed, meters - runBasedInt);
			this.SetRunBasedInt(RUNBASEDVALUETYPE.MaxHeightReached, meters);
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00019F40 File Offset: 0x00018140
	internal void TestAscentAchievements(int maxAscent = -1)
	{
		if (maxAscent == -1)
		{
			this.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out maxAscent);
		}
		int num = 0;
		while (num <= maxAscent - 2 && num < this.ascentAchievements.Length)
		{
			this.ThrowAchievement(this.ascentAchievements[num]);
			num++;
		}
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00019F83 File Offset: 0x00018183
	public void TestPlayerDied(Character c)
	{
		if (c.IsLocal)
		{
			this.SetRunBasedInt(RUNBASEDVALUETYPE.BitByZombie, 0);
		}
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00019F98 File Offset: 0x00018198
	public void AddStatusBlockedByMilk(float amount)
	{
		int num = 0;
		this.statusBlockedByMilk += amount;
		while (this.statusBlockedByMilk >= 0.01f)
		{
			num++;
			this.statusBlockedByMilk -= 0.01f;
		}
		if (num > 0)
		{
			int num2 = this.IncrementSteamStat(STEAMSTATTYPE.DamageBlockedByMilk, num);
			Debug.Log(string.Format("Total damage blocked by milk: {0}", num2));
		}
	}

	// Token: 0x04000485 RID: 1157
	public List<AchievementData> allAchievements;

	// Token: 0x04000486 RID: 1158
	[SerializeField]
	private ACHIEVEMENTTYPE debugAchievement;

	// Token: 0x04000487 RID: 1159
	public const int MAX_GUIDEBOOK_PAGES = 8;

	// Token: 0x04000488 RID: 1160
	public const int DISC_THROW_DISTANCE_REQUIREMENT = 100;

	// Token: 0x04000489 RID: 1161
	public const float MAX_MESA_HEAT_PERCENTAGE = 0.1f;

	// Token: 0x0400048A RID: 1162
	public const float MAX_ALPINE_COLD_PERCENTAGE = 0.2f;

	// Token: 0x0400048B RID: 1163
	public const float MAX_ROOTS_SPORES_PERCENTAGE = 0.25f;

	// Token: 0x0400048C RID: 1164
	public const float DISTANCE_FOR_RESCUE_HOOK_ACHIEVEMENT = 30f;

	// Token: 0x0400048D RID: 1165
	private bool _gotStats;

	// Token: 0x0400048E RID: 1166
	private List<AchievementManager.RunBasedAchievementData> runBasedAchievements = new List<AchievementManager.RunBasedAchievementData>();

	// Token: 0x0400048F RID: 1167
	private List<AchievementManager.SteamStatBasedAchievementData> steamStatBasedAchievements = new List<AchievementManager.SteamStatBasedAchievementData>();

	// Token: 0x04000490 RID: 1168
	public SerializableRunBasedValues runBasedValueData;

	// Token: 0x04000491 RID: 1169
	public bool useDebugAscent;

	// Token: 0x04000492 RID: 1170
	public int debugAscent;

	// Token: 0x04000493 RID: 1171
	public const int TOTAL_GUIDEBOOK_PAGES = 8;

	// Token: 0x04000494 RID: 1172
	public const string STEAMSTAT_GUIDEBOOK_PREFIX = "ReadGuidebookPage_";

	// Token: 0x04000495 RID: 1173
	private const float ONE_HOUR_IN_SECONDS = 3600f;

	// Token: 0x04000496 RID: 1174
	private const int FRUITSNEEDEDFORACHIEVEMENT = 5;

	// Token: 0x04000497 RID: 1175
	private const int MUSHROOMSNEEDEDFORACHIEVEMENT = 4;

	// Token: 0x04000498 RID: 1176
	private const int SHROOMBERRIESNEEDEDFORACHIEVEMENT = 5;

	// Token: 0x04000499 RID: 1177
	private ACHIEVEMENTTYPE[] ascentAchievements = new ACHIEVEMENTTYPE[]
	{
		ACHIEVEMENTTYPE.Ascent1,
		ACHIEVEMENTTYPE.Ascent2,
		ACHIEVEMENTTYPE.Ascent3,
		ACHIEVEMENTTYPE.Ascent4,
		ACHIEVEMENTTYPE.Ascent5,
		ACHIEVEMENTTYPE.Ascent6,
		ACHIEVEMENTTYPE.Ascent7
	};

	// Token: 0x0400049A RID: 1178
	private float statusBlockedByMilk;

	// Token: 0x0200040E RID: 1038
	private class SteamStatBasedAchievementData
	{
		// Token: 0x06001A2C RID: 6700 RVA: 0x0007F6D7 File Offset: 0x0007D8D7
		public SteamStatBasedAchievementData(ACHIEVEMENTTYPE achievementType, STEAMSTATTYPE statType, int requiredValue)
		{
			this.achievementType = achievementType;
			this.statType = statType;
			this.requiredValue = requiredValue;
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x0007F6F4 File Offset: 0x0007D8F4
		public bool IsAchieved()
		{
			int num;
			return SteamUserStats.GetStat(this.statType.ToString(), ref num) && num >= this.requiredValue;
		}

		// Token: 0x0400178B RID: 6027
		public ACHIEVEMENTTYPE achievementType;

		// Token: 0x0400178C RID: 6028
		public STEAMSTATTYPE statType;

		// Token: 0x0400178D RID: 6029
		public int requiredValue;
	}

	// Token: 0x0200040F RID: 1039
	private class RunBasedAchievementData
	{
		// Token: 0x06001A2E RID: 6702 RVA: 0x0007F729 File Offset: 0x0007D929
		public RunBasedAchievementData(ACHIEVEMENTTYPE achievementType, RUNBASEDVALUETYPE valueType, int requiredValue)
		{
			this.achievementType = achievementType;
			this.valueType = valueType;
			this.requiredValue = requiredValue;
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x0007F748 File Offset: 0x0007D948
		public bool IsAchieved()
		{
			try
			{
				bool flag = false;
				if (Singleton<AchievementManager>.Instance.GetRunBasedFloat(this.valueType) >= (float)this.requiredValue)
				{
					flag = true;
				}
				if ((float)Singleton<AchievementManager>.Instance.GetRunBasedInt(this.valueType) >= (float)this.requiredValue)
				{
					flag = true;
				}
				return flag;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
			return false;
		}

		// Token: 0x0400178E RID: 6030
		public ACHIEVEMENTTYPE achievementType;

		// Token: 0x0400178F RID: 6031
		public RUNBASEDVALUETYPE valueType;

		// Token: 0x04001790 RID: 6032
		public int requiredValue;
	}
}
