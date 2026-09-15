using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000090 RID: 144
[CreateAssetMenu(fileName = "CustomizationOption", menuName = "Scriptable Objects/CustomizationOption")]
public class CustomizationOption : ScriptableObject
{
	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060005BE RID: 1470 RVA: 0x0002102B File Offset: 0x0001F22B
	public bool requiresSteamStat
	{
		get
		{
			return this.requiredSteamStat > STEAMSTATTYPE.NONE;
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060005BF RID: 1471 RVA: 0x00021036 File Offset: 0x0001F236
	private bool IsAccessory
	{
		get
		{
			return this.type == Customization.Type.Accessory;
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060005C0 RID: 1472 RVA: 0x00021042 File Offset: 0x0001F242
	private bool IsSkin
	{
		get
		{
			return this.type == Customization.Type.Skin;
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060005C1 RID: 1473 RVA: 0x0002104D File Offset: 0x0001F24D
	private bool IsFit
	{
		get
		{
			return this.type == Customization.Type.Fit;
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060005C2 RID: 1474 RVA: 0x00021059 File Offset: 0x0001F259
	public Material fitPantsMaterial
	{
		get
		{
			if (this.fitMaterialOverridePants != null)
			{
				return this.fitMaterialOverridePants;
			}
			return this.fitMaterial;
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060005C3 RID: 1475 RVA: 0x00021076 File Offset: 0x0001F276
	public Material fitHatMaterial
	{
		get
		{
			if (this.fitMaterialOverrideHat != null)
			{
				return this.fitMaterialOverrideHat;
			}
			return this.fitMaterial;
		}
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060005C4 RID: 1476 RVA: 0x00021094 File Offset: 0x0001F294
	public bool IsLocked
	{
		get
		{
			if (this.requiresAscent)
			{
				return Singleton<AchievementManager>.Instance.GetMaxAscent() < this.requiredAscent;
			}
			if (this.requiredAchievement == ACHIEVEMENTTYPE.NONE && this.customRequirement == CustomizationOption.CUSTOMREQUIREMENT.None)
			{
				return false;
			}
			if (this.customRequirement == CustomizationOption.CUSTOMREQUIREMENT.Goat)
			{
				return Singleton<AchievementManager>.Instance.GetMaxAscent() < 8;
			}
			if (this.customRequirement == CustomizationOption.CUSTOMREQUIREMENT.Crown)
			{
				return !Singleton<AchievementManager>.Instance.AllBaseAchievementsUnlocked();
			}
			return !Singleton<AchievementManager>.Instance.IsAchievementUnlocked(this.requiredAchievement);
		}
	}

	// Token: 0x040005CE RID: 1486
	public Customization.Type type;

	// Token: 0x040005CF RID: 1487
	public Texture texture;

	// Token: 0x040005D0 RID: 1488
	public ACHIEVEMENTTYPE requiredAchievement;

	// Token: 0x040005D1 RID: 1489
	public STEAMSTATTYPE requiredSteamStat;

	// Token: 0x040005D2 RID: 1490
	public int requiredSteamStatValue = 1;

	// Token: 0x040005D3 RID: 1491
	public bool requiresAscent;

	// Token: 0x040005D4 RID: 1492
	public int requiredAscent;

	// Token: 0x040005D5 RID: 1493
	public CustomizationOption.CUSTOMREQUIREMENT customRequirement;

	// Token: 0x040005D6 RID: 1494
	public bool isBlank;

	// Token: 0x040005D7 RID: 1495
	public bool testLocked;

	// Token: 0x040005D8 RID: 1496
	public bool drawUnderEye;

	// Token: 0x040005D9 RID: 1497
	public bool isThirdEye;

	// Token: 0x040005DA RID: 1498
	[ColorUsage(true, false)]
	public Color color;

	// Token: 0x040005DB RID: 1499
	public Mesh fitMesh;

	// Token: 0x040005DC RID: 1500
	public Material fitMaterial;

	// Token: 0x040005DD RID: 1501
	public Material fitMaterialShoes;

	// Token: 0x040005DE RID: 1502
	public Material fitMaterialOverridePants;

	// Token: 0x040005DF RID: 1503
	public Material fitMaterialOverrideHat;

	// Token: 0x040005E0 RID: 1504
	public bool isSkirt;

	// Token: 0x040005E1 RID: 1505
	public bool noPants;

	// Token: 0x040005E2 RID: 1506
	public bool overrideHat;

	// Token: 0x040005E3 RID: 1507
	public int overrideHatIndex;

	// Token: 0x02000424 RID: 1060
	public enum CUSTOMREQUIREMENT
	{
		// Token: 0x040017CF RID: 6095
		None,
		// Token: 0x040017D0 RID: 6096
		Goat,
		// Token: 0x040017D1 RID: 6097
		Crown
	}
}
