using System;
using UnityEngine;

// Token: 0x020002F4 RID: 756
public class PSC_Normal : PropSpawnerConstraint
{
	// Token: 0x060013DA RID: 5082 RVA: 0x00064BE4 File Offset: 0x00062DE4
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		float num = Vector3.Angle(Vector3.up, spawnData.normal);
		return num < this.maxAngle && num > this.minAngle;
	}

	// Token: 0x04001272 RID: 4722
	public float minAngle;

	// Token: 0x04001273 RID: 4723
	public float maxAngle = 50f;
}
