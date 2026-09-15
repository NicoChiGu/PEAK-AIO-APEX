using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Knot
{
	// Token: 0x020003A5 RID: 933
	[ExecuteInEditMode]
	public class KnotTemplate : MonoBehaviour, ISerializationCallbackReceiver
	{
		// Token: 0x06001845 RID: 6213 RVA: 0x0007AE5E File Offset: 0x0007905E
		private void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.SplineToLineRenderer();
			this.LineRendererToMeshColliders();
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x0007AE74 File Offset: 0x00079074
		private void Update()
		{
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x0007AE76 File Offset: 0x00079076
		public void OnBeforeSerialize()
		{
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x0007AE78 File Offset: 0x00079078
		public void OnAfterDeserialize()
		{
			this.Register();
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x0007AE80 File Offset: 0x00079080
		public void SplineToLineRenderer()
		{
			if (this.splineContainer == null)
			{
				return;
			}
			List<Vector3> list = new List<Vector3>();
			float num = 1f / (float)this.lr.positionCount;
			List<Keyframe> list2 = new List<Keyframe>();
			this.lr.transform.localPosition = Vector3.zero;
			this.lr.transform.localRotation = Quaternion.identity;
			for (int i = 0; i < this.lr.positionCount; i++)
			{
				float num2 = num * (float)i;
				float3 @float = SplineUtility.EvaluatePosition<Spline>(this.splineContainer.Spline, num2);
				float num3 = @float.z + this.splineContainer.transform.localPosition.z;
				num3 *= num3;
				float magnitude = this.splineContainer.transform.TransformVector(Vector3.one).magnitude;
				num3 *= magnitude;
				@float = this.splineContainer.transform.TransformPoint(ExtDataTypes.PToV3(@float));
				@float = this.lr.transform.InverseTransformPoint(@float);
				list.Add(ExtSwizzle.xyn(ExtDataTypes.PToV3(@float), -num2 * 0.1f));
				list2.Add(new Keyframe(num2, Mathf.Max(this.minWidth, num3 * this.widthMul)));
			}
			this.lr.widthCurve = new AnimationCurve
			{
				keys = list2.ToArray()
			};
			this.lr.SetPositions(list.ToArray());
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x0007B010 File Offset: 0x00079210
		private void LineRendererToMeshColliders()
		{
			Debug.Log("LineRendererToMeshColliders");
			this.lineMesh = new Mesh();
			this.lr.BakeMesh(this.lineMesh, Camera.main, true);
			ExtTransform.KillAllChildren(this.colliderRoot, true, false, false);
			int num = 0;
			while (num < Mathf.FloorToInt((float)this.lineMesh.triangles.Length / 3f) / 2 && num <= this.lineMesh.triangles.Length)
			{
				GameObject gameObject = new GameObject(string.Format("{0}", num));
				List<int> list = new List<int>();
				List<Vector3> list2 = new List<Vector3>();
				gameObject.transform.parent = this.colliderRoot;
				gameObject.transform.localPosition = ExtDataTypes.ToVec(0);
				for (int i = 0; i < 2; i++)
				{
					int num2 = num * 2 + i;
					for (int j = 0; j < 3; j++)
					{
						Vector3 vector = this.lineMesh.vertices[this.lineMesh.triangles[num2 * 3 + j]];
						list2.Add(vector);
						list.Add(list2.Count - 1);
					}
				}
				Mesh mesh = new Mesh();
				mesh.vertices = list2.ToArray();
				mesh.triangles = list.ToArray();
				PExt.RecalculateAll(mesh);
				gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
				num++;
			}
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x0007B172 File Offset: 0x00079372
		private void Register()
		{
			if (this.registered)
			{
				return;
			}
			this.registered = true;
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x0007B1A4 File Offset: 0x000793A4
		[CompilerGenerated]
		private void <Register>g__EditorSplineUtilityOnAfterSplineWasModified|16_0(Spline spline)
		{
			try
			{
				if (base.gameObject == null)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (spline != this.splineContainer.Spline)
			{
				return;
			}
			this.SplineToLineRenderer();
		}

		// Token: 0x04001677 RID: 5751
		public float widthMul = 0.1f;

		// Token: 0x04001678 RID: 5752
		public float minWidth = 0.001f;

		// Token: 0x04001679 RID: 5753
		public LineRenderer lr;

		// Token: 0x0400167A RID: 5754
		public SplineContainer splineContainer;

		// Token: 0x0400167B RID: 5755
		public Transform colliderRoot;

		// Token: 0x0400167C RID: 5756
		private Mesh lineMesh;

		// Token: 0x0400167D RID: 5757
		private float counter;

		// Token: 0x0400167E RID: 5758
		[NonSerialized]
		private bool registered;

		// Token: 0x0400167F RID: 5759
		public bool editorRefresh;

		// Token: 0x04001680 RID: 5760
		private float timeToRefresh;
	}
}
