using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class AvaragePosition : MonoBehaviour
{
	// Token: 0x06000F73 RID: 3955 RVA: 0x0004CC20 File Offset: 0x0004AE20
	private void Update()
	{
		base.transform.position = (this.p1.position + this.p2.position) / 2f;
	}

	// Token: 0x04000DC7 RID: 3527
	public Transform p1;

	// Token: 0x04000DC8 RID: 3528
	public Transform p2;
}
