using System;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class PSM_NormalOffset : PropSpawnerMod
{
	// Token: 0x060013B4 RID: 5044 RVA: 0x00064190 File Offset: 0x00062390
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.position += spawnData.normal * Mathf.Lerp(this.minOffset, this.maxOffset, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x04001245 RID: 4677
	public float minOffset;

	// Token: 0x04001246 RID: 4678
	public float maxOffset = 2f;

	// Token: 0x04001247 RID: 4679
	public float randomPow = 1f;
}
