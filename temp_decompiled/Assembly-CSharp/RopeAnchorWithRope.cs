using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class RopeAnchorWithRope : MonoBehaviourPunCallbacks
{
	// Token: 0x06000B9E RID: 2974 RVA: 0x0003DF0D File Offset: 0x0003C10D
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.SpawnRope();
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x0003DF1C File Offset: 0x0003C11C
	public Rope SpawnRope()
	{
		if (!base.photonView.IsMine)
		{
			return null;
		}
		this.ropeInstance = PhotonNetwork.Instantiate(this.ropePrefab.name, this.anchor.anchorPoint.position, this.anchor.anchorPoint.rotation, 0, null);
		this.rope = this.ropeInstance.GetComponent<Rope>();
		this.rope.Segments = this.ropeSegmentLength;
		this.rope.photonView.RPC("AttachToAnchor_Rpc", 3, new object[]
		{
			this.anchor.photonView,
			this.ropeSegmentLength
		});
		base.StartCoroutine(this.<SpawnRope>g__SpoolOut|7_0());
		return this.rope;
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x0003DFDD File Offset: 0x0003C1DD
	public virtual void Awake()
	{
		this.anchor = base.GetComponent<RopeAnchor>();
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x0003E009 File Offset: 0x0003C209
	[CompilerGenerated]
	private IEnumerator <SpawnRope>g__SpoolOut|7_0()
	{
		float elapsed = 0f;
		while (elapsed < this.spoolOutTime)
		{
			elapsed += Time.deltaTime;
			this.rope.Segments = Mathf.Lerp(0f, this.ropeSegmentLength, ExtDataTypes.Clamp01(elapsed / this.spoolOutTime));
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000AC9 RID: 2761
	public float ropeSegmentLength = 20f;

	// Token: 0x04000ACA RID: 2762
	public float spoolOutTime = 5f;

	// Token: 0x04000ACB RID: 2763
	public GameObject ropePrefab;

	// Token: 0x04000ACC RID: 2764
	public GameObject ropeInstance;

	// Token: 0x04000ACD RID: 2765
	public RopeAnchor anchor;

	// Token: 0x04000ACE RID: 2766
	public Rope rope;
}
