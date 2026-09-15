using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001B9 RID: 441
public class BackpackWheelSlice : UIWheelSlice, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000D96 RID: 3478 RVA: 0x00044500 File Offset: 0x00042700
	// (set) Token: 0x06000D97 RID: 3479 RVA: 0x00044508 File Offset: 0x00042708
	public byte backpackSlot { get; private set; }

	// Token: 0x06000D98 RID: 3480 RVA: 0x00044511 File Offset: 0x00042711
	private void UpdateInteractable()
	{
		this.button.interactable = this.canInteract;
	}

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000D99 RID: 3481 RVA: 0x00044524 File Offset: 0x00042724
	private bool canInteract
	{
		get
		{
			return this.isBackpackWear || this.stashSlice || this.hasItem || (Character.localCharacter.data.currentItem != null && Character.localCharacter.data.currentItem.UIData.canBackpack);
		}
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x00044584 File Offset: 0x00042784
	public void InitItemSlot([TupleElementNames(new string[] { null, "slotID" })] ValueTuple<BackpackReference, byte> slot, BackpackWheel wheel)
	{
		this.SharedInit(slot.Item1, wheel);
		this.backpackSlot = slot.Item2;
		this.backpackData = this.backpack.GetData();
		this.itemSlot = this.backpackData.itemSlots[(int)this.backpackSlot];
		Item prefab = this.itemSlot.prefab;
		this.SetItemIcon(prefab, this.itemSlot.data);
		this.UpdateInteractable();
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x000445F9 File Offset: 0x000427F9
	public void InitPickupBackpack(BackpackReference backpack, BackpackWheel wheel)
	{
		this.backpackSlot = byte.MaxValue;
		this.SharedInit(backpack, wheel);
		this.UpdateInteractable();
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x00044614 File Offset: 0x00042814
	public void InitStashSlot(BackpackReference bpRef, BackpackWheel wheel)
	{
		this.backpack = bpRef;
		this.backpackWheel = wheel;
		this.SetItemIcon(Character.localCharacter.data.currentItem, (Character.localCharacter.data.currentItem != null) ? Character.localCharacter.data.currentItem.data : null);
		this.UpdateInteractable();
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x00044678 File Offset: 0x00042878
	private void SharedInit(BackpackReference bpRef, BackpackWheel wheel)
	{
		this.backpack = bpRef;
		this.backpackWheel = wheel;
		if (bpRef.type == BackpackReference.BackpackType.Item)
		{
			Backpack component = Resources.Load<GameObject>("0_Items/Backpack").GetComponent<Backpack>();
			if (this.backpackSlot == 255)
			{
				base.gameObject.SetActive(true);
			}
			this.SetItemIcon(component, null);
			return;
		}
		this.SetItemIcon(null, null);
		if (this.backpackSlot == 255)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x000446F0 File Offset: 0x000428F0
	private void SetItemIcon(Item iconHolder, ItemInstanceData itemInstanceData)
	{
		if (iconHolder == null)
		{
			this.image.enabled = false;
			this.hasItem = false;
		}
		else
		{
			this.image.enabled = true;
			this.image.texture = iconHolder.UIData.GetIcon();
			this.hasItem = true;
		}
		this.UpdateCookedAmount(iconHolder, itemInstanceData);
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x0004474C File Offset: 0x0004294C
	private void UpdateCookedAmount(Item item, ItemInstanceData itemInstanceData)
	{
		if (item == null || itemInstanceData == null)
		{
			this.cookedAmount = 0;
			this.image.color = Color.white;
			return;
		}
		IntItemData intItemData;
		if (itemInstanceData.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) && this.cookedAmount != intItemData.Value)
		{
			this.image.color = Color.white;
			this.image.color = ItemCooking.GetCookColor(intItemData.Value);
			this.cookedAmount = intItemData.Value;
		}
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000DA0 RID: 3488 RVA: 0x000447C8 File Offset: 0x000429C8
	public bool isBackpackWear
	{
		get
		{
			return this.backpackSlot == byte.MaxValue;
		}
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x000447D8 File Offset: 0x000429D8
	public void Hover()
	{
		if (!this.canInteract)
		{
			return;
		}
		BackpackWheelSlice.SliceData sliceData = new BackpackWheelSlice.SliceData
		{
			isBackpackWear = this.isBackpackWear,
			isStashSlice = this.stashSlice,
			backpackReference = this.backpack,
			slotID = this.backpackSlot
		};
		this.backpackWheel.Hover(sliceData);
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x00044838 File Offset: 0x00042A38
	public void Dehover()
	{
		BackpackWheelSlice.SliceData sliceData = new BackpackWheelSlice.SliceData
		{
			isBackpackWear = (this.backpackSlot == byte.MaxValue),
			isStashSlice = this.stashSlice,
			backpackReference = this.backpack,
			slotID = this.backpackSlot
		};
		this.backpackWheel.Dehover(sliceData);
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x00044896 File Offset: 0x00042A96
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x0004489E File Offset: 0x00042A9E
	public void OnPointerExit(PointerEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x04000BB4 RID: 2996
	private BackpackWheel backpackWheel;

	// Token: 0x04000BB6 RID: 2998
	private BackpackReference backpack;

	// Token: 0x04000BB7 RID: 2999
	private BackpackData backpackData;

	// Token: 0x04000BB8 RID: 3000
	private ItemSlot itemSlot;

	// Token: 0x04000BB9 RID: 3001
	public RawImage image;

	// Token: 0x04000BBA RID: 3002
	public bool stashSlice;

	// Token: 0x04000BBB RID: 3003
	private int cookedAmount;

	// Token: 0x04000BBC RID: 3004
	private bool hasItem;

	// Token: 0x020004A9 RID: 1193
	public struct SliceData : IEquatable<BackpackWheelSlice.SliceData>
	{
		// Token: 0x06001BEC RID: 7148 RVA: 0x000838B4 File Offset: 0x00081AB4
		public bool Equals(BackpackWheelSlice.SliceData other)
		{
			return this.isBackpackWear == other.isBackpackWear && this.slotID == other.slotID;
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x000838D4 File Offset: 0x00081AD4
		public override bool Equals(object obj)
		{
			if (obj is BackpackWheelSlice.SliceData)
			{
				BackpackWheelSlice.SliceData sliceData = (BackpackWheelSlice.SliceData)obj;
				return this.Equals(sliceData);
			}
			return false;
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x000838F9 File Offset: 0x00081AF9
		public override int GetHashCode()
		{
			return HashCode.Combine<bool, BackpackReference, byte>(this.isBackpackWear, this.backpackReference, this.slotID);
		}

		// Token: 0x040019F9 RID: 6649
		public bool isBackpackWear;

		// Token: 0x040019FA RID: 6650
		public bool isStashSlice;

		// Token: 0x040019FB RID: 6651
		public BackpackReference backpackReference;

		// Token: 0x040019FC RID: 6652
		public byte slotID;
	}
}
