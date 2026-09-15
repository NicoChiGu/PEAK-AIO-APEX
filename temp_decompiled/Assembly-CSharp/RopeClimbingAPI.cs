using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class RopeClimbingAPI : MonoBehaviour
{
	// Token: 0x06000BB0 RID: 2992 RVA: 0x0003E863 File Offset: 0x0003CA63
	private void Awake()
	{
		this.rope = base.GetComponent<Rope>();
		this.photonView = base.GetComponentInParent<PhotonView>();
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0003E87D File Offset: 0x0003CA7D
	public float GetMove()
	{
		return -1f * (1f / this.rope.GetTotalLength());
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0003E896 File Offset: 0x0003CA96
	public float GetPercentFromSegmentIndex(int segmentIndex)
	{
		return (float)segmentIndex / ((float)this.rope.SegmentCount - 1f);
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x0003E8B0 File Offset: 0x0003CAB0
	public float GetAngleAtPercent(float percent)
	{
		Transform segmentFromPercent = this.GetSegmentFromPercent(percent);
		Debug.DrawLine(segmentFromPercent.transform.position, segmentFromPercent.transform.position + segmentFromPercent.up, Color.red);
		return segmentFromPercent.GetComponent<RopeSegment>().GetAngle();
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x0003E8FC File Offset: 0x0003CAFC
	public Matrix4x4 GetSegmentMatrixFromPercent(float percent)
	{
		int num = Mathf.RoundToInt(Mathf.Lerp(0f, (float)(this.rope.SegmentCount - 1), percent));
		Transform transform = this.rope.GetRopeSegments()[num];
		return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0003E950 File Offset: 0x0003CB50
	public Vector3 GetUp(float ropePercent)
	{
		Transform segmentFromPercent = this.GetSegmentFromPercent(ropePercent);
		Vector3 vector = segmentFromPercent.up;
		if (Vector3.Angle(Vector3.up, segmentFromPercent.up) > 90f)
		{
			vector *= -1f;
		}
		return vector;
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0003E990 File Offset: 0x0003CB90
	public float UpMult(float percent)
	{
		return (float)((Vector3.Angle(Vector3.up, this.GetSegmentFromPercent(percent).up) < 90f) ? (-1) : 1);
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0003E9B4 File Offset: 0x0003CBB4
	public Vector3 GetPosition(float percent)
	{
		percent = Mathf.Clamp01(percent);
		float num = percent * (float)(this.rope.SegmentCount - 1);
		int num2 = Mathf.FloorToInt(num);
		int num3 = num2;
		if (num2 == 0)
		{
			num2 = 1;
		}
		if (percent < 1f)
		{
			num3 = num2 + 1;
		}
		float num4 = num - (float)num2;
		List<Transform> ropeSegments = this.rope.GetRopeSegments();
		num2 = math.clamp(num2, 0, ropeSegments.Count - 1);
		num3 = math.clamp(num3, num2, ropeSegments.Count - 1);
		return Vector3.Lerp(ropeSegments[num2].position, ropeSegments[num3].position, num4);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x0003EA44 File Offset: 0x0003CC44
	public Transform GetSegmentFromPercent(float percent)
	{
		percent = Mathf.Clamp01(percent);
		float num = percent * (float)(this.rope.SegmentCount - 1);
		int num2 = Mathf.FloorToInt(num);
		int num3 = num2;
		if (num2 == 0)
		{
			num2 = 1;
		}
		if (percent < 1f)
		{
			num3 = num2 + 1;
		}
		float num4 = num - (float)num2;
		List<Transform> ropeSegments = this.rope.GetRopeSegments();
		num2 = math.clamp(num2, 0, ropeSegments.Count - 1);
		num3 = math.clamp(num3, num2, ropeSegments.Count - 1);
		return ropeSegments[(num4 > 0.5f) ? num3 : num2];
	}

	// Token: 0x04000ADD RID: 2781
	private Rope rope;

	// Token: 0x04000ADE RID: 2782
	private PhotonView photonView;
}
