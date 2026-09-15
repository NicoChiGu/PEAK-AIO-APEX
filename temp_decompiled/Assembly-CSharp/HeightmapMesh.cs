using System;
using UnityEngine;

// Token: 0x0200026B RID: 619
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HeightmapMesh : MonoBehaviour
{
	// Token: 0x06001185 RID: 4485 RVA: 0x00058094 File Offset: 0x00056294
	public void Generate(float[,] heights)
	{
		if (heights == null)
		{
			Debug.LogError("heights is null");
			return;
		}
		int length = heights.GetLength(0);
		int length2 = heights.GetLength(1);
		if (length2 < 2 || length < 2)
		{
			Debug.LogError("heights must be at least 2x2.");
			return;
		}
		if (this._mesh == null)
		{
			this._mesh = new Mesh();
			this._mesh.name = "Heightmap Mesh";
		}
		else
		{
			this._mesh.Clear();
		}
		int num = length2 * length;
		if (num > 65535)
		{
			this._mesh.indexFormat = 1;
		}
		else
		{
			this._mesh.indexFormat = 0;
		}
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[(length2 - 1) * (length - 1) * 6];
		Vector3 zero = Vector3.zero;
		if (this.center)
		{
			float num2 = (float)(length2 - 1) * this.cellSize;
			float num3 = (float)(length - 1) * this.cellSize;
			zero..ctor(-num2 * 0.5f, 0f, -num3 * 0.5f);
		}
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				int num4 = i * length2 + j;
				float num5 = heights[i, j];
				array[num4] = new Vector3((float)j * this.cellSize, num5, (float)i * this.cellSize) + zero;
				float num6 = ((length2 == 1) ? 0f : ((float)j / (float)(length2 - 1)));
				float num7 = ((length == 1) ? 0f : ((float)i / (float)(length - 1)));
				array2[num4] = new Vector2(num6, num7);
			}
		}
		int num8 = 0;
		for (int k = 0; k < length - 1; k++)
		{
			for (int l = 0; l < length2 - 1; l++)
			{
				int num9 = k * length2 + l;
				int num10 = num9 + 1;
				int num11 = num9 + length2;
				int num12 = num11 + 1;
				array3[num8++] = num9;
				array3[num8++] = num11;
				array3[num8++] = num10;
				array3[num8++] = num10;
				array3[num8++] = num11;
				array3[num8++] = num12;
			}
		}
		this._mesh.SetVertices(array);
		this._mesh.SetUVs(0, array2);
		this._mesh.SetTriangles(array3, 0, true);
		this._mesh.RecalculateNormals();
		this._mesh.RecalculateBounds();
		base.GetComponent<MeshFilter>().sharedMesh = this._mesh;
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x00058314 File Offset: 0x00056514
	[ContextMenu("Generate Test Mesh (Perlin)")]
	private void GenerateTest()
	{
		int num = 129;
		int num2 = 129;
		float[,] array = new float[num2, num];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				array[i, j] = Mathf.PerlinNoise((float)j * 0.05f, (float)i * 0.05f) * 2f;
			}
		}
		this.Generate(array);
	}

	// Token: 0x04000FFE RID: 4094
	[Tooltip("World-space distance between adjacent grid points.")]
	public float cellSize = 1f;

	// Token: 0x04000FFF RID: 4095
	[Tooltip("If true, the mesh is centered around the GameObject origin.")]
	public bool center;

	// Token: 0x04001000 RID: 4096
	private Mesh _mesh;
}
