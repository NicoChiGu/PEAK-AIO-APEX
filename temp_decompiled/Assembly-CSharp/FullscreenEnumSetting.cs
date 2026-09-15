using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Device;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

// Token: 0x0200018E RID: 398
public class FullscreenEnumSetting : Setting, IEnumSetting, IExposedSetting, ICustomLocalizedEnumSetting
{
	// Token: 0x06000C85 RID: 3205 RVA: 0x000416C7 File Offset: 0x0003F8C7
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x000416C9 File Offset: 0x0003F8C9
	public override SettingUI GetDebugUI(ISettingHandler settingHandler)
	{
		return new EnumSettingsUI(this, settingHandler);
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x000416D2 File Offset: 0x0003F8D2
	public override GameObject GetSettingUICell()
	{
		return SingletonAsset<InputCellMapper>.Instance.EnumSettingCell;
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x000416DE File Offset: 0x0003F8DE
	public override void Load(ISettingsSaveLoad loader)
	{
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x000416E0 File Offset: 0x0003F8E0
	public override void Save(ISettingsSaveLoad saver)
	{
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x000416E2 File Offset: 0x0003F8E2
	public string GetDisplayName()
	{
		return "Window Mode";
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x000416E9 File Offset: 0x0003F8E9
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x000416F0 File Offset: 0x0003F8F0
	public List<string> GetUnlocalizedChoices()
	{
		List<string> list = new List<string>();
		list.Add("Windowed");
		list.Add("Fullscreen");
		list.Add("Windowed Fullscreen");
		return list;
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x00041718 File Offset: 0x0003F918
	public int GetValue()
	{
		switch (Screen.fullScreenMode)
		{
		case 0:
			return 1;
		case 1:
			return 2;
		case 3:
			return 0;
		}
		return 0;
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0004174A File Offset: 0x0003F94A
	public void SetValue(int v, ISettingHandler settingHandler, bool fromUI)
	{
		switch (v)
		{
		case 0:
			Screen.fullScreenMode = 3;
			return;
		case 1:
			Screen.fullScreenMode = 0;
			return;
		case 2:
			Screen.fullScreenMode = 1;
			return;
		default:
			return;
		}
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x00041773 File Offset: 0x0003F973
	public List<string> GetCustomLocalizedChoices()
	{
		return Enumerable.ToList<string>(Enumerable.Select<string, string>(this.GetUnlocalizedChoices(), (string s) => LocalizedText.GetText(s, true)));
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x000417A4 File Offset: 0x0003F9A4
	public void DeregisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Remove(LocalizedText.OnLangugageChanged, action);
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x000417BB File Offset: 0x0003F9BB
	public void RegisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Combine(LocalizedText.OnLangugageChanged, action);
	}
}
