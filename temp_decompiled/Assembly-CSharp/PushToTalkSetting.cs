using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200019C RID: 412
public class PushToTalkSetting : CustomLocalizedEnumSetting<PushToTalkSetting.PushToTalkType>, IExposedSetting
{
	// Token: 0x06000CEF RID: 3311 RVA: 0x00041D32 File Offset: 0x0003FF32
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x00041D34 File Offset: 0x0003FF34
	protected override PushToTalkSetting.PushToTalkType GetDefaultValue()
	{
		return PushToTalkSetting.PushToTalkType.VoiceActivation;
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x00041D37 File Offset: 0x0003FF37
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x00041D3A File Offset: 0x0003FF3A
	public string GetDisplayName()
	{
		return "Microphone mode";
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x00041D41 File Offset: 0x0003FF41
	public string GetCategory()
	{
		return "Audio";
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x00041D48 File Offset: 0x0003FF48
	public override List<string> GetUnlocalizedChoices()
	{
		List<string> list = new List<string>();
		list.Add("Voice Activation");
		list.Add("Push To Talk");
		list.Add("Push To Mute");
		return list;
	}

	// Token: 0x02000496 RID: 1174
	public enum PushToTalkType
	{
		// Token: 0x040019A3 RID: 6563
		VoiceActivation,
		// Token: 0x040019A4 RID: 6564
		PushToTalk,
		// Token: 0x040019A5 RID: 6565
		PushToMute
	}
}
