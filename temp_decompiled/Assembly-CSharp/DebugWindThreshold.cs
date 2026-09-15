using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class DebugWindThreshold : MonoBehaviour
{
	// Token: 0x0600064B RID: 1611 RVA: 0x00024078 File Offset: 0x00022278
	public void GenerateMap()
	{
		this.ClearMap();
		this.min = this.zone.bounds.min;
		this.max = this.zone.bounds.max;
		Vector3 vector = this.min;
		while (vector.z < this.max.z)
		{
			while (vector.y < this.max.y)
			{
				while (vector.x < this.max.x)
				{
					this.nodes.Add(new DebugWindThreshold.WindNode(vector));
					vector.x += this.nodeSpacing;
				}
				vector.y += this.nodeSpacing;
				vector.x = this.min.x;
			}
			vector.z += this.nodeSpacing;
			vector.y = this.min.y;
			vector.x = this.min.x;
		}
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x0002417C File Offset: 0x0002237C
	public void ClearMap()
	{
		this.nodes.Clear();
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0002418C File Offset: 0x0002238C
	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < this.nodes.Count; i++)
		{
			float num;
			if (this.nodes[i].wind > this.lowerThreshold + this.thresholdMargin)
			{
				num = 1f;
			}
			else if (this.nodes[i].wind < this.lowerThreshold)
			{
				num = 0f;
			}
			else
			{
				num = Util.RangeLerp(0f, 1f, this.lowerThreshold, this.lowerThreshold + this.thresholdMargin, this.nodes[i].wind, true, null);
			}
			this.nodes[i].DrawGizmo_HeatMap(num);
		}
	}

	// Token: 0x04000654 RID: 1620
	[Range(0f, 1f)]
	public float lowerThreshold;

	// Token: 0x04000655 RID: 1621
	[Range(0f, 1f)]
	public float thresholdMargin;

	// Token: 0x04000656 RID: 1622
	public Collider zone;

	// Token: 0x04000657 RID: 1623
	public float nodeSpacing = 5f;

	// Token: 0x04000658 RID: 1624
	public const float MIN_NODE_SPACING = 2f;

	// Token: 0x04000659 RID: 1625
	public List<DebugWindThreshold.WindNode> nodes = new List<DebugWindThreshold.WindNode>();

	// Token: 0x0400065A RID: 1626
	public Vector3 min;

	// Token: 0x0400065B RID: 1627
	public Vector3 max;

	// Token: 0x0200042C RID: 1068
	[Serializable]
	public class WindNode
	{
		// Token: 0x06001A76 RID: 6774 RVA: 0x000802ED File Offset: 0x0007E4ED
		public WindNode(Vector3 position)
		{
			this.position = position;
			this.wind = LightVolume.Instance().SamplePositionAlpha(position);
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x00080310 File Offset: 0x0007E510
		public void DrawGizmo_HeatMap(float amt)
		{
			Color color;
			if (amt == 1f)
			{
				color = Color.red;
			}
			else if (amt == 0f)
			{
				color = Color.green;
			}
			else
			{
				color = Color.Lerp(Color.yellow, Color.red, amt);
			}
			color.a = 0.5f;
			Gizmos.color = color;
			Gizmos.DrawSphere(this.position, 1f);
		}

		// Token: 0x040017F9 RID: 6137
		public float wind;

		// Token: 0x040017FA RID: 6138
		public Vector3 position;
	}
}
