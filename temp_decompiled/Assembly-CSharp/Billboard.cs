using System;
using UnityEngine;

// Token: 0x0200020F RID: 527
public class Billboard : MonoBehaviour
{
	// Token: 0x06000F96 RID: 3990 RVA: 0x0004D778 File Offset: 0x0004B978
	private void LateUpdate()
	{
		Vector3 vector = MainCamera.instance.transform.position - base.transform.position;
		if (vector.sqrMagnitude < 0.001f)
		{
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(-vector);
	}
}
