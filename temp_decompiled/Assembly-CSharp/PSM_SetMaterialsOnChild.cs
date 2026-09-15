using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002EE RID: 750
public class PSM_SetMaterialsOnChild : PropSpawnerMod
{
	// Token: 0x060013CA RID: 5066 RVA: 0x000646F0 File Offset: 0x000628F0
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		List<Renderer> rends = new List<Renderer>();
		TransformExtensions.FindChildrenRecursive(spawned.transform, this.childName).ForEach(delegate(Transform c)
		{
			rends.AddRange(c.GetComponentsInChildren<Renderer>());
		});
		for (int i = 0; i < rends.Count; i++)
		{
			Material[] sharedMaterials = rends[i].sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				foreach (MatAndID matAndID in this.edits)
				{
					if (matAndID.id == j)
					{
						sharedMaterials[j] = matAndID.mat;
					}
				}
			}
			rends[i].sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x04001261 RID: 4705
	public string childName;

	// Token: 0x04001262 RID: 4706
	public MatAndID[] edits;
}
