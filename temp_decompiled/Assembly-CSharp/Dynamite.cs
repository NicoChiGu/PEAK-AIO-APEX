using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000114 RID: 276
public class Dynamite : ItemComponent
{
	// Token: 0x060008DD RID: 2269 RVA: 0x0002FC8E File Offset: 0x0002DE8E
	public override void Awake()
	{
		base.Awake();
		this.trackable = base.GetComponent<TrackableNetworkObject>();
		this.item.UIData.canPocket = false;
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x0002FCB3 File Offset: 0x0002DEB3
	private void Start()
	{
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>();
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0002FCCA File Offset: 0x0002DECA
	public override void OnInstanceDataSet()
	{
		this.fuseTime = base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0002FCEC File Offset: 0x0002DEEC
	private void Update()
	{
		if (!base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
		{
			this.TestLightWick();
			return;
		}
		if (!this.trackable.hasTracker)
		{
			this.EnableFlareVisuals();
		}
		if (this.setting.Value != 1)
		{
			this.sparks.gameObject.SetActive(true);
		}
		else
		{
			this.sparksPhotosensitive.gameObject.SetActive(true);
		}
		this.fuseTime = base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
		this.item.SetUseRemainingPercentage(this.fuseTime / this.startingFuseTime);
		if (this.photonView.IsMine)
		{
			this.fuseTime -= Time.deltaTime;
			if (this.fuseTime <= 0f)
			{
				if (this._hasExploded)
				{
					Debug.LogError("Attempting to explode an already exploded object!");
				}
				if (Character.localCharacter.data.currentItem == this.item)
				{
					Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f, false, true, true);
					Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
					Character.localCharacter.refs.afflictions.UpdateWeight();
				}
				this.photonView.RPC("RPC_Explode", 0, Array.Empty<object>());
				Debug.Log("<color=Red>Exploded</color>");
				PhotonNetwork.Destroy(base.gameObject);
				this.item.ClearDataFromBackpack();
				this.fuseTime = 0f;
			}
			base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value = this.fuseTime;
		}
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x0002FE9C File Offset: 0x0002E09C
	[PunRPC]
	private void RPC_Explode()
	{
		if (this.DEBUG_PauseOnExplode)
		{
			Debug.Break();
		}
		Object.Instantiate<GameObject>(this.explosionPrefab, base.transform.position, base.transform.rotation);
		base.gameObject.SetActive(false);
		this._hasExploded = true;
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x0002FEEC File Offset: 0x0002E0EC
	private void TestLightWick()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
		{
			return;
		}
		using (List<Character>.Enumerator enumerator = Character.AllCharacters.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.Center, base.transform.position) < this.lightFuseRadius)
				{
					this.LightFlare();
				}
			}
		}
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x0002FF70 File Offset: 0x0002E170
	private FloatItemData SetupDefaultFuel()
	{
		return new FloatItemData
		{
			Value = this.startingFuseTime
		};
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0002FF83 File Offset: 0x0002E183
	[PunRPC]
	public void TriggerHelicopter()
	{
		Singleton<PeakHandler>.Instance.SummonHelicopter();
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0002FF8F File Offset: 0x0002E18F
	public void LightFlare()
	{
		base.GetComponent<PhotonView>().RPC("SetFlareLitRPC", 0, Array.Empty<object>());
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0002FFA7 File Offset: 0x0002E1A7
	[PunRPC]
	public void SetFlareLitRPC()
	{
		base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = true;
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x0002FFB8 File Offset: 0x0002E1B8
	public void EnableFlareVisuals()
	{
		Debug.Log(string.Format("Lighting flare with photon ID {0} with instance ID {1}", this.photonView.ViewID, this.trackable.instanceID));
		Object.Instantiate<TrackNetworkedObject>(this.smokeVFXPrefab, base.transform.position, base.transform.rotation).GetComponent<TrackNetworkedObject>().SetObject(this.trackable);
	}

	// Token: 0x04000863 RID: 2147
	private bool _hasExploded;

	// Token: 0x04000864 RID: 2148
	private TrackableNetworkObject trackable;

	// Token: 0x04000865 RID: 2149
	public TrackNetworkedObject smokeVFXPrefab;

	// Token: 0x04000866 RID: 2150
	public GameObject explosionPrefab;

	// Token: 0x04000867 RID: 2151
	public float startingFuseTime;

	// Token: 0x04000868 RID: 2152
	public float lightFuseRadius;

	// Token: 0x04000869 RID: 2153
	[SerializeField]
	private float fuseTime;

	// Token: 0x0400086A RID: 2154
	public Transform sparks;

	// Token: 0x0400086B RID: 2155
	public Transform sparksPhotosensitive;

	// Token: 0x0400086C RID: 2156
	private PhotosensitiveSetting setting;

	// Token: 0x0400086D RID: 2157
	public bool DEBUG_PauseOnExplode;
}
