using System;
using UnityEngine;

// Token: 0x020002DC RID: 732
public class PSM_SingleItemSpawner : PropSpawnerMod
{
	// Token: 0x060013A6 RID: 5030 RVA: 0x00063D72 File Offset: 0x00061F72
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.GetComponentInChildren<SingleItemSpawner>().prefab = this.objToSpawn;
		spawned.gameObject.name = this.objToSpawn.gameObject.name + " (spawner)";
	}

	// Token: 0x04001231 RID: 4657
	public GameObject objToSpawn;
}
