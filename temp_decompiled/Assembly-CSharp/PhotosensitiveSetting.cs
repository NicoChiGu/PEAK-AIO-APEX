using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200019B RID: 411
public class PhotosensitiveSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000CE8 RID: 3304 RVA: 0x00041D0A File Offset: 0x0003FF0A
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00041D0C File Offset: 0x0003FF0C
	protected override OffOnMode GetDefaultValue()
	{
		return 0;
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00041D0F File Offset: 0x0003FF0F
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x00041D12 File Offset: 0x0003FF12
	public string GetDisplayName()
	{
		return "PHOTOSENSITIVEMODE";
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00041D19 File Offset: 0x0003FF19
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x00041D20 File Offset: 0x0003FF20
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}
