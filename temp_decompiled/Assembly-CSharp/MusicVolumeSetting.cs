using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x0200019A RID: 410
public class MusicVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x06000CE4 RID: 3300 RVA: 0x00041CEC File Offset: 0x0003FEEC
	public MusicVolumeSetting(AudioMixerGroup mixerGroup)
		: base(mixerGroup)
	{
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x00041CF5 File Offset: 0x0003FEF5
	public override string GetParameterName()
	{
		return "MusicVolume";
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00041CFC File Offset: 0x0003FEFC
	public string GetDisplayName()
	{
		return "Music Volume";
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x00041D03 File Offset: 0x0003FF03
	public string GetCategory()
	{
		return "Audio";
	}
}
