using System;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public static class Vector3Extensions
{
	// Token: 0x060007BA RID: 1978 RVA: 0x0002AFB9 File Offset: 0x000291B9
	public static Vector2 XZ(this Vector3 vector)
	{
		return new Vector2(vector.x, vector.z);
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0002AFCC File Offset: 0x000291CC
	public static Vector3 Flat(this Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0002AFE4 File Offset: 0x000291E4
	public static bool Same(this Vector3 v1, Vector3 v2, float threshold = 0.01f)
	{
		return Mathf.Abs(v1.x - v2.x) < threshold && Mathf.Abs(v1.y - v2.y) < threshold && Mathf.Abs(v1.z - v2.z) < threshold;
	}
}
