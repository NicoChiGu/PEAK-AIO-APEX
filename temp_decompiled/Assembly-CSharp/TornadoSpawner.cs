using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000355 RID: 853
public class TornadoSpawner : MonoBehaviour
{
	// Token: 0x060015E5 RID: 5605 RVA: 0x000712A5 File Offset: 0x0006F4A5
	private void Start()
	{
		this.untilNext = Random.Range(this.minSpawnTimeFirst, this.maxSpawnTimeFirst);
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x000712CC File Offset: 0x0006F4CC
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.untilNext -= Time.deltaTime;
		if (this.untilNext <= 0f)
		{
			this.SpawnTornado();
			this.untilNext = Random.Range(this.minSpawnTime, this.maxSpawnTime);
		}
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x00071320 File Offset: 0x0006F520
	private void SpawnTornado()
	{
		PhotonNetwork.Instantiate("Tornado", this.GetSpawnPos(), Quaternion.identity, 0, null).GetComponent<PhotonView>().RPC("RPCA_InitTornado", 0, new object[] { this.view.ViewID });
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x00071370 File Offset: 0x0006F570
	private Vector3 GetSpawnPos()
	{
		Transform transform = base.transform.Find("TornadoPoints");
		return transform.GetChild(Random.Range(0, transform.childCount)).position;
	}

	// Token: 0x040014BE RID: 5310
	public float minSpawnTimeFirst = 30f;

	// Token: 0x040014BF RID: 5311
	public float maxSpawnTimeFirst = 300f;

	// Token: 0x040014C0 RID: 5312
	public float minSpawnTime = 30f;

	// Token: 0x040014C1 RID: 5313
	public float maxSpawnTime = 300f;

	// Token: 0x040014C2 RID: 5314
	private float untilNext;

	// Token: 0x040014C3 RID: 5315
	private bool firstTime = true;

	// Token: 0x040014C4 RID: 5316
	private PhotonView view;
}
