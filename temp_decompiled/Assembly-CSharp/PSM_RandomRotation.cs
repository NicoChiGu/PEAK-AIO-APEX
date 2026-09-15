using System;
using UnityEngine;

// Token: 0x020002DE RID: 734
public class PSM_RandomRotation : PropSpawnerMod
{
	// Token: 0x060013AA RID: 5034 RVA: 0x00063E24 File Offset: 0x00062024
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, Random.rotation, Mathf.Lerp(this.minRotation, this.maxRotation, Mathf.Pow(Random.value, this.randomPow)));
		float num = Mathf.Round(spawned.transform.eulerAngles.x / this.increment) * this.increment;
		float num2 = Mathf.Round(spawned.transform.eulerAngles.y / this.increment) * this.increment;
		float num3 = Mathf.Round(spawned.transform.eulerAngles.z / this.increment) * this.increment;
		spawned.transform.eulerAngles = Vector3.Lerp(spawned.transform.eulerAngles, new Vector3(num, num2, num3), this.snapToIncrement);
	}

	// Token: 0x04001235 RID: 4661
	[Range(0f, 1f)]
	public float minRotation;

	// Token: 0x04001236 RID: 4662
	[Range(0f, 1f)]
	public float maxRotation = 0.5f;

	// Token: 0x04001237 RID: 4663
	public float randomPow = 1f;

	// Token: 0x04001238 RID: 4664
	[Range(0f, 1f)]
	public float snapToIncrement;

	// Token: 0x04001239 RID: 4665
	public float increment = 90f;
}
