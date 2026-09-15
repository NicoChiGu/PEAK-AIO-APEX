using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B4 RID: 436
[RequireComponent(typeof(PhotonView))]
public class TrackableNetworkObject : ItemComponent
{
	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000D70 RID: 3440 RVA: 0x00043746 File Offset: 0x00041946
	// (set) Token: 0x06000D71 RID: 3441 RVA: 0x0004374E File Offset: 0x0004194E
	public new PhotonView photonView { get; private set; }

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000D72 RID: 3442 RVA: 0x00043757 File Offset: 0x00041957
	public bool hasTracker
	{
		get
		{
			return this.currentTracker != null;
		}
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x00043765 File Offset: 0x00041965
	public override void Awake()
	{
		base.Awake();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x00043779 File Offset: 0x00041979
	private void OnEnable()
	{
		TrackableNetworkObject.ALL_TRACKABLES.Add(this);
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x000437A6 File Offset: 0x000419A6
	public void OnDisable()
	{
		TrackableNetworkObject.ALL_TRACKABLES.Remove(this);
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x000437D4 File Offset: 0x000419D4
	private void TestItemConsumed(Item consumedItem, Character consumer)
	{
		if (consumedItem == this.item && TrackableNetworkObject.OnTrackableObjectConsumed != null)
		{
			TrackableNetworkObject.OnTrackableObjectConsumed.Invoke(this.instanceID);
		}
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x000437FB File Offset: 0x000419FB
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		this.TryInitIfMaster();
		yield break;
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x0004380A File Offset: 0x00041A0A
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.TryInitIfMaster();
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00043818 File Offset: 0x00041A18
	private void TryInitIfMaster()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.Init();
		}
		if (TrackableNetworkObject.OnTrackableObjectCreated != null)
		{
			Debug.Log(string.Format("OnTrackableObjectCreated on photon view {0} with instance ID {1}", this.photonView.ViewID, this.instanceID));
			TrackableNetworkObject.OnTrackableObjectCreated.Invoke(this.instanceID);
		}
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x00043874 File Offset: 0x00041A74
	public static TrackableNetworkObject GetTrackableObject(int instanceID)
	{
		for (int i = 0; i < TrackableNetworkObject.ALL_TRACKABLES.Count; i++)
		{
			if (TrackableNetworkObject.ALL_TRACKABLES[i] != null && TrackableNetworkObject.ALL_TRACKABLES[i].instanceID == instanceID)
			{
				return TrackableNetworkObject.ALL_TRACKABLES[i];
			}
		}
		return null;
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x000438CC File Offset: 0x00041ACC
	private void Init()
	{
		if (!PhotonNetwork.InRoom)
		{
			Debug.Log("Couldn't init trackable network object " + base.gameObject.name + ": not in room.");
		}
		if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		int value = base.GetData<IntItemData>(DataEntryKey.InstanceID).Value;
		if (value != 0)
		{
			Debug.Log(string.Format("{0} already initialized with ID {1}", base.gameObject.name, value));
			return;
		}
		this.instanceID = TrackableNetworkObject.currentMaxInstanceID;
		TrackableNetworkObject.currentMaxInstanceID++;
		this.photonView.RPC("SetInstanceIDRPC", 0, new object[]
		{
			this.instanceID,
			TrackableNetworkObject.currentMaxInstanceID,
			this.autoSpawnTracker
		});
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00043996 File Offset: 0x00041B96
	public override void OnInstanceDataSet()
	{
		this.instanceID = base.GetData<IntItemData>(DataEntryKey.InstanceID).Value;
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x000439AC File Offset: 0x00041BAC
	[PunRPC]
	public void SetInstanceIDRPC(int instanceID, int maxInstanceID, bool autoSpawnTracker)
	{
		Debug.Log(string.Format("{0} ACTUALLY Setting instance id to {1}", base.gameObject.name, instanceID));
		base.GetData<IntItemData>(DataEntryKey.InstanceID).Value = instanceID;
		this.instanceID = instanceID;
		TrackableNetworkObject.currentMaxInstanceID = maxInstanceID;
		if (autoSpawnTracker)
		{
			Object.Instantiate<TrackNetworkedObject>(this.trackerToSpawn, base.transform.position, base.transform.rotation).SetObject(this);
		}
	}

	// Token: 0x04000B9B RID: 2971
	public static List<TrackableNetworkObject> ALL_TRACKABLES = new List<TrackableNetworkObject>();

	// Token: 0x04000B9C RID: 2972
	public int instanceID;

	// Token: 0x04000B9D RID: 2973
	private static int currentMaxInstanceID = 1;

	// Token: 0x04000B9E RID: 2974
	public TrackNetworkedObject currentTracker;

	// Token: 0x04000BA0 RID: 2976
	public static Action<int> OnTrackableObjectCreated;

	// Token: 0x04000BA1 RID: 2977
	public static Action<int> OnTrackableObjectConsumed;

	// Token: 0x04000BA2 RID: 2978
	public bool autoSpawnTracker;

	// Token: 0x04000BA3 RID: 2979
	public TrackNetworkedObject trackerToSpawn;
}
