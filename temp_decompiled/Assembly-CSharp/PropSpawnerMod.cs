using System;
using UnityEngine;

// Token: 0x020002D6 RID: 726
[Serializable]
public abstract class PropSpawnerMod
{
	// Token: 0x0600139A RID: 5018
	public abstract void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData);

	// Token: 0x04001228 RID: 4648
	public bool mute;
}
