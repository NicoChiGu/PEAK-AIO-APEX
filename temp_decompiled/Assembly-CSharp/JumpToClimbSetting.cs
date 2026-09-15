using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000192 RID: 402
public class JumpToClimbSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000CA6 RID: 3238 RVA: 0x0004183E File Offset: 0x0003FA3E
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x00041840 File Offset: 0x0003FA40
	protected override OffOnMode GetDefaultValue()
	{
		return 1;
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x00041843 File Offset: 0x0003FA43
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x00041846 File Offset: 0x0003FA46
	public string GetDisplayName()
	{
		return "JUMPTOCLIMB";
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0004184D File Offset: 0x0003FA4D
	public string GetCategory()
	{
		return "General";
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x00041854 File Offset: 0x0003FA54
	public bool ShouldShow()
	{
		return true;
	}
}
