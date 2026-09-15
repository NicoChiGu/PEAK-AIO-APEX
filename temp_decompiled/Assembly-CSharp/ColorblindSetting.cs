using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000186 RID: 390
public class ColorblindSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000C58 RID: 3160 RVA: 0x000414E3 File Offset: 0x0003F6E3
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x000414E5 File Offset: 0x0003F6E5
	protected override OffOnMode GetDefaultValue()
	{
		return 0;
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x000414E8 File Offset: 0x0003F6E8
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x000414EB File Offset: 0x0003F6EB
	public string GetDisplayName()
	{
		return "COLORBLINDNESSMODE";
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x000414F2 File Offset: 0x0003F6F2
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x000414F9 File Offset: 0x0003F6F9
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}
