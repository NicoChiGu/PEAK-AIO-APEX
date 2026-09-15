using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x020001A2 RID: 418
public class VSyncSetting : CustomLocalizedEnumSetting<VSyncSetting.VSyncMode>, IExposedSetting
{
	// Token: 0x06000D11 RID: 3345 RVA: 0x00041FAF File Offset: 0x000401AF
	public override void ApplyValue()
	{
		QualitySettings.vSyncCount = (int)base.Value;
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00041FBC File Offset: 0x000401BC
	protected override VSyncSetting.VSyncMode GetDefaultValue()
	{
		return (VSyncSetting.VSyncMode)QualitySettings.vSyncCount;
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00041FC3 File Offset: 0x000401C3
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00041FC6 File Offset: 0x000401C6
	public string GetDisplayName()
	{
		return "Vsync";
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x00041FCD File Offset: 0x000401CD
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x00041FD4 File Offset: 0x000401D4
	public override List<string> GetUnlocalizedChoices()
	{
		List<string> list = new List<string>();
		list.Add("OFF");
		list.Add("ON");
		return list;
	}

	// Token: 0x0200049A RID: 1178
	public enum VSyncMode
	{
		// Token: 0x040019B6 RID: 6582
		None,
		// Token: 0x040019B7 RID: 6583
		Enabled
	}
}
