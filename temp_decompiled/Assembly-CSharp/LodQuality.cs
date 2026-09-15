using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000195 RID: 405
public class LodQuality : CustomLocalizedEnumSetting<LodQuality.Quality>, IExposedSetting
{
	// Token: 0x06000CBF RID: 3263 RVA: 0x000419C8 File Offset: 0x0003FBC8
	public override void ApplyValue()
	{
		QualitySettings.lodBias = this.GetBias(base.Value);
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x000419DB File Offset: 0x0003FBDB
	private float GetBias(LodQuality.Quality value)
	{
		if (value == LodQuality.Quality.High)
		{
			return 1f;
		}
		if (value == LodQuality.Quality.Medium)
		{
			return 0.85f;
		}
		return 0.7f;
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x000419F6 File Offset: 0x0003FBF6
	protected override LodQuality.Quality GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return LodQuality.Quality.Low;
		}
		return LodQuality.Quality.Medium;
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x00041A02 File Offset: 0x0003FC02
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x00041A05 File Offset: 0x0003FC05
	public string GetDisplayName()
	{
		return "World Quality";
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x00041A0C File Offset: 0x0003FC0C
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x02000492 RID: 1170
	public enum Quality
	{
		// Token: 0x04001999 RID: 6553
		Low,
		// Token: 0x0400199A RID: 6554
		Medium,
		// Token: 0x0400199B RID: 6555
		High
	}
}
