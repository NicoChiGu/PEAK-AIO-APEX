using System;
using UnityEngine;

// Token: 0x020002E2 RID: 738
public class PSM_LocalOffset : PropSpawnerMod
{
	// Token: 0x060013B2 RID: 5042 RVA: 0x00064060 File Offset: 0x00062260
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = Vector3.zero;
		vector += spawned.transform.right * Mathf.Lerp(-this.offset.x, this.offset.x, Random.value) * Mathf.Pow(Random.value, this.randomPow);
		vector += spawned.transform.up * Mathf.Lerp(-this.offset.y, this.offset.y, Random.value) * Mathf.Pow(Random.value, this.randomPow);
		vector += spawned.transform.forward * Mathf.Lerp(-this.offset.z, this.offset.z, Random.value) * Mathf.Pow(Random.value, this.randomPow);
		spawned.transform.position += vector;
	}

	// Token: 0x04001241 RID: 4673
	public Vector3 offset;

	// Token: 0x04001242 RID: 4674
	[Range(0f, 1f)]
	public float minEffect;

	// Token: 0x04001243 RID: 4675
	[Range(0f, 1f)]
	public float maxEffect = 1f;

	// Token: 0x04001244 RID: 4676
	public float randomPow = 1f;
}
