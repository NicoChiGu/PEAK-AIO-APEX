using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class ItemBackpackVisuals : BackpackVisuals
{
	// Token: 0x060007C9 RID: 1993 RVA: 0x0002BF69 File Offset: 0x0002A169
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0002BF77 File Offset: 0x0002A177
	public override BackpackData GetBackpackData()
	{
		return base.GetComponent<Item>().GetData<BackpackData>(DataEntryKey.BackpackData);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0002BF85 File Offset: 0x0002A185
	protected override void PutItemInBackpack(GameObject visual, byte slotID)
	{
		visual.GetComponent<PhotonView>().RPC("PutInBackpackRPC", 0, new object[]
		{
			slotID,
			BackpackReference.GetFromBackpackItem(this.item)
		});
	}

	// Token: 0x040007A1 RID: 1953
	private Item item;
}
