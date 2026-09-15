using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001A9 RID: 425
[Serializable]
public class DrawPool
{
	// Token: 0x04000B77 RID: 2935
	public Material material;

	// Token: 0x04000B78 RID: 2936
	public Mesh mesh;

	// Token: 0x04000B79 RID: 2937
	[HideInInspector]
	public Matrix4x4[] matricies;

	// Token: 0x04000B7A RID: 2938
	[FormerlySerializedAs("pool")]
	public GameObject transformsParent;
}
