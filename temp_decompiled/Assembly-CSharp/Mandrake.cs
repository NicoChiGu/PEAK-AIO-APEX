using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002A3 RID: 675
public class Mandrake : ItemComponent
{
	// Token: 0x0600127D RID: 4733 RVA: 0x0005E10E File Offset: 0x0005C30E
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x0005E116 File Offset: 0x0005C316
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		this.CheckNearby();
		if (base.HasData(DataEntryKey.Used))
		{
			this.waitBeforeScreamTime = Random.Range(this.screamWaitMin, this.screamWaitMax);
		}
		else
		{
			this.waitBeforeScreamTime = this.screamWaitMax;
			base.GetData<BoolItemData>(DataEntryKey.Used).Value = true;
		}
		this.sfxVolumeSetting = GameHandler.Instance.SettingsHandler.GetSetting<SFXVolumeSetting>();
		this.masterVolumeSetting = GameHandler.Instance.SettingsHandler.GetSetting<MasterVolumeSetting>();
		yield break;
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x0005E128 File Offset: 0x0005C328
	private void CheckNearby()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.checkTime += Time.deltaTime;
		if (this.checkTime > 1f)
		{
			bool flag = false;
			this.checkTime = 0f;
			if (this.item.cooking.timesCookedLocal > 0)
			{
				base.enabled = false;
				flag = false;
				return;
			}
			foreach (Character character in Character.AllCharacters)
			{
				if (Vector3.Distance(base.transform.position, character.Center) < this.nearPlayersDistance && character.data.fullyConscious)
				{
					flag = true;
					break;
				}
			}
			this._closeToSomebody = flag;
		}
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x0005E200 File Offset: 0x0005C400
	public void Update()
	{
		this.CheckNearby();
		this.CheckScream();
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x0005E210 File Offset: 0x0005C410
	private void CheckScream()
	{
		if (PhotonNetwork.IsMasterClient && this._closeToSomebody && !this.screaming && (this.item.itemState != ItemState.Ground || (this.item.itemState == ItemState.Ground && !this.item.rig.isKinematic)))
		{
			this._time += Time.deltaTime;
			if (this._time > this.waitBeforeScreamTime)
			{
				this.view.RPC("RPC_Scream", 0, Array.Empty<object>());
				this._time = 0f;
				this.waitBeforeScreamTime = Random.Range(this.screamWaitMin, this.screamWaitMax);
			}
		}
		this.aoe.enabled = this.sfxVolumeSetting.Value > 0.01f && this.masterVolumeSetting.Value > 0.01f;
		this.timeEvent.enabled = this.aoe.enabled;
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x0005E309 File Offset: 0x0005C509
	[PunRPC]
	public void RPC_Scream()
	{
		base.StartCoroutine(this.ScreamRoutine());
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x0005E318 File Offset: 0x0005C518
	private IEnumerator ScreamRoutine()
	{
		if (this.item.cooking.timesCookedLocal > 0)
		{
			yield break;
		}
		this.screaming = true;
		this.anim.SetBool("Scream", true);
		while (this.currentScreamTime < this.screamWaitTime)
		{
			this.currentScreamTime += Time.deltaTime;
			base.GetData<FloatItemData>(DataEntryKey.ScreamTime).Value = this.currentScreamTime;
			yield return null;
			if (this.item.cooking.timesCookedLocal > 0)
			{
				break;
			}
		}
		this.screaming = false;
		this.currentScreamTime = 0f;
		this.anim.SetBool("Scream", false);
		yield break;
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x0005E327 File Offset: 0x0005C527
	private void OnDestroy()
	{
		SFX_Player.StopPlaying(this.handle, 0f);
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x0005E339 File Offset: 0x0005C539
	public override void OnInstanceDataSet()
	{
		this.currentScreamTime = base.GetData<FloatItemData>(DataEntryKey.ScreamTime).Value;
		if (this.currentScreamTime > 0f && !this.screaming)
		{
			base.StartCoroutine(this.ScreamRoutine());
		}
	}

	// Token: 0x0400112A RID: 4394
	public new Item item;

	// Token: 0x0400112B RID: 4395
	public Animator anim;

	// Token: 0x0400112C RID: 4396
	public SFX_Instance sfxScream;

	// Token: 0x0400112D RID: 4397
	public PhotonView view;

	// Token: 0x0400112E RID: 4398
	public float screamWaitMin;

	// Token: 0x0400112F RID: 4399
	public float screamWaitMax;

	// Token: 0x04001130 RID: 4400
	private float _time;

	// Token: 0x04001131 RID: 4401
	private float screamWaitTime = 3f;

	// Token: 0x04001132 RID: 4402
	private float currentScreamTime;

	// Token: 0x04001133 RID: 4403
	private MandrakeScreamFX mandrakeScreamFX;

	// Token: 0x04001134 RID: 4404
	private SFX_Player.SoundEffectHandle handle;

	// Token: 0x04001135 RID: 4405
	public ParticleSystem vfxScream;

	// Token: 0x04001136 RID: 4406
	private bool _closeToSomebody = true;

	// Token: 0x04001137 RID: 4407
	public bool screaming;

	// Token: 0x04001138 RID: 4408
	public float nearPlayersDistance = 20f;

	// Token: 0x04001139 RID: 4409
	private SFXVolumeSetting sfxVolumeSetting;

	// Token: 0x0400113A RID: 4410
	private MasterVolumeSetting masterVolumeSetting;

	// Token: 0x0400113B RID: 4411
	public AOE aoe;

	// Token: 0x0400113C RID: 4412
	public TimeEvent timeEvent;

	// Token: 0x0400113D RID: 4413
	private float checkTime;

	// Token: 0x0400113E RID: 4414
	[SerializeField]
	private float waitBeforeScreamTime;
}
