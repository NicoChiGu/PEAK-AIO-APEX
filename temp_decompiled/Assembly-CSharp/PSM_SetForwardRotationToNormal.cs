using System;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class PSM_SetForwardRotationToNormal : PropSpawnerMod
{
	// Token: 0x060013AE RID: 5038 RVA: 0x00063FBC File Offset: 0x000621BC
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, Quaternion.LookRotation(spawnData.normal), Mathf.Lerp(this.minEffect, this.maxEffect, Mathf.Pow(Random.value, this.randomPow)));
	}

	// Token: 0x0400123E RID: 4670
	[Range(0f, 1f)]
	public float minEffect;

	// Token: 0x0400123F RID: 4671
	[Range(0f, 1f)]
	public float maxEffect = 1f;

	// Token: 0x04001240 RID: 4672
	public float randomPow = 1f;
}
