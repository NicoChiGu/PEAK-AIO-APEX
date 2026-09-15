using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002ED RID: 749
public class PSM_SetMaterialOnChild : PropSpawnerMod
{
	// Token: 0x060013C8 RID: 5064 RVA: 0x00064678 File Offset: 0x00062878
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		List<Renderer> rends = new List<Renderer>();
		TransformExtensions.FindChildrenRecursive(spawned.transform, this.childName).ForEach(delegate(Transform c)
		{
			rends.AddRange(c.GetComponentsInChildren<Renderer>());
		});
		for (int i = 0; i < rends.Count; i++)
		{
			rends[i].sharedMaterial = this.mat;
		}
	}

	// Token: 0x0400125F RID: 4703
	public string childName;

	// Token: 0x04001260 RID: 4704
	public Material mat;
}
