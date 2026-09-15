using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x02000101 RID: 257
public struct BackpackReference : IBinarySerializable
{
	// Token: 0x06000877 RID: 2167 RVA: 0x0002E4B8 File Offset: 0x0002C6B8
	public static BackpackReference GetFromBackpackItem(Item item)
	{
		return new BackpackReference
		{
			type = BackpackReference.BackpackType.Item,
			view = item.GetComponent<PhotonView>(),
			locationTransform = item.transform
		};
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0002E4F0 File Offset: 0x0002C6F0
	public static BackpackReference GetFromEquippedBackpack(Character character)
	{
		return new BackpackReference
		{
			type = BackpackReference.BackpackType.Equipped,
			view = character.GetComponent<PhotonView>(),
			locationTransform = character.GetBodypart(BodypartType.Torso).transform
		};
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0002E52E File Offset: 0x0002C72E
	public BackpackVisuals GetVisuals()
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			return this.view.GetComponent<ItemBackpackVisuals>();
		}
		return this.view.GetComponent<CharacterBackpackHandler>().backpackVisuals;
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0002E554 File Offset: 0x0002C754
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteByte((byte)this.type);
		serializer.WriteInt(this.view.ViewID);
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0002E573 File Offset: 0x0002C773
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.type = (BackpackReference.BackpackType)deserializer.ReadByte();
		this.view = PhotonView.Find(deserializer.ReadInt());
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x0002E592 File Offset: 0x0002C792
	public ItemInstanceData GetItemInstanceData()
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			return this.view.GetComponent<Item>().data;
		}
		return this.view.GetComponent<Character>().player.backpackSlot.data;
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0002E5C8 File Offset: 0x0002C7C8
	public BackpackData GetData()
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			return this.view.GetComponent<Item>().GetData<BackpackData>(DataEntryKey.BackpackData);
		}
		BackpackData backpackData;
		if (!this.view.GetComponent<Character>().player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			backpackData = this.view.GetComponent<Character>().player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
		}
		return backpackData;
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0002E635 File Offset: 0x0002C835
	public bool IsOnMyBack()
	{
		return this.type != BackpackReference.BackpackType.Item && this.view.IsMine;
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0002E64C File Offset: 0x0002C84C
	public bool TryGetBackpackItem(out Backpack backpack)
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			backpack = this.view.GetComponent<Backpack>();
			return true;
		}
		backpack = null;
		return false;
	}

	// Token: 0x0400081F RID: 2079
	public BackpackReference.BackpackType type;

	// Token: 0x04000820 RID: 2080
	public PhotonView view;

	// Token: 0x04000821 RID: 2081
	public Transform locationTransform;

	// Token: 0x0200044D RID: 1101
	public enum BackpackType : byte
	{
		// Token: 0x04001889 RID: 6281
		Item,
		// Token: 0x0400188A RID: 6282
		Equipped
	}
}
