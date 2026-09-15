using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002AF RID: 687
public class NetworkedObjectSpawner : MonoBehaviour
{
	// Token: 0x060012BD RID: 4797 RVA: 0x0005F98B File Offset: 0x0005DB8B
	private void Start()
	{
		this.SetCounter();
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x0005F993 File Offset: 0x0005DB93
	private void SetCounter()
	{
		this.untilNext = Mathf.Lerp(this.minRate, this.maxRate, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x0005F9BC File Offset: 0x0005DBBC
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.untilNext -= Time.deltaTime;
		if (this.untilNext < 0f)
		{
			this.SetCounter();
			this.SpawnObject();
		}
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x0005F9F1 File Offset: 0x0005DBF1
	private void SpawnObject()
	{
		PhotonNetwork.Instantiate(this.objToSpawn.name, base.transform.position + this.spawnOffset, base.transform.rotation, 0, null);
	}

	// Token: 0x0400116F RID: 4463
	public GameObject objToSpawn;

	// Token: 0x04001170 RID: 4464
	public float minRate = 3f;

	// Token: 0x04001171 RID: 4465
	public float maxRate = 6f;

	// Token: 0x04001172 RID: 4466
	public float randomPow = 2f;

	// Token: 0x04001173 RID: 4467
	public Vector3 spawnOffset;

	// Token: 0x04001174 RID: 4468
	private float untilNext;
}
