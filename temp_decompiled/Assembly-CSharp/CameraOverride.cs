using System;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class CameraOverride : MonoBehaviour
{
	// Token: 0x06001014 RID: 4116 RVA: 0x0004FDC9 File Offset: 0x0004DFC9
	public void DoOverride()
	{
		MainCamera.instance.SetCameraOverride(this);
	}

	// Token: 0x04000E80 RID: 3712
	public float fov = 35f;
}
