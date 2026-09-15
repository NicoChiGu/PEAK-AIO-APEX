using System;
using UnityEngine;

// Token: 0x02000348 RID: 840
[RequireComponent(typeof(HeightmapMesh), typeof(MeshFilter), typeof(MeshRenderer))]
public class SwampMeshGen : MonoBehaviour
{
	// Token: 0x0600158E RID: 5518 RVA: 0x0006ECDB File Offset: 0x0006CEDB
	private void Awake()
	{
		this._hm = base.GetComponent<HeightmapMesh>();
	}

	// Token: 0x0600158F RID: 5519 RVA: 0x0006ECEC File Offset: 0x0006CEEC
	public void Generate()
	{
		if (this.resolution < 2)
		{
			Debug.LogError("Resolution must be at least 2.");
			return;
		}
		if (this._hm == null)
		{
			this._hm = base.GetComponent<HeightmapMesh>();
		}
		this._hm.cellSize = this.cellSize;
		this._hm.center = this.center;
		int num = this.resolution;
		int num2 = this.resolution;
		float[,] array = new float[num2, num];
		Vector3 zero = Vector3.zero;
		if (this.center)
		{
			float num3 = (float)(num - 1) * this.cellSize;
			float num4 = (float)(num2 - 1) * this.cellSize;
			zero..ctor(-num3 * 0.5f, 0f, -num4 * 0.5f);
		}
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Vector3 vector;
				vector..ctor((float)j * this.cellSize, 0f, (float)i * this.cellSize);
				Vector3 vector2 = base.transform.position + zero + vector;
				RaycastHit raycastHit;
				float num5;
				if (Physics.Raycast(new Ray(vector2, Vector3.down), ref raycastHit, this.maxRaycastLength, this.layerMask, this.triggerInteraction))
				{
					num5 = raycastHit.point.y - base.transform.position.y;
				}
				else
				{
					num5 = -this.maxRaycastLength;
				}
				array[i, j] = num5;
			}
		}
		if (this.clampHeights)
		{
			for (int k = 0; k < num2; k++)
			{
				for (int l = 0; l < num; l++)
				{
					array[k, l] = Mathf.Clamp(array[k, l], this.minHeight, this.maxHeight);
				}
			}
		}
		for (int m = 0; m < num2; m++)
		{
			for (int n = 0; n < num; n++)
			{
				array[m, n] += this.heightOffset;
			}
		}
		if (this.blurIterations > 0 && this.blurRadius > 0)
		{
			this.BoxBlurInPlace(array, this.blurRadius, this.blurIterations);
		}
		this._hm.Generate(array);
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x0006EF20 File Offset: 0x0006D120
	private void BoxBlurInPlace(float[,] data, int radius, int iterations)
	{
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		float[,] array = new float[length, length2];
		for (int i = 0; i < iterations; i++)
		{
			for (int j = 0; j < length; j++)
			{
				float num = 0f;
				int num2 = radius * 2 + 1;
				for (int k = -radius; k <= radius; k++)
				{
					int num3 = Mathf.Clamp(k, 0, length2 - 1);
					num += data[j, num3];
				}
				for (int l = 0; l < length2; l++)
				{
					array[j, l] = num / (float)num2;
					int num4 = l - radius;
					int num5 = l + radius + 1;
					if (num4 >= 0 && num4 < length2)
					{
						num -= data[j, num4];
					}
					else if (num4 < 0)
					{
						num -= data[j, 0];
					}
					else
					{
						num -= data[j, length2 - 1];
					}
					if (num5 >= 0 && num5 < length2)
					{
						num += data[j, num5];
					}
					else if (num5 < 0)
					{
						num += data[j, 0];
					}
					else
					{
						num += data[j, length2 - 1];
					}
				}
			}
			for (int m = 0; m < length2; m++)
			{
				float num6 = 0f;
				int num7 = radius * 2 + 1;
				for (int n = -radius; n <= radius; n++)
				{
					int num8 = Mathf.Clamp(n, 0, length - 1);
					num6 += array[num8, m];
				}
				for (int num9 = 0; num9 < length; num9++)
				{
					data[num9, m] = num6 / (float)num7;
					int num10 = num9 - radius;
					int num11 = num9 + radius + 1;
					if (num10 >= 0 && num10 < length)
					{
						num6 -= array[num10, m];
					}
					else if (num10 < 0)
					{
						num6 -= array[0, m];
					}
					else
					{
						num6 -= array[length - 1, m];
					}
					if (num11 >= 0 && num11 < length)
					{
						num6 += array[num11, m];
					}
					else if (num11 < 0)
					{
						num6 += array[0, m];
					}
					else
					{
						num6 += array[length - 1, m];
					}
				}
			}
		}
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x0006F164 File Offset: 0x0006D364
	private void OnDrawGizmosSelected()
	{
		if (!this.drawRayGizmos && !this.drawSamplePoints)
		{
			return;
		}
		int num = Mathf.Max(2, this.resolution);
		int num2 = Mathf.Max(2, this.resolution);
		Vector3 zero = Vector3.zero;
		if (this.center)
		{
			float num3 = (float)(num - 1) * this.cellSize;
			float num4 = (float)(num2 - 1) * this.cellSize;
			zero..ctor(-num3 * 0.5f, 0f, -num4 * 0.5f);
		}
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Vector3 vector;
				vector..ctor((float)j * this.cellSize, 0f, (float)i * this.cellSize);
				Vector3 vector2 = base.transform.position + zero + vector;
				if (this.drawSamplePoints)
				{
					Gizmos.DrawSphere(vector2, Mathf.Min(0.05f, this.cellSize * 0.1f));
				}
				if (this.drawRayGizmos)
				{
					Gizmos.DrawLine(vector2, vector2 + Vector3.down * this.maxRaycastLength);
				}
			}
		}
	}

	// Token: 0x0400145A RID: 5210
	[Header("Grid")]
	[Min(2f)]
	public int resolution = 64;

	// Token: 0x0400145B RID: 5211
	public float cellSize = 1f;

	// Token: 0x0400145C RID: 5212
	public bool center = true;

	// Token: 0x0400145D RID: 5213
	[Header("Raycast")]
	public float maxRaycastLength = 10f;

	// Token: 0x0400145E RID: 5214
	public LayerMask layerMask = -1;

	// Token: 0x0400145F RID: 5215
	public QueryTriggerInteraction triggerInteraction = 1;

	// Token: 0x04001460 RID: 5216
	[Header("Clamp (optional)")]
	public bool clampHeights;

	// Token: 0x04001461 RID: 5217
	public float minHeight = -5f;

	// Token: 0x04001462 RID: 5218
	public float maxHeight = 5f;

	// Token: 0x04001463 RID: 5219
	[Header("Smoothing")]
	[Tooltip("Number of box-blur passes to apply after clamping.")]
	[Min(0f)]
	public int blurIterations = 1;

	// Token: 0x04001464 RID: 5220
	[Tooltip("Radius in cells for box blur. 0 disables blur.")]
	[Min(0f)]
	public int blurRadius = 1;

	// Token: 0x04001465 RID: 5221
	[Header("Offset")]
	public float heightOffset;

	// Token: 0x04001466 RID: 5222
	[Header("Debug")]
	public bool drawRayGizmos;

	// Token: 0x04001467 RID: 5223
	public bool drawSamplePoints;

	// Token: 0x04001468 RID: 5224
	private HeightmapMesh _hm;
}
