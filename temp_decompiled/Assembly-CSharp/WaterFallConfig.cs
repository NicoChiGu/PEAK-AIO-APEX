using System;
using UnityEngine;

// Token: 0x02000370 RID: 880
public class WaterFallConfig : CustomSpawnCondition
{
	// Token: 0x0600165F RID: 5727 RVA: 0x00073E24 File Offset: 0x00072024
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		RaycastHit raycastHit = HelperFunctions.LineCheck(this.rayStart.position, this.rayEnd.position, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		if (raycastHit.transform)
		{
			this.endRock.transform.position = raycastHit.point;
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			this.mesh.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat("_WorldPositionY", raycastHit.point.y);
			this.mesh.SetPropertyBlock(materialPropertyBlock);
		}
		return true;
	}

	// Token: 0x04001540 RID: 5440
	public MeshRenderer mesh;

	// Token: 0x04001541 RID: 5441
	public Transform endRock;

	// Token: 0x04001542 RID: 5442
	public Transform rayStart;

	// Token: 0x04001543 RID: 5443
	public Transform rayEnd;
}
