using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
public static class Extens
{
	// Token: 0x06000365 RID: 869 RVA: 0x00017434 File Offset: 0x00015634
	public static Vector3 EulerRescaled(this Quaternion quaternion)
	{
		Vector3 eulerAngles = quaternion.eulerAngles;
		return new Vector3(Mathf.Repeat(eulerAngles.x + 180f, 360f) - 180f, Mathf.Repeat(eulerAngles.y + 180f, 360f) - 180f, Mathf.Repeat(eulerAngles.z + 180f, 360f) - 180f);
	}

	// Token: 0x06000366 RID: 870 RVA: 0x000174A2 File Offset: 0x000156A2
	public static Quaternion Inverse(this Quaternion quaterion)
	{
		return Quaternion.Inverse(quaterion);
	}
}
