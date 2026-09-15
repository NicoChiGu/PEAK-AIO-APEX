using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000191 RID: 401
public class InvertYSetting : CustomLocalizedOffOnSetting, IExposedSetting
{
	// Token: 0x06000CA0 RID: 3232 RVA: 0x00041820 File Offset: 0x0003FA20
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x00041822 File Offset: 0x0003FA22
	protected override OffOnMode GetDefaultValue()
	{
		return 0;
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x00041825 File Offset: 0x0003FA25
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x00041828 File Offset: 0x0003FA28
	public string GetDisplayName()
	{
		return "Invert Y";
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0004182F File Offset: 0x0003FA2F
	public string GetCategory()
	{
		return "General";
	}
}
