using System;
using UnityEngine;

// Token: 0x0200034C RID: 844
public class TerrainSplatMesh : MonoBehaviour
{
	// Token: 0x060015B3 RID: 5555 RVA: 0x000700A4 File Offset: 0x0006E2A4
	private Mesh GetMesh()
	{
		if (this.mesh == null)
		{
			this.mesh = base.GetComponent<MeshFilter>().sharedMesh;
			this.verts = this.mesh.vertices;
			this.colors = this.mesh.colors;
		}
		return this.mesh;
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x000700F8 File Offset: 0x0006E2F8
	internal bool PointIsValid(Vector3 point)
	{
		if (this.vertexColorMask)
		{
			this.GetMesh();
			if (HelperFunctions.GetValue(HelperFunctions.GetVertexColorAtPoint(this.verts, this.colors, base.transform, point)) < 0.9f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400148A RID: 5258
	public bool vertexColorMask;

	// Token: 0x0400148B RID: 5259
	private Mesh mesh;

	// Token: 0x0400148C RID: 5260
	private Vector3[] verts;

	// Token: 0x0400148D RID: 5261
	private Color[] colors;
}
