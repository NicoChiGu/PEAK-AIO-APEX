using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x020000B1 RID: 177
public class Spawner : MonoBehaviourPunCallbacks, ISpawner
{
	// Token: 0x1700007D RID: 125
	// (get) Token: 0x0600067B RID: 1659 RVA: 0x00025055 File Offset: 0x00023255
	protected bool isWeightedSpawnPoints
	{
		get
		{
			return this.spawnPointMode == Spawner.SpawnPointMode.WeightedLists;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x0600067C RID: 1660 RVA: 0x00025060 File Offset: 0x00023260
	private bool isSpawnPool
	{
		get
		{
			return this.spawnMode == Spawner.SpawnMode.SpawnPool;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x0600067D RID: 1661 RVA: 0x0002506B File Offset: 0x0002326B
	private bool isSingleItem
	{
		get
		{
			return this.spawnMode == Spawner.SpawnMode.SingleItem;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x0600067E RID: 1662 RVA: 0x00025076 File Offset: 0x00023276
	private bool isHeightBasedSpawnPool
	{
		get
		{
			return this.spawnMode == Spawner.SpawnMode.HeightBasedSpawnPools;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x0600067F RID: 1663 RVA: 0x00025081 File Offset: 0x00023281
	public bool hasSpawnList
	{
		get
		{
			return this.isSpawnPool && this.spawns != null && this.spawnPool == SpawnPool.None;
		}
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x000250A4 File Offset: 0x000232A4
	public void ForceSpawn()
	{
		this.TrySpawnItems();
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x000250B0 File Offset: 0x000232B0
	public List<PhotonView> TrySpawnItems()
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		if (!this.spawnOnStart)
		{
			return list;
		}
		if (this.playersInRoomRequirement > PhotonNetwork.PlayerList.Length)
		{
			Debug.Log(string.Format("Not spawning: {0} because of players in room req: {1}", base.gameObject.name, this.playersInRoomRequirement));
			return list;
		}
		if (this.belowAscentRequirement != -1 && Ascents.currentAscent >= this.belowAscentRequirement)
		{
			Debug.Log(string.Format("Not spawning: {0} because ascent is too high: {1}", base.gameObject.name, Ascents.currentAscent));
			return list;
		}
		if (Random.Range(0f, 1f) <= this.baseSpawnChance)
		{
			list.AddRange(this.SpawnItems(this.GetSpawnSpots()));
		}
		return list;
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00025174 File Offset: 0x00023374
	protected virtual List<Transform> GetSpawnSpots()
	{
		Spawner.SpawnPointMode spawnPointMode = this.spawnPointMode;
		if (spawnPointMode == Spawner.SpawnPointMode.SingleList)
		{
			return this.spawnSpots;
		}
		if (spawnPointMode != Spawner.SpawnPointMode.WeightedLists)
		{
			return new List<Transform>();
		}
		return this.weightedSpawnSpots.RandomSelection((Spawner.WeightedSpawnPointEntry w) => w.weight).spawnSpots;
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x000251D0 File Offset: 0x000233D0
	public virtual List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		Debug.Log(string.Format("Spawning: {0}", base.gameObject));
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		if (spawnSpots.Count == 0)
		{
			return list;
		}
		List<GameObject> objectsToSpawn = this.GetObjectsToSpawn(spawnSpots.Count, this.canRepeatSpawns);
		int num = 0;
		while (num < spawnSpots.Count && num < objectsToSpawn.Count)
		{
			if (!(objectsToSpawn[num] == null))
			{
				Item component = PhotonNetwork.InstantiateItemRoom(objectsToSpawn[num].name, spawnSpots[num].position, spawnSpots[num].rotation).GetComponent<Item>();
				list.Add(component.GetComponent<PhotonView>());
				if (this.spawnUpTowardsTarget)
				{
					component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
				}
				if (this.centerItemsVisually)
				{
					Vector3 vector = spawnSpots[num].position - component.Center();
					component.transform.position += vector;
				}
				this.OffsetSpawn(component);
				component.ForceSyncForFrames(10);
				if (component != null && this.isKinematic)
				{
					component.GetComponent<PhotonView>().RPC("SetKinematicRPC", 3, new object[]
					{
						true,
						component.transform.position,
						component.transform.rotation
					});
				}
			}
			num++;
		}
		return list;
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x00025367 File Offset: 0x00023567
	protected virtual void OffsetSpawn(Item item)
	{
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x0002536C File Offset: 0x0002356C
	protected SpawnPool GetSpawnPool()
	{
		if (this.isHeightBasedSpawnPool)
		{
			for (int i = this.heightBasedSpawnPools.Count - 1; i >= 0; i--)
			{
				Spawner.HeightBasedSpawnListEntry heightBasedSpawnListEntry = this.heightBasedSpawnPools[i];
				if (i == 0 || (base.transform.position.z > heightBasedSpawnListEntry.minimumZ && (!heightBasedSpawnListEntry.hasBiomeRequirement || Singleton<MapHandler>.Instance.BiomeIsPresent(heightBasedSpawnListEntry.biomeRequirement))))
				{
					return heightBasedSpawnListEntry.spawnPool;
				}
			}
		}
		return this.spawnPool;
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x000253E8 File Offset: 0x000235E8
	private List<GameObject> GetObjectsToSpawn(int spawnCount, bool canRepeat = false)
	{
		if (this.isSingleItem)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < spawnCount; i++)
			{
				list.Add(this.spawnedObjectPrefab);
			}
			return list;
		}
		if (this.isSpawnPool)
		{
			return LootData.GetRandomItems(this.spawnPool, spawnCount, canRepeat);
		}
		if (this.isHeightBasedSpawnPool)
		{
			for (int j = this.heightBasedSpawnPools.Count - 1; j >= 0; j--)
			{
				Spawner.HeightBasedSpawnListEntry heightBasedSpawnListEntry = this.heightBasedSpawnPools[j];
				if (j == 0 || (base.transform.position.z > heightBasedSpawnListEntry.minimumZ && (!heightBasedSpawnListEntry.hasBiomeRequirement || Singleton<MapHandler>.Instance.BiomeIsPresent(heightBasedSpawnListEntry.biomeRequirement))))
				{
					return LootData.GetRandomItems(heightBasedSpawnListEntry.spawnPool, spawnCount, canRepeat);
				}
			}
		}
		List<GameObject> list2 = new List<GameObject>();
		for (int k = 0; k < spawnCount; k++)
		{
			list2.Add(null);
		}
		return list2;
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x000254C8 File Offset: 0x000236C8
	private void FindOutdatedSpawners()
	{
		bool flag = false;
		Spawner[] array = Object.FindObjectsOfType<Spawner>();
		string text = "";
		foreach (Spawner spawner in array)
		{
			if (spawner.hasSpawnList)
			{
				text = text + "Found outdated spawner: " + spawner.gameObject.name + "\n";
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.Log("NO OUTDATED SPAWNERS! YIPPEEEE");
			return;
		}
		Debug.Log(text);
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00025534 File Offset: 0x00023734
	[ContextMenu("Test Weighted Spawn Points")]
	private void TestWeightedSpawnPoints()
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 1000;
		for (int i = 0; i < num; i++)
		{
			Spawner.WeightedSpawnPointEntry weightedSpawnPointEntry = this.weightedSpawnSpots.RandomSelection((Spawner.WeightedSpawnPointEntry w) => w.weight);
			int num2 = this.weightedSpawnSpots.IndexOf(weightedSpawnPointEntry);
			if (dictionary.ContainsKey(num2))
			{
				Dictionary<int, int> dictionary2 = dictionary;
				int num3 = num2;
				int num4 = dictionary2[num3];
				dictionary2[num3] = num4 + 1;
			}
			else
			{
				dictionary.Add(num2, 1);
			}
		}
		string text = string.Format("Test spawned {0} times.\n", num);
		foreach (int num5 in dictionary.Keys)
		{
			text += string.Format("Chose #{0} {1} times. ({2}%)\n", num5, dictionary[num5], (float)dictionary[num5] / (float)num * 100f);
		}
		Debug.Log(text);
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x00025658 File Offset: 0x00023858
	public void DebugSpawnRates()
	{
		SpawnPool spawnPool = this.GetSpawnPool();
		if (spawnPool != SpawnPool.None)
		{
			LootData.PrintOdds(spawnPool);
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x0600068A RID: 1674 RVA: 0x00025678 File Offset: 0x00023878
	private bool hasMultipleFlagsSet
	{
		get
		{
			int num = 0;
			foreach (object obj in Enum.GetValues(typeof(SpawnPool)))
			{
				SpawnPool spawnPool = (SpawnPool)obj;
				if (spawnPool != SpawnPool.None && this.spawnPool.HasFlag(spawnPool))
				{
					if (num >= 1)
					{
						return true;
					}
					num++;
				}
			}
			return false;
		}
	}

	// Token: 0x04000684 RID: 1668
	public int playersInRoomRequirement;

	// Token: 0x04000685 RID: 1669
	public int belowAscentRequirement = -1;

	// Token: 0x04000686 RID: 1670
	public Spawner.SpawnMode spawnMode = Spawner.SpawnMode.SpawnPool;

	// Token: 0x04000687 RID: 1671
	[FormerlySerializedAs("spawnCountMode")]
	public Spawner.SpawnPointMode spawnPointMode;

	// Token: 0x04000688 RID: 1672
	[Range(0f, 1f)]
	public float baseSpawnChance;

	// Token: 0x04000689 RID: 1673
	public GameObject spawnedObjectPrefab;

	// Token: 0x0400068A RID: 1674
	public SpawnList spawns;

	// Token: 0x0400068B RID: 1675
	public SpawnPool spawnPool;

	// Token: 0x0400068C RID: 1676
	public bool canRepeatSpawns;

	// Token: 0x0400068D RID: 1677
	public List<Transform> spawnSpots;

	// Token: 0x0400068E RID: 1678
	public List<Spawner.WeightedSpawnPointEntry> weightedSpawnSpots = new List<Spawner.WeightedSpawnPointEntry>();

	// Token: 0x0400068F RID: 1679
	public Transform spawnUpTowardsTarget;

	// Token: 0x04000690 RID: 1680
	public bool spawnTransformIsSpawnerTransform;

	// Token: 0x04000691 RID: 1681
	public bool spawnAwayFromUpTarget;

	// Token: 0x04000692 RID: 1682
	public bool centerItemsVisually;

	// Token: 0x04000693 RID: 1683
	public bool spawnOnStart;

	// Token: 0x04000694 RID: 1684
	public bool isKinematic = true;

	// Token: 0x04000695 RID: 1685
	public List<Spawner.HeightBasedSpawnListEntry> heightBasedSpawnPools;

	// Token: 0x02000430 RID: 1072
	public enum SpawnMode
	{
		// Token: 0x04001806 RID: 6150
		SingleItem,
		// Token: 0x04001807 RID: 6151
		SpawnPool,
		// Token: 0x04001808 RID: 6152
		HeightBasedSpawnPools,
		// Token: 0x04001809 RID: 6153
		Guidebook
	}

	// Token: 0x02000431 RID: 1073
	public enum SpawnPointMode
	{
		// Token: 0x0400180B RID: 6155
		SingleList,
		// Token: 0x0400180C RID: 6156
		WeightedLists
	}

	// Token: 0x02000432 RID: 1074
	[Serializable]
	public class HeightBasedSpawnListEntry
	{
		// Token: 0x0400180D RID: 6157
		public SpawnPool spawnPool;

		// Token: 0x0400180E RID: 6158
		public float minimumHeight;

		// Token: 0x0400180F RID: 6159
		public float minimumZ;

		// Token: 0x04001810 RID: 6160
		public bool hasBiomeRequirement;

		// Token: 0x04001811 RID: 6161
		public Biome.BiomeType biomeRequirement;
	}

	// Token: 0x02000433 RID: 1075
	[Serializable]
	public class WeightedSpawnPointEntry
	{
		// Token: 0x04001812 RID: 6162
		public List<Transform> spawnSpots;

		// Token: 0x04001813 RID: 6163
		public int weight;

		// Token: 0x04001814 RID: 6164
		[SerializeField]
		internal float percentageOdds;
	}
}
