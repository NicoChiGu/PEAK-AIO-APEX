using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002FC RID: 764
public class PSC_SurfaceRestrictions : PropSpawnerConstraint
{
	// Token: 0x060013EA RID: 5098 RVA: 0x00064FB8 File Offset: 0x000631B8
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		if ((this.effectedLayers.value & (1 << spawnData.hit.transform.gameObject.layer)) != 0)
		{
			for (int i = 0; i < this.whitelistedTagWords.Count; i++)
			{
				if (spawnData.hit.transform.tag.ToUpper().Contains(this.whitelistedTagWords[i].ToUpper()))
				{
					return true;
				}
			}
			return false;
		}
		for (int j = 0; j < this.blacklistedTagWords.Count; j++)
		{
			if (spawnData.hit.transform.tag.ToUpper().Contains(this.blacklistedTagWords[j].ToUpper()))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04001285 RID: 4741
	public LayerMask effectedLayers;

	// Token: 0x04001286 RID: 4742
	public List<string> whitelistedTagWords = new List<string>();

	// Token: 0x04001287 RID: 4743
	public List<string> blacklistedTagWords = new List<string>();
}
