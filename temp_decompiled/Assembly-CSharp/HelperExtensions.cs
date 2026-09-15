using System;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public static class HelperExtensions
{
	// Token: 0x06000775 RID: 1909 RVA: 0x00029DC3 File Offset: 0x00027FC3
	public static LayerMask ToLayerMask(this HelperFunctions.LayerType me)
	{
		return HelperFunctions.GetMask(me);
	}
}
