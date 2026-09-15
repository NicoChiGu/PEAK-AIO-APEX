using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002D9 RID: 729
public class PSM_SetSpawnerPlayerCountRequirement : PropSpawnerMod
{
	// Token: 0x060013A0 RID: 5024 RVA: 0x00063C88 File Offset: 0x00061E88
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		if (spawned.GetComponent<PhotonView>())
		{
			Spawner component = spawned.GetComponent<Spawner>();
			if (this.onePerPlayer)
			{
				component.playersInRoomRequirement = spawnData.spawnCount + 1;
			}
		}
	}

	// Token: 0x0400122C RID: 4652
	public bool onePerPlayer;
}
