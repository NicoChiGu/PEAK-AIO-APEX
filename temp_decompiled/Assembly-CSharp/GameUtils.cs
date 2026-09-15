using System;
using System.Collections.Generic;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000BB RID: 187
public class GameUtils : MonoBehaviourPunCallbacks
{
	// Token: 0x060006D8 RID: 1752 RVA: 0x00026F18 File Offset: 0x00025118
	private void Awake()
	{
		GameUtils.instance = this;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00026F2C File Offset: 0x0002512C
	public void StartFeed(int giverID, int receiverID, ushort itemID, float totalItemTime)
	{
		this.feedData.Add(new FeedData
		{
			giverID = giverID,
			receiverID = receiverID,
			itemID = itemID,
			totalItemTime = totalItemTime
		});
		Action onUpdatedFeedData = this.OnUpdatedFeedData;
		if (onUpdatedFeedData == null)
		{
			return;
		}
		onUpdatedFeedData.Invoke();
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00026F6C File Offset: 0x0002516C
	public List<FeedData> GetFeedDataForReceiver(int receiverID)
	{
		return this.feedData.FindAll((FeedData x) => x.receiverID == receiverID);
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x00026FA0 File Offset: 0x000251A0
	public void EndFeed(int giverID)
	{
		for (int i = this.feedData.Count - 1; i >= 0; i--)
		{
			if (this.feedData[i].giverID == giverID)
			{
				this.feedData.RemoveAt(i);
			}
		}
		Action onUpdatedFeedData = this.OnUpdatedFeedData;
		if (onUpdatedFeedData == null)
		{
			return;
		}
		onUpdatedFeedData.Invoke();
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00026FF5 File Offset: 0x000251F5
	private void FixedUpdate()
	{
		this.UpdateCollisionIgnores();
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00027000 File Offset: 0x00025200
	private void UpdateCollisionIgnores()
	{
		for (int i = this.ignoredCollidersCache.Count - 1; i >= 0; i--)
		{
			this.ignoredCollidersCache[i].time -= Time.fixedDeltaTime;
			if (this.ignoredCollidersCache[i].time <= 0f)
			{
				if (this.ignoredCollidersCache[i].colliderA != null && this.ignoredCollidersCache[i].colliderB != null)
				{
					Physics.IgnoreCollision(this.ignoredCollidersCache[i].colliderA, this.ignoredCollidersCache[i].colliderB, false);
				}
				this.ignoredCollidersCache.RemoveAt(i);
			}
		}
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x000270C8 File Offset: 0x000252C8
	public void IgnoreCollisions(GameObject object1, GameObject object2, float time)
	{
		Collider[] componentsInChildren = object1.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = object2.GetComponentsInChildren<Collider>();
		this.IgnoreCollisions(componentsInChildren, componentsInChildren2, time);
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x000270EC File Offset: 0x000252EC
	public void IgnoreCollisions(Character c, Item item, float time)
	{
		foreach (Collider collider in item.GetComponentsInChildren<Collider>())
		{
			foreach (Collider collider2 in c.refs.ragdoll.colliderList)
			{
				Physics.IgnoreCollision(collider2, collider);
				this.ignoredCollidersCache.Add(new GameUtils.IgnoredCollidersEntry(collider2, collider, time));
			}
		}
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00027178 File Offset: 0x00025378
	public void IgnoreCollisions(Collider[] collidersA, Collider[] collidersB, float time)
	{
		foreach (Collider collider in collidersA)
		{
			foreach (Collider collider2 in collidersB)
			{
				Physics.IgnoreCollision(collider, collider2);
				this.ignoredCollidersCache.Add(new GameUtils.IgnoredCollidersEntry(collider, collider2, time));
			}
		}
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x000271D0 File Offset: 0x000253D0
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (NetCode.Session.IsHost)
		{
			if (!NetCode.Matchmaking.PlayerIsInLobby(newPlayer.UserId))
			{
				Debug.LogError(newPlayer.NickName + " (" + newPlayer.UserId + ") is not in our Steam lobby. That's too sussy to allow. Kicking them.");
				NetCode.Session.Kick(newPlayer.UserId);
				return;
			}
			GameHandler.GetService<PersistentPlayerDataService>().SyncToPlayer(newPlayer);
			this.photonView.RPC("RPC_SyncAscent", newPlayer, new object[] { Ascents.currentAscent });
		}
		GlobalEvents.TriggerPlayerConnected(newPlayer);
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00027268 File Offset: 0x00025468
	public override void OnPlayerLeftRoom(Player newPlayer)
	{
		base.OnPlayerLeftRoom(newPlayer);
		GlobalEvents.TriggerPlayerDisconnected(newPlayer);
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00027277 File Offset: 0x00025477
	internal void SyncAscentAll(int ascent)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.photonView.RPC("RPC_SyncAscent", 0, new object[] { ascent });
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x000272A1 File Offset: 0x000254A1
	[PunRPC]
	internal void RPC_SyncAscent(int ascent)
	{
		Ascents.currentAscent = ascent;
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x000272A9 File Offset: 0x000254A9
	[PunRPC]
	internal void RPC_WarpToSegment(int segment)
	{
		Debug.LogWarning("A naughty player attempted to use debug commands to warp everyone somewhere. That's not allowed!");
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x000272B5 File Offset: 0x000254B5
	internal void ThrowBingBongAchievement()
	{
		this.photonView.RPC("ThrowBingBongAchievementRpc", 0, Array.Empty<object>());
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x000272CD File Offset: 0x000254CD
	[PunRPC]
	private void ThrowBingBongAchievementRpc()
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.BingBongBadge);
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x000272DB File Offset: 0x000254DB
	internal void ThrowSacrificeAchievement()
	{
		this.photonView.RPC("ThrowSacrificeAchievementRpc", 0, Array.Empty<object>());
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x000272F3 File Offset: 0x000254F3
	[PunRPC]
	private void ThrowSacrificeAchievementRpc()
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.TwentyFourKaratBadge);
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00027301 File Offset: 0x00025501
	internal void IncrementPermanentItemsPlaced()
	{
		this.photonView.RPC("IncrementPermanentItemsPlacedRpc", 0, Array.Empty<object>());
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x00027319 File Offset: 0x00025519
	[PunRPC]
	private void IncrementPermanentItemsPlacedRpc()
	{
		Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.PermanentItemsPlaced, 1);
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x00027327 File Offset: 0x00025527
	internal void IncrementFriendHealing(int amt, Player target)
	{
		this.photonView.RPC("IncrementFriendHealingRpc", target, new object[] { amt });
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00027349 File Offset: 0x00025549
	[PunRPC]
	private void IncrementFriendHealingRpc(int amt)
	{
		Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.FriendsHealedAmount, amt);
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00027357 File Offset: 0x00025557
	internal void IncrementFriendPoisonHealing(int amt, Player target)
	{
		this.photonView.RPC("IncrementPoisonHealedStat", target, new object[] { amt });
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x00027379 File Offset: 0x00025579
	[PunRPC]
	protected void IncrementPoisonHealedStat(int amt)
	{
		Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PoisonHealed, amt);
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x00027388 File Offset: 0x00025588
	internal void ThrowEmergencyPreparednessAchievement(Player target)
	{
		this.photonView.RPC("ThrowEmergencyPreparednessAchievementRpc", target, Array.Empty<object>());
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x000273A0 File Offset: 0x000255A0
	[PunRPC]
	private void ThrowEmergencyPreparednessAchievementRpc()
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.EmergencyPreparednessBadge);
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x000273B0 File Offset: 0x000255B0
	[PunRPC]
	private void InstantiateAndGrabRPC(string itemPrefabName, PhotonView characterView, int cookedAmount)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		Character component = characterView.GetComponent<Character>();
		component.refs.items.lastEquippedSlotTime = 0f;
		Bodypart bodypart = component.GetBodypart(BodypartType.Hip);
		PhotonNetwork.InstantiateItemRoom(itemPrefabName, bodypart.transform.position + bodypart.transform.forward * 0.5f, Quaternion.identity).GetComponent<Item>().Interact(component);
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00027424 File Offset: 0x00025624
	public void InstantiateAndGrab(Item item, Character character, int cookedAmount = 0)
	{
		this.photonView.RPC("InstantiateAndGrabRPC", 2, new object[]
		{
			item.gameObject.name,
			character.photonView,
			cookedAmount
		});
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x0002745D File Offset: 0x0002565D
	public void SyncLava(bool started, bool ended, float time, float timeWaited)
	{
		this.photonView.RPC("RPC_SyncLava", 1, new object[] { started, ended, time, timeWaited });
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x0002749B File Offset: 0x0002569B
	[PunRPC]
	public void RPC_SyncLava(bool started, bool ended, float time, float timeWaited)
	{
		Singleton<LavaRising>.Instance.RecieveLavaData(started, ended, time, timeWaited);
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x000274AC File Offset: 0x000256AC
	[ContextMenu("Debug All Items")]
	private void DebugAllItems()
	{
		string text = "";
		foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			text = text + keyValuePair.Value.UIData.itemName + "\n";
		}
		Debug.Log(text);
		text = "";
		foreach (KeyValuePair<ushort, Item> keyValuePair2 in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			text = text + keyValuePair2.Value.gameObject.name + "\n";
		}
		Debug.Log(text);
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x00027590 File Offset: 0x00025790
	public void SpawnResourceAtPositionNetworked(string resourcePath, Vector3 position, RpcTarget target)
	{
		this.photonView.RPC("RPC_SpawnResourceAtPosition", target, new object[] { resourcePath, position });
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x000275B6 File Offset: 0x000257B6
	[PunRPC]
	public void RPC_SpawnResourceAtPosition(string resourcePath, Vector3 pos)
	{
		GameObject gameObject = Resources.Load(resourcePath) as GameObject;
		if (gameObject == null)
		{
			Debug.Log("OBJECT WAS NULL");
		}
		Object.Instantiate<GameObject>(gameObject, pos, Quaternion.identity);
	}

	// Token: 0x040006DE RID: 1758
	public static GameUtils instance;

	// Token: 0x040006DF RID: 1759
	[SerializeField]
	public List<FeedData> feedData = new List<FeedData>();

	// Token: 0x040006E0 RID: 1760
	public Action OnUpdatedFeedData;

	// Token: 0x040006E1 RID: 1761
	internal PhotonView photonView;

	// Token: 0x040006E2 RID: 1762
	private List<GameUtils.IgnoredCollidersEntry> ignoredCollidersCache = new List<GameUtils.IgnoredCollidersEntry>();

	// Token: 0x02000439 RID: 1081
	private class IgnoredCollidersEntry
	{
		// Token: 0x06001A8A RID: 6794 RVA: 0x000806C7 File Offset: 0x0007E8C7
		public IgnoredCollidersEntry(Collider A, Collider B, float time)
		{
			this.colliderA = A;
			this.colliderB = B;
			this.time = time;
		}

		// Token: 0x04001824 RID: 6180
		public Collider colliderA;

		// Token: 0x04001825 RID: 6181
		public Collider colliderB;

		// Token: 0x04001826 RID: 6182
		public float time;
	}
}
