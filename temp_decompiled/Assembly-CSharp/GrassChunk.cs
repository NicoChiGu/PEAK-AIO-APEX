using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class GrassChunk : GrassDataProvider
{
	// Token: 0x06000713 RID: 1811 RVA: 0x00028017 File Offset: 0x00026217
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(base.transform.position + Vector3.one * 50f, Vector3.one * GrassChunking.CHUNK_SIZE);
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00028056 File Offset: 0x00026256
	public override bool IsDirty()
	{
		return this.isDirty;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0002805E File Offset: 0x0002625E
	public override ComputeBuffer GetData()
	{
		ComputeBuffer computeBuffer = new ComputeBuffer(this.GrassPoints.Count, UnsafeUtility.SizeOf<GrassPoint>());
		computeBuffer.SetData<GrassPoint>(this.GrassPoints);
		this.isDirty = false;
		return computeBuffer;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00028088 File Offset: 0x00026288
	public void SetData(List<GrassPoint> grassPoints)
	{
		this.GrassPoints = grassPoints;
		this.isDirty = true;
	}

	// Token: 0x040006FA RID: 1786
	public List<GrassPoint> GrassPoints;

	// Token: 0x040006FB RID: 1787
	public bool isDirty = true;
}
