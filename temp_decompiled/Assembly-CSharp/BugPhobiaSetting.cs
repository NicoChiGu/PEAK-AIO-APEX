using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000184 RID: 388
public class BugPhobiaSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000C4A RID: 3146 RVA: 0x00041493 File Offset: 0x0003F693
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x00041495 File Offset: 0x0003F695
	protected override OffOnMode GetDefaultValue()
	{
		return 0;
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00041498 File Offset: 0x0003F698
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x0004149B File Offset: 0x0003F69B
	public string GetDisplayName()
	{
		return "BUGPHOBIAMODE";
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x000414A2 File Offset: 0x0003F6A2
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x000414A9 File Offset: 0x0003F6A9
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}
