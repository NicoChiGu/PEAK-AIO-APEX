using System;
using Peak.Network;
using UnityEngine;
using UnityEngine.UI.Extensions;

// Token: 0x02000123 RID: 291
public class MagicBugle : ItemComponent
{
	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000949 RID: 2377 RVA: 0x0003142D File Offset: 0x0002F62D
	public float currentFuel
	{
		get
		{
			return this.fuel;
		}
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x00031438 File Offset: 0x0002F638
	public override void Awake()
	{
		base.Awake();
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.StartToot));
		Item item2 = this.item;
		item2.OnPrimaryCancelled = (Action)Delegate.Combine(item2.OnPrimaryCancelled, new Action(this.CancelToot));
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x0003149C File Offset: 0x0002F69C
	public void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryHeld = (Action)Delegate.Remove(item.OnPrimaryHeld, new Action(this.StartToot));
		Item item2 = this.item;
		item2.OnPrimaryCancelled = (Action)Delegate.Remove(item2.OnPrimaryCancelled, new Action(this.CancelToot));
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x000314F8 File Offset: 0x0002F6F8
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.Fuel))
		{
			this.fuel = base.GetData<FloatItemData>(DataEntryKey.Fuel).Value;
			this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
			return;
		}
		if (this.photonView.IsMine)
		{
			this.fuel = this.totalTootTime;
			this.item.SetUseRemainingPercentage(1f);
		}
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x00031564 File Offset: 0x0002F764
	private void Update()
	{
		this.UpdateToot();
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x0003156C File Offset: 0x0002F76C
	private void UpdateToot()
	{
		if (this.tooting && this.HasAuthority())
		{
			this.fuel -= Time.deltaTime;
			if (this.fuel <= 0f)
			{
				this.fuel = 0f;
				if (this.photonView.IsMine)
				{
					this.CancelToot();
				}
			}
			else if (this.photonView.IsMine)
			{
				this.tootTick -= Time.deltaTime;
				if (this.tootTick <= 0f)
				{
					this.massAffliction.RunAction();
					this.tootTick = 0.1f;
				}
			}
			base.GetData<FloatItemData>(DataEntryKey.Fuel).Value = this.fuel;
			this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
		}
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x0003163C File Offset: 0x0002F83C
	private void StartToot()
	{
		Debug.Log("Started toot");
		if (this.fuel >= this.initialTootCost)
		{
			this.fuel -= this.initialTootCost;
			this.tooting = true;
			this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
		}
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x00031693 File Offset: 0x0002F893
	private void CancelToot()
	{
		Debug.Log("Cancelled toot");
		this.tooting = false;
	}

	// Token: 0x040008AA RID: 2218
	public float initialTootCost;

	// Token: 0x040008AB RID: 2219
	public float totalTootTime;

	// Token: 0x040008AC RID: 2220
	private bool tooting;

	// Token: 0x040008AD RID: 2221
	[SerializeField]
	[ReadOnly]
	private float fuel;

	// Token: 0x040008AE RID: 2222
	public Action_ApplyMassAffliction massAffliction;

	// Token: 0x040008AF RID: 2223
	private float tootTick;
}
