using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200016F RID: 367
public class RopeAnchorProjectile : MonoBehaviourPunCallbacks
{
	// Token: 0x06000B9A RID: 2970 RVA: 0x0003DDE8 File Offset: 0x0003BFE8
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient && this.shot)
		{
			this.photonView.RPC("GetShot", newPlayer, new object[] { this.lastShotTo, this.lastShotTravelTime, this.lastShotRopeLength, this.lastShotFlyingRotation });
		}
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x0003DE5A File Offset: 0x0003C05A
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x0003DE68 File Offset: 0x0003C068
	[PunRPC]
	public void GetShot(Vector3 to, float travelTime, float ropeLength, Vector3 flyingRotation)
	{
		RopeAnchorProjectile.<>c__DisplayClass10_0 CS$<>8__locals1 = new RopeAnchorProjectile.<>c__DisplayClass10_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.flyingRotation = flyingRotation;
		CS$<>8__locals1.to = to;
		CS$<>8__locals1.travelTime = travelTime;
		CS$<>8__locals1.ropeLength = ropeLength;
		this.lastShotTo = CS$<>8__locals1.to;
		this.lastShotTravelTime = CS$<>8__locals1.travelTime;
		this.lastShotRopeLength = CS$<>8__locals1.ropeLength;
		this.lastShotFlyingRotation = CS$<>8__locals1.flyingRotation;
		this.shot = true;
		this.startRotation = base.transform.rotation;
		this.startPosition = base.transform.position;
		base.StartCoroutine(CS$<>8__locals1.<GetShot>g__SpawnRopeRoutine|0());
	}

	// Token: 0x04000AC1 RID: 2753
	public PhotonView photonView;

	// Token: 0x04000AC2 RID: 2754
	public bool shot;

	// Token: 0x04000AC3 RID: 2755
	private Vector3 startPosition;

	// Token: 0x04000AC4 RID: 2756
	private Quaternion startRotation;

	// Token: 0x04000AC5 RID: 2757
	private Vector3 lastShotTo;

	// Token: 0x04000AC6 RID: 2758
	private float lastShotTravelTime;

	// Token: 0x04000AC7 RID: 2759
	private float lastShotRopeLength;

	// Token: 0x04000AC8 RID: 2760
	private Vector3 lastShotFlyingRotation;
}
