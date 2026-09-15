using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000190 RID: 400
public class InvertXSetting : CustomLocalizedOffOnSetting, IExposedSetting
{
	// Token: 0x06000C9A RID: 3226 RVA: 0x00041802 File Offset: 0x0003FA02
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x00041804 File Offset: 0x0003FA04
	protected override OffOnMode GetDefaultValue()
	{
		return 0;
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x00041807 File Offset: 0x0003FA07
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x0004180A File Offset: 0x0003FA0A
	public string GetDisplayName()
	{
		return "Invert X";
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x00041811 File Offset: 0x0003FA11
	public string GetCategory()
	{
		return "General";
	}
}
