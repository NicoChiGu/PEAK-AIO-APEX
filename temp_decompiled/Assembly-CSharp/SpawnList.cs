using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class SpawnList : MonoBehaviour
{
	// Token: 0x06000984 RID: 2436 RVA: 0x000328E0 File Offset: 0x00030AE0
	private void RefreshPercentageOdds()
	{
		int num = 0;
		foreach (SpawnEntry spawnEntry in this.items)
		{
			num += spawnEntry.weight;
		}
		foreach (SpawnEntry spawnEntry2 in this.items)
		{
			spawnEntry2.percentageOdds = (float)spawnEntry2.weight / (float)num;
			spawnEntry2.percentageOdds = (float)Mathf.FloorToInt(spawnEntry2.percentageOdds * 1000f) / 10f;
		}
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x000329A0 File Offset: 0x00030BA0
	private void SortByWeight()
	{
		this.items = Enumerable.ToList<SpawnEntry>(Enumerable.OrderByDescending<SpawnEntry, int>(this.items, (SpawnEntry item) => item.weight));
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x000329D7 File Offset: 0x00030BD7
	public GameObject GetSingleSpawn()
	{
		return this.items.RandomSelection((SpawnEntry i) => i.weight).prefab;
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00032A08 File Offset: 0x00030C08
	public List<GameObject> GetSpawns(int count, bool canRepeat = true)
	{
		List<SpawnEntry> list = new List<SpawnEntry>(this.items);
		List<GameObject> list2 = new List<GameObject>();
		for (int j = 0; j < count; j++)
		{
			SpawnEntry spawnEntry = list.RandomSelection((SpawnEntry i) => i.weight);
			if (spawnEntry != null)
			{
				list2.Add(spawnEntry.prefab);
				if (!canRepeat)
				{
					if (list.Count <= 1)
					{
						list = new List<SpawnEntry>(this.items);
					}
					list.Remove(spawnEntry);
				}
			}
			else
			{
				list2.Add(null);
			}
		}
		return list2;
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00032A94 File Offset: 0x00030C94
	private void FindReferencesInScene()
	{
		Spawner[] array = Object.FindObjectsOfType<Spawner>();
		bool flag = false;
		foreach (Spawner spawner in array)
		{
			if (spawner.spawns.gameObject.name == base.gameObject.name)
			{
				Debug.Log("Found " + base.gameObject.name + " on " + spawner.gameObject.name);
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.Log(base.gameObject.name + " not present in scene.");
		}
	}

	// Token: 0x040008F6 RID: 2294
	public List<SpawnEntry> items;
}
