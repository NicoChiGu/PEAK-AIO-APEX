using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000183 RID: 387
public class AudioModeSetting : EnumSetting<AudioModeSetting.AudioMode>, IExposedSetting
{
	// Token: 0x06000C44 RID: 3140 RVA: 0x00041475 File Offset: 0x0003F675
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x00041477 File Offset: 0x0003F677
	protected override AudioModeSetting.AudioMode GetDefaultValue()
	{
		return AudioModeSetting.AudioMode.Stereo;
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x0004147A File Offset: 0x0003F67A
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x0004147D File Offset: 0x0003F67D
	public string GetDisplayName()
	{
		return "Audio Mode";
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00041484 File Offset: 0x0003F684
	public string GetCategory()
	{
		return "Audio";
	}

	// Token: 0x0200048B RID: 1163
	public enum AudioMode
	{
		// Token: 0x04001979 RID: 6521
		Stereo,
		// Token: 0x0400197A RID: 6522
		Mono
	}
}
