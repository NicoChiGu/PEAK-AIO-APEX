using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x020000CC RID: 204
public class ItemCooking : ItemComponent
{
	// Token: 0x1700008F RID: 143
	// (get) Token: 0x060007CD RID: 1997 RVA: 0x0002BFC2 File Offset: 0x0002A1C2
	// (set) Token: 0x060007CE RID: 1998 RVA: 0x0002BFCA File Offset: 0x0002A1CA
	public int timesCookedLocal { get; protected set; }

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x060007CF RID: 1999 RVA: 0x0002BFD3 File Offset: 0x0002A1D3
	public bool canBeCooked
	{
		get
		{
			return !this.disableCooking;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x060007D0 RID: 2000 RVA: 0x0002BFE0 File Offset: 0x0002A1E0
	private bool hasExplosion
	{
		get
		{
			AdditionalCookingBehavior[] array = this.additionalCookingBehaviors;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is CookingBehavior_Explode)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0002C00F File Offset: 0x0002A20F
	public override void OnInstanceDataSet()
	{
		this.UpdateCookedBehavior();
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0002C018 File Offset: 0x0002A218
	public virtual void UpdateCookedBehavior()
	{
		IntItemData data = this.item.GetData<IntItemData>(DataEntryKey.CookedAmount);
		if (data.Value == 0)
		{
			data.Value += this.preCooked;
		}
		if (!this.setup)
		{
			this.setup = true;
			Renderer[] array = base.GetComponentsInChildren<MeshRenderer>();
			this.renderers = array;
			array = base.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			Renderer[] array2 = array;
			if (array2.Length != 0)
			{
				this.renderers = Enumerable.ToArray<Renderer>(Enumerable.Concat<Renderer>(this.renderers, array2));
			}
			this.defaultTints = new Color[this.renderers.Length];
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.defaultTints[i] = this.renderers[i].material.GetColor("_Tint");
			}
		}
		int num = data.Value - this.timesCookedLocal;
		this.CookVisually(data.Value);
		if (!this.ignoreDefaultCookBehavior && num > 0)
		{
			for (int j = 1 + this.timesCookedLocal; j <= data.Value; j++)
			{
				this.ChangeStatsCooked(j);
			}
		}
		this.RunAdditionalCookingBehaviors(data.Value);
		this.timesCookedLocal = data.Value;
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0002C144 File Offset: 0x0002A344
	protected void RunAdditionalCookingBehaviors(int cookedAmount)
	{
		AdditionalCookingBehavior[] array = this.additionalCookingBehaviors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Cook(this, cookedAmount);
		}
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0002C170 File Offset: 0x0002A370
	protected virtual void CookVisually(int cookedAmount)
	{
		if (cookedAmount > 0)
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				for (int j = 0; j < this.renderers[i].materials.Length; j++)
				{
					this.renderers[i].materials[j].SetColor("_Tint", this.defaultTints[i] * ItemCooking.GetCookColor(cookedAmount));
				}
			}
		}
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0002C1E0 File Offset: 0x0002A3E0
	public static Color GetCookColor(int cookAmount)
	{
		Color color = Color.white;
		if (cookAmount == 1)
		{
			color = ItemCooking.DefaultCookColorMultiplier;
		}
		else if (cookAmount == 2)
		{
			color = ItemCooking.DefaultCookColorMultiplier * 0.5f;
		}
		else if (cookAmount > 2)
		{
			color = ItemCooking.BurntCookColorMultiplier;
		}
		color.a = 1f;
		return color;
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0002C22C File Offset: 0x0002A42C
	[PunRPC]
	private void FinishCookingRPC()
	{
		this.CancelCookingVisuals();
		IntItemData data = base.GetData<IntItemData>(DataEntryKey.CookedAmount);
		if (this.wreckWhenCooked)
		{
			data.Value = 5;
		}
		else if (data.Value < 12)
		{
			data.Value++;
		}
		this.item.WasActive();
		this.UpdateCookedBehavior();
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0002C281 File Offset: 0x0002A481
	public void StartCookingVisuals()
	{
		this.photonView.RPC("EnableCookingSmokeRPC", 0, new object[] { true });
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0002C2A3 File Offset: 0x0002A4A3
	[PunRPC]
	private void EnableCookingSmokeRPC(bool active)
	{
		this.item.particles.EnableSmoke(active);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0002C2B8 File Offset: 0x0002A4B8
	public void Wreck()
	{
		ItemComponent[] components = base.GetComponents<ItemComponent>();
		for (int i = components.Length - 1; i >= 0; i--)
		{
			if (components[i] != this)
			{
				Object.Destroy(components[i]);
			}
		}
		ItemAction[] components2 = base.GetComponents<ItemAction>();
		for (int j = components2.Length - 1; j >= 0; j--)
		{
			Object.Destroy(components2[j]);
		}
		this.item.overrideUsability = Optionable<bool>.Some(false);
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0002C320 File Offset: 0x0002A520
	private void ChangeStatsCooked(int totalCooked)
	{
		if (this.wreckWhenCooked && totalCooked > 0)
		{
			this.Wreck();
			return;
		}
		Action_RestoreHunger component = base.GetComponent<Action_RestoreHunger>();
		if (component)
		{
			if (totalCooked < 2)
			{
				component.restorationAmount *= 2f;
			}
			else if (totalCooked > 2)
			{
				component.restorationAmount = Mathf.Max(component.restorationAmount - 0.05f, 0f);
			}
		}
		Action_GiveExtraStamina action_GiveExtraStamina = base.GetComponent<Action_GiveExtraStamina>();
		if (!action_GiveExtraStamina)
		{
			action_GiveExtraStamina = base.gameObject.AddComponent<Action_GiveExtraStamina>();
			action_GiveExtraStamina.OnConsumed = true;
		}
		if (totalCooked < 2)
		{
			action_GiveExtraStamina.amount = Mathf.Max(0.1f, action_GiveExtraStamina.amount * 1.5f);
		}
		else if (totalCooked > 2)
		{
			action_GiveExtraStamina.amount = 0f;
		}
		Action_ModifyStatus action_ModifyStatus = Enumerable.FirstOrDefault<Action_ModifyStatus>(base.GetComponents<Action_ModifyStatus>(), (Action_ModifyStatus a) => a.statusType == CharacterAfflictions.STATUSTYPE.Poison);
		if (totalCooked > 3)
		{
			if (!action_ModifyStatus)
			{
				action_ModifyStatus = base.gameObject.AddComponent<Action_ModifyStatus>();
				action_ModifyStatus.OnConsumed = true;
				action_ModifyStatus.statusType = CharacterAfflictions.STATUSTYPE.Poison;
			}
			action_ModifyStatus.changeAmount += 0.1f;
		}
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0002C43D File Offset: 0x0002A63D
	public void CancelCookingVisuals()
	{
		this.photonView.RPC("EnableCookingSmokeRPC", 0, new object[] { false });
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0002C460 File Offset: 0x0002A660
	public void FinishCooking()
	{
		if (!this.photonView.AmController)
		{
			return;
		}
		this.photonView.RPC("FinishCookingRPC", 0, Array.Empty<object>());
		if (this.item.holderCharacter)
		{
			Action<ItemSlot[]> itemsChangedAction = this.item.holderCharacter.player.itemsChangedAction;
			if (itemsChangedAction != null)
			{
				itemsChangedAction.Invoke(this.item.holderCharacter.player.itemSlots);
			}
			CharacterItems items = this.item.holderCharacter.refs.items;
			if (((items != null) ? items.cookSfx : null) != null)
			{
				this.item.holderCharacter.refs.items.cookSfx.Play(base.transform.position);
			}
		}
		Debug.Log("Cooking Finished");
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0002C532 File Offset: 0x0002A732
	public void Explode()
	{
		if (this.photonView.IsMine)
		{
			this.photonView.RPC("RPC_CookingExplode", 0, Array.Empty<object>());
		}
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0002C558 File Offset: 0x0002A758
	[PunRPC]
	private void RPC_CookingExplode()
	{
		if (this.explosionPrefab)
		{
			Object.Instantiate<GameObject>(this.explosionPrefab, base.transform.position, base.transform.rotation);
		}
		if (Character.localCharacter.data.currentItem == this.item)
		{
			Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
			Character.localCharacter.refs.afflictions.UpdateWeight();
		}
		this.item.ClearDataFromBackpack();
		if (this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x040007A2 RID: 1954
	public int preCooked;

	// Token: 0x040007A4 RID: 1956
	[SerializeField]
	protected bool disableCooking;

	// Token: 0x040007A5 RID: 1957
	[FormerlySerializedAs("burnInstantly")]
	public bool wreckWhenCooked;

	// Token: 0x040007A6 RID: 1958
	public bool ignoreDefaultCookBehavior;

	// Token: 0x040007A7 RID: 1959
	[SerializeReference]
	public AdditionalCookingBehavior[] additionalCookingBehaviors = new AdditionalCookingBehavior[0];

	// Token: 0x040007A8 RID: 1960
	private Renderer[] renderers;

	// Token: 0x040007A9 RID: 1961
	private Color[] defaultTints;

	// Token: 0x040007AA RID: 1962
	private bool setup;

	// Token: 0x040007AB RID: 1963
	public static Color DefaultCookColorMultiplier = new Color(0.66f, 0.47f, 0.25f);

	// Token: 0x040007AC RID: 1964
	public static Color BurntCookColorMultiplier = new Color(0.05f, 0.05f, 0.1f);

	// Token: 0x040007AD RID: 1965
	public const int COOKING_MAX = 12;

	// Token: 0x040007AE RID: 1966
	public GameObject explosionPrefab;
}
