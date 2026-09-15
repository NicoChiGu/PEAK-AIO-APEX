using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x02000188 RID: 392
public class ControllerSensitivitySetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000C65 RID: 3173 RVA: 0x00041529 File Offset: 0x0003F729
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x0004152B File Offset: 0x0003F72B
	protected override float GetDefaultValue()
	{
		return 2f;
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x00041532 File Offset: 0x0003F732
	protected override float2 GetMinMaxValue()
	{
		return new float2(0.1f, 5f);
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x00041543 File Offset: 0x0003F743
	public string GetDisplayName()
	{
		return "Controller Sensitivity";
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x0004154A File Offset: 0x0003F74A
	public string GetCategory()
	{
		return "General";
	}
}
