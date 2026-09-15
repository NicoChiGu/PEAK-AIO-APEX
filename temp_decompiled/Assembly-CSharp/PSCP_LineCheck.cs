using System;
using UnityEngine;

// Token: 0x02000300 RID: 768
public class PSCP_LineCheck : PropSpawnerConstraintPost, IValidationConstraint
{
	// Token: 0x060013F4 RID: 5108 RVA: 0x00065118 File Offset: 0x00063318
	public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		return !HelperFunctions.LineCheck(spawned.transform.TransformPoint(this.localStart), spawned.transform.TransformPoint(this.localEnd), HelperFunctions.LayerType.TerrainMap, 0f, 1).transform;
	}

	// Token: 0x0400128A RID: 4746
	public Vector3 localStart = new Vector3(0f, 0.1f, 0f);

	// Token: 0x0400128B RID: 4747
	public Vector3 localEnd = new Vector3(0f, 5f, 0f);
}
