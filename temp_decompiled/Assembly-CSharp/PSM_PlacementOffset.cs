using System;
using UnityEngine;

// Token: 0x020002E5 RID: 741
public class PSM_PlacementOffset : PropSpawnerMod
{
	// Token: 0x060013B8 RID: 5048 RVA: 0x00064270 File Offset: 0x00062470
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		float num = Mathf.Lerp(this.minHeight.x, this.maxHeight.x, spawnData.placement.x);
		float num2 = Mathf.Lerp(this.minHeight.y, this.maxHeight.y, spawnData.placement.y);
		spawned.transform.position += Vector3.right * (num + num2) * this.xMult;
		spawned.transform.position += Vector3.up * (num + num2) * this.yMult;
		spawned.transform.position += Vector3.forward * (num + num2) * this.zMult;
	}

	// Token: 0x0400124B RID: 4683
	public float xMult;

	// Token: 0x0400124C RID: 4684
	public float yMult = 1f;

	// Token: 0x0400124D RID: 4685
	public float zMult;

	// Token: 0x0400124E RID: 4686
	public Vector2 minHeight;

	// Token: 0x0400124F RID: 4687
	public Vector2 maxHeight;
}
