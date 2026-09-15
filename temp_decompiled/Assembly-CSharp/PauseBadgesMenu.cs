using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001D5 RID: 469
public class PauseBadgesMenu : MonoBehaviour
{
	// Token: 0x06000E6D RID: 3693 RVA: 0x000471F0 File Offset: 0x000453F0
	private void OnEnable()
	{
		int num = 0;
		int num2;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.TimesPeaked, out num2))
		{
			num = num2;
		}
		this.peaksSummitedText.text = LocalizedText.GetText("PEAKSSUMMITTTED", true).Replace("#", num.ToString() ?? "");
		this.scoutTitleText.text = this.ascentData.ascents[Singleton<AchievementManager>.Instance.GetMaxAscent()].localizedReward;
		this.badgeSashImage.color = this.ascentData.ascents[Singleton<AchievementManager>.Instance.GetMaxAscent()].color;
	}

	// Token: 0x04000C65 RID: 3173
	public Image badgeSashImage;

	// Token: 0x04000C66 RID: 3174
	public TMP_Text scoutTitleText;

	// Token: 0x04000C67 RID: 3175
	public AscentData ascentData;

	// Token: 0x04000C68 RID: 3176
	public TMP_Text peaksSummitedText;
}
