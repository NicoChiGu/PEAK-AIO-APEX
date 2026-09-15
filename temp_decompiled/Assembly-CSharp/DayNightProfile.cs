using System;
using UnityEngine;

// Token: 0x02000091 RID: 145
[CreateAssetMenu(fileName = "DayNightProfile", menuName = "Scriptable Objects/DayNightProfile")]
public class DayNightProfile : ScriptableObject
{
	// Token: 0x040005E4 RID: 1508
	public float sunIntensity;

	// Token: 0x040005E5 RID: 1509
	public float moonIntensity;

	// Token: 0x040005E6 RID: 1510
	public Gradient sunGradient;

	// Token: 0x040005E7 RID: 1511
	public Gradient skyTopGradient;

	// Token: 0x040005E8 RID: 1512
	public Gradient skyMidGradient;

	// Token: 0x040005E9 RID: 1513
	public Gradient skyBottomGradient;

	// Token: 0x040005EA RID: 1514
	public Gradient fogGradient;

	// Token: 0x040005EB RID: 1515
	public ShaderParameters[] globalShaderFloats;

	// Token: 0x040005EC RID: 1516
	public AnimatedShaderParameters[] animatedGlobalShaderFloats;
}
