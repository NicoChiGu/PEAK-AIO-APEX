using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200008F RID: 143
public class Customization : Singleton<Customization>
{
	// Token: 0x060005BA RID: 1466 RVA: 0x00020E90 File Offset: 0x0001F090
	public List<CustomizationOption> TryGetUnlockedCosmetics(BadgeData badge)
	{
		bool flag = false;
		List<CustomizationOption> list = new List<CustomizationOption>();
		foreach (object obj in Enum.GetValues(typeof(Customization.Type)))
		{
			Customization.Type type = (Customization.Type)obj;
			foreach (CustomizationOption customizationOption in this.GetList(type))
			{
				if (!(customizationOption == null) && customizationOption.requiredAchievement != ACHIEVEMENTTYPE.NONE && customizationOption.requiredAchievement == badge.linkedAchievement)
				{
					if (customizationOption.type == Customization.Type.Fit)
					{
						if (flag)
						{
							goto IL_007A;
						}
						flag = true;
					}
					list.Add(customizationOption);
				}
				IL_007A:;
			}
		}
		return list;
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00020F54 File Offset: 0x0001F154
	public CustomizationOption[] GetList(Customization.Type type)
	{
		if (type <= Customization.Type.Eyes)
		{
			if (type == Customization.Type.Skin)
			{
				return this.skins;
			}
			if (type == Customization.Type.Accessory)
			{
				return this.accessories;
			}
			if (type == Customization.Type.Eyes)
			{
				return this.eyes;
			}
		}
		else if (type <= Customization.Type.Fit)
		{
			if (type == Customization.Type.Mouth)
			{
				return this.mouths;
			}
			if (type == Customization.Type.Fit)
			{
				return this.fits;
			}
		}
		else
		{
			if (type == Customization.Type.Hat)
			{
				return this.hats;
			}
			if (type == Customization.Type.Sash)
			{
				return this.sashes;
			}
		}
		return this.skins;
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00020FCC File Offset: 0x0001F1CC
	public int GetRandomUnlockedIndex(Customization.Type type)
	{
		CustomizationOption[] list = this.GetList(type);
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Length; i++)
		{
			if (!list[i].IsLocked)
			{
				list2.Add(i);
			}
		}
		if (list2.Count <= 0)
		{
			return 0;
		}
		return list2[Random.Range(0, list2.Count)];
	}

	// Token: 0x040005C5 RID: 1477
	public CustomizationOption[] skins;

	// Token: 0x040005C6 RID: 1478
	public CustomizationOption[] accessories;

	// Token: 0x040005C7 RID: 1479
	public CustomizationOption[] eyes;

	// Token: 0x040005C8 RID: 1480
	public CustomizationOption[] mouths;

	// Token: 0x040005C9 RID: 1481
	public CustomizationOption[] fits;

	// Token: 0x040005CA RID: 1482
	public CustomizationOption[] hats;

	// Token: 0x040005CB RID: 1483
	public CustomizationOption[] sashes;

	// Token: 0x040005CC RID: 1484
	public CustomizationOption goatHat;

	// Token: 0x040005CD RID: 1485
	public CustomizationOption crownHat;

	// Token: 0x02000423 RID: 1059
	public enum Type
	{
		// Token: 0x040017C7 RID: 6087
		Skin,
		// Token: 0x040017C8 RID: 6088
		Accessory = 10,
		// Token: 0x040017C9 RID: 6089
		Eyes = 20,
		// Token: 0x040017CA RID: 6090
		Mouth = 30,
		// Token: 0x040017CB RID: 6091
		Fit = 40,
		// Token: 0x040017CC RID: 6092
		Hat = 50,
		// Token: 0x040017CD RID: 6093
		Sash = 60
	}
}
