using System;
using UnityEngine;

// Token: 0x02000309 RID: 777
public class RadiusCheck : CustomSpawnCondition
{
	// Token: 0x0600142B RID: 5163 RVA: 0x0006643C File Offset: 0x0006463C
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		LayerMask mask = HelperFunctions.GetMask(this.layerType);
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.radius, mask);
		return array == null || array.Length == 0;
	}

	// Token: 0x040012AE RID: 4782
	public HelperFunctions.LayerType layerType;

	// Token: 0x040012AF RID: 4783
	public float radius = 5f;
}
