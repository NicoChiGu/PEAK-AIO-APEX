using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002D8 RID: 728
public class PSM_AddPlayerCountBasedDespawner : PropSpawnerMod
{
	// Token: 0x0600139E RID: 5022 RVA: 0x00063C3C File Offset: 0x00061E3C
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		if (spawned.GetComponent<PhotonView>())
		{
			DestroyBasedOnPlayerCount destroyBasedOnPlayerCount = spawned.AddComponent<DestroyBasedOnPlayerCount>();
			if (this.onePerPlayer)
			{
				destroyBasedOnPlayerCount.destroyIfPlayerCountIsLessThan = spawnData.spawnCount + 1;
				return;
			}
			destroyBasedOnPlayerCount.destroyIfPlayerCountIsLessThan = this.destroyAllIfLessThan;
		}
	}

	// Token: 0x0400122A RID: 4650
	public bool onePerPlayer;

	// Token: 0x0400122B RID: 4651
	public int destroyAllIfLessThan;
}
