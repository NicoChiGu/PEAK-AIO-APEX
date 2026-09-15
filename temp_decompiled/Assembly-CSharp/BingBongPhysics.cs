using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000212 RID: 530
[DefaultExecutionOrder(1000001)]
public class BingBongPhysics : MonoBehaviour
{
	// Token: 0x06000FA4 RID: 4004 RVA: 0x0004DC1A File Offset: 0x0004BE1A
	private void OnEnable()
	{
		this.bingBongPowers = base.GetComponent<BingBongPowers>();
		this.bingBongPowers.SetTexts("PHYSICS", this.descr);
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x0004DC3E File Offset: 0x0004BE3E
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x0004DC4C File Offset: 0x0004BE4C
	private void Update()
	{
		this.CheckInuput();
		float cd = this.GetCD();
		bool auto = this.GetAuto();
		this.counter += Time.unscaledDeltaTime;
		if (this.counter < cd)
		{
			return;
		}
		if (auto && !Input.GetKey(323))
		{
			return;
		}
		if (!auto && !Input.GetKeyDown(323))
		{
			return;
		}
		this.DoEffect();
		this.counter = 0f;
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x0004DCBC File Offset: 0x0004BEBC
	private void DoEffect()
	{
		PhotonNetwork.Instantiate(this.GetEffect().name, base.transform.position, base.transform.rotation, 0, null).GetComponent<PhotonView>().RPC("RPCA_BingBongInitObj", 0, new object[] { this.view.ViewID });
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x0004DD1C File Offset: 0x0004BF1C
	private GameObject GetEffect()
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return this.effect_Blow;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return this.effect_Suck;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return this.effect_Push;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return this.effect_Push_Gentle;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForceGrab)
		{
			return this.effect_Grab;
		}
		return null;
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x0004DD79 File Offset: 0x0004BF79
	private bool GetAuto()
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return true;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return true;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return false;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return false;
		}
		BingBongPhysics.PhysicsType physicsType = this.physicsType;
		return true;
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x0004DDB0 File Offset: 0x0004BFB0
	private float GetCD()
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return 0.25f;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return 0.25f;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return 0f;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return 0f;
		}
		BingBongPhysics.PhysicsType physicsType = this.physicsType;
		return 0.25f;
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x0004DE08 File Offset: 0x0004C008
	private void CheckInuput()
	{
		if (Input.GetKeyDown(114))
		{
			this.SetState(BingBongPhysics.PhysicsType.Blow);
		}
		if (Input.GetKeyDown(116))
		{
			this.SetState(BingBongPhysics.PhysicsType.Suck);
		}
		if (Input.GetKeyDown(102))
		{
			this.SetState(BingBongPhysics.PhysicsType.ForceGrab);
		}
		if (Input.GetKeyDown(99))
		{
			this.SetState(BingBongPhysics.PhysicsType.ForcePush);
		}
		if (Input.GetKeyDown(118))
		{
			this.SetState(BingBongPhysics.PhysicsType.ForcePush_Gentle);
		}
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x0004DE65 File Offset: 0x0004C065
	private void SetState(BingBongPhysics.PhysicsType setType)
	{
		this.physicsType = setType;
		this.bingBongPowers.SetTip(setType.ToString(), 0);
	}

	// Token: 0x04000E01 RID: 3585
	public BingBongPhysics.PhysicsType physicsType;

	// Token: 0x04000E02 RID: 3586
	private PhotonView view;

	// Token: 0x04000E03 RID: 3587
	private BingBongPowers bingBongPowers;

	// Token: 0x04000E04 RID: 3588
	private string descr = "Blow: [R]\n\nSuck: [T]\n\nForce Grab: [F]\n\nForce Push: [C]\n\nForce Push Gentle: [V]";

	// Token: 0x04000E05 RID: 3589
	private float counter;

	// Token: 0x04000E06 RID: 3590
	public GameObject effect_Blow;

	// Token: 0x04000E07 RID: 3591
	public GameObject effect_Suck;

	// Token: 0x04000E08 RID: 3592
	public GameObject effect_Push;

	// Token: 0x04000E09 RID: 3593
	public GameObject effect_Push_Gentle;

	// Token: 0x04000E0A RID: 3594
	public GameObject effect_Grab;

	// Token: 0x020004CC RID: 1228
	public enum PhysicsType
	{
		// Token: 0x04001A86 RID: 6790
		Blow,
		// Token: 0x04001A87 RID: 6791
		Suck,
		// Token: 0x04001A88 RID: 6792
		ForcePush,
		// Token: 0x04001A89 RID: 6793
		ForcePush_Gentle,
		// Token: 0x04001A8A RID: 6794
		ForceGrab
	}
}
