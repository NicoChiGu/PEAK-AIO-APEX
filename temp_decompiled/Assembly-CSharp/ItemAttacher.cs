using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class ItemAttacher : MonoBehaviour
{
	// Token: 0x06000902 RID: 2306 RVA: 0x00030615 File Offset: 0x0002E815
	private void Update()
	{
		if (!PhotonNetwork.InRoom || !Character.localCharacter)
		{
			return;
		}
		if (!this.attached)
		{
			this.TryAttach();
		}
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x0003063C File Offset: 0x0002E83C
	private void TryAttach()
	{
		foreach (Item item in Item.ALL_ITEMS)
		{
			if (item.itemID == this.itemPrefabReference.itemID && item.itemState == ItemState.Ground && item.rig.isKinematic && Vector3.Distance(item.transform.position, base.transform.position) < 1f)
			{
				item.transform.SetParent(base.transform, true);
				item.GetComponent<ItemPhysicsSyncer>().shouldSync = false;
				item.transform.localPosition = this.offset;
				this.attached = true;
				this.attachedItem = item;
			}
		}
	}

	// Token: 0x04000881 RID: 2177
	private bool attached;

	// Token: 0x04000882 RID: 2178
	public Item itemPrefabReference;

	// Token: 0x04000883 RID: 2179
	public Item attachedItem;

	// Token: 0x04000884 RID: 2180
	public Vector3 offset;
}
