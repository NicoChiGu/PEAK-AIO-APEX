using System;
using UnityEngine;

// Token: 0x020001A5 RID: 421
[Serializable]
public class SFX_Settings
{
	// Token: 0x04000B5C RID: 2908
	[Range(0f, 1f)]
	public float volume = 0.5f;

	// Token: 0x04000B5D RID: 2909
	[Range(0f, 1f)]
	[Tooltip("0.2 variation means random between 80% of specified volume and 100% of specified volume")]
	public float volume_Variation = 0.2f;

	// Token: 0x04000B5E RID: 2910
	public float pitch = 1f;

	// Token: 0x04000B5F RID: 2911
	[Range(0f, 1f)]
	[Tooltip("0.1 variation means random between 95% of specified volume and 105% of specified volume")]
	public float pitch_Variation = 0.1f;

	// Token: 0x04000B60 RID: 2912
	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	// Token: 0x04000B61 RID: 2913
	[Range(0f, 1f)]
	public float dopplerLevel = 1f;

	// Token: 0x04000B62 RID: 2914
	public float range = 150f;

	// Token: 0x04000B63 RID: 2915
	public float cooldown = 0.02f;

	// Token: 0x04000B64 RID: 2916
	public int maxInstances_NOT_IMPLEMENTED = 5;
}
