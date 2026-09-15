using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020000BF RID: 191
public static class GrassChunking
{
	// Token: 0x06000718 RID: 1816 RVA: 0x000280A8 File Offset: 0x000262A8
	public static int3 GetChunkFromPosition(float3 p)
	{
		int num = Mathf.FloorToInt(p.x * GrassChunking.CHUNK_SIZE_INV);
		int num2 = Mathf.FloorToInt(p.y * GrassChunking.CHUNK_SIZE_INV);
		int num3 = Mathf.FloorToInt(p.z * GrassChunking.CHUNK_SIZE_INV);
		return new int3(num, num2, num3);
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x000280F4 File Offset: 0x000262F4
	public static bool ShouldDrawChunk(int3 cameraChunk, int3 renderChunk)
	{
		return Mathf.Abs(cameraChunk.x - renderChunk.x) <= 1 && Mathf.Abs(cameraChunk.y - renderChunk.y) <= 1 && Mathf.Abs(cameraChunk.z - renderChunk.z) <= 1;
	}

	// Token: 0x040006FC RID: 1788
	public static readonly float CHUNK_SIZE = 35f;

	// Token: 0x040006FD RID: 1789
	public static readonly float CHUNK_SIZE_INV = 1f / GrassChunking.CHUNK_SIZE;
}
