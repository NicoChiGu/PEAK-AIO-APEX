using System;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class PSM_ReplaceMaterialWithRayTargetMaterial : PropSpawnerMod
{
	// Token: 0x060013BE RID: 5054 RVA: 0x00064458 File Offset: 0x00062658
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		if (spawnData.hit.transform == null)
		{
			return;
		}
		MeshRenderer[] componentsInChildren = spawnData.hit.transform.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer meshRenderer = null;
		foreach (MeshRenderer meshRenderer2 in componentsInChildren)
		{
			if (meshRenderer2.enabled)
			{
				meshRenderer = meshRenderer2;
				break;
			}
		}
		if (meshRenderer == null)
		{
			return;
		}
		foreach (Renderer renderer in spawned.GetComponentsInChildren<Renderer>())
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if (sharedMaterials[j] == this.replaceThis)
				{
					sharedMaterials[j] = meshRenderer.sharedMaterial;
				}
			}
			renderer.sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x04001255 RID: 4693
	public Material replaceThis;
}
