using System;
using System.Collections.Generic;
using System.Linq;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200016D RID: 365
[DefaultExecutionOrder(100000)]
public class Rope : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x06000B76 RID: 2934 RVA: 0x0003D0FA File Offset: 0x0003B2FA
	// (set) Token: 0x06000B77 RID: 2935 RVA: 0x0003D102 File Offset: 0x0003B302
	public float Segments
	{
		get
		{
			return this.segments;
		}
		set
		{
			this.segments = Mathf.Clamp(value, 0f, (float)Rope.MaxSegments);
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x06000B78 RID: 2936 RVA: 0x0003D11B File Offset: 0x0003B31B
	public static int MaxSegments
	{
		get
		{
			return 40;
		}
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x06000B79 RID: 2937 RVA: 0x0003D11F File Offset: 0x0003B31F
	public int SegmentCount
	{
		get
		{
			if (base.photonView.IsMine)
			{
				return this.simulationSegments.Count;
			}
			return this.remoteColliderSegments.Count;
		}
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x0003D145 File Offset: 0x0003B345
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient)
		{
			this.view.RPC("OnRejoinSyncRPC", newPlayer, new object[] { this.attachmenState });
		}
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x0003D17A File Offset: 0x0003B37A
	[PunRPC]
	public void OnRejoinSyncRPC(Rope.ATTACHMENT attachmentState)
	{
		this.attachmenState = attachmentState;
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x0003D183 File Offset: 0x0003B383
	private void Awake()
	{
		this.itemSpool = base.GetComponentInParent<Item>();
		this.climbingAPI = base.GetComponent<RopeClimbingAPI>();
		this.view = base.GetComponent<PhotonView>();
		this.ropeBoneVisualizer = base.GetComponentInChildren<RopeBoneVisualizer>();
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x0003D1B8 File Offset: 0x0003B3B8
	private void Update()
	{
		bool flag;
		switch (this.attachmenState)
		{
		case Rope.ATTACHMENT.unattached:
			flag = false;
			break;
		case Rope.ATTACHMENT.inSpool:
			flag = false;
			break;
		case Rope.ATTACHMENT.anchored:
			flag = true;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.isClimbable = flag;
		if (!base.photonView.IsMine || this.creatorLeft)
		{
			if (this.simulationSegments.Count > 0)
			{
				this.Clear(false);
			}
			return;
		}
		this.timeSinceRemoved += Time.deltaTime;
		int num = Mathf.Clamp(Mathf.FloorToInt(this.Segments), 1, int.MaxValue);
		if (this.simulationSegments.Count > num)
		{
			if (this.simulationSegments.Count > 1)
			{
				this.RemoveSegment();
			}
		}
		else if (this.simulationSegments.Count < num)
		{
			this.AddSegment();
		}
		if (this.simulationSegments.Count > 1)
		{
			float num2 = this.Segments % 1f;
			List<Transform> list = this.simulationSegments;
			ConfigurableJoint component = list[list.Count - 1].GetComponent<ConfigurableJoint>();
			Vector3 vector = Vector3.Lerp(this.startAnchorOf2ndSegment, -ExtSwizzle.oxo(this.spacing), Mathf.Clamp01(this.timeSinceRemoved / this.slurpTime));
			component.connectedAnchor = Vector3.Lerp(ExtSwizzle.oxo(this.spacing), vector, num2);
			component.GetComponent<Collider>().enabled = true;
		}
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x0003D30C File Offset: 0x0003B50C
	private void FixedUpdate()
	{
		if (!base.photonView.IsMine || this.creatorLeft)
		{
			return;
		}
		if (this.antigrav)
		{
			using (List<Transform>.Enumerator enumerator = this.simulationSegments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Transform transform = enumerator.Current;
					transform.GetComponent<Rigidbody>().AddForce(-Physics.gravity * 2f, 5);
				}
				return;
			}
		}
		foreach (Character character in this.charactersClimbing)
		{
			float ropePercent = character.data.ropePercent;
			this.climbingAPI.GetSegmentFromPercent(ropePercent).GetComponent<Rigidbody>().AddForce(Vector3.down * this.climberGravity, 5);
		}
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x0003D400 File Offset: 0x0003B600
	public override void OnEnable()
	{
		base.OnEnable();
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x0003D40E File Offset: 0x0003B60E
	public override void OnDisable()
	{
		base.OnDisable();
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x0003D41C File Offset: 0x0003B61C
	public List<Transform> GetRopeSegments()
	{
		if (base.photonView.IsMine)
		{
			return this.simulationSegments;
		}
		return this.remoteColliderSegments;
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x0003D438 File Offset: 0x0003B638
	public bool IsActive()
	{
		bool flag = true;
		if (this.itemSpool != null && this.itemSpool.itemState != ItemState.Held)
		{
			flag = false;
		}
		return flag;
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x0003D468 File Offset: 0x0003B668
	[PunRPC]
	public void Detach_Rpc(float segmentLength)
	{
		if (this.spool != null)
		{
			Debug.Log(string.Format("Detaching {0} of rope from spool", segmentLength));
			this.spool.ropeInstance = null;
			this.spool.rope = null;
			this.spool.Segments = 0f;
			this.spool.ClearRope();
			if (base.photonView.IsMine && !Mathf.Approximately(segmentLength, this.segments))
			{
				Debug.LogWarning(string.Format("We own this rope and it should be {0} long but RPC says it's {1}", this.segments, segmentLength));
			}
			if (this.HasAuthority())
			{
				this.spool.RopeFuel -= segmentLength;
			}
		}
		if (this.view.IsMine)
		{
			Object.DestroyImmediate(Enumerable.First<Transform>(this.simulationSegments).GetComponent<ConfigurableJoint>());
		}
		this.spool = null;
		this.attachmenState = Rope.ATTACHMENT.unattached;
		Debug.Log(string.Format("Detach_Rpc: {0}", this.attachmenState));
		this.ropeBoneVisualizer.StartTransform = null;
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x0003D57A File Offset: 0x0003B77A
	public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x0003D57C File Offset: 0x0003B77C
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		this.creatorLeft = true;
		Debug.Log(string.Format("OnMasterClientSwitched: {0}, isMaster: {1}, frame: {2}", newMasterClient, PhotonNetwork.IsMasterClient, Time.frameCount));
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x0003D5B0 File Offset: 0x0003B7B0
	public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		if (targetView != base.photonView)
		{
			return;
		}
		Debug.Log("Trasfered ownership to me");
		this.creatorLeft = true;
		if (this.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			Debug.Log(string.Format("attached to spool, deleting rope: {0}", this.view));
			PhotonNetwork.Destroy(this.view);
		}
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x0003D606 File Offset: 0x0003B806
	public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0003D608 File Offset: 0x0003B808
	[PunRPC]
	public void AttachToAnchor_Rpc(PhotonView anchorView, float ropeLength)
	{
		if (this.ropeBoneVisualizer == null)
		{
			this.ropeBoneVisualizer = base.GetComponentInChildren<RopeBoneVisualizer>();
		}
		if (this.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			this.Detach_Rpc(ropeLength);
		}
		this.attachedToAnchor = anchorView.GetComponent<RopeAnchor>();
		this.attachmenState = Rope.ATTACHMENT.anchored;
		Debug.Log(string.Format("AttachToAnchor_Rpc: {0}", this.attachmenState));
		this.ropeBoneVisualizer.StartTransform = this.attachedToAnchor.anchorPoint;
		if (!base.photonView.IsMine)
		{
			return;
		}
		List<Transform> ropeSegments = this.GetRopeSegments();
		if (ropeSegments.Count > 0)
		{
			ropeSegments[0].GetComponent<RopeSegment>().Tie(this.attachedToAnchor.anchorPoint.position);
		}
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x0003D6C1 File Offset: 0x0003B8C1
	public float GetLengthInMeters()
	{
		return Rope.GetLengthInMeters((float)this.GetRopeSegments().Count);
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x0003D6D4 File Offset: 0x0003B8D4
	public static float GetLengthInMeters(float segmentCount)
	{
		return segmentCount * 0.25f;
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x0003D6E0 File Offset: 0x0003B8E0
	[PunRPC]
	public void AttachToSpool_Rpc(PhotonView viewSpool)
	{
		this.spool = viewSpool.GetComponent<RopeSpool>();
		if (this.spool == null)
		{
			Debug.LogError("Spool is null");
			return;
		}
		this.spool.ropeInstance = base.gameObject;
		this.spool.rope = this;
		this.ropeBoneVisualizer.StartTransform = this.spool.ropeStart;
		base.transform.position = this.spool.ropeBase.position;
		base.transform.rotation = this.spool.ropeBase.rotation;
		this.attachmenState = Rope.ATTACHMENT.inSpool;
		Debug.Log(string.Format("AttachToSpool_Rpc: {0}", this.attachmenState));
		this.Segments = 0f;
		Physics.SyncTransforms();
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x0003D7AC File Offset: 0x0003B9AC
	public void AddSegment()
	{
		bool flag = this.simulationSegments.Count == 0;
		Transform transform = null;
		if (!flag)
		{
			transform = this.simulationSegments[0];
		}
		Vector3 vector = (flag ? base.transform.position : transform.transform.position);
		Quaternion quaternion = (flag ? base.transform.rotation : transform.transform.rotation);
		GameObject gameObject = Object.Instantiate<GameObject>(this.ropeSegmentPrefab, vector, quaternion, base.transform);
		gameObject.gameObject.name = "RopeSegment: " + this.simulationSegments.Count.ToString();
		ConfigurableJoint component = gameObject.GetComponent<ConfigurableJoint>();
		if (flag)
		{
			component.autoConfigureConnectedAnchor = true;
			if (this.spool != null)
			{
				component.transform.position = this.spool.ropeBase.position;
				component.transform.rotation = this.spool.ropeBase.rotation;
				component.autoConfigureConnectedAnchor = true;
				component.connectedBody = this.spool.rig;
				component.angularXMotion = 1;
				ConfigurableJoint configurableJoint = component;
				SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
				softJointLimitSpring.spring = 35f;
				softJointLimitSpring.damper = 45f;
				configurableJoint.angularXLimitSpring = softJointLimitSpring;
				ConfigurableJoint configurableJoint2 = component;
				softJointLimitSpring = default(SoftJointLimitSpring);
				softJointLimitSpring.spring = 35f;
				softJointLimitSpring.damper = 45f;
				configurableJoint2.angularYZLimitSpring = softJointLimitSpring;
				component.angularZMotion = 1;
			}
		}
		else
		{
			component.connectedBody = transform.GetComponent<Rigidbody>();
		}
		this.simulationSegments.Add(gameObject.transform);
		if (this.simulationSegments.Count > 2)
		{
			List<Transform> list = this.simulationSegments;
			Component component2 = list[list.Count - 2];
			Rigidbody component3 = gameObject.GetComponent<Rigidbody>();
			ConfigurableJoint component4 = component2.GetComponent<ConfigurableJoint>();
			component4.connectedBody = component3;
			this.startAnchorOf2ndSegment = new Vector3(0f, -this.spacing, 0f);
			component4.connectedAnchor = this.startAnchorOf2ndSegment;
		}
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x0003D9A8 File Offset: 0x0003BBA8
	private void RemoveSegment()
	{
		List<Transform> list = this.simulationSegments;
		Transform transform = list[list.Count - 1];
		List<Transform> list2 = this.simulationSegments;
		Transform transform2 = list2[list2.Count - 2];
		Transform transform3 = this.simulationSegments[0];
		Object.DestroyImmediate(transform.gameObject);
		ExtCollections.RemoveLast<Transform>(this.simulationSegments);
		ConfigurableJoint component = transform2.GetComponent<ConfigurableJoint>();
		if (transform2 == transform3)
		{
			Debug.LogError("Attempting to connect joint to itself");
			return;
		}
		this.timeSinceRemoved = 0f;
		component.connectedBody = transform3.GetComponent<Rigidbody>();
		this.startAnchorOf2ndSegment = transform3.InverseTransformPoint(component.transform.position);
		component.connectedAnchor = this.startAnchorOf2ndSegment;
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x0003DA54 File Offset: 0x0003BC54
	public RopeSyncData GetSyncData()
	{
		RopeSyncData ropeSyncData = new RopeSyncData
		{
			isVisible = this.isClimbable,
			segments = new RopeSyncData.SegmentData[this.simulationSegments.Count]
		};
		for (int i = 0; i < this.simulationSegments.Count; i++)
		{
			ropeSyncData.segments[i] = new RopeSyncData.SegmentData
			{
				position = this.simulationSegments[i].position,
				rotation = this.simulationSegments[i].rotation
			};
		}
		return ropeSyncData;
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x0003DAF0 File Offset: 0x0003BCF0
	public void SetSyncData(RopeSyncData data)
	{
		if (data.updateVisualizerManually)
		{
			this.ropeBoneVisualizer.ManuallyUpdateNextFrame = Optionable<bool>.Some(true);
		}
		if (this.creatorLeft)
		{
			return;
		}
		this.isClimbable = data.isVisible;
		int num = data.segments.Length;
		int count = this.remoteColliderSegments.Count;
		if (num < count)
		{
			int num2 = count - num;
			for (int i = 0; i < num2; i++)
			{
				List<Transform> list = this.remoteColliderSegments;
				Component component = list[list.Count - 1];
				ExtCollections.RemoveLast<Transform>(this.remoteColliderSegments);
				Object.Destroy(component.gameObject);
			}
		}
		else if (num > count)
		{
			int num3 = num - count;
			for (int j = 0; j < num3; j++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.remoteSegmentPrefab, Vector3.zero, Quaternion.identity, base.transform);
				gameObject.GetComponent<RopeSegment>().rope = this;
				this.remoteColliderSegments.Add(gameObject.transform);
			}
		}
		if (num != this.remoteColliderSegments.Count)
		{
			Debug.LogError("Remote Segment Logic Failed");
			return;
		}
		for (int k = 0; k < data.segments.Length; k++)
		{
			this.remoteColliderSegments[k].position = data.segments[k].position;
			this.remoteColliderSegments[k].rotation = data.segments[k].rotation;
		}
		this.ropeBoneVisualizer.SetData(data);
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x0003DC5D File Offset: 0x0003BE5D
	public float GetTotalLength()
	{
		return (float)this.SegmentCount * this.spacing;
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x0003DC70 File Offset: 0x0003BE70
	public void Clear(bool alsoRemoveRemote = false)
	{
		Debug.Log("Rope Clear!");
		if (this.simulationSegments.Count > 0)
		{
			for (int i = this.simulationSegments.Count - 1; i >= 0; i--)
			{
				Object.Destroy(this.simulationSegments[i].gameObject);
			}
			this.simulationSegments.Clear();
		}
		if (alsoRemoveRemote)
		{
			for (int j = this.remoteColliderSegments.Count - 1; j >= 0; j--)
			{
				Object.Destroy(this.remoteColliderSegments[j].gameObject);
			}
			this.remoteColliderSegments.Clear();
		}
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x0003DD0A File Offset: 0x0003BF0A
	public void AddCharacterClimbing(Character character)
	{
		this.charactersClimbing.Add(character);
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x0003DD18 File Offset: 0x0003BF18
	public void RemoveCharacterClimbing(Character character)
	{
		this.charactersClimbing.Remove(character);
	}

	// Token: 0x04000AA6 RID: 2726
	public float spacing = 0.75f;

	// Token: 0x04000AA7 RID: 2727
	public float climberGravity = 1f;

	// Token: 0x04000AA8 RID: 2728
	public float slurpTime = 10f;

	// Token: 0x04000AA9 RID: 2729
	public bool antigrav;

	// Token: 0x04000AAA RID: 2730
	public bool isHelicopterRope;

	// Token: 0x04000AAB RID: 2731
	public GameObject ropeSegmentPrefab;

	// Token: 0x04000AAC RID: 2732
	public GameObject remoteSegmentPrefab;

	// Token: 0x04000AAD RID: 2733
	public Rope.ATTACHMENT attachmenState;

	// Token: 0x04000AAE RID: 2734
	public bool isClimbable;

	// Token: 0x04000AAF RID: 2735
	public PhotonView view;

	// Token: 0x04000AB0 RID: 2736
	private readonly List<Transform> remoteColliderSegments = new List<Transform>();

	// Token: 0x04000AB1 RID: 2737
	private readonly List<Transform> simulationSegments = new List<Transform>();

	// Token: 0x04000AB2 RID: 2738
	[NonSerialized]
	public List<Character> charactersClimbing = new List<Character>();

	// Token: 0x04000AB3 RID: 2739
	[NonSerialized]
	public RopeClimbingAPI climbingAPI;

	// Token: 0x04000AB4 RID: 2740
	private Item itemSpool;

	// Token: 0x04000AB5 RID: 2741
	private RopeBoneVisualizer ropeBoneVisualizer;

	// Token: 0x04000AB6 RID: 2742
	private float segments;

	// Token: 0x04000AB7 RID: 2743
	private RopeSpool spool;

	// Token: 0x04000AB8 RID: 2744
	private Vector3 startAnchorOf2ndSegment;

	// Token: 0x04000AB9 RID: 2745
	private float timeSinceRemoved;

	// Token: 0x04000ABA RID: 2746
	public bool creatorLeft;

	// Token: 0x04000ABB RID: 2747
	private RopeAnchor attachedToAnchor;

	// Token: 0x0200047F RID: 1151
	public enum ATTACHMENT
	{
		// Token: 0x04001956 RID: 6486
		unattached,
		// Token: 0x04001957 RID: 6487
		inSpool,
		// Token: 0x04001958 RID: 6488
		anchored
	}
}
