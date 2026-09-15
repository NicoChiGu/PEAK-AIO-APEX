using System;
using UnityEngine;

// Token: 0x020002EB RID: 747
public class PSM_BlastVine : PropSpawnerMod
{
	// Token: 0x060013C4 RID: 5060 RVA: 0x0006462A File Offset: 0x0006282A
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.GetComponent<VinePlane>().Blast();
	}
}
