using System;
using UnityEngine;

// Token: 0x020002FB RID: 763
public class PSC_CircleMask : PropSpawnerConstraint
{
	// Token: 0x060013E8 RID: 5096 RVA: 0x00064F34 File Offset: 0x00063134
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		float num = Vector2.Distance(new Vector2(spawnData.pos.x, spawnData.pos.z), new Vector2(spawnData.spawnerTransform.position.x, spawnData.spawnerTransform.position.z));
		if (this.inverted)
		{
			return num > this.circleSize;
		}
		return num < this.circleSize;
	}

	// Token: 0x04001283 RID: 4739
	public float circleSize = 10f;

	// Token: 0x04001284 RID: 4740
	public bool inverted;
}
