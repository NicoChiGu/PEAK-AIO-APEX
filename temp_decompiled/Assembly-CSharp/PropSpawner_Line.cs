using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000305 RID: 773
public class PropSpawner_Line : LevelGenStep
{
	// Token: 0x06001402 RID: 5122 RVA: 0x00065388 File Offset: 0x00063588
	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.position + this.height * 0.5f * base.transform.up;
		Gizmos.DrawLine(base.transform.position - this.height * 0.5f * base.transform.up, vector);
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x000653F4 File Offset: 0x000635F4
	public override void Execute()
	{
		this.Clear();
		this.Add();
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x00065404 File Offset: 0x00063604
	public void Add()
	{
		int num = 50000;
		int num2 = 0;
		Physics.SyncTransforms();
		while (num2 < this.nrOfSpawns && num > 0)
		{
			num--;
			if (this.TryToSpawn())
			{
				num2++;
				if (this.syncTransforms)
				{
					Physics.SyncTransforms();
				}
			}
		}
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x0006544C File Offset: 0x0006364C
	public override void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x00065487 File Offset: 0x00063687
	public void GoAll()
	{
		base.GetComponentInParent<PropGrouper>().RunAll(true);
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x00065495 File Offset: 0x00063695
	public void ClearAll()
	{
		base.GetComponentInParent<PropGrouper>().ClearAll();
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x000654A4 File Offset: 0x000636A4
	private bool TryToSpawn()
	{
		PropSpawner.SpawnData randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return false;
		}
		for (int i = 0; i < this.constraints.Count; i++)
		{
			if (!this.constraints[i].mute && !this.constraints[i].CheckConstraint(randomPoint))
			{
				return false;
			}
		}
		return this.Spawn(randomPoint) != null;
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x0006550C File Offset: 0x0006370C
	private GameObject Spawn(PropSpawner.SpawnData spawnData)
	{
		GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], spawnData.pos, HelperFunctions.GetRandomRotationWithUp(Vector3.up), base.transform);
		for (int i = 0; i < this.modifiers.Count; i++)
		{
			if (!this.modifiers[i].mute)
			{
				this.modifiers[i].ModifyObject(gameObject, spawnData);
			}
		}
		for (int j = 0; j < this.postConstraints.Count; j++)
		{
			if (!this.postConstraints[j].mute && !this.postConstraints[j].CheckConstraint(gameObject, spawnData))
			{
				Object.DestroyImmediate(gameObject);
				return null;
			}
		}
		return gameObject;
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x000655D0 File Offset: 0x000637D0
	private PropSpawner.SpawnData GetRandomPoint()
	{
		Vector3 vector = base.transform.position + base.transform.up * Mathf.Lerp(-0.5f, 0.5f, Random.value) * this.height;
		Vector3 normalized = Vector3.ProjectOnPlane(Random.onUnitSphere, base.transform.up).normalized;
		if (!this.rayCastSpawn)
		{
			return new PropSpawner.SpawnData
			{
				pos = vector,
				normal = normalized,
				rayDir = normalized,
				hit = default(RaycastHit),
				spawnerTransform = base.transform
			};
		}
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + normalized * this.rayLength, this.layerType, 0f, 1);
		if (raycastHit.transform)
		{
			return new PropSpawner.SpawnData
			{
				pos = raycastHit.point,
				normal = raycastHit.normal,
				rayDir = normalized,
				hit = raycastHit,
				spawnerTransform = base.transform
			};
		}
		return null;
	}

	// Token: 0x0400128F RID: 4751
	public float height = 200f;

	// Token: 0x04001290 RID: 4752
	public float rayLength = 5000f;

	// Token: 0x04001291 RID: 4753
	public int nrOfSpawns = 500;

	// Token: 0x04001292 RID: 4754
	public bool rayCastSpawn = true;

	// Token: 0x04001293 RID: 4755
	public GameObject[] props;

	// Token: 0x04001294 RID: 4756
	public bool syncTransforms = true;

	// Token: 0x04001295 RID: 4757
	public HelperFunctions.LayerType layerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x04001296 RID: 4758
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x04001297 RID: 4759
	[SerializeReference]
	public List<PropSpawnerConstraint> constraints = new List<PropSpawnerConstraint>();

	// Token: 0x04001298 RID: 4760
	[SerializeReference]
	public List<PropSpawnerConstraintPost> postConstraints = new List<PropSpawnerConstraintPost>();
}
