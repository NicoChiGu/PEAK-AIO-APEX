using System;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class PSCP_Custom : PropSpawnerConstraintPost
{
	// Token: 0x060013F6 RID: 5110 RVA: 0x000651A0 File Offset: 0x000633A0
	public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		CustomSpawnCondition[] components = spawned.GetComponents<CustomSpawnCondition>();
		for (int i = 0; i < components.Length; i++)
		{
			if (!components[i].CheckCondition(spawnData))
			{
				return false;
			}
		}
		return true;
	}
}
