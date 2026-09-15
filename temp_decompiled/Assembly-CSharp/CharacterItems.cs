using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000013 RID: 19
public class CharacterItems : MonoBehaviourPunCallbacks
{
	// Token: 0x06000192 RID: 402 RVA: 0x0000B668 File Offset: 0x00009868
	private IEnumerator SubscribeRoutine(bool subscribe)
	{
		while (!this.character.player)
		{
			yield return null;
		}
		if (subscribe)
		{
			Player player = this.character.player;
			player.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Combine(player.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount));
		}
		else
		{
			Player player2 = this.character.player;
			player2.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player2.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount));
		}
		if (subscribe)
		{
			Player player3 = this.character.player;
			player3.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Combine(player3.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateBalloonCount));
		}
		else
		{
			Player player4 = this.character.player;
			player4.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player4.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateBalloonCount));
		}
		yield break;
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0000B67E File Offset: 0x0000987E
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.photonView = base.GetComponent<PhotonView>();
		this.currentSelectedSlot = Optionable<byte>.None;
		this.lastSelectedSlot = Optionable<byte>.Some(0);
		base.StartCoroutine(this.SubscribeRoutine(true));
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0000B6C0 File Offset: 0x000098C0
	private void OnDestroy()
	{
		if (this.character.player)
		{
			Player player = this.character.player;
			player.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount));
			Player player2 = this.character.player;
			player2.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player2.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateBalloonCount));
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x0000B737 File Offset: 0x00009937
	private void FixedUpdate()
	{
		if (this.character.data.currentItem)
		{
			this.HoldItem(this.character.data.currentItem);
		}
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0000B768 File Offset: 0x00009968
	private void HoldItem(Item item)
	{
		Vector3 vector = this.GetItemHoldPos(item) - item.transform.position;
		item.rig.AddForce(vector * this.holdForce, 5);
		Vector3 itemHoldForward = this.GetItemHoldForward(item);
		Vector3 itemHoldUp = this.GetItemHoldUp(item);
		Vector3 vector2 = Vector3.Cross(item.transform.forward, itemHoldForward).normalized * Vector3.Angle(item.transform.forward, itemHoldForward);
		vector2 += Vector3.Cross(item.transform.up, itemHoldUp).normalized * Vector3.Angle(item.transform.up, itemHoldUp);
		item.rig.AddTorque(vector2 * this.holdTorque, 5);
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000B838 File Offset: 0x00009A38
	private void Update()
	{
		if (this.character.refs.afflictions.isWebbed)
		{
			return;
		}
		this.DoSwitching();
		this.DoDropping();
		this.DoUsing();
		this.UpdateClimbingSpikeUse();
		if (this.character.IsLocal)
		{
			this.isChargingThrow = this.throwChargeLevel > 0f;
		}
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000B898 File Offset: 0x00009A98
	private void DoUsing()
	{
		if (!this.character.data.currentItem)
		{
			return;
		}
		if (this.character.data.passedOut || this.character.data.fullyPassedOut)
		{
			return;
		}
		if (this.character.input.usePrimaryWasPressed && this.character.data.currentItem.CanUsePrimary())
		{
			this.character.data.currentItem.StartUsePrimary();
		}
		if (this.character.input.usePrimaryIsPressed && this.character.data.currentItem.CanUsePrimary())
		{
			this.character.data.currentItem.ContinueUsePrimary();
		}
		if (this.character.input.usePrimaryWasReleased || (this.character.data.currentItem.isUsingPrimary && !this.character.data.currentItem.CanUsePrimary()))
		{
			this.character.data.currentItem.CancelUsePrimary();
		}
		if (!this.character.CanDoInput())
		{
			this.character.data.currentItem.CancelUsePrimary();
		}
		if (this.character.input.useSecondaryIsPressed && this.character.data.currentItem.CanUseSecondary())
		{
			this.character.data.currentItem.StartUseSecondary();
		}
		if (this.character.input.useSecondaryIsPressed && this.character.data.currentItem.CanUseSecondary())
		{
			this.character.data.currentItem.ContinueUseSecondary();
		}
		if (this.character.input.useSecondaryWasReleased || (this.character.data.currentItem.isUsingSecondary && !this.character.data.currentItem.CanUseSecondary()))
		{
			this.character.data.currentItem.CancelUseSecondary();
		}
		if (this.character.input.scrollBackwardWasPressed)
		{
			this.character.data.currentItem.ScrollButtonBackwardPressed();
		}
		if (this.character.input.scrollForwardWasPressed)
		{
			this.character.data.currentItem.ScrollButtonForwardPressed();
		}
		if (this.character.input.scrollBackwardIsPressed)
		{
			this.character.data.currentItem.ScrollButtonBackwardHeld();
		}
		if (this.character.input.scrollForwardIsPressed)
		{
			this.character.data.currentItem.ScrollButtonForwardHeld();
		}
		if (this.character.input.scrollInput != 0f)
		{
			this.character.data.currentItem.Scroll(this.character.input.scrollInput);
		}
	}

	// Token: 0x06000199 RID: 409 RVA: 0x0000BB80 File Offset: 0x00009D80
	private void DoDropping()
	{
		if (this.character.data.currentItem != null && this.character.data.currentItem.progress > 0f)
		{
			this.throwChargeLevel = 0f;
			return;
		}
		if (this.character.input.dropWasPressed && this.character.data.currentItem && this.character.data.currentItem.UIData.canDrop)
		{
			this.lastPressedDrop = Time.time;
			this.pressedDrop = true;
		}
		if (this.pressedDrop && this.character.input.dropWasReleased && this.character.data.currentItem && this.currentSelectedSlot.IsSome)
		{
			Vector3 position = this.character.data.currentItem.transform.position;
			Vector3 vector = this.character.data.currentItem.rig.linearVelocity;
			if (this.character.data.currentItem is Backpack)
			{
				vector = Vector3.zero;
			}
			bool flag = false;
			if (this.character.data.currentItem != null)
			{
				StickyItemComponent currentStickyItem = this.character.data.currentStickyItem;
				if (currentStickyItem && this.throwChargeLevel < currentStickyItem.throwChargeRequirement)
				{
					flag = true;
				}
			}
			if ((this.throwChargeLevel > 0.1f || flag) && this.character.refs.animations)
			{
				this.character.refs.animations.throwTime = 0.125f;
			}
			if (flag)
			{
				return;
			}
			ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
			if (itemSlot == null)
			{
				Debug.LogError(string.Format("Attempted to get an item in invalid slot {0}", this.currentSelectedSlot.Value));
				return;
			}
			this.photonView.RPC("DropItemRpc", 0, new object[]
			{
				this.throwChargeLevel,
				this.currentSelectedSlot.Value,
				position,
				vector,
				this.character.data.currentItem.transform.rotation,
				itemSlot.data,
				false
			});
			this.throwChargeLevel = 0f;
			this.EquipSlot(Optionable<byte>.None);
		}
		if (this.pressedDrop && this.character.input.dropIsPressed && Time.time - this.lastPressedDrop > this.delayBeforeThrowCharge)
		{
			this.throwChargeLevel = Mathf.Min(this.throwChargeLevel + 1f / this.throwChargeTime * Time.deltaTime, 1f);
			return;
		}
		this.throwChargeLevel = 0f;
	}

	// Token: 0x0600019A RID: 410 RVA: 0x0000BE84 File Offset: 0x0000A084
	internal void DropAllItems(bool includeBackpack)
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		Transform transform = this.character.GetBodypart(BodypartType.Hip).transform;
		Vector3 vector = transform.forward;
		if (Vector3.Dot(vector, Vector3.up) < 0f)
		{
			vector = -vector;
		}
		Vector3 vector2 = transform.position + vector * 0.6f;
		if (this.currentSelectedSlot.IsSome && this.character.data.currentItem)
		{
			ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
			if (itemSlot == null)
			{
				Debug.LogError(string.Format("Couldn't get item slot for index {0}", this.currentSelectedSlot.Value));
			}
			else
			{
				this.photonView.RPC("DropItemRpc", 0, new object[]
				{
					this.throwChargeLevel,
					this.currentSelectedSlot.Value,
					this.character.data.currentItem.transform.position,
					Vector3.zero,
					this.character.data.currentItem.transform.rotation,
					itemSlot.data,
					true
				});
				vector2 += Vector3.up * 0.5f;
			}
		}
		for (int i = (includeBackpack ? 3 : 2); i >= 0; i--)
		{
			this.photonView.RPC("DropItemFromSlotRPC", 0, new object[]
			{
				(byte)i,
				vector2
			});
			vector2 += Vector3.up * 0.5f;
		}
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000C058 File Offset: 0x0000A258
	[PunRPC]
	internal void DropItemFromSlotRPC(byte slotID, Vector3 spawnPosition)
	{
		ItemSlot itemSlot = this.character.player.GetItemSlot(slotID);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Can't drop item from non-existent slot {0}", slotID));
			return;
		}
		if (!itemSlot.IsEmpty())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonView component = PhotonNetwork.Instantiate("0_Items/" + itemSlot.GetPrefabName(), spawnPosition, Quaternion.identity, 0, null).GetComponent<PhotonView>();
				component.RPC("SetItemInstanceDataRPC", 0, new object[] { itemSlot.data });
				component.RPC("SetKinematicRPC", 0, new object[]
				{
					false,
					component.transform.position,
					component.transform.rotation
				});
				this.droppedItems.Add(component);
			}
			this.character.player.EmptySlot(Optionable<byte>.Some(slotID));
		}
		this.character.refs.afflictions.UpdateWeight();
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000C15C File Offset: 0x0000A35C
	[PunRPC]
	public void DestroyHeldItemRpc()
	{
		Item currentItem = this.character.data.currentItem;
		if (currentItem == null)
		{
			return;
		}
		this.UnAttachEquippedItem();
		if (currentItem.photonView.IsMine || (currentItem.photonView.Controller.IsMasterClient && PhotonNetwork.IsMasterClient))
		{
			PhotonNetwork.Destroy(currentItem.gameObject);
		}
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000C1BC File Offset: 0x0000A3BC
	[PunRPC]
	public void DropItemRpc(float throwCharge, byte slotID, Vector3 spawnPos, Vector3 velocity, Quaternion rotation, ItemInstanceData itemInstanceData, bool cacheToDroppedItems)
	{
		if (!this.character.data.currentItem)
		{
			return;
		}
		float num = 0f;
		if (throwCharge > 0f)
		{
			num = this.minThrowForce + (this.maxThrowForce - this.minThrowForce) * throwCharge;
		}
		Item currentItem = this.character.data.currentItem;
		this.UnAttachEquippedItem();
		if (currentItem.photonView.IsMine || (currentItem.photonView.Controller.IsMasterClient && PhotonNetwork.IsMasterClient))
		{
			PhotonNetwork.Destroy(currentItem.gameObject);
		}
		ItemSlot itemSlot = this.character.player.GetItemSlot(slotID);
		if (itemSlot == null)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			string prefabName = itemSlot.GetPrefabName();
			if (string.IsNullOrEmpty(prefabName))
			{
				return;
			}
			Vector3 normalized = HelperFunctions.LookToDirection(this.character.data.lookValues, Vector3.forward).normalized;
			PhotonView component = PhotonNetwork.InstantiateItemRoom(prefabName, spawnPos, rotation).GetComponent<PhotonView>();
			GameUtils.instance.IgnoreCollisions(this.character.gameObject, component.gameObject, 0.5f);
			Rigidbody component2 = component.GetComponent<Rigidbody>();
			component.RPC("SetKinematicRPC", 3, new object[]
			{
				false,
				component.transform.position,
				component.transform.rotation
			});
			if (cacheToDroppedItems)
			{
				this.droppedItems.Add(component);
			}
			Item component3 = component.GetComponent<Item>();
			if (component3)
			{
				component3.photonView.RPC("RPC_SetThrownData", 0, new object[]
				{
					this.character.photonView.ViewID,
					throwCharge
				});
			}
			component2.linearVelocity = velocity + normalized * num * 0.5f * component3.throwForceMultiplier;
			if (!component3.GetComponent<Frisbee>())
			{
				component2.angularVelocity = Vector3.Cross(normalized, Vector3.up) * num * 0.5f;
			}
			component.RPC("SetItemInstanceDataRPC", 0, new object[] { itemInstanceData });
		}
		this.character.player.EmptySlot(Optionable<byte>.Some(slotID));
		this.pressedDrop = false;
		this.character.refs.afflictions.UpdateWeight();
	}

	// Token: 0x0600019E RID: 414 RVA: 0x0000C42C File Offset: 0x0000A62C
	[PunRPC]
	public void OnPickupAccepted(byte slotID)
	{
		if (slotID != 3)
		{
			if (this.character.data.isClimbingAnything)
			{
				if (slotID == 250)
				{
					Debug.LogWarning("Accidental wall stow in temp slot detected! Dropping item.");
					Transform transform = this.character.GetBodypart(BodypartType.Hip).transform;
					Vector3 forward = transform.forward;
					Vector3 vector = transform.position - forward * 0.6f;
					this.photonView.RPC("DropItemFromSlotRPC", 0, new object[] { slotID, vector });
				}
			}
			else
			{
				this.character.refs.items.EquipSlot(Optionable<byte>.Some(slotID));
			}
		}
		else if (this.character.data.carriedPlayer != null)
		{
			this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
		}
		this.RefreshAllCharacterCarryWeight();
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000C520 File Offset: 0x0000A720
	public void RefreshAllCharacterCarryWeight()
	{
		this.photonView.RPC("RefreshAllCharacterCarryWeightRPC", 0, Array.Empty<object>());
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000C538 File Offset: 0x0000A738
	[PunRPC]
	public void RefreshAllCharacterCarryWeightRPC()
	{
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			allPlayerCharacters[i].refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000C574 File Offset: 0x0000A774
	public void EquipSlot(Optionable<byte> slotID)
	{
		CharacterItems.<>c__DisplayClass32_0 CS$<>8__locals1 = new CharacterItems.<>c__DisplayClass32_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.slotID = slotID;
		this.lastEquippedSlotTime = Time.time;
		CS$<>8__locals1.waitForFrames = false;
		if (CS$<>8__locals1.slotID.IsSome)
		{
			this.lastSelectedSlot = CS$<>8__locals1.slotID;
		}
		if (this.photonView.IsMine && this.character.data.currentItem != null)
		{
			this.character.data.currentItem.CancelUsePrimary();
			this.character.data.currentItem.CancelUseSecondary();
			if (!this.character.data.currentItem.UIData.canPocket || (this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == 250 && !this.character.player.GetItemSlot(this.currentSelectedSlot.Value).IsEmpty()))
			{
				Vector3 vector = this.character.data.currentItem.transform.position + Vector3.down * 0.2f;
				Vector3 linearVelocity = this.character.data.currentItem.rig.linearVelocity;
				CS$<>8__locals1.waitForFrames = true;
				ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
				this.photonView.RPC("DropItemRpc", 0, new object[]
				{
					this.throwChargeLevel,
					this.currentSelectedSlot.Value,
					vector,
					linearVelocity,
					this.character.data.currentItem.transform.rotation,
					itemSlot.data,
					false
				});
			}
		}
		base.StartCoroutine(CS$<>8__locals1.<EquipSlot>g__TheRest|0());
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000C774 File Offset: 0x0000A974
	[PunRPC]
	public void EquipSlotRpc(int slotID, int objectViewID)
	{
		if (!this.photonView.IsMine)
		{
			if (slotID == -1)
			{
				if (this.currentSelectedSlot.IsSome)
				{
					this.lastSelectedSlot = this.currentSelectedSlot;
				}
				this.currentSelectedSlot = Optionable<byte>.None;
			}
			else
			{
				this.currentSelectedSlot = Optionable<byte>.Some((byte)slotID);
			}
		}
		PhotonView photonView = null;
		if (objectViewID != -1)
		{
			photonView = PhotonNetwork.GetPhotonView(objectViewID);
		}
		Item item;
		if (photonView != null)
		{
			item = this.Equip(photonView.GetComponent<Item>());
		}
		else
		{
			item = this.Equip(null);
		}
		if (this.photonView.IsMine && item != null)
		{
			item.OnStash();
			Debug.Log(this.character.gameObject.name + " destroying " + item.gameObject.name);
			PhotonNetwork.Destroy(item.GetComponent<PhotonView>());
		}
		if (this.character.player.itemsChangedAction != null)
		{
			this.character.player.itemsChangedAction.Invoke(this.character.player.itemSlots);
		}
		Action action = this.onSlotEquipped;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000C88C File Offset: 0x0000AA8C
	public Item Equip(Item item)
	{
		CharacterItems.<>c__DisplayClass35_0 CS$<>8__locals1 = new CharacterItems.<>c__DisplayClass35_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.item = item;
		Item currentItem = this.character.data.currentItem;
		this.pressedDrop = false;
		if (this.character.data.currentItem)
		{
			this.UnAttachEquippedItem();
		}
		if (CS$<>8__locals1.item == null)
		{
			return currentItem;
		}
		this.character.data.currentItem = CS$<>8__locals1.item;
		CS$<>8__locals1.item.holderCharacter = this.character;
		CS$<>8__locals1.item.SetState(ItemState.Held, this.character);
		base.StartCoroutine(CS$<>8__locals1.<Equip>g__IWait|0());
		return currentItem;
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000C93C File Offset: 0x0000AB3C
	private void AttachItem(Item item)
	{
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.position = this.GetItemPosRightWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.position = this.GetItemPosLeftWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.rotation = this.GetItemRotRightWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.rotation = this.GetItemRotLeftWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_R).gameObject.AddComponent<FixedJoint>().connectedBody = item.rig;
		this.character.GetBodypartRig(BodypartType.Hand_L).gameObject.AddComponent<FixedJoint>().connectedBody = item.rig;
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000CA04 File Offset: 0x0000AC04
	public void UpdateAttachedItem()
	{
		if (!this.character.data.currentItem)
		{
			return;
		}
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.position = this.GetItemPosRightWorld(this.character.data.currentItem);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.position = this.GetItemPosLeftWorld(this.character.data.currentItem);
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.rotation = this.GetItemRotRightWorld(this.character.data.currentItem);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.rotation = this.GetItemRotLeftWorld(this.character.data.currentItem);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000CADB File Offset: 0x0000ACDB
	private void UnAttachItem()
	{
		if (this.<UnAttachItem>g__TryDestroyJoint|38_0(BodypartType.Hand_L) && this.<UnAttachItem>g__TryDestroyJoint|38_0(BodypartType.Hand_R))
		{
			return;
		}
		Debug.LogError("WOOPS! Failed to destroy FixedJoints for some reason. Something must have gone wrong with the item we were trying to hold.", this);
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000CAFC File Offset: 0x0000ACFC
	private Quaternion GetItemHoldRotation(Item item)
	{
		return Quaternion.LookRotation(this.GetItemHoldForward(item), this.GetItemHoldUp(item));
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000CB11 File Offset: 0x0000AD11
	private Vector3 GetItemHoldUp(Item item)
	{
		return this.character.data.lookDirection_Up;
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000CB23 File Offset: 0x0000AD23
	private Vector3 GetItemHoldForward(Item item)
	{
		return this.character.data.lookDirection;
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0000CB38 File Offset: 0x0000AD38
	public Vector3 GetItemHoldPos(Item item)
	{
		Vector3 vector = this.character.refs.animationItemTransform.position - this.character.refs.animationHipTransform.position;
		return this.character.refs.hip.transform.position + vector;
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000CB95 File Offset: 0x0000AD95
	public void UnAttachEquippedItem()
	{
		this.UnAttachItem();
		this.character.data.currentItem = null;
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x060001AC RID: 428 RVA: 0x0000CBAE File Offset: 0x0000ADAE
	private float equippedSlotCooldown
	{
		get
		{
			if (this.timesSwitchedRecently >= 3)
			{
				return 0.25f;
			}
			return 0.1f;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060001AD RID: 429 RVA: 0x0000CBC4 File Offset: 0x0000ADC4
	private bool lockedFromSwitching
	{
		get
		{
			return this.lastEquippedSlotTime + this.equippedSlotCooldown > Time.time;
		}
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000CBDC File Offset: 0x0000ADDC
	private void DoSwitching()
	{
		if (this.timesSwitchedRecently > 0 && this.lastSwitched + 0.4f < Time.time)
		{
			this.timesSwitchedRecently = 0;
		}
		if (this.character.data.currentItem != null)
		{
			if (this.character.data.currentItem.progress > 0f)
			{
				return;
			}
			if (this.character.data.currentItem.lastFinishedCast + 0.1f > Time.time)
			{
				return;
			}
		}
		if (!this.character.data.fullyConscious)
		{
			return;
		}
		if (this.character.IsLocal && this.character.CanDoInput() && !this.lockedFromSwitching)
		{
			if (this.character.data.isClimbing || this.character.data.isRopeClimbing)
			{
				return;
			}
			if (this.character.input.selectSlotForwardWasPressed)
			{
				if (this.character.data.currentStickyItem)
				{
					this.character.refs.animations.throwTime = 0.125f;
					return;
				}
				this.character.player.GetItemSlot(3).IsEmpty();
				byte b = decimal.ToByte((int)(this.lastSelectedSlot.Value + 1));
				if ((int)b > this.character.player.itemSlots.Length)
				{
					Debug.Log("Looping to start");
					b = 0;
				}
				this.lastSwitched = Time.time;
				this.timesSwitchedRecently++;
				this.EquipSlot(Optionable<byte>.Some(b));
			}
			else if (this.character.input.selectSlotBackwardWasPressed)
			{
				if (this.character.data.currentStickyItem)
				{
					this.character.refs.animations.throwTime = 0.125f;
					return;
				}
				int num = ((this.lastSelectedSlot.Value == 250) ? (-1) : ((int)(this.lastSelectedSlot.Value - 1)));
				if (num < 0)
				{
					num = (int)decimal.ToByte(3m);
					Debug.Log("Looping to end");
				}
				this.lastSwitched = Time.time;
				this.timesSwitchedRecently++;
				this.EquipSlot(Optionable<byte>.Some(decimal.ToByte(num)));
			}
			else if (this.character.input.unselectSlotWasPressed && !this.lockedFromSwitching)
			{
				if (this.character.data.currentStickyItem)
				{
					this.character.refs.animations.throwTime = 0.125f;
					return;
				}
				if (this.currentSelectedSlot.IsSome)
				{
					this.lastSwitched = Time.time;
					this.timesSwitchedRecently++;
					this.EquipSlot(Optionable<byte>.None);
				}
				else
				{
					this.lastSwitched = Time.time;
					this.timesSwitchedRecently++;
					this.EquipSlot(this.lastSelectedSlot);
				}
			}
			for (byte b2 = 0; b2 <= 3; b2 += 1)
			{
				if (this.character.input.SelectSlotWasPressed((int)b2))
				{
					if (this.character.data.currentStickyItem)
					{
						this.character.refs.animations.throwTime = 0.125f;
						return;
					}
					if (!ArrayExtensions.WithinRange<ItemSlot>(this.character.player.itemSlots, (int)b2) && b2 != 3)
					{
						this.lastSwitched = Time.time;
						this.timesSwitchedRecently++;
						this.EquipSlot(Optionable<byte>.None);
					}
					else
					{
						if (this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == b2)
						{
							this.lastSwitched = Time.time;
							this.timesSwitchedRecently++;
							this.EquipSlot(Optionable<byte>.None);
						}
						else
						{
							this.lastSwitched = Time.time;
							this.timesSwitchedRecently++;
							this.EquipSlot(Optionable<byte>.Some(b2));
						}
						if (b2 == 3 && this.character.data.carriedPlayer != null)
						{
							this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
						}
					}
				}
			}
		}
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000D033 File Offset: 0x0000B233
	internal void AddGravity(Vector3 gravity)
	{
		this.character.data.currentItem.rig.AddForce(gravity, 5);
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000D051 File Offset: 0x0000B251
	internal void AddMovementForce(float movementForce)
	{
		this.character.data.currentItem.rig.AddForce(movementForce * this.character.data.worldMovementInput_Grounded, 5);
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000D084 File Offset: 0x0000B284
	internal void AddDrag(float drag, float factor = 1f)
	{
		drag = Mathf.Lerp(1f, drag, factor);
		this.character.data.currentItem.rig.linearVelocity *= drag;
		this.character.data.currentItem.rig.angularVelocity *= drag;
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000D0EC File Offset: 0x0000B2EC
	internal void AddParasolDrag(float drag, float xzDrag, float factor = 1f)
	{
		drag = Mathf.Lerp(1f, drag, factor);
		if (this.character.data.currentItem.rig.linearVelocity.y < 0f)
		{
			this.character.data.currentItem.rig.linearVelocity = new Vector3(this.character.data.currentItem.rig.linearVelocity.x * xzDrag, this.character.data.currentItem.rig.linearVelocity.y * drag, this.character.data.currentItem.rig.linearVelocity.z * xzDrag);
		}
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000D1B2 File Offset: 0x0000B3B2
	internal Vector3 GetItemPosRightWorld(Item item)
	{
		return item.transform.Find("Hand_R").position;
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000D1C9 File Offset: 0x0000B3C9
	internal Vector3 GetItemPosLeftWorld(Item item)
	{
		return item.transform.Find("Hand_L").position;
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000D1E0 File Offset: 0x0000B3E0
	internal Quaternion GetItemRotRightWorld(Item item)
	{
		return item.transform.Find("Hand_R").rotation;
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000D1F7 File Offset: 0x0000B3F7
	internal Quaternion GetItemRotLeftWorld(Item item)
	{
		return item.transform.Find("Hand_L").rotation;
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0000D210 File Offset: 0x0000B410
	internal Vector3 GetItemPosRight(Item item)
	{
		Vector3 localPosition = item.transform.Find("Hand_R").localPosition;
		return this.character.refs.animationItemTransform.TransformPoint(localPosition);
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x0000D24C File Offset: 0x0000B44C
	internal Quaternion GetItemRotRight(Item item)
	{
		Transform transform = item.transform.Find("Hand_R");
		Vector3 vector = item.transform.InverseTransformDirection(transform.forward);
		Vector3 vector2 = item.transform.InverseTransformDirection(transform.up);
		Vector3 vector3 = this.character.refs.animationItemTransform.TransformDirection(vector);
		Vector3 vector4 = this.character.refs.animationItemTransform.TransformDirection(vector2);
		return Quaternion.LookRotation(vector3, vector4);
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000D2C4 File Offset: 0x0000B4C4
	internal Quaternion GetItemRotLeft(Item item)
	{
		Transform transform = item.transform.Find("Hand_L");
		Vector3 vector = item.transform.InverseTransformDirection(transform.forward);
		Vector3 vector2 = item.transform.InverseTransformDirection(transform.up);
		Vector3 vector3 = this.character.refs.animationItemTransform.TransformDirection(vector);
		Vector3 vector4 = this.character.refs.animationItemTransform.TransformDirection(vector2);
		return Quaternion.LookRotation(vector3, vector4);
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000D33C File Offset: 0x0000B53C
	internal Vector3 GetItemPosLeft(Item item)
	{
		Vector3 vector = HelperFunctions.MultiplyVectors(item.transform.Find("Hand_L").localPosition, item.transform.lossyScale);
		return this.character.refs.animationItemTransform.TransformPoint(vector);
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000D388 File Offset: 0x0000B588
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient && this.character.data.currentItem != null)
		{
			Debug.Log("Setting " + base.gameObject.name + " to hold " + this.character.data.currentItem.name);
			this.photonView.RPC("RPC_InitHoldingItem", newPlayer, new object[] { this.character.data.currentItem.GetComponent<PhotonView>() });
		}
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000D41E File Offset: 0x0000B61E
	[PunRPC]
	public void RPC_InitHoldingItem(PhotonView item)
	{
		Debug.Log("Init holding item: " + item.name);
		this.Equip(item.GetComponent<Item>());
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000D444 File Offset: 0x0000B644
	public void UpdateClimbingSpikeCount(ItemSlot[] slots)
	{
		int num = 0;
		this.currentClimbingSpikeComponent = null;
		this.currentClimbingSpikeItemSlot = null;
		foreach (ItemSlot itemSlot in slots)
		{
			if (itemSlot != null && itemSlot.prefab != null)
			{
				ClimbingSpikeComponent component = itemSlot.prefab.GetComponent<ClimbingSpikeComponent>();
				IntItemData intItemData;
				if (component != null && (!itemSlot.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) || intItemData.Value <= 0))
				{
					num++;
					if (this.currentClimbingSpikeComponent == null)
					{
						this.currentClimbingSpikeComponent = component;
						this.currentClimbingSpikeItemSlot = itemSlot;
					}
				}
			}
		}
		this.character.data.climbingSpikeCount = num;
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000D4E4 File Offset: 0x0000B6E4
	public void UpdateBalloonCount(ItemSlot[] slots)
	{
		if (!this.character.refs.items.currentSelectedSlot.IsSome)
		{
			this.character.refs.balloons.heldBalloonCount = 0;
			return;
		}
		ItemSlot itemSlot = this.character.player.GetItemSlot(this.character.refs.items.currentSelectedSlot.Value);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Can't get invalid item slot {0}", this.character.refs.items.currentSelectedSlot.Value));
		}
		if (!itemSlot.IsEmpty() && this.character.refs.items.currentSelectedSlot.Value != 3 && this.character.refs.items.currentSelectedSlot.IsSome && itemSlot.prefab.GetComponent<Balloon>())
		{
			this.character.refs.balloons.heldBalloonCount = 1;
			return;
		}
		this.character.refs.balloons.heldBalloonCount = 0;
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000D604 File Offset: 0x0000B804
	private bool WithinClimbingSpikePreviewRange()
	{
		if (this.currentClimbingSpikePreview)
		{
			float num = (this.character.data.isClimbingAnything ? this.currentClimbingSpikeComponent.climbingSpikePreviewDisableDistance : this.currentClimbingSpikeComponent.climbingSpikePreviewDisableDistanceGrounded);
			return Vector3.Distance(MainCamera.instance.transform.position, this.currentClimbingSpikePreview.transform.position) <= num;
		}
		return false;
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000D675 File Offset: 0x0000B875
	public float climbingSpikeCastProgress
	{
		get
		{
			if (this.currentClimbingSpikeItemSlot == null)
			{
				return 0f;
			}
			if (this.currentClimbingSpikeItemSlot.prefab == null)
			{
				return 0f;
			}
			return this.climbingSpikeTick / this.currentClimbingSpikeItemSlot.prefab.usingTimePrimary;
		}
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000D6B8 File Offset: 0x0000B8B8
	private void UpdateClimbingSpikeUse()
	{
		if (this.character.data.climbingSpikeCount <= 0 || this.currentClimbingSpikeItemSlot == null)
		{
			this.CancelClimbingSpike();
			return;
		}
		if (this.climbingSpikeTick > 0f)
		{
			if (!this.WithinClimbingSpikePreviewRange())
			{
				this.CancelClimbingSpike();
				return;
			}
			if ((this.spikingWithPrimary && !this.character.input.usePrimaryIsPressed) || (this.spikingWithSecondary && !this.character.input.useSecondaryIsPressed))
			{
				this.CancelClimbingSpike();
				return;
			}
			this.climbingSpikeTick += Time.deltaTime;
			if (this.climbingSpikeTick >= this.currentClimbingSpikeItemSlot.prefab.usingTimePrimary)
			{
				this.HammerClimbingSpike(this.climbingSpikeHit);
				this.CancelClimbingSpike();
			}
			return;
		}
		else
		{
			if (!this.RaycastClimbingSpikeStart())
			{
				this.climbingSpikeTick = 0f;
				return;
			}
			if (this.climbingSpikeTick == 0f)
			{
				if (this.character.input.usePrimaryIsPressed && !this.character.data.isClimbingAnything && this.climbingSpikeSelected)
				{
					this.spikingWithPrimary = true;
					this.spikingWithSecondary = false;
					this.climbingSpikeTick += Time.deltaTime;
					this.InstantiateClimbingSpikePreview(this.climbingSpikeHit);
					return;
				}
				if (this.character.input.useSecondaryIsPressed && (this.climbingSpikeSelected || this.character.data.isClimbingAnything))
				{
					this.spikingWithPrimary = false;
					this.spikingWithSecondary = true;
					this.climbingSpikeTick += Time.deltaTime;
					this.InstantiateClimbingSpikePreview(this.climbingSpikeHit);
				}
			}
			return;
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x060001C2 RID: 450 RVA: 0x0000D853 File Offset: 0x0000BA53
	private bool climbingSpikeSelected
	{
		get
		{
			return this.currentClimbingSpikeItemSlot != null && this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == this.currentClimbingSpikeItemSlot.itemSlotID;
		}
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000D884 File Offset: 0x0000BA84
	private void CancelClimbingSpike()
	{
		if (this.currentClimbingSpikePreview)
		{
			Debug.Log("Cancelling climbing spike");
			Object.Destroy(this.currentClimbingSpikePreview);
		}
		this.climbingSpikeTick = 0f;
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000D8B4 File Offset: 0x0000BAB4
	private void InstantiateClimbingSpikePreview(RaycastHit hit)
	{
		if (!this.currentClimbingSpikePreview && this.currentClimbingSpikeComponent != null)
		{
			this.currentClimbingSpikePreview = Object.Instantiate<GameObject>(this.currentClimbingSpikeComponent.climbingSpikePreviewPrefab);
		}
		if (this.currentClimbingSpikePreview)
		{
			this.currentClimbingSpikePreview.transform.position = this.climbingSpikeHit.point;
			this.currentClimbingSpikePreview.transform.rotation = Quaternion.LookRotation(-this.climbingSpikeHit.normal, Vector3.up);
		}
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000D944 File Offset: 0x0000BB44
	public bool RaycastClimbingSpikeStart()
	{
		float num = (this.character.data.isClimbingAnything ? this.currentClimbingSpikeComponent.climbingSpikeStartDistance : this.currentClimbingSpikeComponent.climbingSpikeStartDistanceGrounded);
		return Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, ref this.climbingSpikeHit, num, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap));
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000D9B4 File Offset: 0x0000BBB4
	private void HammerClimbingSpike(RaycastHit hit)
	{
		if (this.currentClimbingSpikeComponent != null && PhotonNetwork.Instantiate("0_Items/" + this.currentClimbingSpikeComponent.hammeredVersionPrefab.gameObject.name, hit.point, Quaternion.LookRotation(-hit.normal, Vector3.up), 0, null) != null)
		{
			if (this.currentClimbingSpikeItemSlot != null)
			{
				ItemSlot itemSlot = this.currentClimbingSpikeItemSlot;
				this.currentClimbingSpikeItemSlot = null;
				this.currentClimbingSpikeComponent = null;
				this.character.player.EmptySlot(Optionable<byte>.Some(itemSlot.itemSlotID));
				if (this.character.data.currentItem != null)
				{
					this.EquipSlot(Optionable<byte>.None);
				}
				this.UpdateClimbingSpikeCount(this.character.player.itemSlots);
				this.character.data.lastConsumedItem = Time.time;
			}
			this.character.refs.afflictions.UpdateWeight();
			Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PitonsPlaced, 1);
			GameUtils.instance.IncrementPermanentItemsPlaced();
		}
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000DAD2 File Offset: 0x0000BCD2
	internal void SpawnItemInHand(string objName)
	{
		this.photonView.RPC("RPC_SpawnItemInHandMaster", 2, new object[] { objName });
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000DAF0 File Offset: 0x0000BCF0
	[PunRPC]
	private void RPC_SpawnItemInHandMaster(string objName)
	{
		PhotonNetwork.Instantiate("0_Items/" + objName, this.character.Center + Vector3.up * 3f, Quaternion.identity, 0, null).GetComponent<Item>().Interact(this.character);
	}

	// Token: 0x060001CA RID: 458 RVA: 0x0000DB60 File Offset: 0x0000BD60
	[CompilerGenerated]
	private bool <UnAttachItem>g__TryDestroyJoint|38_0(BodypartType bodyPart)
	{
		if (this.character == null)
		{
			return false;
		}
		Rigidbody bodypartRig = this.character.GetBodypartRig(bodyPart);
		if (bodypartRig == null)
		{
			return false;
		}
		FixedJoint fixedJoint;
		if (!bodypartRig.gameObject.TryGetComponent<FixedJoint>(ref fixedJoint))
		{
			return false;
		}
		Object.Destroy(fixedJoint);
		return true;
	}

	// Token: 0x04000195 RID: 405
	public SFX_Instance cookSfx;

	// Token: 0x04000196 RID: 406
	public float holdForce;

	// Token: 0x04000197 RID: 407
	public float holdTorque;

	// Token: 0x04000198 RID: 408
	public float throwChargeTime;

	// Token: 0x04000199 RID: 409
	public float minThrowForce;

	// Token: 0x0400019A RID: 410
	public float maxThrowForce;

	// Token: 0x0400019B RID: 411
	public float delayBeforeThrowCharge;

	// Token: 0x0400019C RID: 412
	[NonSerialized]
	public Optionable<byte> currentSelectedSlot;

	// Token: 0x0400019D RID: 413
	[NonSerialized]
	public Optionable<byte> lastSelectedSlot;

	// Token: 0x0400019E RID: 414
	private Character character;

	// Token: 0x0400019F RID: 415
	private PhotonView photonView;

	// Token: 0x040001A0 RID: 416
	public float lastEquippedSlotTime;

	// Token: 0x040001A1 RID: 417
	public List<PhotonView> droppedItems = new List<PhotonView>();

	// Token: 0x040001A2 RID: 418
	[HideInInspector]
	public float throwChargeLevel;

	// Token: 0x040001A3 RID: 419
	public bool isChargingThrow;

	// Token: 0x040001A4 RID: 420
	private float lastPressedDrop;

	// Token: 0x040001A5 RID: 421
	private bool pressedDrop;

	// Token: 0x040001A6 RID: 422
	public Action onSlotEquipped;

	// Token: 0x040001A7 RID: 423
	public const int MAX_SLOT = 3;

	// Token: 0x040001A8 RID: 424
	private float lastSwitched;

	// Token: 0x040001A9 RID: 425
	private int timesSwitchedRecently;

	// Token: 0x040001AA RID: 426
	private float climbingSpikeTick;

	// Token: 0x040001AB RID: 427
	private bool readyToSpike = true;

	// Token: 0x040001AC RID: 428
	private bool spikingWithPrimary;

	// Token: 0x040001AD RID: 429
	private bool spikingWithSecondary;

	// Token: 0x040001AE RID: 430
	private ItemSlot currentClimbingSpikeItemSlot;

	// Token: 0x040001AF RID: 431
	private ClimbingSpikeComponent currentClimbingSpikeComponent;

	// Token: 0x040001B0 RID: 432
	private GameObject currentClimbingSpikePreview;

	// Token: 0x040001B1 RID: 433
	private RaycastHit climbingSpikeHit;
}
