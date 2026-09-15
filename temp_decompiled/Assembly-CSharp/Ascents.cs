using System;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x0200005B RID: 91
public static class Ascents
{
	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000473 RID: 1139 RVA: 0x0001B393 File Offset: 0x00019593
	// (set) Token: 0x06000474 RID: 1140 RVA: 0x0001B39A File Offset: 0x0001959A
	public static int currentAscent
	{
		get
		{
			return Ascents._currentAscent;
		}
		set
		{
			Ascents._currentAscent = value;
			Debug.Log("Ascent set to " + value.ToString());
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x06000475 RID: 1141 RVA: 0x0001B3B8 File Offset: 0x000195B8
	public static bool fogEnabled
	{
		get
		{
			return Ascents.currentAscent >= 0;
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000476 RID: 1142 RVA: 0x0001B3C5 File Offset: 0x000195C5
	public static float fallDamageMultiplier
	{
		get
		{
			if (Ascents.currentAscent < 1)
			{
				return 1f;
			}
			return 2f;
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x06000477 RID: 1143 RVA: 0x0001B3DA File Offset: 0x000195DA
	public static float hungerRateMultiplier
	{
		get
		{
			if (Ascents.currentAscent == -1)
			{
				return 0.7f;
			}
			if (Ascents.currentAscent >= 2)
			{
				return 1.6f;
			}
			return 1f;
		}
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x06000478 RID: 1144 RVA: 0x0001B3FD File Offset: 0x000195FD
	public static int itemWeightModifier
	{
		get
		{
			if (Ascents.currentAscent < 3)
			{
				return 0;
			}
			return 1;
		}
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x06000479 RID: 1145 RVA: 0x0001B40A File Offset: 0x0001960A
	public static bool shouldSpawnFlare
	{
		get
		{
			return Ascents.currentAscent < 4;
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x0600047A RID: 1146 RVA: 0x0001B414 File Offset: 0x00019614
	public static bool isNightCold
	{
		get
		{
			return Ascents.currentAscent >= 5;
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x0600047B RID: 1147 RVA: 0x0001B421 File Offset: 0x00019621
	public static float nightColdRate
	{
		get
		{
			return 0.005f;
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x0600047C RID: 1148 RVA: 0x0001B428 File Offset: 0x00019628
	public static bool canReviveDead
	{
		get
		{
			return Ascents.currentAscent < 7;
		}
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x0600047D RID: 1149 RVA: 0x0001B432 File Offset: 0x00019632
	public static float climbStaminaMultiplier
	{
		get
		{
			if (Ascents.currentAscent >= 6)
			{
				return 1.4f;
			}
			if (Ascents.currentAscent == -1)
			{
				return 0.7f;
			}
			return 1f;
		}
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0001B455 File Offset: 0x00019655
	[ConsoleCommand]
	public static void UnlockAll()
	{
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, 7);
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x0001B464 File Offset: 0x00019664
	[ConsoleCommand]
	public static void UnlockOne()
	{
		int num;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out num))
		{
			Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, num + 1);
		}
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x0001B490 File Offset: 0x00019690
	[ConsoleCommand]
	public static void LockAll()
	{
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, 0);
	}

	// Token: 0x04000504 RID: 1284
	internal static int _currentAscent;
}
