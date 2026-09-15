using System;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class PSM_PitonNormal : PropSpawnerMod
{
	// Token: 0x060013B0 RID: 5040 RVA: 0x0006402E File Offset: 0x0006222E
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.LookRotation(-spawnData.hit.normal, Vector3.up);
	}
}
