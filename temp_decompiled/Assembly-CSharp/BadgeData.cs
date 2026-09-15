using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001BA RID: 442
[CreateAssetMenu(fileName = "BadgeData", menuName = "Scriptable Objects/BadgeData")]
public class BadgeData : ScriptableObject
{
	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x000448AE File Offset: 0x00042AAE
	public bool IsLocked
	{
		get
		{
			return !Singleton<AchievementManager>.Instance.IsAchievementUnlocked(this.linkedAchievement);
		}
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x000448C4 File Offset: 0x00042AC4
	public void AddToCSV()
	{
		string text = string.Concat(new string[]
		{
			"NAME_",
			this.displayName.ToUpperInvariant(),
			",",
			this.displayName.ToUpperInvariant(),
			" BADGE,,,,,,,,,,,,,ENDLINE"
		});
		string text2 = string.Concat(new string[]
		{
			"DESC_",
			this.displayName.ToUpperInvariant(),
			",\"",
			this.description,
			"\",,,,,,,,,,,,,,ENDLINE"
		});
		LocalizedText.AppendCSVLine(text, "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		LocalizedText.AppendCSVLine(text2, "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
	}

	// Token: 0x04000BBD RID: 3005
	public Texture icon;

	// Token: 0x04000BBE RID: 3006
	public string displayName;

	// Token: 0x04000BBF RID: 3007
	public string description;

	// Token: 0x04000BC0 RID: 3008
	public ACHIEVEMENTTYPE linkedAchievement;

	// Token: 0x04000BC1 RID: 3009
	public bool testLocked;

	// Token: 0x04000BC2 RID: 3010
	public int visualID;
}
