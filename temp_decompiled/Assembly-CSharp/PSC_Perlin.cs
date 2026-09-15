using System;
using UnityEngine;

// Token: 0x020002FA RID: 762
public class PSC_Perlin : PropSpawnerConstraint
{
	// Token: 0x060013E6 RID: 5094 RVA: 0x00064E90 File Offset: 0x00063090
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		float num = Mathf.PerlinNoise((spawnData.pos.x + 500f + this.perlinOffset) * this.perlinSize * 0.1f, (spawnData.pos.z + 500f + this.perlinOffset) * this.perlinSize * 0.1f);
		return num > this.minMax.x && num < this.minMax.y;
	}

	// Token: 0x04001280 RID: 4736
	public float perlinSize = 10f;

	// Token: 0x04001281 RID: 4737
	public float perlinOffset;

	// Token: 0x04001282 RID: 4738
	public Vector2 minMax = new Vector2(0f, 0.5f);
}
