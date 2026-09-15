using System;
using UnityEngine;

// Token: 0x0200016B RID: 363
[Serializable]
public class SpawnObject
{
	// Token: 0x04000A9A RID: 2714
	public int maxCount;

	// Token: 0x04000A9B RID: 2715
	public GameObject prefab;

	// Token: 0x04000A9C RID: 2716
	public Vector3 inversion;

	// Token: 0x04000A9D RID: 2717
	public Vector3 randomRot;

	// Token: 0x04000A9E RID: 2718
	public Vector3 randomScale;

	// Token: 0x04000A9F RID: 2719
	public float uniformScale;

	// Token: 0x04000AA0 RID: 2720
	public float scaleMultiplier = 1f;

	// Token: 0x04000AA1 RID: 2721
	public Vector3 posJitter;
}
