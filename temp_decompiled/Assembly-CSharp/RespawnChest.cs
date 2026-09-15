using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000312 RID: 786
public class RespawnChest : Luggage, IInteractible
{
	// Token: 0x17000142 RID: 322
	// (get) Token: 0x0600144F RID: 5199 RVA: 0x000675A1 File Offset: 0x000657A1
	// (set) Token: 0x06001450 RID: 5200 RVA: 0x000675A9 File Offset: 0x000657A9
	public Segment SegmentNumber { get; set; }

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06001451 RID: 5201 RVA: 0x000675B2 File Offset: 0x000657B2
	// (set) Token: 0x06001452 RID: 5202 RVA: 0x000675BA File Offset: 0x000657BA
	public bool IsSpent { get; private set; }

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06001453 RID: 5203 RVA: 0x000675C4 File Offset: 0x000657C4
	// (remove) Token: 0x06001454 RID: 5204 RVA: 0x000675FC File Offset: 0x000657FC
	public event Action<RespawnChest> ReviveUsed;

	// Token: 0x06001455 RID: 5205 RVA: 0x00067631 File Offset: 0x00065831
	public override string GetInteractionText()
	{
		if (Character.PlayerIsDeadOrDown())
		{
			return LocalizedText.GetText("REVIVESCOUTS", true);
		}
		return LocalizedText.GetText("TOUCH", true);
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x00067651 File Offset: 0x00065851
	private void DebugSpawn()
	{
		this.SpawnItems(this.GetSpawnSpots());
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x00067660 File Offset: 0x00065860
	public override void Interact(Character interactor)
	{
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x00067662 File Offset: 0x00065862
	public override void Interact_CastFinished(Character interactor)
	{
		base.Interact_CastFinished(interactor);
		GlobalEvents.TriggerRespawnChestOpened(this, interactor);
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x00067674 File Offset: 0x00065874
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		this.IsSpent = true;
		if (Ascents.canReviveDead && Character.PlayerIsDeadOrDown())
		{
			base.photonView.RPC("RemoveSkeletonRPC", 3, Array.Empty<object>());
			this.RespawnAllPlayersHere();
		}
		else
		{
			base.SpawnItems(spawnSpots);
		}
		return list;
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x000676CC File Offset: 0x000658CC
	[PunRPC]
	private void RemoveSkeletonRPC()
	{
		this.skeleton.SetActive(false);
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x0600145B RID: 5211 RVA: 0x000676DA File Offset: 0x000658DA
	public Vector3 RandomRevivePoint
	{
		get
		{
			return base.transform.position + base.transform.up * 6f + Random.onUnitSphere;
		}
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x0006770C File Offset: 0x0006590C
	private void RespawnAllPlayersHere()
	{
		Action<RespawnChest> reviveUsed = this.ReviveUsed;
		if (reviveUsed != null)
		{
			reviveUsed.Invoke(this);
		}
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.dead || character.data.fullyPassedOut)
			{
				character.photonView.RPC("RPCA_ReviveAtPosition", 0, new object[]
				{
					this.RandomRevivePoint,
					true,
					(int)this.SegmentNumber
				});
			}
		}
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x000677C0 File Offset: 0x000659C0
	public new bool IsInteractible(Character interactor)
	{
		return this.state == Luggage.LuggageState.Closed;
	}

	// Token: 0x040012EF RID: 4847
	public GameObject skeleton;
}
