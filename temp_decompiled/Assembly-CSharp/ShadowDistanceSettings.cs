using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zorro.Settings;

// Token: 0x0200019F RID: 415
public class ShadowDistanceSettings : CustomLocalizedEnumSetting<ShadowDistanceSettings.ShadowDistanceQuality>, IExposedSetting
{
	// Token: 0x06000D01 RID: 3329 RVA: 0x00041E74 File Offset: 0x00040074
	public override void ApplyValue()
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
		if (universalRenderPipelineAsset != null)
		{
			switch (base.Value)
			{
			case ShadowDistanceSettings.ShadowDistanceQuality.High:
				universalRenderPipelineAsset.shadowDistance = 200f;
				universalRenderPipelineAsset.shadowCascadeCount = 2;
				return;
			case ShadowDistanceSettings.ShadowDistanceQuality.Medium:
				universalRenderPipelineAsset.shadowDistance = 150f;
				universalRenderPipelineAsset.shadowCascadeCount = 2;
				return;
			case ShadowDistanceSettings.ShadowDistanceQuality.Low:
				universalRenderPipelineAsset.shadowDistance = 75f;
				universalRenderPipelineAsset.shadowCascadeCount = 1;
				return;
			case ShadowDistanceSettings.ShadowDistanceQuality.Off:
				universalRenderPipelineAsset.shadowDistance = 0f;
				universalRenderPipelineAsset.shadowCascadeCount = 1;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00041EF8 File Offset: 0x000400F8
	protected override ShadowDistanceSettings.ShadowDistanceQuality GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return ShadowDistanceSettings.ShadowDistanceQuality.Low;
		}
		return ShadowDistanceSettings.ShadowDistanceQuality.Medium;
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00041F04 File Offset: 0x00040104
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00041F07 File Offset: 0x00040107
	public string GetDisplayName()
	{
		return "Shadow Distance";
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x00041F0E File Offset: 0x0004010E
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x02000498 RID: 1176
	public enum ShadowDistanceQuality
	{
		// Token: 0x040019AC RID: 6572
		High,
		// Token: 0x040019AD RID: 6573
		Medium,
		// Token: 0x040019AE RID: 6574
		Low,
		// Token: 0x040019AF RID: 6575
		Off
	}
}
