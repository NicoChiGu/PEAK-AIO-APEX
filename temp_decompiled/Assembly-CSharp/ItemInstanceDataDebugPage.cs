using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Zorro.Core.CLI;

// Token: 0x020000D0 RID: 208
public class ItemInstanceDataDebugPage : DebugPage
{
	// Token: 0x060007EB RID: 2027 RVA: 0x0002C7FD File Offset: 0x0002A9FD
	public ItemInstanceDataDebugPage()
	{
		this.ScrollView = new ScrollView();
		base.Add(this.ScrollView);
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0002C828 File Offset: 0x0002AA28
	public override void Update()
	{
		base.Update();
		List<ItemInstanceData> list = new List<ItemInstanceData>();
		foreach (Player player in PlayerHandler.GetAllPlayers())
		{
			foreach (ItemSlot itemSlot in player.itemSlots)
			{
				if (!itemSlot.IsEmpty())
				{
					list.Add(itemSlot.data);
				}
			}
		}
		foreach (ItemInstanceData itemInstanceData in list)
		{
			if (!this.m_spawnedCells.ContainsKey(itemInstanceData.guid))
			{
				DataEntryValue dataEntryValue;
				if (itemInstanceData.data.Count == 1 && itemInstanceData.data.TryGetValue(DataEntryKey.ItemUses, ref dataEntryValue))
				{
					OptionableIntItemData optionableIntItemData = dataEntryValue as OptionableIntItemData;
					if (optionableIntItemData != null && !optionableIntItemData.HasData)
					{
						continue;
					}
				}
				ItemInstanceDataUICell itemInstanceDataUICell = new ItemInstanceDataUICell(itemInstanceData);
				this.ScrollView.Add(itemInstanceDataUICell);
				this.m_spawnedCells.Add(itemInstanceData.guid, itemInstanceDataUICell);
			}
		}
		foreach (KeyValuePair<Guid, ItemInstanceDataUICell> keyValuePair in this.m_spawnedCells)
		{
			keyValuePair.Value.Update();
		}
	}

	// Token: 0x040007C0 RID: 1984
	private Dictionary<Guid, ItemInstanceDataUICell> m_spawnedCells = new Dictionary<Guid, ItemInstanceDataUICell>();

	// Token: 0x040007C1 RID: 1985
	private ScrollView ScrollView;
}
