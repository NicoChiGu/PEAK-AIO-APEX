using System;
using UnityEngine;

// Token: 0x0200024E RID: 590
[ExecuteAlways]
public class DistanceCheck : MonoBehaviour
{
	// Token: 0x06001112 RID: 4370 RVA: 0x00055FDF File Offset: 0x000541DF
	private void Update()
	{
		if (this.object1 == null || this.object2 == null)
		{
			return;
		}
		this.distance = Vector3.Distance(this.object1.position, this.object2.position);
	}

	// Token: 0x04000F7F RID: 3967
	public Transform object1;

	// Token: 0x04000F80 RID: 3968
	public Transform object2;

	// Token: 0x04000F81 RID: 3969
	public float distance;
}
