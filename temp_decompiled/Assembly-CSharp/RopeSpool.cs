using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class RopeSpool : ItemComponent
{
	// Token: 0x170000DC RID: 220
	// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x0003F487 File Offset: 0x0003D687
	public bool IsOutOfRope
	{
		get
		{
			return this.ropeFuel <= 2f;
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x0003F499 File Offset: 0x0003D699
	// (set) Token: 0x06000BDA RID: 3034 RVA: 0x0003F4B4 File Offset: 0x0003D6B4
	public float RopeFuel
	{
		get
		{
			return base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.DefaultFuel)).Value;
		}
		set
		{
			base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.DefaultFuel)).Value = value;
			this.ropeFuel = value;
			if (this.ropeFuel <= 2f)
			{
				int num = ((this.item.holderCharacter == null) ? (-1) : this.item.holderCharacter.photonView.ViewID);
				this.photonView.RPC("Consume", 0, new object[] { num });
			}
			this.item.SetUseRemainingPercentage(this.ropeFuel / this.ropeStartFuel);
		}
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x0003F553 File Offset: 0x0003D753
	private FloatItemData DefaultFuel()
	{
		return new FloatItemData
		{
			Value = this.ropeStartFuel
		};
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x06000BDC RID: 3036 RVA: 0x0003F566 File Offset: 0x0003D766
	// (set) Token: 0x06000BDD RID: 3037 RVA: 0x0003F56E File Offset: 0x0003D76E
	public float Segments
	{
		get
		{
			return this.segments;
		}
		set
		{
			this.segments = value;
		}
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x0003F577 File Offset: 0x0003D777
	public override void Awake()
	{
		base.Awake();
		this.ropeTier = base.GetComponent<RopeTier>();
		this.rig = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x0003F598 File Offset: 0x0003D798
	private void OnDestroy()
	{
		if (this.item.itemState == ItemState.Held && this.photonView.IsMine)
		{
			this.ClearRope();
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		this.ropeFuel = this.RopeFuel;
		this.item.SetUseRemainingPercentage(this.ropeFuel / this.ropeStartFuel);
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0003F5F8 File Offset: 0x0003D7F8
	private void Update()
	{
		if (this.item.itemState != ItemState.Held || this.IsOutOfRope)
		{
			return;
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (this.ropeInstance == null && !this.IsOutOfRope)
		{
			this.ropeInstance = PhotonNetwork.Instantiate(this.ropePrefab.name, this.ropeBase.position, this.ropeBase.rotation, 0, null);
			this.rope = this.ropeInstance.GetComponent<Rope>();
			this.rope.photonView.RPC("AttachToSpool_Rpc", 3, new object[] { this.photonView });
			this.Segments = 0f;
			this.segsVel = 0f;
			this.scroll = 0f;
			this.rope.Segments = this.Segments;
		}
		this.item.SetUseRemainingPercentage(ExtDataTypes.Clamp01((this.ropeFuel - this.rope.Segments) / this.ropeStartFuel));
		if (this.item.holderCharacter.input.scrollForwardIsPressed)
		{
			this.scroll = 0.4f;
		}
		else if (this.item.holderCharacter.input.scrollBackwardIsPressed)
		{
			this.scroll = -0.4f;
		}
		else
		{
			this.scroll = this.item.holderCharacter.input.scrollInput;
		}
		if (this.ropeTier.LookingToPlaceAnchor)
		{
			this.scroll = 0f;
			this.segsVel = 0f;
		}
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x0003F78C File Offset: 0x0003D98C
	private void FixedUpdate()
	{
		this.segsVel = Mathf.Lerp(this.segsVel, this.scroll, Time.fixedDeltaTime * 4f);
		this.segsVel = Mathf.Clamp(this.segsVel, -1f, 5f);
		if (this.photonView.IsMine && this.rope != null)
		{
			this.Segments += this.segsVel * Time.fixedDeltaTime * 25f;
			this.Segments = Mathf.Clamp(this.Segments, this.minSegments, Mathf.Min(this.ropeFuel, (float)Rope.MaxSegments));
			float num = this.Segments - this.rope.Segments;
			this.ropeSpoolTf.transform.localEulerAngles += new Vector3(0f, 0f, num * -50f);
			this.rope.Segments = this.Segments;
		}
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x0003F894 File Offset: 0x0003DA94
	public void ClearRope()
	{
		Debug.Log(string.Format("ClearRope{0}", this.ropeInstance));
		if (this.ropeInstance != null)
		{
			Debug.Log("Destroy rope");
			PhotonNetwork.Destroy(this.rope.view);
		}
		this.rope = null;
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x0003F8E8 File Offset: 0x0003DAE8
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.Fuel))
		{
			Debug.Log("HasData");
			this.ropeFuel = base.GetData<FloatItemData>(DataEntryKey.Fuel).Value;
			Debug.Log(string.Format("ropeFuel {0}", this.ropeFuel));
		}
	}

	// Token: 0x04000AF6 RID: 2806
	public float segments;

	// Token: 0x04000AF7 RID: 2807
	public float minSegments = 3.5f;

	// Token: 0x04000AF8 RID: 2808
	public float ropeStartFuel = 60f;

	// Token: 0x04000AF9 RID: 2809
	private float ropeFuel = 60f;

	// Token: 0x04000AFA RID: 2810
	public GameObject ropePrefab;

	// Token: 0x04000AFB RID: 2811
	public Transform ropeBase;

	// Token: 0x04000AFC RID: 2812
	public Transform ropeStart;

	// Token: 0x04000AFD RID: 2813
	public Transform ropeSpoolTf;

	// Token: 0x04000AFE RID: 2814
	public GameObject ropeInstance;

	// Token: 0x04000AFF RID: 2815
	public Rigidbody rig;

	// Token: 0x04000B00 RID: 2816
	public Rope rope;

	// Token: 0x04000B01 RID: 2817
	private float scroll;

	// Token: 0x04000B02 RID: 2818
	private float segsVel;

	// Token: 0x04000B03 RID: 2819
	private RopeTier ropeTier;

	// Token: 0x04000B04 RID: 2820
	public bool isAntiRope;
}
