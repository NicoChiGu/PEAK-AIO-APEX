using System;
using UnityEngine;

// Token: 0x020002E7 RID: 743
public class PSM_ReplaceMaterial : PropSpawnerMod
{
	// Token: 0x060013BC RID: 5052 RVA: 0x000643EC File Offset: 0x000625EC
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		foreach (Renderer renderer in spawned.GetComponentsInChildren<Renderer>())
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if (sharedMaterials[j] == this.replaceThis)
				{
					sharedMaterials[j] = this.withThis;
				}
			}
			renderer.sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x04001253 RID: 4691
	public Material replaceThis;

	// Token: 0x04001254 RID: 4692
	public Material withThis;
}
