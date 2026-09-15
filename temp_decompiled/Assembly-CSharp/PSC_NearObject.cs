using System;
using UnityEngine;

// Token: 0x020002F8 RID: 760
public class PSC_NearObject : PropSpawnerConstraint
{
	// Token: 0x060013E2 RID: 5090 RVA: 0x00064D6C File Offset: 0x00062F6C
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		this.outVal = this.inverted;
		foreach (Collider collider in Physics.OverlapSphere(spawnData.hit.point, this.radius))
		{
			for (int j = 0; j < this.objects.Length; j++)
			{
				if (collider.transform.parent.name == this.objects[j].name)
				{
					this.outVal = !this.inverted;
				}
			}
		}
		return this.outVal;
	}

	// Token: 0x04001279 RID: 4729
	public bool inverted;

	// Token: 0x0400127A RID: 4730
	public GameObject[] objects;

	// Token: 0x0400127B RID: 4731
	public float radius;

	// Token: 0x0400127C RID: 4732
	private bool outVal;
}
