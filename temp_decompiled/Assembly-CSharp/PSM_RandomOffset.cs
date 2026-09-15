using System;
using UnityEngine;

// Token: 0x020002DD RID: 733
public class PSM_RandomOffset : PropSpawnerMod
{
	// Token: 0x060013A8 RID: 5032 RVA: 0x00063DB4 File Offset: 0x00061FB4
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		float num = Mathf.Lerp(this.minOffset, this.maxOffset, Mathf.Pow(Random.value, this.randomPow));
		spawned.transform.position += Random.onUnitSphere * num;
	}

	// Token: 0x04001232 RID: 4658
	public float minOffset;

	// Token: 0x04001233 RID: 4659
	public float maxOffset = 0.5f;

	// Token: 0x04001234 RID: 4660
	public float randomPow = 1f;
}
