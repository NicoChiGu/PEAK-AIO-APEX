using System;

// Token: 0x02000026 RID: 38
[Serializable]
public class ItemSlot
{
	// Token: 0x060002CA RID: 714 RVA: 0x00013BD4 File Offset: 0x00011DD4
	public ItemSlot(byte slotID)
	{
		this.itemSlotID = slotID;
	}

	// Token: 0x060002CB RID: 715 RVA: 0x00013BE3 File Offset: 0x00011DE3
	public virtual bool IsEmpty()
	{
		return this.prefab == null;
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00013BF1 File Offset: 0x00011DF1
	public void SetItem(Item itemPrefab, ItemInstanceData itemData)
	{
		this.data = itemData;
		this.prefab = itemPrefab;
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00013C01 File Offset: 0x00011E01
	public virtual void EmptyOut()
	{
		this.prefab = null;
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00013C0C File Offset: 0x00011E0C
	public override string ToString()
	{
		string text = ((this.prefab == null) ? "null" : this.prefab.name);
		return string.Format("Slot ({0}): {1}", this.itemSlotID, text);
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00013C50 File Offset: 0x00011E50
	public virtual string GetPrefabName()
	{
		if (this.prefab == null)
		{
			return "";
		}
		return this.prefab.gameObject.name;
	}

	// Token: 0x0400029D RID: 669
	public Item prefab;

	// Token: 0x0400029E RID: 670
	public ItemInstanceData data;

	// Token: 0x0400029F RID: 671
	public byte itemSlotID;
}
