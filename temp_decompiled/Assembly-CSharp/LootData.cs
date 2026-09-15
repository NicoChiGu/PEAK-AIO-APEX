using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000298 RID: 664
public class LootData : MonoBehaviour
{
	// Token: 0x06001249 RID: 4681 RVA: 0x0005C910 File Offset: 0x0005AB10
	public static List<Item> GetAllItemsInPool(SpawnPool pool)
	{
		List<Item> list = new List<Item>();
		LootData.PopulateLootData();
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(pool, ref dictionary))
		{
			foreach (KeyValuePair<ushort, int> keyValuePair in dictionary)
			{
				Item item;
				if (ItemDatabase.TryGetItem(keyValuePair.Key, out item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x0005C98C File Offset: 0x0005AB8C
	public bool IsValidToSpawn()
	{
		if (this.banInSolo)
		{
			if (PhotonNetwork.OfflineMode)
			{
				return false;
			}
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount <= 1)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x0005C9B8 File Offset: 0x0005ABB8
	private void PrintOdds()
	{
		LootData.PopulateLootData();
		Item component = base.GetComponent<Item>();
		if (!component)
		{
			Debug.LogError("Loot data only works on Items right now.");
		}
		string text = base.gameObject.name + " appears in pools:\n";
		foreach (KeyValuePair<SpawnPool, Dictionary<ushort, int>> keyValuePair in LootData.AllSpawnWeightData)
		{
			if (keyValuePair.Value.ContainsKey(component.itemID))
			{
				text += string.Format("{0} ({1}%)\n", keyValuePair.Key.ToString(), LootData.GetPercentageOdds(component.itemID, keyValuePair.Key));
			}
		}
		Debug.Log(text);
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x0005CA90 File Offset: 0x0005AC90
	public static void PrintOdds(SpawnPool pool)
	{
		LootData.PopulateLootData();
		string text = pool.ToString() + " contains items:\n";
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(pool, ref dictionary))
		{
			foreach (KeyValuePair<ushort, int> keyValuePair in dictionary)
			{
				Item item;
				if (ItemDatabase.TryGetItem(keyValuePair.Key, out item))
				{
					LootData component = item.GetComponent<LootData>();
					if (component)
					{
						text += string.Format("{0} ({1}% ({2}))\n", item.gameObject.name, LootData.GetPercentageOdds(keyValuePair.Key, pool), component.Rarity.ToString());
					}
					else
					{
						text += string.Format("{0} ({1}%)\n", item.gameObject.name, LootData.GetPercentageOdds(keyValuePair.Key, pool));
					}
				}
			}
		}
		Debug.Log(text);
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x0005CBAC File Offset: 0x0005ADAC
	public static GameObject GetRandomItem(SpawnPool spawnPool)
	{
		if (LootData.AllSpawnWeightData == null)
		{
			LootData.PopulateLootData();
		}
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(spawnPool, ref dictionary))
		{
			Item item;
			ItemDatabase.TryGetItem(dictionary.RandomSelection((KeyValuePair<ushort, int> i) => i.Value).Key, out item);
			return item.gameObject;
		}
		return null;
	}

	// Token: 0x0600124E RID: 4686 RVA: 0x0005CC14 File Offset: 0x0005AE14
	public static List<GameObject> GetRandomItems(SpawnPool spawnPool, int count, bool canRepeat = false)
	{
		if (LootData.AllSpawnWeightData == null)
		{
			LootData.PopulateLootData();
		}
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(spawnPool, ref dictionary))
		{
			Dictionary<ushort, int> dictionary2 = new Dictionary<ushort, int>(dictionary);
			List<GameObject> list = new List<GameObject>();
			for (int j = 0; j < count; j++)
			{
				ushort key = dictionary2.RandomSelection((KeyValuePair<ushort, int> i) => i.Value).Key;
				Item item;
				if (ItemDatabase.TryGetItem(key, out item))
				{
					if (!item.IsValidToSpawn())
					{
						Debug.Log(item.gameObject.name + " IS INVALID TO SPAWN");
						dictionary2.Remove(key);
						j--;
					}
					else
					{
						list.Add(item.gameObject);
						if (!canRepeat)
						{
							dictionary2.Remove(key);
						}
					}
				}
			}
			return list;
		}
		return null;
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x0005CCE8 File Offset: 0x0005AEE8
	public static float GetPercentageOdds(ushort itemID, SpawnPool pool)
	{
		if (LootData.AllSpawnWeightData.ContainsKey(pool))
		{
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<ushort, int> keyValuePair in LootData.AllSpawnWeightData[pool])
			{
				num += keyValuePair.Value;
				if (keyValuePair.Key == itemID)
				{
					num2 = keyValuePair.Value;
				}
			}
			return (float)Mathf.FloorToInt((float)num2 / (float)num * 1000f) / 10f;
		}
		return 0f;
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x0005CD84 File Offset: 0x0005AF84
	public static void PopulateLootData()
	{
		LootData.AllSpawnWeightData = new Dictionary<SpawnPool, Dictionary<ushort, int>>();
		Array values = Enum.GetValues(typeof(SpawnPool));
		foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			LootData component = keyValuePair.Value.GetComponent<LootData>();
			if (component)
			{
				foreach (object obj in values)
				{
					SpawnPool spawnPool = (SpawnPool)obj;
					if (spawnPool != SpawnPool.None && component.spawnLocations.HasFlag(spawnPool))
					{
						int num = LootData.RarityWeights[component.Rarity];
						if (!LootData.AllSpawnWeightData.ContainsKey(spawnPool))
						{
							Dictionary<SpawnPool, Dictionary<ushort, int>> allSpawnWeightData = LootData.AllSpawnWeightData;
							SpawnPool spawnPool2 = spawnPool;
							Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
							dictionary.Add(keyValuePair.Key, num);
							allSpawnWeightData.Add(spawnPool2, dictionary);
						}
						else
						{
							LootData.AllSpawnWeightData[spawnPool].Add(keyValuePair.Key, num);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x0005CEE4 File Offset: 0x0005B0E4
	// Note: this type is marked as 'beforefieldinit'.
	static LootData()
	{
		Dictionary<Rarity, int> dictionary = new Dictionary<Rarity, int>();
		dictionary.Add(Rarity.Common, 100);
		dictionary.Add(Rarity.Uncommon, 50);
		dictionary.Add(Rarity.Rare, 30);
		dictionary.Add(Rarity.Epic, 20);
		dictionary.Add(Rarity.Legendary, 15);
		dictionary.Add(Rarity.Mythic, 5);
		dictionary.Add(Rarity.RidiculouslyRare, 1);
		LootData.RarityWeights = dictionary;
	}

	// Token: 0x040010CA RID: 4298
	public Rarity Rarity;

	// Token: 0x040010CB RID: 4299
	public SpawnPool spawnLocations;

	// Token: 0x040010CC RID: 4300
	public List<ItemRarityOverride> rarityOverrides = new List<ItemRarityOverride>();

	// Token: 0x040010CD RID: 4301
	public bool banInSolo;

	// Token: 0x040010CE RID: 4302
	public static Dictionary<SpawnPool, Dictionary<ushort, int>> AllSpawnWeightData = null;

	// Token: 0x040010CF RID: 4303
	public static Dictionary<Rarity, int> RarityWeights;
}
