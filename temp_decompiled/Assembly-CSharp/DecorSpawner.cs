using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class DecorSpawner : LevelGenStep
{
	// Token: 0x060005D9 RID: 1497 RVA: 0x00021438 File Offset: 0x0001F638
	public override void Execute()
	{
		this.Clear();
		this.Add();
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x00021448 File Offset: 0x0001F648
	public void Add()
	{
		if (this.overallSpawnChance < 0.999f && Random.value > this.overallSpawnChance)
		{
			return;
		}
		int num = Random.Range(this.minMaxSpawn.x, this.minMaxSpawn.y);
		if (num > this.spawnPoints.Length)
		{
			num = this.spawnPoints.Length;
		}
		if (num < 0)
		{
			num = 0;
		}
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < this.spawnPoints.Length; i++)
		{
			list.Add(this.spawnPoints[i].position);
		}
		for (int j = 0; j < num; j++)
		{
			int num2 = Random.Range(0, list.Count);
			GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], list[num2], HelperFunctions.GetRandomRotationWithUp(Vector3.up), base.transform);
			gameObject.transform.localScale *= Random.RandomRange(this.scaleMinMax.x, this.scaleMinMax.y);
			list.RemoveAt(num2);
			this.spawnedObjects.Add(gameObject);
		}
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00021570 File Offset: 0x0001F770
	public override void Clear()
	{
		for (int i = 0; i < this.spawnedObjects.Count; i++)
		{
			Object.DestroyImmediate(this.spawnedObjects[i]);
		}
		this.spawnedObjects.Clear();
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x000215B0 File Offset: 0x0001F7B0
	public void getSpawnSpots()
	{
		LazyGizmo[] componentsInChildren = base.GetComponentsInChildren<LazyGizmo>();
		this.spawnPoints = new Transform[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.spawnPoints[i] = componentsInChildren[i].transform;
		}
	}

	// Token: 0x040005FE RID: 1534
	public GameObject[] props;

	// Token: 0x040005FF RID: 1535
	[Range(0f, 1f)]
	public float overallSpawnChance;

	// Token: 0x04000600 RID: 1536
	public Vector2Int minMaxSpawn;

	// Token: 0x04000601 RID: 1537
	public Transform[] spawnPoints;

	// Token: 0x04000602 RID: 1538
	private List<GameObject> spawnedObjects = new List<GameObject>();

	// Token: 0x04000603 RID: 1539
	[Header("Spawn Customization")]
	public Vector2 scaleMinMax = Vector2.one;
}
