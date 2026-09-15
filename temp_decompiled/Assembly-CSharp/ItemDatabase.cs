using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x0200011C RID: 284
[ConsoleClassCustomizer("Item")]
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scouts/ItemDatabase")]
public class ItemDatabase : ObjectDatabaseAsset<ItemDatabase, Item>
{
	// Token: 0x0600090C RID: 2316 RVA: 0x000307BC File Offset: 0x0002E9BC
	public override void OnLoaded()
	{
		base.OnLoaded();
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x000307C4 File Offset: 0x0002E9C4
	public void LoadItems()
	{
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x000307C6 File Offset: 0x0002E9C6
	[ContextMenu("Reload entire database")]
	public void ReloadAllItems()
	{
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x000307C8 File Offset: 0x0002E9C8
	private ushort GetAvailableID()
	{
		for (ushort num = 0; num < 65535; num += 1)
		{
			if (!this.itemLookup.ContainsKey(num))
			{
				return num;
			}
		}
		return 0;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x000307F8 File Offset: 0x0002E9F8
	private bool ItemExistsInDatabase(Item item, out ushort itemID)
	{
		foreach (ushort num in this.itemLookup.Keys)
		{
			if (this.itemLookup[num].Equals(item))
			{
				itemID = num;
				return true;
			}
		}
		itemID = 0;
		return false;
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x0003086C File Offset: 0x0002EA6C
	[ConsoleCommand]
	public static void Add(Item item)
	{
		if (MainCamera.instance == null)
		{
			return;
		}
		if (!PhotonNetwork.IsConnected)
		{
			return;
		}
		Transform transform = MainCamera.instance.transform;
		RaycastHit raycastHit;
		if (!Physics.Raycast(transform.position, transform.forward, ref raycastHit))
		{
			raycastHit = default(RaycastHit);
		}
		ItemDatabase.Add(item, raycastHit.point + raycastHit.normal);
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000308D0 File Offset: 0x0002EAD0
	public static void Add(Item item, Vector3 point)
	{
		if (!PhotonNetwork.IsConnected)
		{
			return;
		}
		Debug.Log(string.Format("Spawn item: {0} at {1}", item, point));
		PhotonNetwork.Instantiate("0_Items/" + item.name, point, Quaternion.identity, 0, null).GetComponent<Item>().RequestPickup(Character.localCharacter.GetComponent<PhotonView>());
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x0003092C File Offset: 0x0002EB2C
	public static bool TryGetItem(ushort itemID, out Item item)
	{
		return SingletonAsset<ItemDatabase>.Instance.itemLookup.TryGetValue(itemID, ref item);
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00030940 File Offset: 0x0002EB40
	public void AddAllNamesToCSV()
	{
		for (int i = 0; i < this.Objects.Count; i++)
		{
			this.Objects[i].AddNameToCSV();
		}
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00030974 File Offset: 0x0002EB74
	public void AddAllPromptsToCSV()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < this.Objects.Count; i++)
		{
			List<string> list2 = this.Objects[i].AddPromptToCSV(list);
			list.AddRange(list2);
		}
	}

	// Token: 0x04000887 RID: 2183
	public Dictionary<ushort, Item> itemLookup = new Dictionary<ushort, Item>();
}
