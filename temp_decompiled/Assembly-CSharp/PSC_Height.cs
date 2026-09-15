using System;

// Token: 0x020002F7 RID: 759
public class PSC_Height : PropSpawnerConstraint
{
	// Token: 0x060013E0 RID: 5088 RVA: 0x00064D24 File Offset: 0x00062F24
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		return spawnData.pos.y > this.minHeight && spawnData.pos.y < this.maxHeight;
	}

	// Token: 0x04001277 RID: 4727
	public float maxHeight = 10000f;

	// Token: 0x04001278 RID: 4728
	public float minHeight = -10000f;
}
