using System;
using UnityEngine;
using Zorro.UI;

// Token: 0x020001CE RID: 462
public class MainMenuPageSelector : UIPageHandlerStartPageSelector
{
	// Token: 0x06000E32 RID: 3634 RVA: 0x00046B58 File Offset: 0x00044D58
	public override UIPage GetStartPage()
	{
		string text = "FirstTimeStartup2";
		if (PlayerPrefs.HasKey(text))
		{
			return this.mainPage;
		}
		PlayerPrefs.SetInt(text, 1);
		PlayerPrefs.Save();
		return this.firstTimeSetupPage;
	}

	// Token: 0x04000C55 RID: 3157
	public MainMenuMainPage mainPage;

	// Token: 0x04000C56 RID: 3158
	public MainMenuFirstTimeSetupPage firstTimeSetupPage;
}
