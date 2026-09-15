using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

// Token: 0x02000198 RID: 408
public class MicrophoneSetting : Setting, IEnumSetting, IExposedSetting
{
	// Token: 0x06000CD1 RID: 3281 RVA: 0x00041A64 File Offset: 0x0003FC64
	public List<MicrophoneSetting.MicrophoneInfo> GetChoices()
	{
		string[] devices = Microphone.devices;
		List<MicrophoneSetting.MicrophoneInfo> list = new List<MicrophoneSetting.MicrophoneInfo>();
		foreach (string text in devices)
		{
			list.Add(new MicrophoneSetting.MicrophoneInfo
			{
				id = text,
				name = text
			});
		}
		return list;
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00041AB0 File Offset: 0x0003FCB0
	public override void Load(ISettingsSaveLoad loader)
	{
		string value;
		if (loader.TryLoadString(base.GetType(), ref value))
		{
			List<MicrophoneSetting.MicrophoneInfo> choices = this.GetChoices();
			this.Value = choices.Find((MicrophoneSetting.MicrophoneInfo x) => x.id == value);
			if (string.IsNullOrEmpty(this.Value.id))
			{
				Debug.LogWarning("Failed to load setting of type " + base.GetType().FullName + " from PlayerPrefs. Value not found in choices.");
				this.Value = this.GetDefaultValue();
				return;
			}
		}
		else
		{
			Debug.LogWarning("Failed to load setting of type " + base.GetType().FullName + " from PlayerPrefs.");
			this.Value = this.GetDefaultValue();
		}
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x00041B60 File Offset: 0x0003FD60
	private MicrophoneSetting.MicrophoneInfo GetDefaultValue()
	{
		if (this.GetChoices().Count == 0)
		{
			Debug.LogError("No voice devices found.");
			return default(MicrophoneSetting.MicrophoneInfo);
		}
		return Enumerable.First<MicrophoneSetting.MicrophoneInfo>(this.GetChoices());
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x00041B99 File Offset: 0x0003FD99
	public override void Save(ISettingsSaveLoad saver)
	{
		saver.SaveString(base.GetType(), this.Value.id);
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x00041BB4 File Offset: 0x0003FDB4
	public override void ApplyValue()
	{
		string text = "Voice setting applied: ";
		MicrophoneSetting.MicrophoneInfo value = this.Value;
		Debug.Log(text + value.ToString());
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x00041BE4 File Offset: 0x0003FDE4
	public override SettingUI GetDebugUI(ISettingHandler settingHandler)
	{
		return new EnumSettingsUI(this, settingHandler);
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00041BED File Offset: 0x0003FDED
	public override GameObject GetSettingUICell()
	{
		return SingletonAsset<InputCellMapper>.Instance.EnumSettingCell;
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00041BF9 File Offset: 0x0003FDF9
	public List<string> GetUnlocalizedChoices()
	{
		return Enumerable.ToList<string>(Enumerable.Select<MicrophoneSetting.MicrophoneInfo, string>(this.GetChoices(), (MicrophoneSetting.MicrophoneInfo info) => info.name));
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x00041C2C File Offset: 0x0003FE2C
	public int GetValue()
	{
		return Enumerable.ToList<string>(Enumerable.Select<MicrophoneSetting.MicrophoneInfo, string>(this.GetChoices(), (MicrophoneSetting.MicrophoneInfo info) => info.id)).IndexOf(this.Value.id);
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00041C78 File Offset: 0x0003FE78
	public void SetValue(int v, ISettingHandler settingHandler, bool fromUI)
	{
		MicrophoneSetting.MicrophoneInfo microphoneInfo = this.GetChoices()[v];
		this.Value = microphoneInfo;
		this.ApplyValue();
		settingHandler.SaveSetting(this);
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00041CA6 File Offset: 0x0003FEA6
	public string GetDisplayName()
	{
		return "Microphone";
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00041CAD File Offset: 0x0003FEAD
	public string GetCategory()
	{
		return "Audio";
	}

	// Token: 0x04000B55 RID: 2901
	public MicrophoneSetting.MicrophoneInfo Value;

	// Token: 0x02000493 RID: 1171
	public struct MicrophoneInfo
	{
		// Token: 0x06001BA5 RID: 7077 RVA: 0x00082CB1 File Offset: 0x00080EB1
		public override string ToString()
		{
			return this.id + " (" + this.name + ")";
		}

		// Token: 0x0400199C RID: 6556
		public string id;

		// Token: 0x0400199D RID: 6557
		public string name;
	}
}
