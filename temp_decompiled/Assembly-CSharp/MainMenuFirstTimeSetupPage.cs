using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Settings.UI;
using Zorro.UI;

// Token: 0x020001CB RID: 459
public class MainMenuFirstTimeSetupPage : UIPage, INavigationPage
{
	// Token: 0x06000E1D RID: 3613 RVA: 0x000467D4 File Offset: 0x000449D4
	public void Start()
	{
		SettingsHandler instance = SettingsHandler.Instance;
		MicrophoneSetting setting = instance.GetSetting<MicrophoneSetting>();
		this.MicSettingUI.Setup(setting, instance);
		this.ContinueButton.onClick.AddListener(new UnityAction(this.ContinueClicked));
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x00046817 File Offset: 0x00044A17
	private void ContinueClicked()
	{
		this.pageHandler.TransistionToPage<MainMenuMainPage>();
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x00046825 File Offset: 0x00044A25
	public GameObject GetFirstSelectedGameObject()
	{
		return this.MicSettingUI.dropdown.gameObject;
	}

	// Token: 0x04000C4C RID: 3148
	public EnumSettingUI MicSettingUI;

	// Token: 0x04000C4D RID: 3149
	public Button ContinueButton;
}
