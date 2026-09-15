using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Splines;

namespace Knot
{
	// Token: 0x020003A3 RID: 931
	public class AlignWithSpline : MonoBehaviour
	{
		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06001832 RID: 6194 RVA: 0x0007A4D4 File Offset: 0x000786D4
		public float KnotStepSize
		{
			get
			{
				return this.knotProgressRange * 2f;
			}
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x0007A4E4 File Offset: 0x000786E4
		public void DistanceToSpline(Vector3 position, out float closest, out float atSplineProgress)
		{
			position = ExtSwizzle.xyo(position);
			int num = 200;
			float num2 = 1f / (float)num;
			closest = float.MaxValue;
			atSplineProgress = 0f;
			for (int i = 0; i < num; i++)
			{
				float num3 = num2 * (float)i;
				Vector3 vector = ExtSwizzle.xyo(ExtDataTypes.PToV3(SplineUtility.EvaluatePosition<Spline>(this.splineContainer.Spline, num3))) - position;
				if (vector.magnitude < closest)
				{
					closest = vector.magnitude;
					atSplineProgress = num3;
				}
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06001834 RID: 6196 RVA: 0x0007A561 File Offset: 0x00078761
		public Vector2 KnotProgressRangeRelation
		{
			get
			{
				return this.knotProgressRangeRelation * this.knotProgressRange;
			}
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x0007A574 File Offset: 0x00078774
		private void EvaluateKnot(AlignWithSpline.TiedKnot tiedKnot)
		{
			float templateProgress = tiedKnot.knotPoints[0].templateProgress;
			Vector2 vector = this.KnotProgressRangeRelation;
			vector.x += this.knotProgress;
			vector.y += this.knotProgress;
			Vector2 vector2 = vector;
			vector2.x += this.KnotStepSize;
			vector2.y += this.KnotStepSize;
			if (templateProgress > vector.y && templateProgress > vector2.y)
			{
				Debug.LogError("");
			}
			float x = vector.x;
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x0007A614 File Offset: 0x00078814
		private void TieRope2()
		{
			Plane plane;
			plane..ctor(Camera.main.transform.forward, this.splineContainer.transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float num;
			if (!plane.Raycast(ray, ref num))
			{
				return;
			}
			Vector3 vector = ray.direction * num + ray.origin;
			this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint
			{
				position = ExtSwizzle.xyo(vector),
				templateProgress = this.lastKnotPointProgress,
				inside = false
			});
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x0007A6B4 File Offset: 0x000788B4
		private void TieRope()
		{
			Plane plane;
			plane..ctor(Camera.main.transform.forward, this.splineContainer.transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] array = Physics.RaycastAll(ray);
			if (array.Length != 0)
			{
				IOrderedEnumerable<RaycastHit> orderedEnumerable = Enumerable.OrderBy<RaycastHit, float>(array, (RaycastHit h) => Mathf.Abs(h.textureCoord.x - this.lastKnotPointProgress));
				foreach (RaycastHit raycastHit in orderedEnumerable)
				{
					Debug.Log(string.Format("{0} Hit: {1}", Time.frameCount, raycastHit.textureCoord.x));
				}
				RaycastHit raycastHit2 = Enumerable.First<RaycastHit>(orderedEnumerable);
				if (this.tiedKnot.knotPoints.Count > 0)
				{
					List<AlignWithSpline.TiedKnot.KnotPoint> knotPoints = this.tiedKnot.knotPoints;
					if (Vector3.Distance(knotPoints[knotPoints.Count - 1].position, raycastHit2.point) < this.minKnotPointDistance)
					{
						return;
					}
				}
				this.lastKnotPointProgress = raycastHit2.textureCoord.x;
				this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint
				{
					position = ExtSwizzle.xyo(raycastHit2.point),
					templateProgress = this.lastKnotPointProgress,
					inside = true
				});
				Debug.Log(string.Format("Added: {0}", raycastHit2.textureCoord.x));
				return;
			}
			float num;
			if (plane.Raycast(ray, ref num))
			{
				Vector3 vector = ray.direction * num + ray.origin;
				this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint
				{
					position = ExtSwizzle.xyo(vector),
					templateProgress = this.lastKnotPointProgress,
					inside = false
				});
			}
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x0007A898 File Offset: 0x00078A98
		private void Update()
		{
			if (Input.GetKeyDown(323))
			{
				this.tiedKnot = new AlignWithSpline.TiedKnot();
			}
			else if (Input.GetKey(323))
			{
				this.TieRope();
			}
			Input.GetKeyUp(323);
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x0007A8D0 File Offset: 0x00078AD0
		private void FixedUpdate()
		{
		}

		// Token: 0x04001666 RID: 5734
		public SplineContainer splineContainer;

		// Token: 0x04001667 RID: 5735
		public float knotProgress;

		// Token: 0x04001668 RID: 5736
		public float minKnotPointDistance = 0.001f;

		// Token: 0x04001669 RID: 5737
		public float lastKnotPointProgress;

		// Token: 0x0400166A RID: 5738
		private AlignWithSpline.TiedKnot tiedKnot = new AlignWithSpline.TiedKnot();

		// Token: 0x0400166B RID: 5739
		public TiedKnotVisualizer tiedKnotVisualizer;

		// Token: 0x0400166C RID: 5740
		public float knotProgressRange = 0.025f;

		// Token: 0x0400166D RID: 5741
		public Vector2 knotProgressRangeRelation = new Vector2(-2f, 1f);

		// Token: 0x0400166E RID: 5742
		public float test = -0.3f;

		// Token: 0x02000532 RID: 1330
		public class TiedKnot
		{
			// Token: 0x04001BE8 RID: 7144
			public List<AlignWithSpline.TiedKnot.KnotPoint> knotPoints = new List<AlignWithSpline.TiedKnot.KnotPoint>();

			// Token: 0x02000558 RID: 1368
			public class KnotPoint
			{
				// Token: 0x04001C74 RID: 7284
				public Vector3 position;

				// Token: 0x04001C75 RID: 7285
				public float templateProgress;

				// Token: 0x04001C76 RID: 7286
				public bool inside;
			}
		}
	}
}
