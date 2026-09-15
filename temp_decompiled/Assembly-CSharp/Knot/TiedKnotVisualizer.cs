using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Knot
{
	// Token: 0x020003A8 RID: 936
	public class TiedKnotVisualizer : MonoBehaviour
	{
		// Token: 0x06001855 RID: 6229 RVA: 0x0007B6F6 File Offset: 0x000798F6
		private void Awake()
		{
			this.lr = base.GetComponent<LineRenderer>();
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x0007B704 File Offset: 0x00079904
		public void Refresh()
		{
			this.Visualize(this.knot);
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x0007B714 File Offset: 0x00079914
		public void Go()
		{
			foreach (TiedKnotVisualizer.KnotPart knotPart in this.knot)
			{
				Debug.Log(string.Format("Quality: {0}, Position: {1}", knotPart.quality, knotPart.position));
			}
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x0007B788 File Offset: 0x00079988
		public void Visualize(List<TiedKnotVisualizer.KnotPart> knot)
		{
			this.knot = knot;
			List<Vector3> list = Enumerable.ToList<Vector3>(Enumerable.Select<TiedKnotVisualizer.KnotPart, Vector3>(knot, (TiedKnotVisualizer.KnotPart knotPoint) => knotPoint.position));
			if (!this.splineIt)
			{
				this.lr.positionCount = list.Count;
				this.lr.SetPositions(list.ToArray());
				return;
			}
			Spline spline = new Spline();
			spline.Knots = Enumerable.ToArray<BezierKnot>(Enumerable.Select<Vector3, BezierKnot>(list, (Vector3 knotPoint) => new BezierKnot(knotPoint)));
			List<Vector3> list2 = new List<Vector3>();
			float num = 1f / (float)this.count;
			for (int i = 0; i < this.count; i++)
			{
				float num2 = num * (float)i;
				float3 @float = SplineUtility.EvaluatePosition<Spline>(spline, num2);
				list2.Add(ExtDataTypes.PToV3(@float));
			}
			this.lr.positionCount = this.count;
			this.lr.SetPositions(list2.ToArray());
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x0007B891 File Offset: 0x00079A91
		private void Start()
		{
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x0007B893 File Offset: 0x00079A93
		private void Update()
		{
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x0007B895 File Offset: 0x00079A95
		public void Clear()
		{
			this.knot.Clear();
			this.Refresh();
		}

		// Token: 0x04001690 RID: 5776
		private LineRenderer lr;

		// Token: 0x04001691 RID: 5777
		public int count;

		// Token: 0x04001692 RID: 5778
		public bool splineIt;

		// Token: 0x04001693 RID: 5779
		public List<TiedKnotVisualizer.KnotPart> knot = new List<TiedKnotVisualizer.KnotPart>();

		// Token: 0x02000536 RID: 1334
		public struct KnotPart
		{
			// Token: 0x06001DEC RID: 7660 RVA: 0x00088DC4 File Offset: 0x00086FC4
			public KnotPart(bool quality, Vector3 position, int part)
			{
				this.quality = quality;
				this.position = position;
				this.part = part;
			}

			// Token: 0x04001BEF RID: 7151
			public bool quality;

			// Token: 0x04001BF0 RID: 7152
			public Vector3 position;

			// Token: 0x04001BF1 RID: 7153
			public int part;
		}
	}
}
