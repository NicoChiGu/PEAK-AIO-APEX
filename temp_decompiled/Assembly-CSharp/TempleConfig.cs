using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class TempleConfig : MonoBehaviourPunCallbacks
{
	// Token: 0x06000D6A RID: 3434 RVA: 0x0004359A File Offset: 0x0004179A
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x000435A8 File Offset: 0x000417A8
	private void Start()
	{
		for (int i = 0; i < this.columns.Count; i++)
		{
			this.positions.Add(this.columns[i].transform.position);
		}
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x000435EC File Offset: 0x000417EC
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.view.IsMine)
		{
			this.view.RPC("CreateTemple_RPC", 3, new object[] { (int)DateTime.Now.Ticks });
		}
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x0004363C File Offset: 0x0004183C
	[PunRPC]
	public void CreateTemple_RPC(int seed)
	{
		Debug.Log("Set Seed");
		Random.InitState(seed);
		List<GameObject> list = this.columns;
		list = Enumerable.ToList<GameObject>(Enumerable.OrderBy<GameObject, float>(list, (GameObject x) => Random.value));
		for (int i = 0; i < list.Count; i++)
		{
			list[i].transform.position = this.positions[i];
			this.columns[i].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, (float)((int)(Random.value * 4f) * 90)));
		}
		for (int j = 0; j < this.arrowShooters.Length; j++)
		{
			if (Random.value < this.arrowShooterChance)
			{
				this.arrowShooters[j].SetActive(true);
			}
			else
			{
				this.arrowShooters[j].SetActive(false);
			}
		}
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x00043731 File Offset: 0x00041931
	private void Update()
	{
	}

	// Token: 0x04000B96 RID: 2966
	private PhotonView view;

	// Token: 0x04000B97 RID: 2967
	[Range(0f, 1f)]
	public float arrowShooterChance;

	// Token: 0x04000B98 RID: 2968
	public List<GameObject> columns;

	// Token: 0x04000B99 RID: 2969
	private List<Vector3> positions = new List<Vector3>();

	// Token: 0x04000B9A RID: 2970
	public GameObject[] arrowShooters;
}
