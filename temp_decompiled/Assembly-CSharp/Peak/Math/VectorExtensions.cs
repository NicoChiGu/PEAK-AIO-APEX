using System;
using UnityEngine;

namespace Peak.Math
{
	// Token: 0x020003D0 RID: 976
	public static class VectorExtensions
	{
		// Token: 0x06001927 RID: 6439 RVA: 0x0007D3BE File Offset: 0x0007B5BE
		public static bool NearZero(this Vector3 self)
		{
			return self.sqrMagnitude <= 0.0001f;
		}
	}
}
