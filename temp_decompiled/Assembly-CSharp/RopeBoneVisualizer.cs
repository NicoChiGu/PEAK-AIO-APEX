using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Peak.Math;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000171 RID: 369
public class RopeBoneVisualizer : MonoBehaviour
{
	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06000BA3 RID: 2979 RVA: 0x0003E018 File Offset: 0x0003C218
	// (set) Token: 0x06000BA4 RID: 2980 RVA: 0x0003E020 File Offset: 0x0003C220
	public Transform StartTransform { get; set; }

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x06000BA5 RID: 2981 RVA: 0x0003E029 File Offset: 0x0003C229
	// (set) Token: 0x06000BA6 RID: 2982 RVA: 0x0003E031 File Offset: 0x0003C231
	public Optionable<bool> ManuallyUpdateNextFrame { get; set; }

	// Token: 0x06000BA7 RID: 2983 RVA: 0x0003E03C File Offset: 0x0003C23C
	private void Awake()
	{
		this.view = base.GetComponentInParent<PhotonView>();
		this.rope = base.GetComponent<Rope>();
		this.bones = Enumerable.ToList<Transform>(PExt.PGetComponentsInChildrenButNotMe<Transform>(this.boneRoot, false));
		this.bones.Reverse();
		this.meshRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
		this.CheckVisible();
		this.ghostMaterial = Object.Instantiate<Material>(this.ghostMaterial);
		this.ropeMaterial = Object.Instantiate<Material>(this.ropeMaterial);
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x0003E0B7 File Offset: 0x0003C2B7
	private void OnDestroy()
	{
		Object.Destroy(this.ropeMaterial);
		Object.Destroy(this.ghostMaterial);
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x0003E0D0 File Offset: 0x0003C2D0
	private void LateUpdate()
	{
		RopeBoneVisualizer.<>c__DisplayClass22_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.CheckVisible();
		if (this.targetPoints.Count != this.remoteRenderingPoints.Count)
		{
			Debug.LogError("Target points count mismatch");
			return;
		}
		float num = 1f / (float)PhotonNetwork.SerializationRate;
		this.sinceLastPackage += Time.deltaTime;
		if (this.ManuallyUpdateNextFrame.IsSome)
		{
			if (!this.ManuallyUpdateNextFrame.Value)
			{
				return;
			}
			this.ManuallyUpdateNextFrame = Optionable<bool>.Some(false);
		}
		float num2 = this.sinceLastPackage / num;
		for (int i = 0; i < this.remoteRenderingPoints.Count; i++)
		{
			Vector3 vector = Vector3.Lerp(this.remoteRenderingPoints[i].position, this.targetPoints[i].position, num2 * 0.5f);
			Quaternion quaternion = Quaternion.Lerp(this.remoteRenderingPoints[i].rotation, this.targetPoints[i].rotation, num2 * 0.5f);
			this.remoteRenderingPoints[i] = new RopeSyncData.SegmentData
			{
				position = vector,
				rotation = quaternion
			};
		}
		List<RopeSyncData.SegmentData> list;
		if (!this.view.IsMine)
		{
			list = this.remoteRenderingPoints;
		}
		else
		{
			list = Enumerable.ToList<RopeSyncData.SegmentData>(Enumerable.Select<Transform, RopeSyncData.SegmentData>(this.rope.GetRopeSegments(), (Transform transform1) => new RopeSyncData.SegmentData
			{
				position = transform1.position,
				rotation = transform1.rotation
			}));
		}
		List<RopeSyncData.SegmentData> list2 = list;
		CS$<>8__locals1.positions = new List<RopeSyncData.SegmentData>();
		if (this.StartTransform != null && this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			CS$<>8__locals1.positions.Add(new RopeSyncData.SegmentData
			{
				position = this.StartTransform.position,
				rotation = this.StartTransform.rotation
			});
		}
		if (list2.Count > 0)
		{
			CS$<>8__locals1.positions.Add(list2[0]);
		}
		for (int j = list2.Count - 1; j >= 1; j--)
		{
			CS$<>8__locals1.positions.Add(list2[j]);
		}
		CS$<>8__locals1.positions.Reverse();
		this.meshRenderer.sharedMaterial.SetFloat("_RopeCutoff", (Mathf.Floor((float)CS$<>8__locals1.positions.Count) - this.segmentMod) * (1f / (float)(this.bones.Count - 1)));
		if (CS$<>8__locals1.positions.Count == 0)
		{
			return;
		}
		if (this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			this.<LateUpdate>g__RenderInSpool|22_2(ref CS$<>8__locals1);
		}
		else
		{
			this.<LateUpdate>g__RenderInNotSpool|22_1(ref CS$<>8__locals1);
		}
		this.<LateUpdate>g__RenderInSpool|22_2(ref CS$<>8__locals1);
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0003E39C File Offset: 0x0003C59C
	public void OnDrawGizmosSelected()
	{
		foreach (Transform transform in this.bones)
		{
			DrawArrow.ForGizmo(transform.position, transform.up, Color.green, 0.25f);
			DrawArrow.ForGizmo(transform.position, transform.forward, Color.blue, 0.25f);
			DrawArrow.ForGizmo(transform.position, transform.right, Color.red, 0.25f);
		}
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x0003E43C File Offset: 0x0003C63C
	private void CheckVisible()
	{
		this.meshRenderer.sharedMaterial = ((this.rope.attachmenState == Rope.ATTACHMENT.inSpool) ? this.ghostMaterial : this.ropeMaterial);
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0003E468 File Offset: 0x0003C668
	public void SetData(RopeSyncData data)
	{
		if (this.rope.creatorLeft)
		{
			return;
		}
		this.sinceLastPackage = 0f;
		this.targetPoints = Enumerable.ToList<RopeSyncData.SegmentData>(data.segments);
		int num = data.segments.Length;
		int count = this.remoteRenderingPoints.Count;
		if (num < count)
		{
			int num2 = count - num;
			for (int i = 0; i < num2; i++)
			{
				ExtCollections.RemoveLast<RopeSyncData.SegmentData>(this.remoteRenderingPoints);
			}
			return;
		}
		if (num > count)
		{
			int num3 = num - count;
			for (int j = 0; j < num3; j++)
			{
				int num4 = count + j;
				this.remoteRenderingPoints.Add(data.segments[num4]);
			}
		}
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x0003E538 File Offset: 0x0003C738
	[CompilerGenerated]
	private void <LateUpdate>g__RenderInNotSpool|22_1(ref RopeBoneVisualizer.<>c__DisplayClass22_0 A_1)
	{
		int num = 0;
		for (int i = 0; i < this.bones.Count; i++)
		{
			Transform transform = this.bones[i];
			if (i > A_1.positions.Count - 1 && i > 0)
			{
				transform.gameObject.name = num.ToString();
				if (num == 0 && this.StartTransform != null)
				{
					transform.position = this.StartTransform.position;
					Vector3 vector = transform.position - this.bones[i - 1].position;
					if (!vector.NearZero())
					{
						transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up, -vector);
					}
					num++;
				}
				else
				{
					transform.position = this.bones[i - 1].position;
					transform.rotation = this.bones[i - 1].rotation;
					num++;
					transform.localScale = Vector3.zero;
				}
			}
			else
			{
				transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up + Vector3.forward * 0.05f, -ExtQuaternion.GetUp(A_1.positions[i].rotation));
				transform.localScale = ExtSwizzle.xxx(1f);
				transform.position = ExtDataTypes.PToV3(A_1.positions[i].position);
				transform.gameObject.name = i.ToString();
			}
		}
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0003E6C4 File Offset: 0x0003C8C4
	[CompilerGenerated]
	private void <LateUpdate>g__RenderInSpool|22_2(ref RopeBoneVisualizer.<>c__DisplayClass22_0 A_1)
	{
		int num = 0;
		for (int i = 0; i < this.bones.Count; i++)
		{
			Transform transform = this.bones[i];
			if (i > A_1.positions.Count - 3 && i > 0)
			{
				transform.gameObject.name = num.ToString();
				if (num == 0 && this.StartTransform != null)
				{
					transform.position = this.StartTransform.position;
					if (this.withRotOfStartPos)
					{
						transform.rotation = this.StartTransform.rotation;
					}
					else
					{
						Vector3 vector = transform.position - this.bones[i - 1].position;
						transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up, -vector);
					}
					num++;
				}
				else
				{
					transform.position = this.bones[i - 1].position;
					transform.rotation = this.bones[i - 1].rotation;
					num++;
					transform.localScale = Vector3.zero;
				}
			}
			else
			{
				transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up + Vector3.forward * 0.05f, -ExtQuaternion.GetUp(A_1.positions[i].rotation));
				transform.localScale = ExtSwizzle.xxx(1f);
				transform.position = ExtDataTypes.PToV3(A_1.positions[i].position);
				transform.gameObject.name = i.ToString();
			}
		}
	}

	// Token: 0x04000ACF RID: 2767
	public Material ghostMaterial;

	// Token: 0x04000AD0 RID: 2768
	public Material ropeMaterial;

	// Token: 0x04000AD1 RID: 2769
	public GameObject boneRoot;

	// Token: 0x04000AD2 RID: 2770
	public List<Transform> bones;

	// Token: 0x04000AD3 RID: 2771
	public float segmentMod = 1f;

	// Token: 0x04000AD4 RID: 2772
	public bool withRotOfStartPos;

	// Token: 0x04000AD5 RID: 2773
	private readonly List<RopeSyncData.SegmentData> remoteRenderingPoints = new List<RopeSyncData.SegmentData>();

	// Token: 0x04000AD6 RID: 2774
	private SkinnedMeshRenderer meshRenderer;

	// Token: 0x04000AD7 RID: 2775
	private Rope rope;

	// Token: 0x04000AD8 RID: 2776
	private float sinceLastPackage;

	// Token: 0x04000AD9 RID: 2777
	[NonSerialized]
	private List<RopeSyncData.SegmentData> targetPoints = new List<RopeSyncData.SegmentData>();

	// Token: 0x04000ADA RID: 2778
	private PhotonView view;
}
