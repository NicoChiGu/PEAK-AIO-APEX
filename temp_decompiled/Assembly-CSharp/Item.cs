using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEngine;
using WebSocketSharp;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000024 RID: 36
public class Item : MonoBehaviourPunCallbacks, IInteractible
{
	// Token: 0x17000033 RID: 51
	// (get) Token: 0x0600026C RID: 620 RVA: 0x00011FA3 File Offset: 0x000101A3
	// (set) Token: 0x0600026D RID: 621 RVA: 0x00011FAB File Offset: 0x000101AB
	public ItemState itemState { get; set; }

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x0600026E RID: 622 RVA: 0x00011FB4 File Offset: 0x000101B4
	public int CarryWeight
	{
		get
		{
			return this.carryWeight + Ascents.itemWeightModifier;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600026F RID: 623 RVA: 0x00011FC2 File Offset: 0x000101C2
	// (set) Token: 0x06000270 RID: 624 RVA: 0x00011FCA File Offset: 0x000101CA
	public bool isUsingPrimary { get; private set; }

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000271 RID: 625 RVA: 0x00011FD3 File Offset: 0x000101D3
	// (set) Token: 0x06000272 RID: 626 RVA: 0x00011FDB File Offset: 0x000101DB
	public ItemCooking cooking { get; private set; }

	// Token: 0x06000273 RID: 627 RVA: 0x00011FE4 File Offset: 0x000101E4
	public override void OnEnable()
	{
		base.OnEnable();
		if (!this.rig.isKinematic)
		{
			this.WasActive();
		}
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00011FFF File Offset: 0x000101FF
	public override void OnDisable()
	{
		base.OnDisable();
		this.RemoveFromActiveList();
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00012010 File Offset: 0x00010210
	protected virtual void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.cooking = ExtGameObject.GetOrAddComponent<ItemCooking>(base.gameObject);
		this._hasUseFeedback = base.TryGetComponent<ItemUseFeedback>(ref this._useFeedback);
		this.AddPhysics();
		this.GetItemActions();
		this.AddPropertyBlock();
		this.particles = base.GetComponent<ItemParticles>();
		if (!this.particles)
		{
			this.particles = base.gameObject.AddComponent<ItemParticles>();
		}
		this.itemComponents = base.GetComponents<ItemComponent>();
		this.physicsSyncer = base.GetComponent<ItemPhysicsSyncer>();
		Item.ALL_ITEMS.Add(this);
	}

	// Token: 0x06000276 RID: 630 RVA: 0x000120AC File Offset: 0x000102AC
	protected virtual void Start()
	{
		if (!this.HasData(DataEntryKey.ItemUses))
		{
			OptionableIntItemData optionableIntItemData = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
			optionableIntItemData.HasData = this.totalUses != -1;
			optionableIntItemData.Value = this.totalUses;
			if (this.totalUses > 0)
			{
				this.SetUseRemainingPercentage(1f);
			}
		}
		if (!this.rig.isKinematic)
		{
			this.WasActive();
		}
		this.packLayer = 1 << LayerMask.NameToLayer("Exclude Collisions");
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00012124 File Offset: 0x00010324
	public string GetItemName(ItemInstanceData data = null)
	{
		int num = 0;
		IntItemData intItemData;
		if (data == null)
		{
			num = this.GetData<IntItemData>(DataEntryKey.CookedAmount).Value;
		}
		else if (data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
		{
			num = intItemData.Value;
		}
		string text;
		if (num < 4)
		{
			switch (num)
			{
			case 1:
				text = LocalizedText.GetText("COOKED_COOKED", true).Replace("#", this.GetName());
				break;
			case 2:
				text = LocalizedText.GetText("COOKED_WELLDONE", true).Replace("#", this.GetName());
				break;
			case 3:
				text = LocalizedText.GetText("COOKED_BURNT", true).Replace("#", this.GetName());
				break;
			default:
				text = this.GetName();
				break;
			}
		}
		else
		{
			text = LocalizedText.GetText("COOKED_INCINERATED", true).Replace("#", this.GetName());
		}
		return text;
	}

	// Token: 0x06000278 RID: 632 RVA: 0x000121F4 File Offset: 0x000103F4
	private void AddPropertyBlock()
	{
		this.mpb = new MaterialPropertyBlock();
		this.mainRenderer = base.GetComponentInChildren<MeshRenderer>();
		if (!this.mainRenderer)
		{
			this.mainRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
		}
		this.mainRenderer.GetPropertyBlock(this.mpb);
	}

	// Token: 0x06000279 RID: 633 RVA: 0x00012242 File Offset: 0x00010442
	private void GetItemActions()
	{
		this.itemActions = base.GetComponentsInChildren<ItemActionBase>();
	}

	// Token: 0x0600027A RID: 634 RVA: 0x00012250 File Offset: 0x00010450
	protected virtual void AddPhysics()
	{
		this.rig = ExtGameObject.GetOrAddComponent<Rigidbody>(base.gameObject);
		this.rig.mass = this.mass;
		this.centerOfMass = this.rig.centerOfMass;
		this.rig.interpolation = 1;
		this.rig.collisionDetectionMode = 2;
		this.colliders = base.GetComponentsInChildren<Collider>();
	}

	// Token: 0x0600027B RID: 635 RVA: 0x000122B4 File Offset: 0x000104B4
	protected virtual void Update()
	{
		if (this.itemState == ItemState.InBackpack)
		{
			if (this.backpackSlotTransform == null || !ClassExtensionsMethods.UnityObjectExists<Transform>(this.backpackSlotTransform))
			{
				base.transform.position = new Vector3(0f, -500f, 0f);
			}
			else
			{
				base.transform.position = this.backpackSlotTransform.position - this.backpackSlotTransform.rotation * this.centerOfMass * 0.5f;
				base.transform.rotation = this.backpackSlotTransform.rotation;
			}
		}
		else if (this.itemState == ItemState.Ground && base.photonView.IsMine)
		{
			if (base.transform.position.y < -2000f || base.transform.position.y > 4000f)
			{
				this.destroyTick += Time.deltaTime;
				if (this.destroyTick > 2f)
				{
					PhotonNetwork.Destroy(base.gameObject);
				}
			}
			else
			{
				this.destroyTick = 0f;
			}
		}
		else if (this.itemState == ItemState.Held)
		{
			this.WasActive();
		}
		this.UpdateEntryInActiveList();
		this.UpdateCollisionDetectionMode();
	}

	// Token: 0x0600027C RID: 636 RVA: 0x000123F9 File Offset: 0x000105F9
	private void UpdateCollisionDetectionMode()
	{
		if (this.itemState == ItemState.Ground)
		{
			this.rig.collisionDetectionMode = 2;
			return;
		}
		this.rig.collisionDetectionMode = 0;
	}

	// Token: 0x0600027D RID: 637 RVA: 0x0001241C File Offset: 0x0001061C
	public virtual void Interact(Character interactor)
	{
		if (!interactor.player.HasEmptySlot(this.itemID))
		{
			return;
		}
		if (interactor.refs.items.lastEquippedSlotTime + 0.25f > Time.time)
		{
			return;
		}
		if (interactor.data.isClimbing && !this.UIData.canPocket)
		{
			this.SetKinematicNetworked(false);
			return;
		}
		GlobalEvents.TriggerItemRequested(this, interactor);
		base.gameObject.SetActive(false);
		this.view.RPC("RequestPickup", 2, new object[] { interactor.GetComponent<PhotonView>() });
		Debug.Log("Picking up " + base.gameObject.name);
		ItemBackpackVisuals itemBackpackVisuals;
		if (base.TryGetComponent<ItemBackpackVisuals>(ref itemBackpackVisuals))
		{
			itemBackpackVisuals.RemoveVisuals();
		}
	}

	// Token: 0x0600027E RID: 638 RVA: 0x000124DA File Offset: 0x000106DA
	[PunRPC]
	public void DenyPickupRPC()
	{
		base.gameObject.SetActive(true);
		this.SetKinematicNetworked(false, base.transform.position, base.transform.rotation);
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00012508 File Offset: 0x00010708
	[PunRPC]
	public void RequestPickup(PhotonView characterView)
	{
		Character component = characterView.GetComponent<Character>();
		if (this.isSecretlyOtherItemPrefab && !this.isSecretlyOtherItemPrefab.UIData.canPocket && component.data.isClimbing)
		{
			PhotonNetwork.Instantiate("0_Items/" + this.isSecretlyOtherItemPrefab.gameObject.name, base.transform.position, base.transform.rotation, 0, null);
			PhotonNetwork.Destroy(base.gameObject);
			return;
		}
		ushort num = (this.isSecretlyOtherItemPrefab ? this.isSecretlyOtherItemPrefab.itemID : this.itemID);
		ItemSlot itemSlot;
		bool flag = component.player.AddItem(num, this.data, out itemSlot);
		if (this.itemState == ItemState.InBackpack)
		{
			if (this.backpackReference.IsSome)
			{
				if (flag)
				{
					this.ClearDataFromBackpack();
					component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, new object[] { itemSlot.itemSlotID });
					return;
				}
				this.view.RPC("DenyPickupRPC", component.player.photonView.Owner, Array.Empty<object>());
			}
			return;
		}
		if (flag)
		{
			component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, new object[] { itemSlot.itemSlotID });
			PhotonNetwork.Destroy(this.view);
			return;
		}
		this.view.RPC("DenyPickupRPC", component.player.photonView.Owner, Array.Empty<object>());
	}

	// Token: 0x06000280 RID: 640 RVA: 0x000126B0 File Offset: 0x000108B0
	public void ClearDataFromBackpack()
	{
		if (this.backpackReference.IsNone)
		{
			return;
		}
		ValueTuple<byte, BackpackReference> value = this.backpackReference.Value;
		byte item = value.Item1;
		BackpackReference item2 = value.Item2;
		item2.GetData().itemSlots[(int)item].EmptyOut();
		if (item2.type == BackpackReference.BackpackType.Item)
		{
			item2.view.RPC("SetItemInstanceDataRPC", 1, new object[] { item2.GetItemInstanceData() });
		}
		else
		{
			Character component = item2.view.GetComponent<Character>();
			ItemSlot[] itemSlots = component.player.itemSlots;
			BackpackSlot backpackSlot = component.player.backpackSlot;
			byte[] array = IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(itemSlots, backpackSlot, component.player.tempFullSlot));
			component.player.photonView.RPC("SyncInventoryRPC", 1, new object[] { array, false });
		}
		item2.GetVisuals().RefreshVisuals();
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00012798 File Offset: 0x00010998
	public Vector3 Center()
	{
		if (!ClassExtensionsMethods.UnityObjectExists<Renderer>(this.mainRenderer))
		{
			return base.transform.position;
		}
		return this.mainRenderer.bounds.center;
	}

	// Token: 0x06000282 RID: 642 RVA: 0x000127D1 File Offset: 0x000109D1
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000283 RID: 643 RVA: 0x000127D9 File Offset: 0x000109D9
	public virtual string GetInteractionText()
	{
		return LocalizedText.GetText("PICKUP", true);
	}

	// Token: 0x06000284 RID: 644 RVA: 0x000127E6 File Offset: 0x000109E6
	public string GetName()
	{
		return LocalizedText.GetText(LocalizedText.GetNameIndex(this.UIData.itemName), true);
	}

	// Token: 0x06000285 RID: 645 RVA: 0x000127FE File Offset: 0x000109FE
	public virtual bool IsInteractible(Character interactor)
	{
		return !this.blockInteraction && this.itemState != ItemState.Held && this.itemState != ItemState.InBackpack;
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00012824 File Offset: 0x00010A24
	internal void Move(Vector3 position, Quaternion rotation)
	{
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.rig.position = position;
		this.rig.rotation = rotation;
		this.rig.linearVelocity *= 0f;
		this.rig.angularVelocity *= 0f;
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000287 RID: 647 RVA: 0x00012897 File Offset: 0x00010A97
	// (set) Token: 0x06000288 RID: 648 RVA: 0x000128B3 File Offset: 0x00010AB3
	public Character holderCharacter
	{
		get
		{
			if (this.overrideHolderCharacter)
			{
				return this.overrideHolderCharacter;
			}
			return this._holderCharacter;
		}
		set
		{
			if (value != null)
			{
				this.lastHolderCharacter = value;
			}
			this._holderCharacter = value;
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000289 RID: 649 RVA: 0x000128CC File Offset: 0x00010ACC
	public Character trueHolderCharacter
	{
		get
		{
			return this._holderCharacter;
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x000128D4 File Offset: 0x00010AD4
	private void SetColliders(bool enabled, bool isTrigger, bool excludeLayer = false)
	{
		for (int i = 0; i < this.colliders.Length; i++)
		{
			this.colliders[i].enabled = enabled;
			this.colliders[i].isTrigger = isTrigger;
		}
		if (excludeLayer)
		{
			this.rig.excludeLayers = 1 << LayerMask.NameToLayer("Default");
			return;
		}
		this.rig.excludeLayers = 0;
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00012944 File Offset: 0x00010B44
	internal void SetState(ItemState setState, Character character = null)
	{
		Debug.Log(string.Format("Setting Item State for {0}: {1}", base.name, setState));
		this.itemState = setState;
		Action<ItemState> onStateChange = this.OnStateChange;
		if (onStateChange != null)
		{
			onStateChange.Invoke(setState);
		}
		if (setState == ItemState.InBackpack)
		{
			this.holderCharacter = null;
			this.rig.useGravity = false;
			this.rig.isKinematic = true;
			this.rig.interpolation = 0;
			this.SetColliders(false, true, false);
			if (this.forceScale)
			{
				base.transform.localScale = Vector3.one * 0.5f;
				return;
			}
		}
		else if (setState == ItemState.Ground)
		{
			this.holderCharacter = null;
			this.rig.useGravity = true;
			this.rig.isKinematic = false;
			this.rig.interpolation = 1;
			this.centerOfMass = this.rig.centerOfMass;
			if (this is Backpack)
			{
				this.wearerCharacter = null;
			}
			this.SetColliders(true, false, false);
			if (this.forceScale)
			{
				base.transform.localScale = Vector3.one;
				return;
			}
		}
		else if (setState == ItemState.Held)
		{
			this.holderCharacter = character;
			this.rig.useGravity = false;
			this.rig.isKinematic = false;
			this.rig.interpolation = 1;
			if (this is Backpack)
			{
				this.wearerCharacter = null;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				if (character == null)
				{
					Debug.LogError("{name} set to Held but no HolderCharacter assigned! Something broken??");
				}
				else
				{
					base.photonView.TransferOwnership(character.GetComponent<PhotonView>().Owner);
				}
			}
			this.SetColliders(true, false, true);
			if (this.forceScale)
			{
				base.transform.localScale = Vector3.one;
			}
		}
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00012AEA File Offset: 0x00010CEA
	private void HideRenderers()
	{
		LinqExtensions.ForEach<Renderer>(base.GetComponentsInChildren<Renderer>(), delegate(Renderer meshRenderer)
		{
			meshRenderer.enabled = false;
		});
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x0600028D RID: 653 RVA: 0x00012B17 File Offset: 0x00010D17
	// (set) Token: 0x0600028E RID: 654 RVA: 0x00012B1F File Offset: 0x00010D1F
	public bool isUsingSecondary { get; private set; }

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600028F RID: 655 RVA: 0x00012B28 File Offset: 0x00010D28
	// (set) Token: 0x06000290 RID: 656 RVA: 0x00012B30 File Offset: 0x00010D30
	public float castProgress { get; private set; }

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000291 RID: 657 RVA: 0x00012B39 File Offset: 0x00010D39
	public float progress
	{
		get
		{
			return Mathf.Max(this.overrideProgress, this.castProgress);
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x06000292 RID: 658 RVA: 0x00012B4C File Offset: 0x00010D4C
	public bool shouldShowCastProgress
	{
		get
		{
			return (this.showUseProgress && this.castProgress > 0f && !this.finishedCast) || this.overrideForceProgress;
		}
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00012B74 File Offset: 0x00010D74
	public virtual bool CanUsePrimary()
	{
		if (!this.overrideUsability.IsNone)
		{
			return this.overrideUsability.Value;
		}
		OptionableIntItemData optionableIntItemData = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		return !optionableIntItemData.HasData || optionableIntItemData.Value == -1 || optionableIntItemData.Value > 0;
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00012BC0 File Offset: 0x00010DC0
	public virtual bool CanUseSecondary()
	{
		bool flag = true;
		OptionableIntItemData optionableIntItemData = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (optionableIntItemData.HasData)
		{
			flag = optionableIntItemData.Value == -1 || optionableIntItemData.Value > 0;
		}
		if (!flag)
		{
			return false;
		}
		if (this.canUseOnFriend)
		{
			if (Interaction.instance.hasValidTargetCharacter)
			{
				return true;
			}
		}
		else if (this.UIData.hasSecondInteract)
		{
			return true;
		}
		return false;
	}

	// Token: 0x06000295 RID: 661 RVA: 0x00012C20 File Offset: 0x00010E20
	public void StartUsePrimary()
	{
		if (this.isUsingSecondary)
		{
			this.CancelUseSecondary();
		}
		this.isUsingPrimary = true;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.OnPrimaryStarted != null)
		{
			this.OnPrimaryStarted.Invoke();
		}
	}

	// Token: 0x06000296 RID: 662 RVA: 0x00012C5C File Offset: 0x00010E5C
	public void ContinueUsePrimary()
	{
		if (this.isUsingSecondary)
		{
			this.CancelUseSecondary();
		}
		if (this.isUsingPrimary)
		{
			if (this.usingTimePrimary > 0f)
			{
				this.castProgress += 1f / this.usingTimePrimary * Time.deltaTime;
				if (this.castProgress >= 1f)
				{
					if (this.OnPrimaryHeld != null)
					{
						this.OnPrimaryHeld.Invoke();
					}
					if (!this.finishedCast)
					{
						this.FinishCastPrimary();
						return;
					}
				}
			}
			else
			{
				if (!this.finishedCast)
				{
					this.FinishCastPrimary();
				}
				if (this.OnPrimaryHeld != null)
				{
					this.OnPrimaryHeld.Invoke();
				}
			}
		}
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00012CFC File Offset: 0x00010EFC
	protected virtual void FinishCastPrimary()
	{
		if (this._hasUseFeedback)
		{
			this.holderCharacter.refs.animator.SetBool(this._useFeedback.useAnimation, false);
			SFX_Instance sfxUsed = this._useFeedback.sfxUsed;
			if (sfxUsed != null)
			{
				sfxUsed.Play(base.transform.position, this._holderCharacter.refs.carryPosRef);
			}
		}
		this.finishedCast = true;
		this.lastFinishedCast = Time.time;
		this.castProgress = 0f;
		if (this.OnPrimaryFinishedCast != null)
		{
			this.OnPrimaryFinishedCast.Invoke();
		}
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00012D94 File Offset: 0x00010F94
	public void CancelUsePrimary()
	{
		this.isUsingPrimary = false;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.OnPrimaryCancelled != null)
		{
			this.OnPrimaryCancelled.Invoke();
		}
		if (Player.localPlayer == null)
		{
			Debug.LogError("Player.localPlayer is null, cannot play movement animation");
			return;
		}
		if (Player.localPlayer.character == null)
		{
			Debug.LogError("Player.localPlayer.character is null, cannot play movement animation");
			return;
		}
		if (Player.localPlayer.character.refs == null)
		{
			Debug.LogError("Player.localPlayer.character.refs is null, cannot play movement animation");
			return;
		}
		if (Player.localPlayer.character.refs.animations == null)
		{
			Debug.LogError("Player.localPlayer.character.refs.animations is null, cannot play movement animation");
			return;
		}
		Player.localPlayer.character.refs.animations.PlaySpecificAnimation("Movement");
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00012E63 File Offset: 0x00011063
	public void ScrollButtonBackwardPressed()
	{
		if (this.OnScrollBackwardPressed != null)
		{
			this.OnScrollBackwardPressed.Invoke();
		}
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00012E78 File Offset: 0x00011078
	public void ScrollButtonForwardPressed()
	{
		if (this.OnScrollForwardPressed != null)
		{
			this.OnScrollForwardPressed.Invoke();
		}
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00012E8D File Offset: 0x0001108D
	public void ScrollButtonBackwardHeld()
	{
		if (this.OnScrollBackwardHeld != null)
		{
			this.OnScrollBackwardHeld.Invoke();
		}
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00012EA2 File Offset: 0x000110A2
	public void ScrollButtonForwardHeld()
	{
		if (this.OnScrollForwardHeld != null)
		{
			this.OnScrollForwardHeld.Invoke();
		}
	}

	// Token: 0x0600029D RID: 669 RVA: 0x00012EB7 File Offset: 0x000110B7
	public void Scroll(float value)
	{
		if (this.OnScrolled != null)
		{
			this.OnScrolled.Invoke(value);
		}
		if (InputHandler.GetCurrentUsedInputScheme() == null && this.OnScrolledMouseOnly != null)
		{
			this.OnScrolledMouseOnly.Invoke(value);
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x00012EE8 File Offset: 0x000110E8
	public void StartUseSecondary()
	{
		if (this.isUsingPrimary)
		{
			return;
		}
		if (this.isUsingSecondary)
		{
			return;
		}
		this.isUsingSecondary = true;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.holderCharacter && this.canUseOnFriend && Interaction.instance.hasValidTargetCharacter)
		{
			base.photonView.RPC("SendFeedDataRPC", 0, new object[]
			{
				this.holderCharacter.photonView.ViewID,
				Interaction.instance.bestCharacter.character.photonView.ViewID,
				(int)this.itemID,
				this.totalSecondaryUsingTime
			});
		}
		if (this.OnSecondaryStarted != null)
		{
			this.OnSecondaryStarted.Invoke();
		}
	}

	// Token: 0x0600029F RID: 671 RVA: 0x00012FC3 File Offset: 0x000111C3
	[PunRPC]
	internal void SendFeedDataRPC(int giverID, int recieverID, int itemID, float totalUsingTime)
	{
		GameUtils.instance.StartFeed(giverID, recieverID, (ushort)itemID, totalUsingTime);
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x00012FD5 File Offset: 0x000111D5
	[PunRPC]
	internal void RemoveFeedDataRPC(int giverID)
	{
		GameUtils.instance.EndFeed(giverID);
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060002A1 RID: 673 RVA: 0x00012FE2 File Offset: 0x000111E2
	public float totalSecondaryUsingTime
	{
		get
		{
			if (!this.canUseOnFriend)
			{
				return this.usingTimePrimary;
			}
			return this.usingTimePrimary * 0.7f;
		}
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x00013000 File Offset: 0x00011200
	public void ContinueUseSecondary()
	{
		if (this.isUsingPrimary)
		{
			return;
		}
		if (this.isUsingSecondary)
		{
			if (this.usingTimePrimary > 0f)
			{
				this.castProgress += 1f / this.totalSecondaryUsingTime * Time.deltaTime;
				if (this.castProgress >= 1f)
				{
					if (this.OnSecondaryHeld != null)
					{
						this.OnSecondaryHeld.Invoke();
					}
					if (!this.finishedCast)
					{
						this.FinishCastSecondary();
						return;
					}
				}
			}
			else if (this.OnSecondaryHeld != null)
			{
				this.OnSecondaryHeld.Invoke();
			}
		}
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x0001308C File Offset: 0x0001128C
	public void FinishCastSecondary()
	{
		this.finishedCast = true;
		this.lastFinishedCast = Time.time;
		this.castProgress = 0f;
		if (this.canUseOnFriend && Interaction.instance.hasValidTargetCharacter)
		{
			if (this.holderCharacter)
			{
				this.holderCharacter.data.lastConsumedItem = Time.time;
				base.photonView.RPC("RemoveFeedDataRPC", 0, new object[] { this.holderCharacter.photonView.ViewID });
			}
			Interaction.instance.bestCharacter.character.FeedItem(this);
			base.photonView.RPC("RemoveFeedDataRPC", 0, new object[] { (int)this.itemID });
			return;
		}
		if (this.OnSecondaryFinishedCast != null)
		{
			this.OnSecondaryFinishedCast.Invoke();
		}
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x00013170 File Offset: 0x00011370
	public void CancelUseSecondary()
	{
		this.isUsingSecondary = false;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.OnSecondaryCancelled != null)
		{
			this.OnSecondaryCancelled.Invoke();
		}
		Player.localPlayer.character.refs.animations.PlaySpecificAnimation("Movement");
		if (this.lastHolderCharacter)
		{
			base.photonView.RPC("RemoveFeedDataRPC", 0, new object[] { this.lastHolderCharacter.photonView.ViewID });
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060002A5 RID: 677 RVA: 0x00013203 File Offset: 0x00011403
	// (set) Token: 0x060002A6 RID: 678 RVA: 0x0001320B File Offset: 0x0001140B
	public bool consuming { get; private set; }

	// Token: 0x060002A7 RID: 679 RVA: 0x00013214 File Offset: 0x00011414
	public IEnumerator ConsumeDelayed(bool ignoreActions = false)
	{
		int consumerID = -1;
		if (this.holderCharacter)
		{
			consumerID = this.holderCharacter.photonView.ViewID;
		}
		this.consuming = true;
		if (!ignoreActions && this.OnConsumed != null)
		{
			this.OnConsumed.Invoke();
		}
		yield return null;
		base.photonView.RPC("Consume", 0, new object[] { consumerID });
		yield break;
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x0001322C File Offset: 0x0001142C
	[PunRPC]
	public void Consume(int consumerID)
	{
		if (this.holderCharacter)
		{
			PhotonView photonView = PhotonNetwork.GetPhotonView(consumerID);
			if (photonView)
			{
				Character component = photonView.GetComponent<Character>();
				if (component)
				{
					GlobalEvents.TriggerItemConsumed(this, component);
				}
			}
			this.holderCharacter.data.lastConsumedItem = Time.time;
			if (this.holderCharacter.data.currentItem == this)
			{
				Optionable<byte> currentSelectedSlot = this.holderCharacter.refs.items.currentSelectedSlot;
				this.holderCharacter.refs.animator.SetBool("Consumed Item", true);
				if (this.holderCharacter.IsLocal)
				{
					if (currentSelectedSlot.IsSome)
					{
						this.holderCharacter.player.EmptySlot(currentSelectedSlot);
						this.holderCharacter.refs.items.EquipSlot(currentSelectedSlot);
					}
					else
					{
						Debug.LogError("No Item Selected locally but still consuming?? THIS IS BAD. CALL ZORRO");
					}
				}
			}
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00013321 File Offset: 0x00011521
	public virtual void OnStash()
	{
		Action action = this.onStashAction;
		if (action != null)
		{
			action.Invoke();
		}
		this.CancelUsePrimary();
		this.CancelUseSecondary();
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00013340 File Offset: 0x00011540
	[ContextMenu("Add Default Food Scripts")]
	public void AddDefaultFoodScripts()
	{
		this.usingTimePrimary = 1.2f;
		Action_PlayAnimation action_PlayAnimation = base.gameObject.AddComponent<Action_PlayAnimation>();
		action_PlayAnimation.OnPressed = true;
		action_PlayAnimation.animationName = "PlayerEat";
		Action_ModifyStatus action_ModifyStatus = base.gameObject.AddComponent<Action_ModifyStatus>();
		action_ModifyStatus.OnCastFinished = true;
		action_ModifyStatus.statusType = CharacterAfflictions.STATUSTYPE.Hunger;
		action_ModifyStatus.changeAmount = -0.1f;
		base.gameObject.AddComponent<Action_Consume>().OnCastFinished = true;
	}

	// Token: 0x060002AB RID: 683 RVA: 0x000133A8 File Offset: 0x000115A8
	public void HoverEnter()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
		this.mainRenderer.SetPropertyBlock(this.mpb);
		for (int i = 0; i < this.addtlRenderers.Length; i++)
		{
			this.addtlRenderers[i].SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00013404 File Offset: 0x00011604
	public void HoverExit()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
		this.mainRenderer.SetPropertyBlock(this.mpb);
		for (int i = 0; i < this.addtlRenderers.Length; i++)
		{
			this.addtlRenderers[i].SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x060002AD RID: 685 RVA: 0x00013460 File Offset: 0x00011660
	public void SetKinematicNetworked(bool value)
	{
		base.photonView.RPC("SetKinematicRPC", 3, new object[]
		{
			value,
			base.transform.position,
			base.transform.rotation
		});
	}

	// Token: 0x060002AE RID: 686 RVA: 0x000134B3 File Offset: 0x000116B3
	public void SetKinematicNetworked(bool value, Vector3 position, Quaternion rotation)
	{
		base.photonView.RPC("SetKinematicRPC", 3, new object[] { value, position, rotation });
	}

	// Token: 0x060002AF RID: 687 RVA: 0x000134E8 File Offset: 0x000116E8
	[PunRPC]
	public void SetKinematicAndResetSyncData(bool value, Vector3 position, Quaternion rotation)
	{
		this.rig.isKinematic = value;
		this.rig.position = position;
		this.rig.rotation = rotation;
		if (value)
		{
			this.rig.linearVelocity = Vector3.zero;
			this.rig.angularVelocity = Vector3.zero;
			this.physicsSyncer.ResetRecievedData();
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00013547 File Offset: 0x00011747
	[PunRPC]
	public void SetKinematicRPC(bool value, Vector3 position, Quaternion rotation)
	{
		this.rig.isKinematic = value;
		this.rig.position = position;
		this.rig.rotation = rotation;
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0001356D File Offset: 0x0001176D
	public bool HasData(DataEntryKey key)
	{
		return this.data != null && this.data.HasData(key);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00013588 File Offset: 0x00011788
	public T GetData<T>(DataEntryKey key, Func<T> createDefault) where T : DataEntryValue, new()
	{
		if (this.data == null)
		{
			this.data = new ItemInstanceData(Guid.NewGuid());
			ItemInstanceDataHandler.AddInstanceData(this.data);
		}
		T t;
		if (this.data.TryGetDataEntry<T>(key, out t))
		{
			return t;
		}
		if (createDefault != null)
		{
			return this.data.RegisterEntry<T>(key, createDefault.Invoke());
		}
		return this.data.RegisterNewEntry<T>(key);
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x000135EC File Offset: 0x000117EC
	public T GetData<T>(DataEntryKey key) where T : DataEntryValue, new()
	{
		return this.GetData<T>(key, null);
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x000135F6 File Offset: 0x000117F6
	internal void ForceSyncForFrames(int frames = 10)
	{
		if (this.physicsSyncer != null)
		{
			this.physicsSyncer.ForceSyncForFrames(frames);
		}
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00013614 File Offset: 0x00011814
	[PunRPC]
	public void SetItemInstanceDataRPC(ItemInstanceData instanceData)
	{
		this.data = instanceData;
		if (this.data != null)
		{
			this.OnInstanceDataRecieved();
			ItemComponent[] array = this.itemComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnInstanceDataSet();
			}
		}
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00013653 File Offset: 0x00011853
	public virtual void OnInstanceDataRecieved()
	{
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00013658 File Offset: 0x00011858
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.ForceSyncForFrames(10);
		ItemState itemState = this.itemState;
		if ((itemState == ItemState.Ground || itemState == ItemState.Held || itemState == ItemState.InBackpack) && this.data != null)
		{
			this.view.RPC("SetItemInstanceDataRPC", newPlayer, new object[] { this.data });
		}
		if (this.itemState == ItemState.InBackpack)
		{
			ValueTuple<byte, BackpackReference> value = this.backpackReference.Value;
			byte item = value.Item1;
			BackpackReference item2 = value.Item2;
			this.view.RPC("PutInBackpackRPC", newPlayer, new object[] { item, item2 });
		}
		if (this.rig.isKinematic)
		{
			this.view.RPC("SetKinematicRPC", newPlayer, new object[]
			{
				this.rig.isKinematic,
				this.rig.position,
				this.rig.rotation
			});
		}
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x00013760 File Offset: 0x00011960
	[PunRPC]
	public void PutInBackpackRPC(byte slotID, BackpackReference backpackReference)
	{
		Transform[] backpackSlots = backpackReference.GetVisuals().backpackSlots;
		this.backpackReference = Optionable<ValueTuple<byte, BackpackReference>>.Some(new ValueTuple<byte, BackpackReference>(slotID, backpackReference));
		this.backpackSlotTransform = backpackSlots[(int)slotID];
		this.SetState(ItemState.InBackpack, null);
		backpackReference.GetVisuals().SetSpawnedBackpackItem(slotID, this);
		if (backpackReference.IsOnMyBack())
		{
			this.HideRenderers();
		}
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x000137BA File Offset: 0x000119BA
	[PunRPC]
	public void SetCookedAmountRPC(int amount)
	{
		this.GetData<IntItemData>(DataEntryKey.CookedAmount).Value = amount;
		this.cooking.UpdateCookedBehavior();
	}

	// Token: 0x060002BA RID: 698 RVA: 0x000137D4 File Offset: 0x000119D4
	public void SetUseRemainingPercentage(float percentage)
	{
		this.GetData<FloatItemData>(DataEntryKey.UseRemainingPercentage).Value = Mathf.Clamp01(percentage);
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060002BB RID: 699 RVA: 0x000137E9 File Offset: 0x000119E9
	// (set) Token: 0x060002BC RID: 700 RVA: 0x000137F1 File Offset: 0x000119F1
	public bool inActiveList { get; private set; }

	// Token: 0x060002BD RID: 701 RVA: 0x000137FA File Offset: 0x000119FA
	public void WasActive()
	{
		if (!this.inActiveList)
		{
			Item.ALL_ACTIVE_ITEMS.Add(this);
		}
		this.inActiveList = true;
		this.timeSinceWasActive = 0f;
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00013821 File Offset: 0x00011A21
	private void UpdateEntryInActiveList()
	{
		if (this.inActiveList)
		{
			this.timeSinceWasActive += Time.deltaTime;
			if (this.timeSinceWasActive > 30f)
			{
				this.RemoveFromActiveList();
			}
		}
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00013850 File Offset: 0x00011A50
	private void RemoveFromActiveList()
	{
		if (this.inActiveList)
		{
			Item.ALL_ACTIVE_ITEMS.Remove(this);
			this.inActiveList = false;
		}
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0001386D File Offset: 0x00011A6D
	private void OnDestroy()
	{
		this.RemoveFromActiveList();
		Item.ALL_ITEMS.Remove(this);
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00013881 File Offset: 0x00011A81
	public bool TryGetFeeder(out Character feeder)
	{
		if (this.trueHolderCharacter != null && this.trueHolderCharacter != this.holderCharacter)
		{
			feeder = this.trueHolderCharacter;
			return true;
		}
		feeder = null;
		return false;
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x000138B4 File Offset: 0x00011AB4
	public bool IsValidToSpawn()
	{
		LootData component = base.GetComponent<LootData>();
		return !component || component.IsValidToSpawn();
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x000138D8 File Offset: 0x00011AD8
	public void AddNameToCSV()
	{
		LocalizedText.AppendCSVLine(string.Concat(new string[]
		{
			"NAME_",
			this.UIData.itemName.ToUpperInvariant(),
			",",
			this.UIData.itemName.ToUpperInvariant(),
			",,,,,,,,,,,,,ENDLINE"
		}), "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00013940 File Offset: 0x00011B40
	public List<string> AddPromptToCSV(List<string> totalStrings)
	{
		List<string> list = new List<string>();
		if (!Ext.IsNullOrEmpty(this.UIData.mainInteractPrompt) && !totalStrings.Contains(this.UIData.mainInteractPrompt.ToUpperInvariant()))
		{
			LocalizedText.AppendCSVLine(this.UIData.mainInteractPrompt.ToUpperInvariant() + "," + this.UIData.mainInteractPrompt.ToLowerInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
			list.Add(this.UIData.mainInteractPrompt.ToUpperInvariant());
		}
		if (!Ext.IsNullOrEmpty(this.UIData.scrollInteractPrompt) && !totalStrings.Contains(this.UIData.scrollInteractPrompt.ToUpperInvariant()))
		{
			LocalizedText.AppendCSVLine(this.UIData.scrollInteractPrompt.ToUpperInvariant() + "," + this.UIData.scrollInteractPrompt.ToLowerInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
			list.Add(this.UIData.scrollInteractPrompt.ToUpperInvariant());
		}
		if (!Ext.IsNullOrEmpty(this.UIData.secondaryInteractPrompt) && !totalStrings.Contains(this.UIData.secondaryInteractPrompt.ToUpperInvariant()))
		{
			LocalizedText.AppendCSVLine(this.UIData.secondaryInteractPrompt.ToUpperInvariant() + "," + this.UIData.secondaryInteractPrompt.ToLowerInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
			list.Add(this.UIData.secondaryInteractPrompt.ToUpperInvariant());
		}
		return list;
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00013AD0 File Offset: 0x00011CD0
	[PunRPC]
	public void RPC_SetThrownData(int characterID, float thrownAmount)
	{
		PhotonView photonView = PhotonNetwork.GetPhotonView(characterID);
		if (photonView)
		{
			photonView.TryGetComponent<Character>(ref this.lastThrownCharacter);
		}
		this.lastThrownAmount = thrownAmount;
		this.lastThrownTime = Time.time;
		GlobalEvents.TriggerItemThrown(this);
	}

	// Token: 0x0400024C RID: 588
	private bool _hasUseFeedback;

	// Token: 0x0400024D RID: 589
	private ItemUseFeedback _useFeedback;

	// Token: 0x0400024E RID: 590
	public static readonly int PROPERTY_INTERACTABLE = Shader.PropertyToID("_Interactable");

	// Token: 0x0400024F RID: 591
	public static List<Item> ALL_ITEMS = new List<Item>();

	// Token: 0x04000250 RID: 592
	public static List<Item> ALL_ACTIVE_ITEMS = new List<Item>();

	// Token: 0x04000251 RID: 593
	public Vector3 defaultPos;

	// Token: 0x04000252 RID: 594
	public Vector3 defaultForward = new Vector3(0f, 0f, 1f);

	// Token: 0x04000253 RID: 595
	public float mass = 5f;

	// Token: 0x04000254 RID: 596
	public float throwForceMultiplier = 1f;

	// Token: 0x04000256 RID: 598
	[SerializeField]
	private int carryWeight = 1;

	// Token: 0x04000257 RID: 599
	internal float lastThrownAmount;

	// Token: 0x04000258 RID: 600
	internal Character lastThrownCharacter;

	// Token: 0x04000259 RID: 601
	internal float lastThrownTime;

	// Token: 0x0400025B RID: 603
	public float usingTimePrimary;

	// Token: 0x0400025C RID: 604
	public bool showUseProgress = true;

	// Token: 0x0400025D RID: 605
	public Item isSecretlyOtherItemPrefab;

	// Token: 0x0400025E RID: 606
	public Action OnPrimaryStarted;

	// Token: 0x0400025F RID: 607
	public Action OnPrimaryHeld;

	// Token: 0x04000260 RID: 608
	public Action OnPrimaryFinishedCast;

	// Token: 0x04000261 RID: 609
	public Action OnPrimaryReleased;

	// Token: 0x04000262 RID: 610
	public Action OnPrimaryCancelled;

	// Token: 0x04000263 RID: 611
	public Action OnConsumed;

	// Token: 0x04000264 RID: 612
	public Action OnSecondaryStarted;

	// Token: 0x04000265 RID: 613
	public Action OnSecondaryHeld;

	// Token: 0x04000266 RID: 614
	public Action OnSecondaryFinishedCast;

	// Token: 0x04000267 RID: 615
	public Action OnSecondaryCancelled;

	// Token: 0x04000268 RID: 616
	public Action<ItemState> OnStateChange;

	// Token: 0x04000269 RID: 617
	public Action<float> OnScrolled;

	// Token: 0x0400026A RID: 618
	public Action<float> OnScrolledMouseOnly;

	// Token: 0x0400026B RID: 619
	public Action OnScrollBackwardPressed;

	// Token: 0x0400026C RID: 620
	public Action OnScrollForwardPressed;

	// Token: 0x0400026D RID: 621
	public Action OnScrollBackwardHeld;

	// Token: 0x0400026E RID: 622
	public Action OnScrollForwardHeld;

	// Token: 0x0400026F RID: 623
	public Item.ItemUIData UIData;

	// Token: 0x04000270 RID: 624
	[NonSerialized]
	public Transform backpackSlotTransform;

	// Token: 0x04000271 RID: 625
	public Optionable<ValueTuple<byte, BackpackReference>> backpackReference;

	// Token: 0x04000272 RID: 626
	private Optionable<RigidbodySyncData> m_lastState = Optionable<RigidbodySyncData>.None;

	// Token: 0x04000273 RID: 627
	protected PhotonView view;

	// Token: 0x04000274 RID: 628
	public int totalUses = -1;

	// Token: 0x04000275 RID: 629
	public ItemInstanceData data;

	// Token: 0x04000276 RID: 630
	public Item.ItemTags itemTags;

	// Token: 0x04000277 RID: 631
	public Rigidbody rig;

	// Token: 0x04000278 RID: 632
	internal ItemActionBase[] itemActions;

	// Token: 0x04000279 RID: 633
	[HideInInspector]
	public Collider[] colliders;

	// Token: 0x0400027A RID: 634
	public ushort itemID;

	// Token: 0x0400027B RID: 635
	private MaterialPropertyBlock mpb;

	// Token: 0x0400027C RID: 636
	public Renderer mainRenderer;

	// Token: 0x0400027D RID: 637
	public Renderer[] addtlRenderers;

	// Token: 0x0400027E RID: 638
	private double timeSinceTick;

	// Token: 0x04000280 RID: 640
	private ItemComponent[] itemComponents;

	// Token: 0x04000281 RID: 641
	protected Color originalTint;

	// Token: 0x04000282 RID: 642
	private ItemPhysicsSyncer physicsSyncer;

	// Token: 0x04000283 RID: 643
	[HideInInspector]
	public ItemParticles particles;

	// Token: 0x04000284 RID: 644
	private int packLayer;

	// Token: 0x04000285 RID: 645
	public bool offsetLuggageSpawn;

	// Token: 0x04000286 RID: 646
	public Vector3 offsetLuggagePosition;

	// Token: 0x04000287 RID: 647
	public Vector3 offsetLuggageRotation;

	// Token: 0x04000288 RID: 648
	private float destroyTick;

	// Token: 0x04000289 RID: 649
	[HideInInspector]
	public bool forceScale = true;

	// Token: 0x0400028A RID: 650
	[HideInInspector]
	public bool blockInteraction;

	// Token: 0x0400028B RID: 651
	public Vector3 centerOfMass;

	// Token: 0x0400028C RID: 652
	private Character lastHolderCharacter;

	// Token: 0x0400028D RID: 653
	[ReadOnly]
	public Character wearerCharacter;

	// Token: 0x0400028E RID: 654
	[SerializeField]
	[ReadOnly]
	private Character _holderCharacter;

	// Token: 0x0400028F RID: 655
	[ReadOnly]
	public Character overrideHolderCharacter;

	// Token: 0x04000291 RID: 657
	public bool canUseOnFriend;

	// Token: 0x04000293 RID: 659
	[HideInInspector]
	public bool finishedCast;

	// Token: 0x04000294 RID: 660
	[HideInInspector]
	public float lastFinishedCast;

	// Token: 0x04000295 RID: 661
	internal float overrideProgress;

	// Token: 0x04000296 RID: 662
	internal Optionable<bool> overrideUsability;

	// Token: 0x04000298 RID: 664
	public Action onStashAction;

	// Token: 0x04000299 RID: 665
	internal bool overrideForceProgress;

	// Token: 0x0400029B RID: 667
	private float timeSinceWasActive;

	// Token: 0x02000403 RID: 1027
	[Flags]
	public enum ItemTags
	{
		// Token: 0x0400174D RID: 5965
		None = 0,
		// Token: 0x0400174E RID: 5966
		Mystical = 1,
		// Token: 0x0400174F RID: 5967
		PackagedFood = 2,
		// Token: 0x04001750 RID: 5968
		Berry = 4,
		// Token: 0x04001751 RID: 5969
		Mushroom = 8,
		// Token: 0x04001752 RID: 5970
		BingBong = 16,
		// Token: 0x04001753 RID: 5971
		GourmandRequirement = 32,
		// Token: 0x04001754 RID: 5972
		GoldenIdol = 64,
		// Token: 0x04001755 RID: 5973
		Bird = 128,
		// Token: 0x04001756 RID: 5974
		BookOfBones = 256
	}

	// Token: 0x02000404 RID: 1028
	[Serializable]
	public class ItemUIData
	{
		// Token: 0x06001A0D RID: 6669 RVA: 0x0007F280 File Offset: 0x0007D480
		public Texture2D GetIcon()
		{
			if (this.hasAltIcon && GameHandler.Instance.SettingsHandler.GetSetting<BugPhobiaSetting>().Value == 1)
			{
				return this.altIcon;
			}
			if (this.hasColorBlindIcon && GUIManager.instance.colorblindness)
			{
				return this.altIcon;
			}
			return this.icon;
		}

		// Token: 0x04001757 RID: 5975
		public string itemName;

		// Token: 0x04001758 RID: 5976
		public Texture2D icon;

		// Token: 0x04001759 RID: 5977
		public bool hasAltIcon;

		// Token: 0x0400175A RID: 5978
		public bool hasColorBlindIcon;

		// Token: 0x0400175B RID: 5979
		public Texture2D altIcon;

		// Token: 0x0400175C RID: 5980
		public bool hasMainInteract = true;

		// Token: 0x0400175D RID: 5981
		public string mainInteractPrompt;

		// Token: 0x0400175E RID: 5982
		public bool hasSecondInteract;

		// Token: 0x0400175F RID: 5983
		public string secondaryInteractPrompt;

		// Token: 0x04001760 RID: 5984
		public bool hasScrollingInteract;

		// Token: 0x04001761 RID: 5985
		public string scrollInteractPrompt;

		// Token: 0x04001762 RID: 5986
		public bool canDrop = true;

		// Token: 0x04001763 RID: 5987
		public bool canPocket = true;

		// Token: 0x04001764 RID: 5988
		public bool canBackpack = true;

		// Token: 0x04001765 RID: 5989
		public bool canThrow = true;

		// Token: 0x04001766 RID: 5990
		public bool isShootable;

		// Token: 0x04001767 RID: 5991
		public bool hideFuel;

		// Token: 0x04001768 RID: 5992
		public Vector3 iconPositionOffset;

		// Token: 0x04001769 RID: 5993
		public Vector3 iconRotationOffset;

		// Token: 0x0400176A RID: 5994
		public float iconScaleOffset = 1f;
	}
}
