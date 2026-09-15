using System;
using System.Collections.Generic;
using pworld.Scripts.Extensions;
using UnityEngine;

namespace Knot
{
	// Token: 0x020003A7 RID: 935
	public class KnotUnmaker : MonoBehaviour
	{
		// Token: 0x06001852 RID: 6226 RVA: 0x0007B2A0 File Offset: 0x000794A0
		public void Update()
		{
			Vector3 vector;
			this.MouseToPlaneRaycast(out vector);
			if (this.visualizer.knot == null || this.visualizer.knot.Count == 0)
			{
				this.grabbing = false;
				this.elapsed = 0f;
				return;
			}
			if (Input.GetKeyDown(324))
			{
				List<TiedKnotVisualizer.KnotPart> knot = this.visualizer.knot;
				float num = Vector3.Distance(knot[knot.Count - 1].position, vector);
				Debug.Log(string.Format("Try Erase Grab {0} < {1}", num, this.grabDistance));
				if (num < this.grabDistance)
				{
					Debug.Log("Grabbing");
					this.grabbing = true;
				}
			}
			if (Input.GetKey(324) && this.grabbing)
			{
				if (this.visualizer.knot.Count < 2)
				{
					this.visualizer.knot.Clear();
					this.grabbing = false;
					return;
				}
				Vector3 vector2 = ExtSwizzle.xyo(vector);
				List<TiedKnotVisualizer.KnotPart> knot2 = this.visualizer.knot;
				Vector3 vector3 = vector2 - ExtSwizzle.xyo(knot2[knot2.Count - 1].position);
				Vector3 vector4 = Vector3.zero;
				int num2 = Mathf.Min(10, this.visualizer.knot.Count) - 1;
				for (int i = 0; i < num2; i++)
				{
					Vector3 vector5 = vector4;
					List<TiedKnotVisualizer.KnotPart> knot3 = this.visualizer.knot;
					int num3 = i + 2;
					Vector3 vector6 = ExtSwizzle.xyo(knot3[knot3.Count - num3].position);
					List<TiedKnotVisualizer.KnotPart> knot4 = this.visualizer.knot;
					vector4 = vector5 + (vector6 - ExtSwizzle.xyo(knot4[knot4.Count - 1].position));
				}
				vector4 /= (float)num2;
				float num4 = Vector3.Angle(vector3, vector4);
				string text = "try Erasing, angle: {0}, Erase Speed {1}";
				object obj = num4;
				List<TiedKnotVisualizer.KnotPart> knot5 = this.visualizer.knot;
				Debug.Log(string.Format(text, obj, knot5[knot5.Count - 1].quality));
				float num5 = Mathf.InverseLerp(0f, this.dragAngle, num4);
				this.lineColor = Color.Lerp(this.good, this.bad, num5);
				if (num4 < this.dragAngle)
				{
					float num6 = this.eraseSpeed * (1f - Mathf.InverseLerp(0f, this.dragAngle, num4));
					float num7 = num6;
					float num8 = this.minDistToDrag;
					float num9 = this.maxDistToDrag;
					List<TiedKnotVisualizer.KnotPart> knot6 = this.visualizer.knot;
					num6 = num7 * Mathf.InverseLerp(num8, num9, Vector3.Distance(knot6[knot6.Count - 1].position, vector));
					float num10 = this.elapsed;
					float deltaTime = Time.deltaTime;
					List<TiedKnotVisualizer.KnotPart> knot7 = this.visualizer.knot;
					this.elapsed = num10 + deltaTime * (knot7[knot7.Count - 1].quality ? num6 : (num6 * 0.1f));
					List<TiedKnotVisualizer.KnotPart> knot8 = this.visualizer.knot;
					Vector3 position = knot8[knot8.Count - 2].position;
					List<TiedKnotVisualizer.KnotPart> knot9 = this.visualizer.knot;
					float num11 = Vector3.Distance(position, knot9[knot9.Count - 1].position);
					if (this.elapsed > num11)
					{
						Debug.Log(string.Format("Removing endKnot, knotDist: {0}", num11));
						this.visualizer.knot.RemoveAt(this.visualizer.knot.Count - 1);
						this.visualizer.Refresh();
						this.elapsed -= num11;
					}
				}
			}
			if (Input.GetKeyUp(324))
			{
				Debug.Log("Stop Grabbing");
				this.grabbing = false;
			}
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x0007B628 File Offset: 0x00079828
		public bool MouseToPlaneRaycast(out Vector3 position)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Plane plane;
			plane..ctor(Camera.main.transform.forward, (KnotTemplateBoss.me != null) ? KnotTemplateBoss.me.displayRoot.position : Vector3.zero);
			float num;
			if (plane.Raycast(ray, ref num))
			{
				position = ray.direction * num + ray.origin;
				return true;
			}
			position = Vector3.zero;
			return false;
		}

		// Token: 0x04001685 RID: 5765
		public TiedKnotVisualizer visualizer;

		// Token: 0x04001686 RID: 5766
		public float grabDistance = 0.05f;

		// Token: 0x04001687 RID: 5767
		public float dragAngle = 45f;

		// Token: 0x04001688 RID: 5768
		public float eraseSpeed = 0.1f;

		// Token: 0x04001689 RID: 5769
		private float elapsed;

		// Token: 0x0400168A RID: 5770
		public bool grabbing;

		// Token: 0x0400168B RID: 5771
		public Color lineColor;

		// Token: 0x0400168C RID: 5772
		public Color good;

		// Token: 0x0400168D RID: 5773
		public Color bad;

		// Token: 0x0400168E RID: 5774
		public float minDistToDrag = 1f;

		// Token: 0x0400168F RID: 5775
		public float maxDistToDrag = 10f;
	}
}
