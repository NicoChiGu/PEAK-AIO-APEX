using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000254 RID: 596
public class EruptionSpawner : MonoBehaviour
{
	// Token: 0x06001123 RID: 4387 RVA: 0x00056355 File Offset: 0x00054555
	private void Start()
	{
		this.min = base.transform.GetChild(0);
		this.max = base.transform.GetChild(1);
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x00056388 File Offset: 0x00054588
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!HelperFunctions.AnyPlayerInZRange(this.min.position.z, this.max.position.z))
		{
			return;
		}
		this.counter -= Time.deltaTime;
		if (this.counter < 0f)
		{
			this.counter = Random.Range(-5f, 15f);
			Vector3 position = base.transform.position;
			position.x += Random.Range(-155f, 155f);
			position.z += Random.Range(-140f, 140f);
			this.photonView.RPC("RPCA_SpawnEruption", 0, new object[] { position });
		}
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x00056459 File Offset: 0x00054659
	[PunRPC]
	public void RPCA_SpawnEruption(Vector3 position)
	{
		Object.Instantiate<GameObject>(this.eruption, position, Quaternion.LookRotation(Vector3.up));
	}

	// Token: 0x04000F8F RID: 3983
	private float counter = 10f;

	// Token: 0x04000F90 RID: 3984
	public GameObject eruption;

	// Token: 0x04000F91 RID: 3985
	private PhotonView photonView;

	// Token: 0x04000F92 RID: 3986
	private Transform min;

	// Token: 0x04000F93 RID: 3987
	private Transform max;
}
