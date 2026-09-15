using System;
using UnityEngine;

// Token: 0x020002F0 RID: 752
public class PSM_SetRandomMaterial : PropSpawnerMod
{
	// Token: 0x060013CD RID: 5069 RVA: 0x000647CC File Offset: 0x000629CC
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Renderer[] componentsInChildren = spawned.GetComponentsInChildren<Renderer>();
		Material material = this.mats[Random.Range(0, this.mats.Length)];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = material;
		}
	}

	// Token: 0x04001265 RID: 4709
	public Material[] mats;
}
