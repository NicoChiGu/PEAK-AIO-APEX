using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using Zorro.Settings;

// Token: 0x020001A0 RID: 416
public class TextureQualitySetting : CustomLocalizedEnumSetting<TextureQualitySetting.TextureQuality>, IExposedSetting
{
	// Token: 0x06000D07 RID: 3335 RVA: 0x00041F20 File Offset: 0x00040120
	public override void ApplyValue()
	{
		RenderPipelineAsset currentRenderPipeline = GraphicsSettings.currentRenderPipeline;
		switch (base.Value)
		{
		case TextureQualitySetting.TextureQuality.Native:
			QualitySettings.globalTextureMipmapLimit = 0;
			return;
		case TextureQualitySetting.TextureQuality.High:
			QualitySettings.globalTextureMipmapLimit = 1;
			return;
		case TextureQualitySetting.TextureQuality.Medium:
			QualitySettings.globalTextureMipmapLimit = 2;
			return;
		case TextureQualitySetting.TextureQuality.Low:
			QualitySettings.globalTextureMipmapLimit = 3;
			return;
		default:
			return;
		}
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x00041F6C File Offset: 0x0004016C
	protected override TextureQualitySetting.TextureQuality GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return TextureQualitySetting.TextureQuality.High;
		}
		return TextureQualitySetting.TextureQuality.Native;
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x00041F78 File Offset: 0x00040178
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x00041F7B File Offset: 0x0004017B
	public string GetDisplayName()
	{
		return "Texture Quality";
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00041F82 File Offset: 0x00040182
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x02000499 RID: 1177
	public enum TextureQuality
	{
		// Token: 0x040019B1 RID: 6577
		Native,
		// Token: 0x040019B2 RID: 6578
		High,
		// Token: 0x040019B3 RID: 6579
		Medium,
		// Token: 0x040019B4 RID: 6580
		Low
	}
}
