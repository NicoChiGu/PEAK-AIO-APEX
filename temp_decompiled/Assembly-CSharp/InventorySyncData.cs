using System;
using Zorro.Core.Serizalization;

// Token: 0x02000148 RID: 328
public struct InventorySyncData : IBinarySerializable
{
	// Token: 0x06000A97 RID: 2711 RVA: 0x000389C8 File Offset: 0x00036BC8
	public InventorySyncData(ItemSlot[] itemSlots, BackpackSlot backpackSlot, ItemSlot tempSlot)
	{
		this.slotCount = (byte)itemSlots.Length;
		this.slots = new InventorySyncData.SlotData[itemSlots.Length];
		InventorySyncData.SlotData slotData;
		for (int i = 0; i < (int)this.slotCount; i++)
		{
			ushort num = ((itemSlots[i].prefab == null) ? ushort.MaxValue : itemSlots[i].prefab.itemID);
			InventorySyncData.SlotData[] array = this.slots;
			int num2 = i;
			slotData = new InventorySyncData.SlotData
			{
				ItemID = num,
				Data = itemSlots[i].data
			};
			array[num2] = slotData;
		}
		slotData = new InventorySyncData.SlotData
		{
			ItemID = ((tempSlot.prefab == null) ? ushort.MaxValue : tempSlot.prefab.itemID),
			Data = tempSlot.data
		};
		this.tempSlot = slotData;
		this.hasBackpack = !backpackSlot.IsEmpty();
		this.backpackInstanceData = backpackSlot.data;
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00038AB0 File Offset: 0x00036CB0
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteByte(this.slotCount);
		for (int i = 0; i < (int)this.slotCount; i++)
		{
			this.slots[i].Serialize(serializer);
		}
		this.tempSlot.Serialize(serializer);
		serializer.WriteBool(this.hasBackpack);
		if (this.hasBackpack)
		{
			if (this.backpackInstanceData == null)
			{
				this.backpackInstanceData = new ItemInstanceData(Guid.NewGuid());
				ItemInstanceDataHandler.AddInstanceData(this.backpackInstanceData);
			}
			serializer.WriteGuid(this.backpackInstanceData.guid);
			this.backpackInstanceData.Serialize(serializer);
		}
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x00038B4C File Offset: 0x00036D4C
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.slotCount = deserializer.ReadByte();
		this.slots = new InventorySyncData.SlotData[(int)this.slotCount];
		this.tempSlot = default(InventorySyncData.SlotData);
		for (int i = 0; i < (int)this.slotCount; i++)
		{
			InventorySyncData.SlotData slotData = default(InventorySyncData.SlotData);
			slotData.Deserialize(deserializer);
			this.slots[i] = slotData;
		}
		this.tempSlot.Deserialize(deserializer);
		this.hasBackpack = deserializer.ReadBool();
		if (this.hasBackpack)
		{
			Guid guid = deserializer.ReadGuid();
			if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out this.backpackInstanceData))
			{
				this.backpackInstanceData = new ItemInstanceData(guid);
				ItemInstanceDataHandler.AddInstanceData(this.backpackInstanceData);
			}
			this.backpackInstanceData.Deserialize(deserializer);
		}
	}

	// Token: 0x040009EE RID: 2542
	public byte slotCount;

	// Token: 0x040009EF RID: 2543
	public InventorySyncData.SlotData[] slots;

	// Token: 0x040009F0 RID: 2544
	public InventorySyncData.SlotData tempSlot;

	// Token: 0x040009F1 RID: 2545
	public bool hasBackpack;

	// Token: 0x040009F2 RID: 2546
	public ItemInstanceData backpackInstanceData;

	// Token: 0x02000470 RID: 1136
	public struct SlotData : IBinarySerializable
	{
		// Token: 0x06001B3F RID: 6975 RVA: 0x0008219A File Offset: 0x0008039A
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteUshort(this.ItemID);
			if (this.ItemID != 65535)
			{
				serializer.WriteGuid(this.Data.guid);
				this.Data.Serialize(serializer);
			}
		}

		// Token: 0x06001B40 RID: 6976 RVA: 0x000821D4 File Offset: 0x000803D4
		public void Deserialize(BinaryDeserializer deserializer)
		{
			this.ItemID = deserializer.ReadUShort();
			if (this.ItemID != 65535)
			{
				Guid guid = deserializer.ReadGuid();
				if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out this.Data))
				{
					this.Data = new ItemInstanceData(guid);
					ItemInstanceDataHandler.AddInstanceData(this.Data);
				}
				this.Data.Deserialize(deserializer);
			}
		}

		// Token: 0x0400192C RID: 6444
		public ushort ItemID;

		// Token: 0x0400192D RID: 6445
		public ItemInstanceData Data;
	}
}
