using System;
using UnityEngine;

// Token: 0x020002EC RID: 748
public class PSM_SetMaterial : PropSpawnerMod
{
	// Token: 0x060013C6 RID: 5062 RVA: 0x00064640 File Offset: 0x00062840
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Renderer[] componentsInChildren = spawned.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = this.mat;
		}
	}

	// Token: 0x0400125E RID: 4702
	public Material mat;
}
