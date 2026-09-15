using System;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class PSC_BannedMaterial : PropSpawnerConstraint
{
	// Token: 0x060013DC RID: 5084 RVA: 0x00064C2C File Offset: 0x00062E2C
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		this.returnVal = true;
		MeshRenderer[] componentsInChildren = spawnData.hit.transform.GetComponentsInChildren<MeshRenderer>();
		foreach (Material material in this.bannedMaterial)
		{
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer != null && meshRenderer.sharedMaterial == material)
				{
					this.returnVal = false;
					break;
				}
			}
		}
		return this.returnVal;
	}

	// Token: 0x04001274 RID: 4724
	public Material[] bannedMaterial;

	// Token: 0x04001275 RID: 4725
	private bool returnVal = true;
}
