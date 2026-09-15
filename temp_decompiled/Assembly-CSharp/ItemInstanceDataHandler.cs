using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class ItemInstanceDataHandler : RetrievableSingleton<ItemInstanceDataHandler>
{
	// Token: 0x060007ED RID: 2029 RVA: 0x0002C9A0 File Offset: 0x0002ABA0
	public IEnumerable<ItemInstanceData> GetAllItemInstances()
	{
		return this.m_instanceData.Values;
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0002C9AD File Offset: 0x0002ABAD
	protected override void OnCreated()
	{
		base.OnCreated();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0002C9C0 File Offset: 0x0002ABC0
	public static void AddInstanceData(ItemInstanceData instanceData)
	{
		if (!RetrievableSingleton<ItemInstanceDataHandler>.Instance.m_instanceData.TryAdd(instanceData.guid, instanceData))
		{
			throw new Exception(string.Format("Adding item instance with duplicate guid: {0}", instanceData.guid));
		}
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0002C9F8 File Offset: 0x0002ABF8
	public static bool TryGetInstanceData(Guid guid, out ItemInstanceData o)
	{
		ItemInstanceData itemInstanceData;
		if (RetrievableSingleton<ItemInstanceDataHandler>.Instance.m_instanceData.TryGetValue(guid, ref itemInstanceData))
		{
			o = itemInstanceData;
			return true;
		}
		o = null;
		return false;
	}

	// Token: 0x040007C2 RID: 1986
	private Dictionary<Guid, ItemInstanceData> m_instanceData = new Dictionary<Guid, ItemInstanceData>();
}
