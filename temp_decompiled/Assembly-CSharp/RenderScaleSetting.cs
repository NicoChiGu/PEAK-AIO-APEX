using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zorro.Settings;

// Token: 0x0200019D RID: 413
public class RenderScaleSetting : CustomLocalizedEnumSetting<RenderScaleSetting.RenderScaleQuality>, IExposedSetting
{
	// Token: 0x06000CF6 RID: 3318 RVA: 0x00041D78 File Offset: 0x0003FF78
	public override void ApplyValue()
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
		if (universalRenderPipelineAsset != null)
		{
			universalRenderPipelineAsset.renderScale = this.GetRenderScale(base.Value);
			Debug.Log(string.Format("Set Render Scale: {0}", universalRenderPipelineAsset.renderScale));
			if (base.Value == RenderScaleSetting.RenderScaleQuality.Native)
			{
				universalRenderPipelineAsset.upscalingFilter = 1;
				return;
			}
			universalRenderPipelineAsset.upscalingFilter = 4;
		}
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x00041DD8 File Offset: 0x0003FFD8
	public float GetRenderScale(RenderScaleSetting.RenderScaleQuality quality)
	{
		float num;
		switch (quality)
		{
		case RenderScaleSetting.RenderScaleQuality.Native:
			num = 1f;
			break;
		case RenderScaleSetting.RenderScaleQuality.High:
			num = 0.8f;
			break;
		case RenderScaleSetting.RenderScaleQuality.Medium:
			num = 0.4f;
			break;
		case RenderScaleSetting.RenderScaleQuality.Low:
			num = 0.2f;
			break;
		default:
			throw new ArgumentOutOfRangeException("quality", quality, null);
		}
		return num;
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x00041E30 File Offset: 0x00040030
	protected override RenderScaleSetting.RenderScaleQuality GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return RenderScaleSetting.RenderScaleQuality.Medium;
		}
		return RenderScaleSetting.RenderScaleQuality.High;
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x00041E3C File Offset: 0x0004003C
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x00041E3F File Offset: 0x0004003F
	public string GetDisplayName()
	{
		return "Render Scale";
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x00041E46 File Offset: 0x00040046
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x02000497 RID: 1175
	public enum RenderScaleQuality
	{
		// Token: 0x040019A7 RID: 6567
		Native,
		// Token: 0x040019A8 RID: 6568
		High,
		// Token: 0x040019A9 RID: 6569
		Medium,
		// Token: 0x040019AA RID: 6570
		Low
	}
}
