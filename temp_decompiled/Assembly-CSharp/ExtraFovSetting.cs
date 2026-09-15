using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x0200018B RID: 395
public class ExtraFovSetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000C73 RID: 3187 RVA: 0x00041627 File Offset: 0x0003F827
	public override void ApplyValue()
	{
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x00041629 File Offset: 0x0003F829
	protected override float GetDefaultValue()
	{
		return 40f;
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x00041630 File Offset: 0x0003F830
	protected override float2 GetMinMaxValue()
	{
		return new float2(0f, 50f);
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x00041641 File Offset: 0x0003F841
	public string GetDisplayName()
	{
		return "Climbing extra field of view";
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00041648 File Offset: 0x0003F848
	public string GetCategory()
	{
		return "General";
	}
}
