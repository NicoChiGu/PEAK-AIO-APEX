using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000175 RID: 373
public class RopeShooter : ItemComponent
{
	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000BCC RID: 3020 RVA: 0x0003EFD9 File Offset: 0x0003D1D9
	// (set) Token: 0x06000BCD RID: 3021 RVA: 0x0003EFF4 File Offset: 0x0003D1F4
	private int Ammo
	{
		get
		{
			return base.GetData<IntItemData>(DataEntryKey.PetterItemUses, new Func<IntItemData>(this.GetNew)).Value;
		}
		set
		{
			base.GetData<IntItemData>(DataEntryKey.PetterItemUses, new Func<IntItemData>(this.GetNew)).Value = value;
			this.item.SetUseRemainingPercentage((float)value / (float)this.startAmmo);
		}
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x0003F025 File Offset: 0x0003D225
	private IntItemData GetNew()
	{
		Debug.Log(string.Format("GetNew startAmmo: {0}", this.startAmmo));
		return new IntItemData
		{
			Value = this.startAmmo
		};
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x0003F052 File Offset: 0x0003D252
	public override void Awake()
	{
		base.Awake();
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Combine(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x0003F081 File Offset: 0x0003D281
	private void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Remove(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x0003F0AC File Offset: 0x0003D2AC
	public void Update()
	{
		RaycastHit raycastHit;
		this.item.overrideUsability = Optionable<bool>.Some(this.WillAttach(out raycastHit));
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x06000BD2 RID: 3026 RVA: 0x0003F0D1 File Offset: 0x0003D2D1
	public bool HasAmmo
	{
		get
		{
			return this.Ammo >= 1;
		}
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x0003F0E0 File Offset: 0x0003D2E0
	private void OnPrimaryFinishedCast()
	{
		RaycastHit raycastHit;
		if (!this.WillAttach(out raycastHit))
		{
			return;
		}
		Debug.Log("OnPrimaryFinishedCast");
		if (!this.HasAmmo)
		{
			this.fumesVFX.Play();
			Debug.Log(string.Format("totalUses < 1,  {0}", this.item.totalUses));
			for (int i = 0; i < this.emptySound.Length; i++)
			{
				this.emptySound[i].Play(base.transform.position);
			}
			return;
		}
		Transform transform = MainCamera.instance.transform;
		RaycastHit raycastHit2;
		if (!PExt.Raycast(ExtCamera.ForwardRay<Transform>(transform), ref raycastHit2, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 0f))
		{
			return;
		}
		Quaternion identity = Quaternion.identity;
		if (Vector3.Angle(raycastHit2.normal, Vector3.up) < 45f)
		{
			Debug.Log("Angle is less than 45");
			ExtQuaternion.FromUpAndRightPrioUp(base.transform.forward, raycastHit2.normal);
		}
		else
		{
			Debug.Log("Angle is more than 45");
			ExtQuaternion.FromUpAndRightPrioUp(Vector3.down, -transform.forward);
		}
		GameObject gameObject = PhotonNetwork.Instantiate(this.ropeAnchorWithRopePref.name, this.spawnPoint.position, ExtQuaternion.FromUpAndRightPrioUp(base.transform.forward, raycastHit2.normal), 0, null);
		float num = Vector3.Distance(this.spawnPoint.position, raycastHit2.point) * 0.01f;
		this.gunshotVFX.Play();
		for (int j = 0; j < this.shotSound.Length; j++)
		{
			this.shotSound[j].Play(base.transform.position);
		}
		GamefeelHandler.instance.AddPerlinShakeProximity(this.gunshotVFX.transform.position, this.screenshakeIntensity, 0.3f, 15f, 10f);
		this.hideOnFire.SetActive(this.HasAmmo);
		int ammo = this.Ammo;
		this.Ammo = ammo - 1;
		this.photonView.RPC("Sync_Rpc", 3, new object[] { this.HasAmmo });
		gameObject.GetComponent<RopeAnchorProjectile>().photonView.RPC("GetShot", 3, new object[]
		{
			raycastHit2.point,
			num,
			this.length,
			-Camera.main.transform.forward
		});
		if (this.photonView.IsMine)
		{
			Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.RopePlaced, Rope.GetLengthInMeters(this.length));
			GameUtils.instance.IncrementPermanentItemsPlaced();
		}
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x0003F37F File Offset: 0x0003D57F
	[PunRPC]
	private void Sync_Rpc(bool show)
	{
		Debug.Log(string.Format("Sync_Rpc: {0}", show));
		this.hideOnFire.SetActive(show);
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x0003F3A4 File Offset: 0x0003D5A4
	public bool WillAttach(out RaycastHit hit)
	{
		hit = default(RaycastHit);
		return Character.localCharacter.data.isGrounded && this.HasAmmo && Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, ref hit, this.maxLength, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 0);
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x0003F410 File Offset: 0x0003D610
	public override void OnInstanceDataSet()
	{
		this.hideOnFire.SetActive(this.HasAmmo);
		Debug.Log(string.Format(" OnInstanceDataSet item.totalUses: {0}", this.Ammo));
		this.item.SetUseRemainingPercentage((float)this.Ammo / (float)this.startAmmo);
	}

	// Token: 0x04000AEA RID: 2794
	public ParticleSystem gunshotVFX;

	// Token: 0x04000AEB RID: 2795
	public ParticleSystem fumesVFX;

	// Token: 0x04000AEC RID: 2796
	public bool cantReFire;

	// Token: 0x04000AED RID: 2797
	public Transform spawnPoint;

	// Token: 0x04000AEE RID: 2798
	public float length;

	// Token: 0x04000AEF RID: 2799
	public GameObject ropeAnchorWithRopePref;

	// Token: 0x04000AF0 RID: 2800
	public GameObject hideOnFire;

	// Token: 0x04000AF1 RID: 2801
	public float screenshakeIntensity = 30f;

	// Token: 0x04000AF2 RID: 2802
	public int startAmmo = 1;

	// Token: 0x04000AF3 RID: 2803
	public SFX_Instance[] shotSound;

	// Token: 0x04000AF4 RID: 2804
	public SFX_Instance[] emptySound;

	// Token: 0x04000AF5 RID: 2805
	public float maxLength = 30f;
}
