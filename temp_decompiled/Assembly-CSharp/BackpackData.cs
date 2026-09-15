using System;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x0200010C RID: 268
public class BackpackData : DataEntryValue
{
	// Token: 0x060008BB RID: 2235 RVA: 0x0002F804 File Offset: 0x0002DA04
	public override void Init()
	{
		base.Init();
		byte b = 0;
		while ((int)b < this.itemSlots.Length)
		{
			this.itemSlots[(int)b] = new ItemSlot(b);
			b += 1;
		}
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0002F83C File Offset: 0x0002DA3C
	public override void SerializeValue(BinarySerializer serializer)
	{
		InventorySyncData inventorySyncData = new InventorySyncData(this.itemSlots, new BackpackSlot(4)
		{
			data = new ItemInstanceData(Guid.Empty)
		}, new TemporaryItemSlot(250));
		inventorySyncData.Serialize(serializer);
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x0002F880 File Offset: 0x0002DA80
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		InventorySyncData inventorySyncData = default(InventorySyncData);
		inventorySyncData.Deserialize(deserializer);
		for (byte b = 0; b < 4; b += 1)
		{
			if (this.itemSlots[(int)b] == null)
			{
				this.itemSlots[(int)b] = new ItemSlot(b);
			}
			Item item;
			this.itemSlots[(int)b].prefab = (ItemDatabase.TryGetItem(inventorySyncData.slots[(int)b].ItemID, out item) ? item : null);
			this.itemSlots[(int)b].data = inventorySyncData.slots[(int)b].Data;
			Debug.Log(string.Format("Sync Back Inventory is setting slot: {0} to {1}", b, this.itemSlots[(int)b].ToString()));
		}
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x0002F934 File Offset: 0x0002DB34
	public void AddItem(Item prefab, ItemInstanceData data, byte backpackSlotID)
	{
		if (data == null)
		{
			Debug.Log("DATA IS NULL??");
			data = new ItemInstanceData(Guid.NewGuid());
			ItemInstanceDataHandler.AddInstanceData(data);
		}
		if ((int)backpackSlotID < this.itemSlots.Length && this.itemSlots[(int)backpackSlotID].IsEmpty())
		{
			Debug.Log(string.Format("Added item: {0} to slot {1}", prefab.gameObject.name, backpackSlotID));
			this.itemSlots[(int)backpackSlotID].prefab = prefab;
			this.itemSlots[(int)backpackSlotID].data = data;
			return;
		}
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x0002F9B8 File Offset: 0x0002DBB8
	public bool HasFreeSlot()
	{
		for (int i = 0; i < this.itemSlots.Length; i++)
		{
			if (this.itemSlots[i].IsEmpty())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x0002F9EC File Offset: 0x0002DBEC
	public int FilledSlotCount()
	{
		int num = this.itemSlots.Length;
		for (int i = 0; i < this.itemSlots.Length; i++)
		{
			if (this.itemSlots[i].IsEmpty())
			{
				num--;
			}
		}
		return num;
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x0002FA2C File Offset: 0x0002DC2C
	public override string ToString()
	{
		string text = "";
		foreach (ItemSlot itemSlot in this.itemSlots)
		{
			text = text + itemSlot.ToString() + ", " + Environment.NewLine;
		}
		return text;
	}

	// Token: 0x04000859 RID: 2137
	public const int slots = 4;

	// Token: 0x0400085A RID: 2138
	public ItemSlot[] itemSlots = new ItemSlot[4];
}
