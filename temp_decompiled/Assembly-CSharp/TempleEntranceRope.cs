using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class TempleEntranceRope : RopeAnchorWithRope
{
	// Token: 0x06000BF3 RID: 3059 RVA: 0x00040025 File Offset: 0x0003E225
	public override void Awake()
	{
		base.Awake();
		this.doorStartingPosition = this.doorRb.transform.position;
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x00040043 File Offset: 0x0003E243
	public void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		this.UpdateWeight();
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x00040053 File Offset: 0x0003E253
	public void FixedUpdate()
	{
		this.UpdateDoorOpen();
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x0004005B File Offset: 0x0003E25B
	[PunRPC]
	private void SetWeightRPC(float weight)
	{
		Debug.Log(string.Format("Received weight RPC. {0}", weight));
		this.currentWeight = weight;
		if (this.currentWeight > this.lockWeight)
		{
			this.lockedOpen = true;
		}
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x0004008E File Offset: 0x0003E28E
	private void UpdateDescent()
	{
		float num = this.currentWeight / this.weightPerSegment;
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x000400A0 File Offset: 0x0003E2A0
	private void UpdateDoorOpen()
	{
		float num = Mathf.Min(this.doorHeightPerWeight * this.currentWeight, this.maxDoorHeight);
		this.currentDoorTarget = this.doorStartingPosition + Vector3.up * num;
		Vector3 vector = this.currentDoorTarget - this.doorRb.transform.position;
		if (vector.y > 0f)
		{
			Vector3 vector2 = Vector3.ClampMagnitude(Vector3.Lerp(this.doorRb.position, this.currentDoorTarget, this.doorLerpSpeedUp * Time.fixedDeltaTime) - this.doorRb.position, this.maxDoorMoveSpeedUp * Time.fixedDeltaTime);
			this.doorRb.MovePosition(this.doorRb.position + vector2);
			return;
		}
		if (vector.y < 0f)
		{
			this.doorRb.MovePosition(Vector3.MoveTowards(this.doorRb.position, this.currentDoorTarget, this.doorMoveSpeedDown * Time.fixedDeltaTime));
		}
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x000401A6 File Offset: 0x0003E3A6
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("SetWeightRPC", 0, new object[] { this.currentWeight });
		}
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x000401E0 File Offset: 0x0003E3E0
	private void UpdateWeight()
	{
		if (!base.photonView.IsMine || !this.rope)
		{
			return;
		}
		float num = 0f;
		foreach (Character character in this.rope.charactersClimbing)
		{
			num += this.baseScoutWeight;
			num += character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Weight);
		}
		if ((!this.lockedOpen || num > this.currentWeight) && this.currentWeight != num)
		{
			base.photonView.RPC("SetWeightRPC", 0, new object[] { num });
		}
	}

	// Token: 0x04000B18 RID: 2840
	public float baseScoutWeight;

	// Token: 0x04000B19 RID: 2841
	public float weightPerSegment;

	// Token: 0x04000B1A RID: 2842
	public float currentWeight;

	// Token: 0x04000B1B RID: 2843
	[Header("Weight at which the door will lock open.")]
	public float lockWeight;

	// Token: 0x04000B1C RID: 2844
	public Rigidbody doorRb;

	// Token: 0x04000B1D RID: 2845
	public float doorHeightPerWeight;

	// Token: 0x04000B1E RID: 2846
	public float maxDoorHeight;

	// Token: 0x04000B1F RID: 2847
	public float doorLerpSpeedUp;

	// Token: 0x04000B20 RID: 2848
	public float maxDoorMoveSpeedUp;

	// Token: 0x04000B21 RID: 2849
	public float doorMoveSpeedDown;

	// Token: 0x04000B22 RID: 2850
	private Vector3 doorStartingPosition;

	// Token: 0x04000B23 RID: 2851
	private Vector3 currentDoorTarget;

	// Token: 0x04000B24 RID: 2852
	private bool lockedOpen;
}
