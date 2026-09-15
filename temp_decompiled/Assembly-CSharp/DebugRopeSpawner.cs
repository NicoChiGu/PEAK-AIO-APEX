using System;
using UnityEngine;

// Token: 0x02000241 RID: 577
public class DebugRopeSpawner : MonoBehaviour
{
	// Token: 0x060010F0 RID: 4336 RVA: 0x00055A38 File Offset: 0x00053C38
	public void Spawn()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.segments; j++)
		{
			GameObject gameObject = HelperFunctions.SpawnPrefab(this.ropeSegment, base.transform.position + base.transform.up * -this.spacing * (float)j, base.transform.rotation, base.transform);
			if (j > 0)
			{
				gameObject.GetComponent<ConfigurableJoint>().connectedBody = base.transform.GetChild(j - 1).GetComponent<Rigidbody>();
			}
		}
	}

	// Token: 0x04000F6A RID: 3946
	public GameObject ropeSegment;

	// Token: 0x04000F6B RID: 3947
	public int segments = 10;

	// Token: 0x04000F6C RID: 3948
	public float spacing = 0.4f;
}
