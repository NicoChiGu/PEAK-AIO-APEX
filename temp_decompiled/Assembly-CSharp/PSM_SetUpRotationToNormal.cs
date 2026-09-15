using System;
using UnityEngine;

// Token: 0x020002DF RID: 735
public class PSM_SetUpRotationToNormal : PropSpawnerMod
{
	// Token: 0x060013AC RID: 5036 RVA: 0x00063F30 File Offset: 0x00062130
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = (this.flipNormal ? (-spawnData.normal) : spawnData.normal);
		spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, HelperFunctions.GetRandomRotationWithUp(vector), Mathf.Lerp(this.minEffect, this.maxEffect, Mathf.Pow(Random.value, this.randomPow)));
	}

	// Token: 0x0400123A RID: 4666
	public bool flipNormal;

	// Token: 0x0400123B RID: 4667
	[Range(0f, 1f)]
	public float minEffect;

	// Token: 0x0400123C RID: 4668
	[Range(0f, 1f)]
	public float maxEffect = 1f;

	// Token: 0x0400123D RID: 4669
	public float randomPow = 1f;
}
