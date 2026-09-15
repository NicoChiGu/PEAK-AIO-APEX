using System;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class PSC_RequiredMaterial : PropSpawnerConstraint
{
	// Token: 0x060013DE RID: 5086 RVA: 0x00064CC0 File Offset: 0x00062EC0
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		MeshRenderer componentInChildren = spawnData.hit.transform.GetComponentInChildren<MeshRenderer>();
		foreach (Material material in this.RequiredMaterial)
		{
			if (componentInChildren != null && componentInChildren.sharedMaterial == material)
			{
				return true;
			}
		}
		return this.RequiredMaterial.Length == 0;
	}

	// Token: 0x04001276 RID: 4726
	public Material[] RequiredMaterial;
}
