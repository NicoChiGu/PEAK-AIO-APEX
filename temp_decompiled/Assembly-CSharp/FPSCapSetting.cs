using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Settings;

// Token: 0x0200018D RID: 397
public class FPSCapSetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000C7F RID: 3199 RVA: 0x00041687 File Offset: 0x0003F887
	public override void ApplyValue()
	{
		Application.targetFrameRate = Mathf.RoundToInt(base.Value);
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00041699 File Offset: 0x0003F899
	public string GetDisplayName()
	{
		return "Max Framerate";
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x000416A0 File Offset: 0x0003F8A0
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x000416A7 File Offset: 0x0003F8A7
	protected override float GetDefaultValue()
	{
		return 400f;
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x000416AE File Offset: 0x0003F8AE
	protected override float2 GetMinMaxValue()
	{
		return new float2(30f, 600f);
	}
}
