using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000115 RID: 277
public class Flare : ItemComponent
{
	// Token: 0x060008E9 RID: 2281 RVA: 0x0003002D File Offset: 0x0002E22D
	public override void Awake()
	{
		base.Awake();
		this.trackable = base.GetComponent<TrackableNetworkObject>();
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00030041 File Offset: 0x0002E241
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.Color))
		{
			this.flareColor = base.GetData<ColorItemData>(DataEntryKey.Color).Value;
		}
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00030060 File Offset: 0x0002E260
	private void Update()
	{
		bool value = base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value;
		this.item.UIData.canPocket = !value;
		this.item.UIData.canBackpack = !value;
		if (value && !this.trackable.hasTracker)
		{
			this.EnableFlareVisuals();
		}
		if (value && this.item.holderCharacter && Singleton<MountainProgressHandler>.Instance.IsAtPeak(this.item.holderCharacter.Center) && !Singleton<PeakHandler>.Instance.summonedHelicopter)
		{
			base.GetComponent<PhotonView>().RPC("TriggerHelicopter", 3, Array.Empty<object>());
		}
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x0003010B File Offset: 0x0002E30B
	[PunRPC]
	public void TriggerHelicopter()
	{
		Singleton<PeakHandler>.Instance.SummonHelicopter();
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00030117 File Offset: 0x0002E317
	public void LightFlare()
	{
		base.GetComponent<PhotonView>().RPC("SetFlareLitRPC", 3, Array.Empty<object>());
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00030130 File Offset: 0x0002E330
	[PunRPC]
	public void SetFlareLitRPC()
	{
		if (this.item.holderCharacter)
		{
			this.flareColor = this.item.holderCharacter.refs.customization.PlayerColor;
			this.flareColor.a = 1f;
			base.GetData<ColorItemData>(DataEntryKey.Color).Value = this.flareColor;
			string text = "Set flare color to ";
			Color value = base.GetData<ColorItemData>(DataEntryKey.Color).Value;
			Debug.Log(text + value.ToString());
		}
		base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = true;
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x000301CC File Offset: 0x0002E3CC
	public void EnableFlareVisuals()
	{
		Debug.Log(string.Format("Lighting flare with photon ID {0} with instance ID {1}", this.photonView.ViewID, this.trackable.instanceID));
		TrackNetworkedObject component = Object.Instantiate<TrackNetworkedObject>(this.flareVFXPrefab, base.transform.position, base.transform.rotation).GetComponent<TrackNetworkedObject>();
		component.SetObject(this.trackable);
		component.gameObject.GetComponent<ParticleSystem>().main.startColor = this.flareColor;
		string text = "Lit flare with color ";
		Color color = this.flareColor;
		Debug.Log(text + color.ToString());
	}

	// Token: 0x0400086E RID: 2158
	private TrackableNetworkObject trackable;

	// Token: 0x0400086F RID: 2159
	public TrackNetworkedObject flareVFXPrefab;

	// Token: 0x04000870 RID: 2160
	public Color flareColor;
}
