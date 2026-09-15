using System;
using UnityEngine;

// Token: 0x020002E9 RID: 745
public class PSM_RandomScale : PropSpawnerMod
{
	// Token: 0x060013C0 RID: 5056 RVA: 0x0006451B File Offset: 0x0006271B
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.localScale *= Mathf.Lerp(this.minScaleMult, this.maxScaleMult, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x04001256 RID: 4694
	public float minScaleMult;

	// Token: 0x04001257 RID: 4695
	public float maxScaleMult = 2f;

	// Token: 0x04001258 RID: 4696
	public float randomPow = 1f;
}
