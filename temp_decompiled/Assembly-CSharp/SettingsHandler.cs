using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Settings;

// Token: 0x020001A3 RID: 419
public class SettingsHandler : ISettingHandler
{
	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x06000D18 RID: 3352 RVA: 0x00041FF9 File Offset: 0x000401F9
	public static bool IsOnSteamDeck
	{
		get
		{
			return !Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "NoSteam") && SteamUtils.IsSteamRunningOnSteamDeck();
		}
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x00042014 File Offset: 0x00040214
	public SettingsHandler()
	{
		this.settings = new List<Setting>(30);
		this._settingsSaveLoad = new DefaultSettingsSaveLoad();
		this.AddSetting(new LanguageSetting());
		this.AddSetting(new FovSetting());
		this.AddSetting(new ExtraFovSetting());
		this.AddSetting(new FullscreenEnumSetting());
		this.AddSetting(new ResolutionSetting());
		this.AddSetting(new FPSCapSetting());
		this.AddSetting(new VSyncSetting());
		this.AddSetting(new MicrophoneSetting());
		this.AddSetting(new RenderScaleSetting());
		this.AddSetting(new ShadowDistanceSettings());
		this.AddSetting(new TextureQualitySetting());
		this.AddSetting(new PushToTalkSetting());
		this.AddSetting(new MasterVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new SFXVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new MusicVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new VoiceVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new MouseSensitivitySetting());
		this.AddSetting(new ControllerSensitivitySetting());
		this.AddSetting(new LodQuality());
		this.AddSetting(new AOSetting());
		this.AddSetting(new ControllerIconSetting());
		this.AddSetting(new InvertXSetting());
		this.AddSetting(new InvertYSetting());
		this.AddSetting(new JumpToClimbSetting());
		this.AddSetting(new LobbyTypeSetting());
		this.AddSetting(new HeadBobSetting());
		this.AddSetting(new CannibalismSetting());
		this.AddSetting(new BugPhobiaSetting());
		this.AddSetting(new PhotosensitiveSetting());
		this.AddSetting(new ColorblindSetting());
		this.AddSetting(new LookerSetting());
		DebugUIHandler instance = Singleton<DebugUIHandler>.Instance;
		if (instance != null)
		{
			instance.RegisterPage("Settings", () => new SettingsPage(this.settings, this));
		}
		SettingsHandler.Instance = this;
		Debug.Log("Settings Initlaized");
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x000421ED File Offset: 0x000403ED
	public void AddSetting(Setting setting)
	{
		this.settings.Add(setting);
		setting.Load(this._settingsSaveLoad);
		setting.ApplyValue();
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x0004220D File Offset: 0x0004040D
	public void SaveSetting(Setting setting)
	{
		setting.Save(this._settingsSaveLoad);
		this._settingsSaveLoad.WriteToDisk();
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x00042228 File Offset: 0x00040428
	public T GetSetting<T>() where T : Setting
	{
		foreach (Setting setting in this.settings)
		{
			T t = setting as T;
			if (t != null)
			{
				return t;
			}
		}
		return default(T);
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00042298 File Offset: 0x00040498
	public IEnumerable<Setting> GetAllSettings()
	{
		return this.settings;
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x000422A0 File Offset: 0x000404A0
	public void Update()
	{
		foreach (Setting setting in this.settings)
		{
			setting.Update();
		}
	}

	// Token: 0x04000B56 RID: 2902
	private List<Setting> settings;

	// Token: 0x04000B57 RID: 2903
	private ISettingsSaveLoad _settingsSaveLoad;

	// Token: 0x04000B58 RID: 2904
	public static SettingsHandler Instance;
}
