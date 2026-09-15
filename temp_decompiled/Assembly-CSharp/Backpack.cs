using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000005 RID: 5
public class Backpack : Item
{
	// Token: 0x06000005 RID: 5 RVA: 0x00002141 File Offset: 0x00000341
	protected override void AddPhysics()
	{
		base.AddPhysics();
		this.rig.sleepThreshold = 0f;
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002159 File Offset: 0x00000359
	public override void Interact(Character interactor)
	{
		GUIManager.instance.OpenBackpackWheel(BackpackReference.GetFromBackpackItem(this));
	}

	// Token: 0x06000007 RID: 7 RVA: 0x0000216B File Offset: 0x0000036B
	protected override void Update()
	{
		this.groundMesh.gameObject.SetActive(base.itemState == ItemState.Ground);
		this.heldMesh.gameObject.SetActive(base.itemState > ItemState.Ground);
		base.Update();
	}

	// Token: 0x06000008 RID: 8 RVA: 0x000021A5 File Offset: 0x000003A5
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x06000009 RID: 9 RVA: 0x000021A7 File Offset: 0x000003A7
	public void Wear(Character interactor)
	{
		base.Interact(interactor);
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000021B0 File Offset: 0x000003B0
	private void DisableVisuals()
	{
		this.mainRenderer.shadowCastingMode = 3;
	}

	// Token: 0x0600000B RID: 11 RVA: 0x000021BE File Offset: 0x000003BE
	private void EnableVisuals()
	{
		this.mainRenderer.shadowCastingMode = 1;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x000021CC File Offset: 0x000003CC
	public void Stash(Character interactor, byte backpackSlotID)
	{
		if (!interactor.data.currentItem)
		{
			return;
		}
		if (!this.HasSpace())
		{
			return;
		}
		CharacterItems items = interactor.refs.items;
		if (items.currentSelectedSlot.IsNone)
		{
			Debug.LogError("Need item slot selected to stash item in backpack!");
			return;
		}
		ItemSlot itemSlot = interactor.player.GetItemSlot(items.currentSelectedSlot.Value);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Failed to get a non-null item slot for {0}", items.currentSelectedSlot.Value));
			return;
		}
		if (itemSlot.IsEmpty())
		{
			Debug.LogError(string.Format("Item slot {0} is empty!", itemSlot.itemSlotID));
			return;
		}
		this.view.RPC("RPCAddItemToBackpack", 0, new object[]
		{
			interactor.player.GetComponent<PhotonView>(),
			items.currentSelectedSlot.Value,
			backpackSlotID
		});
		interactor.player.EmptySlot(items.currentSelectedSlot);
		if (items.currentSelectedSlot.IsSome && items.currentSelectedSlot.Value == 250)
		{
			interactor.photonView.RPC("DestroyHeldItemRpc", 0, Array.Empty<object>());
			return;
		}
		items.EquipSlot(Optionable<byte>.None);
	}

	// Token: 0x0600000D RID: 13 RVA: 0x0000230C File Offset: 0x0000050C
	[PunRPC]
	public void RPCAddItemToBackpack(PhotonView playerView, byte slotID, byte backpackSlotID)
	{
		BackpackData data = base.GetData<BackpackData>(DataEntryKey.BackpackData);
		ItemSlot itemSlot = playerView.GetComponent<Player>().GetItemSlot(slotID);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Can't add item because slot ID {0} is not valid.", slotID));
			return;
		}
		data.AddItem(itemSlot.prefab, itemSlot.data, backpackSlotID);
		if (PhotonNetwork.IsMasterClient)
		{
			base.GetComponent<BackpackVisuals>().RefreshVisuals();
		}
	}

	// Token: 0x0600000E RID: 14 RVA: 0x0000236C File Offset: 0x0000056C
	private void OnDestroy()
	{
		base.GetComponent<BackpackVisuals>().RemoveVisuals();
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002379 File Offset: 0x00000579
	private bool HasSpace()
	{
		return base.GetData<BackpackData>(DataEntryKey.BackpackData).HasFreeSlot();
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002387 File Offset: 0x00000587
	public int FilledSlotCount()
	{
		return base.GetData<BackpackData>(DataEntryKey.BackpackData).FilledSlotCount();
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002395 File Offset: 0x00000595
	public override string GetInteractionText()
	{
		return LocalizedText.GetText("open", true);
	}

	// Token: 0x06000012 RID: 18 RVA: 0x000023A2 File Offset: 0x000005A2
	public override void OnInstanceDataRecieved()
	{
		base.OnInstanceDataRecieved();
		base.GetComponent<BackpackVisuals>().RefreshVisuals();
	}

	// Token: 0x06000013 RID: 19 RVA: 0x000023B8 File Offset: 0x000005B8
	[ConsoleCommand]
	public static void PrintBackpacks()
	{
		foreach (Backpack backpack in Object.FindObjectsByType<Backpack>(0, 1))
		{
			List<ItemSlot> list = Enumerable.ToList<ItemSlot>(Enumerable.Where<ItemSlot>(backpack.GetData<BackpackData>(DataEntryKey.BackpackData).itemSlots, (ItemSlot slot) => !slot.IsEmpty()));
			Debug.Log(string.Format("Backpack: {0}, Full Slots: {1}", backpack.GetInstanceID(), list.Count));
			foreach (ItemSlot itemSlot in list)
			{
				Debug.Log(string.Format("Slot: {0}, data entries: {1}", itemSlot.GetPrefabName(), itemSlot.data.data.Count));
			}
		}
	}

	// Token: 0x06000014 RID: 20 RVA: 0x000024A8 File Offset: 0x000006A8
	public bool IsConstantlyInteractable(Character interactor)
	{
		return false;
	}

	// Token: 0x06000015 RID: 21 RVA: 0x000024AB File Offset: 0x000006AB
	public float GetInteractTime(Character interactor)
	{
		return this.openRadialMenuTime;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x000024B3 File Offset: 0x000006B3
	public void Interact_CastFinished(Character interactor)
	{
	}

	// Token: 0x06000017 RID: 23 RVA: 0x000024B5 File Offset: 0x000006B5
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000018 RID: 24 RVA: 0x000024B7 File Offset: 0x000006B7
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04000002 RID: 2
	public Transform[] backpackSlots;

	// Token: 0x04000003 RID: 3
	public float openRadialMenuTime = 0.25f;

	// Token: 0x04000004 RID: 4
	public GameObject groundMesh;

	// Token: 0x04000005 RID: 5
	public GameObject heldMesh;
}
