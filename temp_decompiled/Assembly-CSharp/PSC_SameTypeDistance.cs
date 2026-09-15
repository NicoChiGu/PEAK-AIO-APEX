using System;
using System.Linq;
using UnityEngine;

// Token: 0x020002F9 RID: 761
public class PSC_SameTypeDistance : PropSpawnerConstraint
{
	// Token: 0x060013E4 RID: 5092 RVA: 0x00064E04 File Offset: 0x00063004
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		PSC_SameTypeDistance.<>c__DisplayClass3_0 CS$<>8__locals1 = new PSC_SameTypeDistance.<>c__DisplayClass3_0();
		CS$<>8__locals1.spawnData = spawnData;
		CS$<>8__locals1.<>4__this = this;
		if (this.findAllSpawners)
		{
			if (Enumerable.Any<LevelGenStep>(Enumerable.Where<LevelGenStep>(Object.FindObjectsByType<LevelGenStep>(0), (LevelGenStep levenGenStep) => levenGenStep.name == CS$<>8__locals1.spawnData.spawnerTransform.name), (LevelGenStep spawner) => base.<CheckConstraint>g__AnyChildrenTooClose|0(spawner.transform)))
			{
				return false;
			}
		}
		else if (CS$<>8__locals1.<CheckConstraint>g__AnyChildrenTooClose|0(CS$<>8__locals1.spawnData.spawnerTransform))
		{
			return false;
		}
		return true;
	}

	// Token: 0x0400127D RID: 4733
	public float minDistance = 5f;

	// Token: 0x0400127E RID: 4734
	public bool findAllSpawners;

	// Token: 0x0400127F RID: 4735
	public Vector3 axisMultipliers = Vector3.one;
}
