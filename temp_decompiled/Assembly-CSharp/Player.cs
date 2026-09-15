using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Core.Serizalization;

// Token: 0x0200002D RID: 45
public class Player : MonoBehaviourPunCallbacks
{
	// Token: 0x17000047 RID: 71
	// (get) Token: 0x0600033A RID: 826 RVA: 0x0001646F File Offset: 0x0001466F
	public Character character
	{
		get
		{
			return PlayerHandler.GetPlayerCharacter(this.view.Owner);
		}
	}

	// Token: 0x0600033B RID: 827 RVA: 0x00016484 File Offset: 0x00014684
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		byte b = 0;
		while ((int)b < this.itemSlots.Length)
		{
			this.itemSlots[(int)b] = new ItemSlot(b);
			b += 1;
		}
		this.tempFullSlot = new TemporaryItemSlot(250);
		this.backpackSlot = new BackpackSlot(3);
		if (this.view != null)
		{
			PlayerHandler.RegisterPlayer(this);
			if (this.view.IsMine)
			{
				Player.localPlayer = this;
			}
		}
		base.gameObject.name = "Player: " + this.view.Owner.NickName;
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00016528 File Offset: 0x00014728
	public bool AddItem(ushort itemID, ItemInstanceData instanceData, out ItemSlot slot)
	{
		Player.<>c__DisplayClass13_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.instanceData = instanceData;
		if (CS$<>8__locals1.instanceData == null)
		{
			CS$<>8__locals1.instanceData = new ItemInstanceData(Guid.NewGuid());
			ItemInstanceDataHandler.AddInstanceData(CS$<>8__locals1.instanceData);
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("Only Master Client can add items!");
			slot = null;
			return false;
		}
		if (!ItemDatabase.TryGetItem(itemID, out CS$<>8__locals1.ItemPrefab))
		{
			Debug.LogError(string.Format("Failed to get item from item ID: {0}", itemID));
			slot = null;
			return false;
		}
		slot = this.<AddItem>g__AddToSlot|13_0(ref CS$<>8__locals1);
		if (slot == null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Failed adding ",
				CS$<>8__locals1.ItemPrefab.name,
				" to ",
				base.name,
				"'s inventory, no slots available!"
			}));
			return false;
		}
		Debug.Log(string.Format("Granting {0}: {1} and added to slot: {2}", base.name, CS$<>8__locals1.ItemPrefab.name, slot.itemSlotID));
		byte[] array = IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot));
		this.view.RPC("SyncInventoryRPC", 1, new object[] { array, false });
		return true;
	}

	// Token: 0x0600033D RID: 829 RVA: 0x00016665 File Offset: 0x00014865
	public static void LeaveCurrentGame()
	{
		if (NetCode.Matchmaking.InLobby)
		{
			NetCode.Matchmaking.LeaveLobby();
		}
		PhotonNetwork.Disconnect();
		Debug.Log("Leaving game and returning to main menu.");
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0001668C File Offset: 0x0001488C
	[PunRPC]
	public void SyncInventoryRPC(byte[] data, bool forceSync)
	{
		if (!forceSync && PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("SyncInventoryRPC should not sync to Master client. They are the boss");
			return;
		}
		InventorySyncData fromManagedArray = IBinarySerializable.GetFromManagedArray<InventorySyncData>(data);
		byte b = 0;
		while ((int)b < this.itemSlots.Length)
		{
			Item item;
			this.itemSlots[(int)b].prefab = (ItemDatabase.TryGetItem(fromManagedArray.slots[(int)b].ItemID, out item) ? item : null);
			this.itemSlots[(int)b].data = fromManagedArray.slots[(int)b].Data;
			b += 1;
		}
		this.backpackSlot.hasBackpack = fromManagedArray.hasBackpack;
		this.backpackSlot.data = fromManagedArray.backpackInstanceData;
		Item item2;
		this.tempFullSlot.prefab = (ItemDatabase.TryGetItem(fromManagedArray.tempSlot.ItemID, out item2) ? item2 : null);
		this.tempFullSlot.data = fromManagedArray.tempSlot.Data;
		if (this.view.IsMine)
		{
			this.character.refs.items.RefreshAllCharacterCarryWeightRPC();
		}
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0001678F File Offset: 0x0001498F
	[PunRPC]
	public void RPC_GetKicked(PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			Debug.LogWarning("Some naughty player attempted to kick me without authority to do so!");
			return;
		}
		NetworkConnector.ChangeConnectionState<KickedState>();
	}

	// Token: 0x06000340 RID: 832 RVA: 0x000167B0 File Offset: 0x000149B0
	[PunRPC]
	public void RPCRemoveItemFromSlot(byte slotID)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("Only Master Client can remove items!");
			return;
		}
		ItemSlot itemSlot = this.GetItemSlot(slotID);
		if (itemSlot != null)
		{
			itemSlot.EmptyOut();
		}
		InventorySyncData inventorySyncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
		this.view.RPC("SyncInventoryRPC", 1, new object[]
		{
			IBinarySerializable.ToManagedArray<InventorySyncData>(inventorySyncData),
			false
		});
	}

	// Token: 0x06000341 RID: 833 RVA: 0x00016824 File Offset: 0x00014A24
	public void EmptySlot(Optionable<byte> slot)
	{
		if (slot.IsNone)
		{
			Debug.LogError("Can't empty none slot");
			return;
		}
		byte value = slot.Value;
		ItemSlot itemSlot = this.GetItemSlot(value);
		if (itemSlot != null)
		{
			itemSlot.EmptyOut();
		}
		if (PhotonNetwork.IsMasterClient)
		{
			InventorySyncData inventorySyncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
			this.view.RPC("SyncInventoryRPC", 1, new object[]
			{
				IBinarySerializable.ToManagedArray<InventorySyncData>(inventorySyncData),
				false
			});
			return;
		}
		this.view.RPC("RPCRemoveItemFromSlot", 2, new object[] { value });
	}

	// Token: 0x06000342 RID: 834 RVA: 0x000168CC File Offset: 0x00014ACC
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		InventorySyncData inventorySyncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
		this.view.RPC("SyncInventoryRPC", newPlayer, new object[]
		{
			IBinarySerializable.ToManagedArray<InventorySyncData>(inventorySyncData),
			false
		});
	}

	// Token: 0x06000343 RID: 835 RVA: 0x00016922 File Offset: 0x00014B22
	[PunRPC]
	public void RPC_SetInventory(byte[] newInventory)
	{
	}

	// Token: 0x06000344 RID: 836 RVA: 0x00016924 File Offset: 0x00014B24
	public ItemSlot GetItemSlot(byte slotID)
	{
		if (slotID == 3)
		{
			return this.backpackSlot;
		}
		if (slotID == 250)
		{
			return this.tempFullSlot;
		}
		if (!ArrayExtensions.WithinRange<ItemSlot>(this.itemSlots, (int)slotID))
		{
			Debug.LogError(string.Format("{0} is attempting to get a non-existent ItemSlot index: {1}", base.name, slotID), this);
			return null;
		}
		return this.itemSlots[(int)slotID];
	}

	// Token: 0x06000345 RID: 837 RVA: 0x00016980 File Offset: 0x00014B80
	public bool HasEmptySlot(ushort itemID)
	{
		Item item;
		if (!ItemDatabase.TryGetItem(itemID, out item))
		{
			Debug.LogError(string.Format("Failed to get item from item ID: {0}", itemID));
			return false;
		}
		if (item is Backpack)
		{
			return this.backpackSlot.IsEmpty();
		}
		ItemSlot[] array = this.itemSlots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsEmpty())
			{
				return true;
			}
		}
		return this.tempFullSlot.IsEmpty();
	}

	// Token: 0x06000346 RID: 838 RVA: 0x000169EE File Offset: 0x00014BEE
	[ContextMenu("Debug Print Player ID")]
	private void DebugPrintPlayerID()
	{
		Debug.Log(base.photonView.Owner.ActorNumber);
	}

	// Token: 0x06000347 RID: 839 RVA: 0x00016A0C File Offset: 0x00014C0C
	public bool HasInAnySlot(ushort itemID)
	{
		foreach (ItemSlot itemSlot in this.itemSlots)
		{
			if (!itemSlot.IsEmpty() && itemSlot.prefab.itemID == itemID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000348 RID: 840 RVA: 0x00016A4C File Offset: 0x00014C4C
	[ConsoleCommand]
	public static void PrintInventory(Player player)
	{
		byte b = 0;
		foreach (ItemSlot itemSlot in player.itemSlots)
		{
			Debug.Log(string.Format("Slot{0}: {1}", b, itemSlot.ToString()));
			if (!itemSlot.IsEmpty())
			{
				Debug.Log(string.Format("Data [{0}, keys: {1}]", itemSlot.data.guid, itemSlot.data.data.Count));
				foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in itemSlot.data.data)
				{
					Debug.Log(string.Format("{0} : {1}", keyValuePair.Key, keyValuePair.Value.GetType().Name));
					Debug.Log(keyValuePair.Value.ToString());
				}
			}
			b += 1;
		}
	}

	// Token: 0x0600034A RID: 842 RVA: 0x00016B70 File Offset: 0x00014D70
	[CompilerGenerated]
	private ItemSlot <AddItem>g__AddToSlot|13_0(ref Player.<>c__DisplayClass13_0 A_1)
	{
		if (A_1.ItemPrefab is Backpack)
		{
			if (this.backpackSlot.IsEmpty())
			{
				this.backpackSlot.hasBackpack = true;
				this.backpackSlot.data = A_1.instanceData;
				return this.backpackSlot;
			}
			return null;
		}
		else
		{
			for (int i = 0; i < this.itemSlots.Length; i++)
			{
				if (this.itemSlots[i].IsEmpty())
				{
					this.itemSlots[i].SetItem(A_1.ItemPrefab, A_1.instanceData);
					return this.itemSlots[i];
				}
			}
			if (this.tempFullSlot.IsEmpty() && !this.character.data.isClimbingAnything)
			{
				this.tempFullSlot.SetItem(A_1.ItemPrefab, A_1.instanceData);
				return this.tempFullSlot;
			}
			return null;
		}
	}

	// Token: 0x04000300 RID: 768
	public const int BACKPACKSLOTINDEX = 3;

	// Token: 0x04000301 RID: 769
	public ItemSlot[] itemSlots = new ItemSlot[3];

	// Token: 0x04000302 RID: 770
	public ItemSlot tempFullSlot;

	// Token: 0x04000303 RID: 771
	public BackpackSlot backpackSlot;

	// Token: 0x04000304 RID: 772
	public Action<int> hotbarSelectionChanged;

	// Token: 0x04000305 RID: 773
	public Action<ItemSlot[]> itemsChangedAction;

	// Token: 0x04000306 RID: 774
	public static Player localPlayer;

	// Token: 0x04000307 RID: 775
	public bool hasClosedEndScreen;

	// Token: 0x04000308 RID: 776
	public bool doneWithCutscene;

	// Token: 0x04000309 RID: 777
	private PhotonView view;
}
