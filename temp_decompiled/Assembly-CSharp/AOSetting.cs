using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000182 RID: 386
public class AOSetting : CustomLocalizedOffOnSetting, IExposedSetting
{
	// Token: 0x06000C3E RID: 3134 RVA: 0x0004144E File Offset: 0x0003F64E
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x00041450 File Offset: 0x0003F650
	protected override OffOnMode GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x0004145C File Offset: 0x0003F65C
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x0004145F File Offset: 0x0003F65F
	public string GetDisplayName()
	{
		return "Ambient Occlusion";
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x00041466 File Offset: 0x0003F666
	public string GetCategory()
	{
		return "Graphics";
	}
}
