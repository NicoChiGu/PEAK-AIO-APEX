using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000302 RID: 770
public class PSCP_ConnectTreePlatforms : PropSpawnerConstraintPost
{
	// Token: 0x060013F8 RID: 5112 RVA: 0x000651D8 File Offset: 0x000633D8
	public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		List<TreePlatform> list = new List<TreePlatform>();
		list.AddRange(this.treePlatformParent.GetComponentsInChildren<TreePlatform>());
		JungleVine[] components = spawned.GetComponents<JungleVine>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].PickTreePlatforms(list);
		}
		return true;
	}

	// Token: 0x0400128C RID: 4748
	public GameObject treePlatformParent;
}
