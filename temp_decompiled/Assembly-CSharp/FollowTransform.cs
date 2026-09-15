using System;
using UnityEngine;

// Token: 0x02000264 RID: 612
public class FollowTransform : MonoBehaviour
{
	// Token: 0x06001162 RID: 4450 RVA: 0x000575A9 File Offset: 0x000557A9
	private void LateUpdate()
	{
		if (this.t)
		{
			base.transform.position = this.t.position;
		}
	}

	// Token: 0x04000FE0 RID: 4064
	public Transform t;
}
