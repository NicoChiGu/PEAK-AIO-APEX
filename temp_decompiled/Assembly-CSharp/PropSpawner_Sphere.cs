using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000306 RID: 774
public class PropSpawner_Sphere : LevelGenStep
{
	// Token: 0x1700013F RID: 319
	// (get) Token: 0x0600140C RID: 5132 RVA: 0x0006574E File Offset: 0x0006394E
	public override DeferredStepTiming DeferredTiming
	{
		get
		{
			if (this._deferredSteps.Count <= 0)
			{
				return DeferredStepTiming.None;
			}
			return DeferredStepTiming.AfterCurrentGroupTiming;
		}
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x00065761 File Offset: 0x00063961
	public override IDeferredStep ConstructDeferred(IMayHaveDeferredStep parent)
	{
		if (this != parent)
		{
			Debug.LogError("What da HECK!!!");
			return null;
		}
		return new ExecuteDeferredStepList(this._deferredSteps);
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x0600140E RID: 5134 RVA: 0x00065783 File Offset: 0x00063983
	// (set) Token: 0x0600140F RID: 5135 RVA: 0x0006578B File Offset: 0x0006398B
	public List<GameObject> spawnedProps { get; private set; } = new List<GameObject>();

	// Token: 0x06001410 RID: 5136 RVA: 0x00065794 File Offset: 0x00063994
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.rayLength);
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x000657AC File Offset: 0x000639AC
	public void Go()
	{
		this.Clear();
		this.SpawnNew(true);
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x000657BB File Offset: 0x000639BB
	public override void Execute()
	{
		this.Clear();
		this.SpawnNew(false);
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x000657CA File Offset: 0x000639CA
	public void Add()
	{
		this.SpawnNew(true);
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x000657D4 File Offset: 0x000639D4
	public void SpawnNew(bool executeDeferredImmediately = false)
	{
		if (this.spawnChance < Random.value)
		{
			return;
		}
		int num = this.nrOfSpawns;
		if (this.randomSpawns)
		{
			num = Random.Range(this.minSpawnCount, this.nrOfSpawns);
		}
		int num2 = 50000;
		int num3 = 0;
		Physics.SyncTransforms();
		while (num3 < num && num2 > 0)
		{
			num2--;
			if (this.TryToSpawn())
			{
				num3++;
				if (this.syncTransforms)
				{
					Physics.SyncTransforms();
				}
			}
		}
		foreach (PostSpawnBehavior postSpawnBehavior in this.postSpawnBehaviors)
		{
			if (!postSpawnBehavior.mute)
			{
				if (executeDeferredImmediately || postSpawnBehavior.DeferredTiming != DeferredStepTiming.AfterCurrentGroupTiming)
				{
					postSpawnBehavior.RunBehavior(this.spawnedProps);
				}
				else
				{
					this._deferredSteps.Add(postSpawnBehavior.ConstructDeferred(this));
				}
			}
		}
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x000658BC File Offset: 0x00063ABC
	public override void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x000658F7 File Offset: 0x00063AF7
	public void GoAll()
	{
		base.GetComponentInParent<PropGrouper>().RunAll(true);
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x00065905 File Offset: 0x00063B05
	public void ClearAll()
	{
		base.GetComponentInParent<PropGrouper>().ClearAll();
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x00065914 File Offset: 0x00063B14
	private bool TryToSpawn()
	{
		PropSpawner.SpawnData randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return false;
		}
		for (int i = 0; i < this.constraints.Count; i++)
		{
			if (!this.constraints[i].CheckConstraint(randomPoint))
			{
				return false;
			}
		}
		GameObject gameObject = this.Spawn(randomPoint);
		if (gameObject != null)
		{
			this.spawnedProps.Add(gameObject);
		}
		return gameObject != null;
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x00065978 File Offset: 0x00063B78
	private GameObject Spawn(PropSpawner.SpawnData spawnData)
	{
		GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], spawnData.pos, HelperFunctions.GetRandomRotationWithUp(Vector3.up), base.transform);
		if (gameObject == null)
		{
			Debug.LogError("Failed to spawn prefab", base.gameObject);
		}
		for (int i = 0; i < this.modifiers.Count; i++)
		{
			this.modifiers[i].ModifyObject(gameObject, spawnData);
		}
		for (int j = 0; j < this.postConstraints.Count; j++)
		{
			if (!this.postConstraints[j].CheckConstraint(gameObject, spawnData))
			{
				Object.DestroyImmediate(gameObject);
				return null;
			}
		}
		return gameObject;
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x00065A2C File Offset: 0x00063C2C
	private PropSpawner.SpawnData GetRandomPoint()
	{
		Vector3 position = base.transform.position;
		Vector3 normalized = Random.onUnitSphere.normalized;
		if (!this.rayCastSpawn)
		{
			return new PropSpawner.SpawnData
			{
				pos = position,
				normal = normalized,
				rayDir = normalized,
				hit = default(RaycastHit),
				spawnerTransform = base.transform
			};
		}
		RaycastHit raycastHit = HelperFunctions.LineCheck(position, position + normalized * this.rayLength, this.layerType, 0f, 1);
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

	// Token: 0x04001299 RID: 4761
	private List<IDeferredStep> _deferredSteps = new List<IDeferredStep>();

	// Token: 0x0400129A RID: 4762
	public float spawnChance = 1f;

	// Token: 0x0400129B RID: 4763
	public float rayLength = 5000f;

	// Token: 0x0400129C RID: 4764
	public int nrOfSpawns = 500;

	// Token: 0x0400129D RID: 4765
	public bool randomSpawns;

	// Token: 0x0400129E RID: 4766
	public int minSpawnCount;

	// Token: 0x0400129F RID: 4767
	public bool rayCastSpawn = true;

	// Token: 0x040012A0 RID: 4768
	public GameObject[] props;

	// Token: 0x040012A2 RID: 4770
	public bool syncTransforms = true;

	// Token: 0x040012A3 RID: 4771
	public HelperFunctions.LayerType layerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x040012A4 RID: 4772
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x040012A5 RID: 4773
	[SerializeReference]
	public List<PropSpawnerConstraint> constraints = new List<PropSpawnerConstraint>();

	// Token: 0x040012A6 RID: 4774
	[SerializeReference]
	public List<PropSpawnerConstraintPost> postConstraints = new List<PropSpawnerConstraintPost>();

	// Token: 0x040012A7 RID: 4775
	[SerializeReference]
	public List<PostSpawnBehavior> postSpawnBehaviors = new List<PostSpawnBehavior>();
}
