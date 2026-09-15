using System;
using DG.Tweening;
using Peak.Network;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class Lantern : ItemComponent
{
	// Token: 0x060002D3 RID: 723 RVA: 0x00013C82 File Offset: 0x00011E82
	public override void Awake()
	{
		base.Awake();
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00013C96 File Offset: 0x00011E96
	public override void OnEnable()
	{
		Item item = this.item;
		item.onStashAction = (Action)Delegate.Combine(item.onStashAction, new Action(this.SnuffLantern));
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00013CBF File Offset: 0x00011EBF
	public override void OnDisable()
	{
		Item item = this.item;
		item.onStashAction = (Action)Delegate.Remove(item.onStashAction, new Action(this.SnuffLantern));
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00013CE8 File Offset: 0x00011EE8
	private void Start()
	{
		if (base.HasData(DataEntryKey.FlareActive) && base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
		{
			this.fireParticle.main.prewarm = true;
			this.fireParticle.Play();
		}
		if (this.activeByDefault && this.item.itemState == ItemState.Held)
		{
			this.lanternLight.gameObject.SetActive(false);
			this.fireParticle.Stop();
		}
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00013D60 File Offset: 0x00011F60
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.FlareActive))
		{
			this.lit = base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value;
		}
		this.fuel = base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
		this.item.SetUseRemainingPercentage(this.fuel / this.startingFuel);
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00013DC0 File Offset: 0x00011FC0
	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (this.activeByDefault && this.item.rig.isKinematic)
		{
			return;
		}
		if (this.lanternLight.gameObject.activeSelf != this.lit)
		{
			this.lanternLight.gameObject.SetActive(this.lit);
			if (this.lit)
			{
				this.fireParticle.Play();
				this.currentLightIntensity = 0f;
				DOTween.To(() => this.currentLightIntensity, delegate(float x)
				{
					this.currentLightIntensity = x;
				}, this.lightIntensity, 1f);
			}
			else
			{
				this.fireParticle.Clear();
				this.fireParticle.Stop();
			}
		}
		this.lanternLight.intensity = this.lightCurve.Evaluate(Time.time * this.lightSpeed) * this.currentLightIntensity;
		this.item.UIData.mainInteractPrompt = (this.lit ? this.actionPromptWhenLit : this.actionPromptWhenUnlit);
		this.item.usingTimePrimary = (this.lit ? this.useTimeWhenLit : this.useTimeWhenUnlit);
		base.GetData<OptionableIntItemData>(DataEntryKey.ItemUses).Value = ((this.fuel > 0f) ? (-1) : 0);
		this.UpdateFuel();
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00013F10 File Offset: 0x00012110
	private void UpdateFuel()
	{
		if (this.lit && this.HasAuthority())
		{
			this.fuel -= Time.deltaTime;
			if (this.fuel <= 0f)
			{
				this.fuel = 0f;
				if (this.photonView.IsMine)
				{
					this.SnuffLantern();
				}
			}
			base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value = this.fuel;
			this.item.SetUseRemainingPercentage(this.fuel / this.startingFuel);
		}
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00013FA1 File Offset: 0x000121A1
	private FloatItemData SetupDefaultFuel()
	{
		return new FloatItemData
		{
			Value = this.startingFuel
		};
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00013FB4 File Offset: 0x000121B4
	public void ToggleLantern()
	{
		this.photonView.RPC("LightLanternRPC", 0, new object[] { !this.lit });
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00013FDE File Offset: 0x000121DE
	public void SnuffLantern()
	{
		this.photonView.RPC("LightLanternRPC", 0, new object[] { false });
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00014000 File Offset: 0x00012200
	[PunRPC]
	public void LightLanternRPC(bool litValue)
	{
		this.fireParticle.main.prewarm = false;
		this.lit = litValue;
		base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = this.lit;
	}

	// Token: 0x040002A0 RID: 672
	[SerializeField]
	private bool lit;

	// Token: 0x040002A1 RID: 673
	public string actionPromptWhenUnlit;

	// Token: 0x040002A2 RID: 674
	public string actionPromptWhenLit;

	// Token: 0x040002A3 RID: 675
	public float useTimeWhenUnlit;

	// Token: 0x040002A4 RID: 676
	public float useTimeWhenLit;

	// Token: 0x040002A5 RID: 677
	public Light lanternLight;

	// Token: 0x040002A6 RID: 678
	public AnimationCurve lightCurve;

	// Token: 0x040002A7 RID: 679
	public float lightSpeed;

	// Token: 0x040002A8 RID: 680
	public float lightIntensity = 10f;

	// Token: 0x040002A9 RID: 681
	public float startingFuel;

	// Token: 0x040002AA RID: 682
	[SerializeField]
	private float fuel;

	// Token: 0x040002AB RID: 683
	public ParticleSystem fireParticle;

	// Token: 0x040002AC RID: 684
	private new Item item;

	// Token: 0x040002AD RID: 685
	private float currentLightIntensity;

	// Token: 0x040002AE RID: 686
	public bool activeByDefault;
}
