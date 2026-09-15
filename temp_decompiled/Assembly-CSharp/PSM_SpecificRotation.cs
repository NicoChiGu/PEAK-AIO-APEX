using System;
using UnityEngine;

// Token: 0x020002EA RID: 746
public class PSM_SpecificRotation : PropSpawnerMod
{
	// Token: 0x060013C2 RID: 5058 RVA: 0x00064574 File Offset: 0x00062774
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = this.eulerAngles;
		if (this.random > 0f)
		{
			vector = Vector3.Lerp(vector, this.eulerAnglesRandom, Random.value * this.random);
		}
		Quaternion quaternion = Quaternion.Lerp(spawned.transform.rotation, Quaternion.Euler(vector), this.blend);
		if (this.addRotation)
		{
			spawned.transform.localRotation = Quaternion.Lerp(spawned.transform.localRotation, Quaternion.Euler(vector) * spawned.transform.localRotation, this.blend);
			return;
		}
		spawned.transform.rotation = quaternion;
	}

	// Token: 0x04001259 RID: 4697
	public bool addRotation;

	// Token: 0x0400125A RID: 4698
	public Vector3 eulerAngles;

	// Token: 0x0400125B RID: 4699
	[Range(0f, 1f)]
	public float random;

	// Token: 0x0400125C RID: 4700
	public Vector3 eulerAnglesRandom;

	// Token: 0x0400125D RID: 4701
	[Range(0f, 1f)]
	public float blend = 1f;
}
