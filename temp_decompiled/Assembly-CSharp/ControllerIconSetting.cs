using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000187 RID: 391
public class ControllerIconSetting : CustomLocalizedEnumSetting<ControllerIconSetting.IconMode>, IExposedSetting
{
	// Token: 0x06000C5F RID: 3167 RVA: 0x0004150B File Offset: 0x0003F70B
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x0004150D File Offset: 0x0003F70D
	protected override ControllerIconSetting.IconMode GetDefaultValue()
	{
		return ControllerIconSetting.IconMode.Auto;
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x00041510 File Offset: 0x0003F710
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x00041513 File Offset: 0x0003F713
	public string GetDisplayName()
	{
		return "INPUTICONS";
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x0004151A File Offset: 0x0003F71A
	public string GetCategory()
	{
		return "General";
	}

	// Token: 0x0200048C RID: 1164
	public enum IconMode
	{
		// Token: 0x0400197C RID: 6524
		Auto,
		// Token: 0x0400197D RID: 6525
		Style1,
		// Token: 0x0400197E RID: 6526
		Style2,
		// Token: 0x0400197F RID: 6527
		KBM
	}
}
