using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001B7 RID: 439
[CreateAssetMenu(fileName = "AscentData", menuName = "Scriptable Objects/AscentData")]
public class AscentData : SingletonAsset<AscentData>
{
	// Token: 0x06000D8B RID: 3467 RVA: 0x00043D00 File Offset: 0x00041F00
	public void AddAllToCSV()
	{
		for (int i = 0; i < this.ascents.Count; i++)
		{
			LocalizedText.AppendCSVLine(this.ascents[i].title.ToUpperInvariant() + "," + this.ascents[i].title.ToUpperInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		}
		for (int j = 0; j < this.ascents.Count; j++)
		{
			LocalizedText.AppendCSVLine(this.ascents[j].titleReward.ToUpperInvariant() + "," + this.ascents[j].titleReward.ToUpperInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		}
		for (int k = 0; k < this.ascents.Count; k++)
		{
			LocalizedText.AppendCSVLine(string.Concat(new string[]
			{
				"DESC_",
				this.ascents[k].title.ToUpperInvariant(),
				",",
				this.ascents[k].description.ToUpperInvariant(),
				",,,,,,,,,,,,,ENDLINE"
			}), "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		}
	}

	// Token: 0x04000BAD RID: 2989
	public List<AscentData.AscentInstanceData> ascents;

	// Token: 0x020004A8 RID: 1192
	[Serializable]
	public class AscentInstanceData
	{
		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06001BE8 RID: 7144 RVA: 0x0008387D File Offset: 0x00081A7D
		public string localizedTitle
		{
			get
			{
				return LocalizedText.GetText(this.title, true);
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06001BE9 RID: 7145 RVA: 0x0008388B File Offset: 0x00081A8B
		public string localizedReward
		{
			get
			{
				return LocalizedText.GetText(this.titleReward, true);
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06001BEA RID: 7146 RVA: 0x00083899 File Offset: 0x00081A99
		public string localizedDescription
		{
			get
			{
				return LocalizedText.GetText(LocalizedText.GetDescriptionIndex(this.title), true);
			}
		}

		// Token: 0x040019F4 RID: 6644
		public string title;

		// Token: 0x040019F5 RID: 6645
		public string titleReward;

		// Token: 0x040019F6 RID: 6646
		public string description;

		// Token: 0x040019F7 RID: 6647
		public Color color;

		// Token: 0x040019F8 RID: 6648
		public Sprite sashSprite;
	}
}
