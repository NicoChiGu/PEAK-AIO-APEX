using System;
using UnityEngine;

// Token: 0x020002A9 RID: 681
public class MultipleGroundPoints : CustomSpawnCondition
{
	// Token: 0x060012A5 RID: 4773 RVA: 0x0005EF74 File Offset: 0x0005D174
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		Transform transform = base.transform.Find("GroundPoints");
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			RaycastHit raycastHit = HelperFunctions.LineCheck(child.position + Vector3.up * this.checkHeight, child.position + Vector3.down * this.checkRange, this.layerType, 0f, 1);
			if (!raycastHit.transform)
			{
				return false;
			}
			if (Vector3.Angle(Vector3.up, raycastHit.normal) > this.maxAngle)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400115E RID: 4446
	public HelperFunctions.LayerType layerType;

	// Token: 0x0400115F RID: 4447
	public float maxAngle = 30f;

	// Token: 0x04001160 RID: 4448
	public float checkRange = 5f;

	// Token: 0x04001161 RID: 4449
	public float checkHeight;
}
