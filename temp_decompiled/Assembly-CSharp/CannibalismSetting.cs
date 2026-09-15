using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000185 RID: 389
public class CannibalismSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000C51 RID: 3153 RVA: 0x000414BB File Offset: 0x0003F6BB
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x000414BD File Offset: 0x0003F6BD
	protected override OffOnMode GetDefaultValue()
	{
		return 1;
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x000414C0 File Offset: 0x0003F6C0
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x000414C3 File Offset: 0x0003F6C3
	public string GetDisplayName()
	{
		return "ENABLECANNIBALISM";
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x000414CA File Offset: 0x0003F6CA
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x000414D1 File Offset: 0x0003F6D1
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}
