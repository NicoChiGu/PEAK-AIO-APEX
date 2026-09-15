using System;
using UnityEngine;

// Token: 0x020002E6 RID: 742
public class PSM_UpLerp : PropSpawnerMod
{
	// Token: 0x060013BA RID: 5050 RVA: 0x00064368 File Offset: 0x00062568
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		float num = Mathf.Pow(Random.value, this.randomPow);
		float num2 = Mathf.Lerp(this.minUpLerp, this.maxUpLerp, num);
		Vector3 vector = spawned.transform.up;
		vector = Vector3.Lerp(vector, Vector3.up, num2);
		spawned.transform.rotation = HelperFunctions.GetRotationWithUp(spawned.transform.forward, vector);
	}

	// Token: 0x04001250 RID: 4688
	[Range(0f, 1f)]
	public float minUpLerp;

	// Token: 0x04001251 RID: 4689
	[Range(0f, 1f)]
	public float maxUpLerp = 1f;

	// Token: 0x04001252 RID: 4690
	public float randomPow = 1f;
}
