using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x020001A1 RID: 417
public class VoiceVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x06000D0D RID: 3341 RVA: 0x00041F91 File Offset: 0x00040191
	public VoiceVolumeSetting(AudioMixerGroup mixerGroup)
		: base(mixerGroup)
	{
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00041F9A File Offset: 0x0004019A
	public override string GetParameterName()
	{
		return "VoiceVolume";
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00041FA1 File Offset: 0x000401A1
	public string GetDisplayName()
	{
		return "Voices Volume";
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00041FA8 File Offset: 0x000401A8
	public string GetCategory()
	{
		return "Audio";
	}
}
