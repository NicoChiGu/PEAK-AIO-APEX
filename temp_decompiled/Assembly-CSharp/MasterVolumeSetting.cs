using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x02000197 RID: 407
public class MasterVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x06000CCD RID: 3277 RVA: 0x00041A43 File Offset: 0x0003FC43
	public MasterVolumeSetting(AudioMixerGroup mixerGroup)
		: base(mixerGroup)
	{
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x00041A4C File Offset: 0x0003FC4C
	public override string GetParameterName()
	{
		return "MasterVolume";
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x00041A53 File Offset: 0x0003FC53
	public string GetDisplayName()
	{
		return "Master Volume";
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x00041A5A File Offset: 0x0003FC5A
	public string GetCategory()
	{
		return "Audio";
	}
}
