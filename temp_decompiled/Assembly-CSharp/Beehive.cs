using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class Beehive : ItemComponent
{
	// Token: 0x0600001E RID: 30 RVA: 0x000024F7 File Offset: 0x000006F7
	public override void OnJoinedRoom()
	{
		this.Init();
	}

	// Token: 0x0600001F RID: 31 RVA: 0x000024FF File Offset: 0x000006FF
	public void Start()
	{
		if (PhotonNetwork.InRoom)
		{
			this.Init();
		}
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002510 File Offset: 0x00000710
	private void Init()
	{
		if (!this.initialized)
		{
			this.initialized = true;
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!base.HasData(DataEntryKey.InstanceID))
			{
				this.instanceID = Beehive.currentMaxInstanceID;
				Beehive.currentMaxInstanceID++;
				this.photonView.RPC("SetInstanceIDRPC", 0, new object[]
				{
					this.instanceID,
					Beehive.currentMaxInstanceID
				});
			}
			if (!base.HasData(DataEntryKey.SpawnedBees) && this.spawnBees)
			{
				this.currentBees = PhotonNetwork.Instantiate(this.beeSwarmPrefab.gameObject.name, base.transform.position, Quaternion.identity, 0, null).GetComponent<BeeSwarm>();
				this.currentBees.SetBeehive(this);
				base.GetData<BoolItemData>(DataEntryKey.SpawnedBees).Value = true;
			}
		}
	}

	// Token: 0x06000021 RID: 33 RVA: 0x000025E5 File Offset: 0x000007E5
	public override void OnInstanceDataSet()
	{
		this.instanceID = base.GetData<IntItemData>(DataEntryKey.InstanceID).Value;
	}

	// Token: 0x06000022 RID: 34 RVA: 0x000025F9 File Offset: 0x000007F9
	[PunRPC]
	public void SetInstanceIDRPC(int instanceID, int maxInstanceID)
	{
		base.GetData<IntItemData>(DataEntryKey.InstanceID).Value = instanceID;
		Beehive.currentMaxInstanceID = maxInstanceID;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x0000260E File Offset: 0x0000080E
	public override void OnEnable()
	{
		base.OnEnable();
		Beehive.ALL_BEEHIVES.Add(this);
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002621 File Offset: 0x00000821
	public override void OnDisable()
	{
		base.OnDisable();
		Beehive.ALL_BEEHIVES.Remove(this);
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00002638 File Offset: 0x00000838
	public static Beehive GetBeehive(int instanceID)
	{
		for (int i = 0; i < Beehive.ALL_BEEHIVES.Count; i++)
		{
			if (Beehive.ALL_BEEHIVES[i] != null && Beehive.ALL_BEEHIVES[i].instanceID == instanceID)
			{
				return Beehive.ALL_BEEHIVES[i];
			}
		}
		return null;
	}

	// Token: 0x06000026 RID: 38 RVA: 0x0000268D File Offset: 0x0000088D
	private void OnDestroy()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.currentBees != null)
		{
			this.currentBees.HiveDestroyed(this.item.Center());
		}
	}

	// Token: 0x04000007 RID: 7
	public bool spawnBees = true;

	// Token: 0x04000008 RID: 8
	public BeeSwarm beeSwarmPrefab;

	// Token: 0x04000009 RID: 9
	public BeeSwarm currentBees;

	// Token: 0x0400000A RID: 10
	public int instanceID;

	// Token: 0x0400000B RID: 11
	private static int currentMaxInstanceID = 1;

	// Token: 0x0400000C RID: 12
	public static List<Beehive> ALL_BEEHIVES = new List<Beehive>();

	// Token: 0x0400000D RID: 13
	private bool initialized;
}
