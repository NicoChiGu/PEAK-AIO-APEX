using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Zorro.Core.Serizalization;

// Token: 0x020000B7 RID: 183
public class FakeItemManager : MonoBehaviourPunCallbacks
{
	// Token: 0x17000084 RID: 132
	// (get) Token: 0x060006B0 RID: 1712 RVA: 0x000264E3 File Offset: 0x000246E3
	// (set) Token: 0x060006B1 RID: 1713 RVA: 0x00026522 File Offset: 0x00024722
	public static FakeItemManager Instance
	{
		get
		{
			if (FakeItemManager._instance == null)
			{
				FakeItemManager._instance = Object.FindFirstObjectByType<FakeItemManager>();
				if (FakeItemManager._instance == null)
				{
					FakeItemManager._instance = GameUtils.instance.gameObject.AddComponent<FakeItemManager>();
				}
			}
			return FakeItemManager._instance;
		}
		private set
		{
			FakeItemManager._instance = value;
		}
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x0002652A File Offset: 0x0002472A
	private void Awake()
	{
		FakeItemManager.Instance = this;
		if (this.fakeItemData.hiddenItems == null)
		{
			this.fakeItemData.hiddenItems = new List<int>();
		}
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x00026550 File Offset: 0x00024750
	public void CullNullItems()
	{
		for (int i = this.allFakeItems.Count - 1; i >= 0; i--)
		{
			if (this.allFakeItems[i] == null)
			{
				this.allFakeItems.RemoveAt(i);
			}
		}
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x00026598 File Offset: 0x00024798
	public int GetAvailableIndex()
	{
		for (int i = 0; i < 99999; i++)
		{
			bool flag = false;
			for (int j = 0; j < this.allFakeItems.Count; j++)
			{
				if (this.allFakeItems[j] != null && this.allFakeItems[j].index == i)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00026600 File Offset: 0x00024800
	public bool TryGetFakeItem(int index, out FakeItem item)
	{
		for (int i = 0; i < this.allFakeItems.Count; i++)
		{
			if (this.allFakeItems[i] != null && this.allFakeItems[i].index == index)
			{
				item = this.allFakeItems[i];
				return true;
			}
		}
		item = null;
		return false;
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00026660 File Offset: 0x00024860
	public void RefreshList()
	{
		this.allFakeItems = Enumerable.ToList<FakeItem>(Object.FindObjectsByType<FakeItem>(1, 0));
		for (int i = 0; i < this.allFakeItems.Count; i++)
		{
			this.allFakeItems[i].index = i;
		}
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x000266A7 File Offset: 0x000248A7
	public void AddToList(FakeItem item)
	{
		if (!Application.isPlaying)
		{
			this.allFakeItems.Add(item);
		}
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x000266BC File Offset: 0x000248BC
	public void RemoveFromList(FakeItem item)
	{
		if (!Application.isPlaying)
		{
			this.allFakeItems.Remove(item);
		}
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x000266D2 File Offset: 0x000248D2
	public bool Contains(FakeItem item)
	{
		return this.allFakeItems.Contains(item);
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x060006BA RID: 1722 RVA: 0x000266E0 File Offset: 0x000248E0
	public int ItemCount
	{
		get
		{
			return this.allFakeItems.Count;
		}
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x000266F0 File Offset: 0x000248F0
	[PunRPC]
	public void RPC_RequestFakeItemPickup(PhotonView characterView, int fakeItemIndex)
	{
		Character component = characterView.GetComponent<Character>();
		FakeItem fakeItem;
		if (!this.TryGetFakeItem(fakeItemIndex, out fakeItem))
		{
			return;
		}
		ItemSlot itemSlot;
		if (component.player.AddItem(fakeItem.realItemPrefab.itemID, null, out itemSlot) && !fakeItem.pickedUp)
		{
			component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, new object[] { itemSlot.itemSlotID });
			base.photonView.RPC("RPC_FakeItemPickupSuccess", 0, new object[] { fakeItemIndex });
			return;
		}
		base.photonView.RPC("RPC_DenyFakeItemPickup", component.player.photonView.Owner, new object[] { fakeItem.index });
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x000267C8 File Offset: 0x000249C8
	[PunRPC]
	internal void RPC_FakeItemPickupSuccess(int fakeItemIndex)
	{
		FakeItem fakeItem;
		if (this.TryGetFakeItem(fakeItemIndex, out fakeItem))
		{
			fakeItem.PickUpVisibly();
			return;
		}
		Debug.LogWarning(string.Format("Uh oh! We should have successfully picked up a fake item at index {0} ", fakeItemIndex) + string.Format("but that doesn't exist in our list which has {0} items.", this.allFakeItems.Count));
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x0002681C File Offset: 0x00024A1C
	[PunRPC]
	public void RPC_DenyFakeItemPickup(int fakeItemIndex)
	{
		FakeItem fakeItem;
		if (this.TryGetFakeItem(fakeItemIndex, out fakeItem))
		{
			fakeItem.UnPickUpVisibly();
			return;
		}
		Debug.LogWarning(string.Format("Uh oh! We should have failed to pick up a fake item at index {0} ", fakeItemIndex) + string.Format("but that doesn't exist in our list which has {0} items. Won't be able to unhide it.", this.allFakeItems.Count));
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00026870 File Offset: 0x00024A70
	[PunRPC]
	public void RPC_RequestStickFakeItemToPlayer(int characterViewID, int fakeItemIndex, int bodyPartType, Vector3 offset)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		PhotonView photonView = PhotonNetwork.GetPhotonView(characterViewID);
		photonView.GetComponent<Character>().GetBodypart((BodypartType)bodyPartType);
		FakeItem fakeItem;
		if (this.TryGetFakeItem(fakeItemIndex, out fakeItem))
		{
			if (fakeItem.gameObject.activeInHierarchy)
			{
				StickyItemComponent stickyItemComponent;
				if (PhotonNetwork.InstantiateItemRoom(fakeItem.realItemPrefab.name, fakeItem.transform.position, fakeItem.transform.rotation).TryGetComponent<StickyItemComponent>(ref stickyItemComponent))
				{
					stickyItemComponent.photonView.RPC("RPC_StickToCharacterRemote", photonView.Owner, new object[] { characterViewID, bodyPartType, offset });
				}
				base.photonView.RPC("RPC_FakeItemPickupSuccess", 0, new object[] { fakeItemIndex });
				return;
			}
			base.photonView.RPC("RPC_DenyFakeItemPickup", 0, new object[] { fakeItemIndex });
		}
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00026960 File Offset: 0x00024B60
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			byte[] array = IBinarySerializable.ToManagedArray<FakeItemManager.FakeItemData>(this.fakeItemData);
			base.photonView.RPC("RPC_SyncFakeItems", newPlayer, new object[] { array });
		}
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x0002699C File Offset: 0x00024B9C
	[PunRPC]
	public void RPC_SyncFakeItems(byte[] data)
	{
		this.fakeItemData = IBinarySerializable.GetFromManagedArray<FakeItemManager.FakeItemData>(data);
		for (int i = 0; i < this.fakeItemData.hiddenItems.Count; i++)
		{
			int num = this.fakeItemData.hiddenItems[i];
			FakeItem fakeItem;
			if (this.TryGetFakeItem(num, out fakeItem))
			{
				fakeItem.gameObject.SetActive(false);
				fakeItem.pickedUp = true;
			}
			else
			{
				Debug.LogWarning(string.Format("Uh oh! Hidden item at index {0} doesn't exist with index {1} in the ", i, num) + "fake item list! Won't be able to properly sync this item.");
			}
		}
	}

	// Token: 0x040006D1 RID: 1745
	private static FakeItemManager _instance;

	// Token: 0x040006D2 RID: 1746
	internal FakeItemManager.FakeItemData fakeItemData;

	// Token: 0x040006D3 RID: 1747
	[SerializeField]
	[ReadOnly]
	private List<FakeItem> allFakeItems = new List<FakeItem>();

	// Token: 0x02000437 RID: 1079
	public struct FakeItemData : IBinarySerializable
	{
		// Token: 0x06001A86 RID: 6790 RVA: 0x0008062C File Offset: 0x0007E82C
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteInt(this.hiddenItems.Count);
			for (int i = 0; i < this.hiddenItems.Count; i++)
			{
				serializer.WriteInt(this.hiddenItems[i]);
			}
		}

		// Token: 0x06001A87 RID: 6791 RVA: 0x00080674 File Offset: 0x0007E874
		public void Deserialize(BinaryDeserializer deserializer)
		{
			int num = deserializer.ReadInt();
			this.hiddenItems = new List<int>();
			for (int i = 0; i < num; i++)
			{
				this.hiddenItems.Add(deserializer.ReadInt());
			}
		}

		// Token: 0x04001821 RID: 6177
		public List<int> hiddenItems;
	}
}
