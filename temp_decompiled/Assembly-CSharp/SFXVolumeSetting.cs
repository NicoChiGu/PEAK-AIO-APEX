using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x0200019E RID: 414
public class SFXVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x06000CFD RID: 3325 RVA: 0x00041E55 File Offset: 0x00040055
	public SFXVolumeSetting(AudioMixerGroup mixerGroup)
		: base(mixerGroup)
	{
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x00041E5E File Offset: 0x0004005E
	public override string GetParameterName()
	{
		return "SFXVolume";
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x00041E65 File Offset: 0x00040065
	public string GetDisplayName()
	{
		return "SFX Volume";
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x00041E6C File Offset: 0x0004006C
	public string GetCategory()
	{
		return "Audio";
	}
}
