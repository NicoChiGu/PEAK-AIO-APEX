using System;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class TrackNetworkedObject : MonoBehaviour
{
	// Token: 0x06000D80 RID: 3456 RVA: 0x00043A38 File Offset: 0x00041C38
	private void OnEnable()
	{
		TrackableNetworkObject.OnTrackableObjectCreated = (Action<int>)Delegate.Combine(TrackableNetworkObject.OnTrackableObjectCreated, new Action<int>(this.TryReattachToTrackedObject));
		TrackableNetworkObject.OnTrackableObjectConsumed = (Action<int>)Delegate.Combine(TrackableNetworkObject.OnTrackableObjectConsumed, new Action<int>(this.TryConsumeTrackedObject));
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x00043A88 File Offset: 0x00041C88
	private void OnDisable()
	{
		TrackableNetworkObject.OnTrackableObjectCreated = (Action<int>)Delegate.Remove(TrackableNetworkObject.OnTrackableObjectCreated, new Action<int>(this.TryReattachToTrackedObject));
		TrackableNetworkObject.OnTrackableObjectConsumed = (Action<int>)Delegate.Remove(TrackableNetworkObject.OnTrackableObjectConsumed, new Action<int>(this.TryConsumeTrackedObject));
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x00043AD5 File Offset: 0x00041CD5
	private void TryReattachToTrackedObject(int ID)
	{
		this.TryGetTrackedObject();
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x00043ADD File Offset: 0x00041CDD
	private void TryConsumeTrackedObject(int ID)
	{
		if (this.trackedObjectID == ID)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00043AF4 File Offset: 0x00041CF4
	private void TryGetTrackedObject()
	{
		if (this.trackedObjectID == -1)
		{
			Debug.LogError("TrackNetworkObject has a value of -1. This should never happen.");
			base.enabled = false;
			return;
		}
		TrackableNetworkObject trackableObject = TrackableNetworkObject.GetTrackableObject(this.trackedObjectID);
		if (trackableObject != null)
		{
			this.SetObject(trackableObject);
			this.lostTrackableTick = 0f;
			return;
		}
		this.lostTrackableTick += Time.deltaTime;
		if (this.lostTrackableTick > 0.3f)
		{
			Debug.Log(string.Format("Object {0} timed out. Destroying...", base.gameObject.GetHashCode()));
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x00043B90 File Offset: 0x00041D90
	public void SetObject(TrackableNetworkObject trackableObject)
	{
		this.trackedObject = trackableObject;
		this.trackedObjectID = trackableObject.instanceID;
		this.trackedObject.currentTracker = this;
		Debug.Log(string.Format("Object {0} Reconnected to trackable object {1} with photon ID {2}", base.gameObject.GetHashCode(), this.trackedObjectID, trackableObject.photonView.ViewID));
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x00043BF8 File Offset: 0x00041DF8
	private void LateUpdate()
	{
		if (this.trackedObject == null)
		{
			this.TryGetTrackedObject();
		}
		if (this.trackedObject != null)
		{
			base.transform.rotation = this.trackedObject.transform.rotation;
			base.transform.position = this.trackedObject.transform.TransformPoint(this.offset);
		}
	}

	// Token: 0x04000BA4 RID: 2980
	public int trackedObjectID = -1;

	// Token: 0x04000BA5 RID: 2981
	public TrackableNetworkObject trackedObject;

	// Token: 0x04000BA6 RID: 2982
	public Vector3 offset;

	// Token: 0x04000BA7 RID: 2983
	private float lostTrackableTick;
}
