using System;
using UnityEngine;

// Token: 0x020002D7 RID: 727
public class Campfire_Set_Segment : PropSpawnerMod
{
	// Token: 0x0600139C RID: 5020 RVA: 0x00063C1E File Offset: 0x00061E1E
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.GetComponentInChildren<Campfire>().advanceToSegment = this.Segment;
	}

	// Token: 0x04001229 RID: 4649
	public Segment Segment;
}
