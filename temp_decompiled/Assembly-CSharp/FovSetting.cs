using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x0200018C RID: 396
public class FovSetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000C79 RID: 3193 RVA: 0x00041657 File Offset: 0x0003F857
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x00041659 File Offset: 0x0003F859
	protected override float GetDefaultValue()
	{
		return 70f;
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00041660 File Offset: 0x0003F860
	protected override float2 GetMinMaxValue()
	{
		return new float2(60f, 100f);
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x00041671 File Offset: 0x0003F871
	public string GetDisplayName()
	{
		return "Field of view";
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x00041678 File Offset: 0x0003F878
	public string GetCategory()
	{
		return "General";
	}
}
