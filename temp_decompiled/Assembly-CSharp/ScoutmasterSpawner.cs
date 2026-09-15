using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class ScoutmasterSpawner : MonoBehaviourPunCallbacks
{
	// Token: 0x06000ABF RID: 2751 RVA: 0x00039A1A File Offset: 0x00037C1A
	private void Awake()
	{
		if (PhotonNetwork.InRoom)
		{
			this.SpawnScoutmaster();
		}
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00039A29 File Offset: 0x00037C29
	public override void OnJoinedRoom()
	{
		this.SpawnScoutmaster();
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00039A34 File Offset: 0x00037C34
	private void SpawnScoutmaster()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		Debug.Log("SPAWN SCOUTMASTER");
		PhotonNetwork.InstantiateRoomObject("Character_Scoutmaster", base.transform.position, base.transform.rotation, 0, null).GetComponent<Character>().data.spawnPoint = base.transform;
	}
}
