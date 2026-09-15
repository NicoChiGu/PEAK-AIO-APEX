using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002D4 RID: 724
public class PropPostSpawnModifiers : MonoBehaviour
{
	// Token: 0x0600137F RID: 4991 RVA: 0x00062E64 File Offset: 0x00061064
	public void ApplyModifiers()
	{
		PropSpawner[] componentsInChildren = base.GetComponentsInChildren<PropSpawner>();
		List<GameObject> list = new List<GameObject>();
		PropSpawner[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i].transform;
			int childCount = transform.childCount;
			for (int j = 0; j < childCount; j++)
			{
				list.Add(transform.GetChild(j).gameObject);
			}
		}
		foreach (GameObject gameObject in list)
		{
			foreach (PropSpawnerMod propSpawnerMod in this.modifiers)
			{
				propSpawnerMod.ModifyObject(gameObject, new PropSpawner.SpawnData
				{
					hit = default(RaycastHit),
					normal = Vector3.up,
					placement = Vector2.zero,
					pos = gameObject.transform.position,
					rayDir = Vector3.zero,
					spawnCount = 0,
					spawnerTransform = null
				});
			}
		}
	}

	// Token: 0x04001210 RID: 4624
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x020004FA RID: 1274
	public enum PropModTiming
	{
		// Token: 0x04001B2E RID: 6958
		Early,
		// Token: 0x04001B2F RID: 6959
		Late
	}
}
