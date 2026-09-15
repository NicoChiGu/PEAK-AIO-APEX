using System;
using UnityEngine;

// Token: 0x020002E4 RID: 740
public class PSM_RayDirectionOffset : PropSpawnerMod
{
	// Token: 0x060013B6 RID: 5046 RVA: 0x00064200 File Offset: 0x00062400
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.position += spawnData.rayDir * Mathf.Lerp(this.minOffset, this.maxOffset, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x04001248 RID: 4680
	public float minOffset;

	// Token: 0x04001249 RID: 4681
	public float maxOffset = 5f;

	// Token: 0x0400124A RID: 4682
	public float randomPow = 1f;
}
