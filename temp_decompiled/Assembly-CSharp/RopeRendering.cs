using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using pworld.Scripts.Extensions;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class RopeRendering : MonoBehaviour
{
	// Token: 0x06000BBA RID: 3002 RVA: 0x0003EACD File Offset: 0x0003CCCD
	private void Awake()
	{
		this.view = base.GetComponentInParent<PhotonView>();
		this.rope = base.GetComponent<Rope>();
		this.lineRenderer = base.GetComponentInChildren<LineRenderer>();
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x0003EAF4 File Offset: 0x0003CCF4
	private void LateUpdate()
	{
		if (this.hide)
		{
			return;
		}
		if (this.targetPoints.Count != this.remoteRenderingPoints.Count)
		{
			Debug.LogError("Target points count mismatch");
			return;
		}
		float num = 1f / (float)PhotonNetwork.SerializationRate;
		this.sinceLastPackage += Time.deltaTime;
		float num2 = this.sinceLastPackage / num;
		for (int i = 0; i < this.remoteRenderingPoints.Count; i++)
		{
			Vector3 vector = Vector3.Lerp(this.remoteRenderingPoints[i], this.targetPoints[i], num2 * 0.5f);
			this.remoteRenderingPoints[i] = vector;
		}
		List<float3> list;
		if (!this.view.IsMine)
		{
			list = this.remoteRenderingPoints;
		}
		else
		{
			list = Enumerable.ToList<float3>(Enumerable.Select<Transform, float3>(this.rope.GetRopeSegments(), (Transform transform1) => transform1.position));
		}
		List<float3> list2 = list;
		List<Vector3> list3 = new List<Vector3>();
		if (this.startPos != null && this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			list3.Add(this.startPos.position);
		}
		if (list2.Count > 0)
		{
			list3.Add(list2[0]);
		}
		for (int j = list2.Count - 1; j >= 1; j--)
		{
			list3.Add(list2[j]);
		}
		this.lineRenderer.positionCount = list3.Count;
		this.lineRenderer.SetPositions(list3.ToArray());
		if (this.rope.IsActive() && list3.Count > 1)
		{
			this.lineRenderer.enabled = true;
			return;
		}
		this.lineRenderer.enabled = false;
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0003ECCC File Offset: 0x0003CECC
	public void SetData(RopeSyncData data)
	{
		this.sinceLastPackage = 0f;
		this.targetPoints = Enumerable.ToList<float3>(Enumerable.Select<RopeSyncData.SegmentData, float3>(data.segments, (RopeSyncData.SegmentData segmentData) => segmentData.position));
		int num = data.segments.Length;
		int count = this.remoteRenderingPoints.Count;
		if (num < count)
		{
			int num2 = count - num;
			for (int i = 0; i < num2; i++)
			{
				ExtCollections.RemoveLast<float3>(this.remoteRenderingPoints);
			}
			return;
		}
		if (num > count)
		{
			int num3 = num - count;
			for (int j = 0; j < num3; j++)
			{
				int num4 = count + j;
				this.remoteRenderingPoints.Add(data.segments[num4].position);
			}
		}
	}

	// Token: 0x04000ADF RID: 2783
	public Transform startPos;

	// Token: 0x04000AE0 RID: 2784
	public LineRenderer lineRenderer;

	// Token: 0x04000AE1 RID: 2785
	private Rope rope;

	// Token: 0x04000AE2 RID: 2786
	private PhotonView view;

	// Token: 0x04000AE3 RID: 2787
	private List<float3> remoteRenderingPoints = new List<float3>();

	// Token: 0x04000AE4 RID: 2788
	private List<float3> targetPoints = new List<float3>();

	// Token: 0x04000AE5 RID: 2789
	private float sinceLastPackage;

	// Token: 0x04000AE6 RID: 2790
	public bool hide = true;
}
