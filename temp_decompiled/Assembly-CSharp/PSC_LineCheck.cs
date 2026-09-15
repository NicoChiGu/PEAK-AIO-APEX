using System;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public class PSC_LineCheck : PropSpawnerConstraint
{
	// Token: 0x060013D3 RID: 5075 RVA: 0x00064830 File Offset: 0x00062A30
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = spawnData.hit.point + Vector3.Scale(spawnData.spawnerTransform.lossyScale, this.localStart);
		Vector3 vector2 = vector + Vector3.Scale(spawnData.spawnerTransform.localScale, this.localEnd);
		bool flag = !HelperFunctions.LineCheck(vector, vector2, HelperFunctions.LayerType.TerrainMap, this.radius, 1).transform;
		if (this.invert)
		{
			flag = !flag;
		}
		Debug.DrawLine(vector, vector2, flag ? Color.green : Color.red, 10f);
		return flag;
	}

	// Token: 0x04001267 RID: 4711
	public bool invert;

	// Token: 0x04001268 RID: 4712
	public float radius;

	// Token: 0x04001269 RID: 4713
	public Vector3 localStart = new Vector3(0f, 0f, 0f);

	// Token: 0x0400126A RID: 4714
	public Vector3 localEnd = new Vector3(0f, 5f, 0f);
}
