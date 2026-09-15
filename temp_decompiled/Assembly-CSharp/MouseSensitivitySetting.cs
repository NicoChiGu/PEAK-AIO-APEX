using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x02000199 RID: 409
public class MouseSensitivitySetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000CDE RID: 3294 RVA: 0x00041CBC File Offset: 0x0003FEBC
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x00041CBE File Offset: 0x0003FEBE
	protected override float GetDefaultValue()
	{
		return 2f;
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00041CC5 File Offset: 0x0003FEC5
	protected override float2 GetMinMaxValue()
	{
		return new float2(0.1f, 5f);
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x00041CD6 File Offset: 0x0003FED6
	public string GetDisplayName()
	{
		return "Mouse Sensitivity";
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x00041CDD File Offset: 0x0003FEDD
	public string GetCategory()
	{
		return "General";
	}
}
