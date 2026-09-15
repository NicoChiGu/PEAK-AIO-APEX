using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200018F RID: 399
public class HeadBobSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000C93 RID: 3219 RVA: 0x000417DA File Offset: 0x0003F9DA
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x000417DC File Offset: 0x0003F9DC
	protected override OffOnMode GetDefaultValue()
	{
		return 0;
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x000417DF File Offset: 0x0003F9DF
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x000417E2 File Offset: 0x0003F9E2
	public string GetDisplayName()
	{
		return "Reduce Camera Bobbing";
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x000417E9 File Offset: 0x0003F9E9
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x000417F0 File Offset: 0x0003F9F0
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}
