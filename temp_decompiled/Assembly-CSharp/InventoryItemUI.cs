using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001C9 RID: 457
public class InventoryItemUI : MonoBehaviour
{
	// Token: 0x06000E0E RID: 3598 RVA: 0x00046011 File Offset: 0x00044211
	public void Start()
	{
		this.startingSizeDelta = this.rectTransform.sizeDelta;
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x00046024 File Offset: 0x00044224
	private void UpdateCookedAmount()
	{
		if (this._itemData == null)
		{
			this.cookedAmount = 0;
			this.icon.color = Color.white;
			return;
		}
		IntItemData intItemData;
		if (this._itemData.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) && this.cookedAmount != intItemData.Value)
		{
			this.icon.color = Color.white;
			this.icon.color = ItemCooking.GetCookColor(intItemData.Value);
			this.cookedAmount = intItemData.Value;
		}
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x000460A4 File Offset: 0x000442A4
	public void SetItem(ItemSlot slot)
	{
		if (slot == null)
		{
			this.Clear();
		}
		if (this.isBackpack)
		{
			if (Character.observedCharacter.data.carriedPlayer)
			{
				this.icon.color = Character.observedCharacter.data.carriedPlayer.refs.customization.PlayerColor;
				this.icon.texture = this.carryingIcon;
				this.backpackFilledSlotsObject.SetActive(false);
				return;
			}
			this.icon.texture = this.backpackIcon;
			if (slot.IsEmpty())
			{
				this._hasBackpack = false;
				this.icon.color = new Color(0f, 0f, 0f, 0.5f);
				this.backpackFilledSlotsObject.SetActive(false);
				this.fill.enabled = false;
				return;
			}
			this._hasBackpack = true;
			this.icon.color = Color.white;
			BackpackData backpackData;
			if (this.backpackFilledSlotsObject != null && slot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
			{
				int num = backpackData.FilledSlotCount();
				this.backpackFilledSlotsObject.SetActive(num > 0);
				this.backpackFilledSlotsAmountText.text = num.ToString();
			}
			return;
		}
		else
		{
			if (this._itemPrefab == slot.prefab)
			{
				this.TrySetFuel(slot.data);
				this.UpdateNameText();
				this.UpdateCookedAmount();
				return;
			}
			this._itemPrefab = slot.prefab;
			this._itemData = slot.data;
			this.UpdateNameText();
			this.UpdateCookedAmount();
			this.SetSelected();
			if (!slot.IsEmpty())
			{
				if (this._itemPrefab == null)
				{
					this.icon.transform.localScale = Vector3.zero;
					TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(this.icon.transform, 1f, 0.5f), 24);
				}
				this.icon.texture = this._itemPrefab.UIData.GetIcon();
				this.icon.enabled = true;
				this.TrySetFuel(slot.data);
				return;
			}
			this.Clear();
			return;
		}
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x000462BC File Offset: 0x000444BC
	public void Clear()
	{
		this.fill.enabled = false;
		this.icon.enabled = false;
		this._itemPrefab = null;
		this._itemData = null;
		this.nameText.enabled = false;
		this.nameText.text = "";
		this.TrySetFuel(null);
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x00046314 File Offset: 0x00044514
	public void TrySetFuel(ItemInstanceData data)
	{
		if (!this.fuelBar)
		{
			return;
		}
		if (Character.observedCharacter != Character.localCharacter)
		{
			this.fuelBar.SetActive(false);
			return;
		}
		if (data == null || this._itemPrefab == null || !data.HasData(DataEntryKey.UseRemainingPercentage) || this._itemPrefab.UIData.hideFuel)
		{
			this.fuelBar.SetActive(false);
			this.fuelBarFill.fillAmount = 1f;
			return;
		}
		this.fuelBar.SetActive(true);
		FloatItemData floatItemData;
		if (data.TryGetDataEntry<FloatItemData>(DataEntryKey.UseRemainingPercentage, out floatItemData))
		{
			this.fuelBarFill.fillAmount = floatItemData.Value;
		}
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x000463C0 File Offset: 0x000445C0
	public void UpdateNameText()
	{
		string text;
		if (this._itemPrefab != null || (this.isBackpack && this._hasBackpack))
		{
			if (this._itemPrefab != null)
			{
				text = this._itemPrefab.GetItemName(this._itemData);
			}
			else
			{
				text = "Backpack";
			}
		}
		else
		{
			text = "";
		}
		if (this.nameText.text != text)
		{
			this.SetSelected();
		}
		this.nameText.text = text;
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x00046440 File Offset: 0x00044640
	public void SetSelected()
	{
		Optionable<byte> currentSelectedSlot = Character.observedCharacter.refs.items.currentSelectedSlot;
		bool flag = currentSelectedSlot.IsSome && (int)currentSelectedSlot.Value == base.transform.GetSiblingIndex();
		if (this.isTemporarySlot)
		{
			flag = true;
		}
		if (this.isBackpack)
		{
			flag = currentSelectedSlot.Value == 3;
		}
		if (this._itemPrefab != null || (this.isBackpack && (this._hasBackpack || Character.observedCharacter.data.carriedPlayer)) || this.isTemporarySlot)
		{
			if (flag)
			{
				TweenExtensions.Kill(this.mySequence, false);
				ShortcutExtensions.DOKill(this.rectTransform, false);
				TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOSizeDelta(this.rectTransform, this.startingSizeDelta * 1.2f, 0.5f, false), 24);
				this.fill.enabled = true;
				this.fill.transform.localScale = Vector3.zero;
				ShortcutExtensions.DOKill(this.fill.transform, false);
				TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(this.fill.transform, 1f, 0.25f), 9);
				this.nameText.enabled = true;
				return;
			}
			TweenExtensions.Kill(this.mySequence, false);
			ShortcutExtensions.DOKill(this.rectTransform, false);
			TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOSizeDelta(this.rectTransform, this.startingSizeDelta, 0.2f, false), 9);
			this.fill.enabled = false;
			this.nameText.enabled = false;
			return;
		}
		else
		{
			if (flag)
			{
				TweenExtensions.Kill(this.mySequence, false);
				this.mySequence = DOTween.Sequence();
				TweenSettingsExtensions.Append(this.mySequence, TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOSizeDelta(this.rectTransform, this.startingSizeDelta * 1.2f, 0.075f, false), 9));
				TweenSettingsExtensions.Append(this.mySequence, TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOSizeDelta(this.rectTransform, this.startingSizeDelta, 0.125f, false), 2));
				return;
			}
			TweenExtensions.Kill(this.mySequence, false);
			ShortcutExtensions.DOKill(this.rectTransform, false);
			this.rectTransform.sizeDelta = this.startingSizeDelta;
			return;
		}
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x0004667C File Offset: 0x0004487C
	private void OnDisable()
	{
		TweenExtensions.Kill(this.mySequence, false);
		ShortcutExtensions.DOKill(this.rectTransform, false);
		this.rectTransform.sizeDelta = this.startingSizeDelta;
		this.fill.enabled = false;
		this.nameText.enabled = false;
		this.nameText.text = "";
	}

	// Token: 0x04000C2F RID: 3119
	public RectTransform rectTransform;

	// Token: 0x04000C30 RID: 3120
	public RawImage icon;

	// Token: 0x04000C31 RID: 3121
	public Image fill;

	// Token: 0x04000C32 RID: 3122
	public Image selectedSlotIcon;

	// Token: 0x04000C33 RID: 3123
	public Texture defaultIcon;

	// Token: 0x04000C34 RID: 3124
	public TextMeshProUGUI nameText;

	// Token: 0x04000C35 RID: 3125
	public bool isBackpack;

	// Token: 0x04000C36 RID: 3126
	public GameObject backpackFilledSlotsObject;

	// Token: 0x04000C37 RID: 3127
	public TextMeshProUGUI backpackFilledSlotsAmountText;

	// Token: 0x04000C38 RID: 3128
	private Sequence mySequence;

	// Token: 0x04000C39 RID: 3129
	private Item _itemPrefab;

	// Token: 0x04000C3A RID: 3130
	private bool _hasBackpack;

	// Token: 0x04000C3B RID: 3131
	public GameObject fuelBar;

	// Token: 0x04000C3C RID: 3132
	public Image fuelBarFill;

	// Token: 0x04000C3D RID: 3133
	public Texture backpackIcon;

	// Token: 0x04000C3E RID: 3134
	public Texture carryingIcon;

	// Token: 0x04000C3F RID: 3135
	public ItemInstanceData _itemData;

	// Token: 0x04000C40 RID: 3136
	private int cookedAmount;

	// Token: 0x04000C41 RID: 3137
	public bool isTemporarySlot;

	// Token: 0x04000C42 RID: 3138
	private Vector2 startingSizeDelta;
}
