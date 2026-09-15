using System;
using UnityEngine;

// Token: 0x0200003A RID: 58
public struct RigidbodySyncData
{
	// Token: 0x060003A5 RID: 933 RVA: 0x0001843F File Offset: 0x0001663F
	public RigidbodySyncData(Rigidbody rig)
	{
		this.position = rig.position;
		this.rotation = rig.rotation;
	}

	// Token: 0x040003FC RID: 1020
	public Vector3 position;

	// Token: 0x040003FD RID: 1021
	public Quaternion rotation;
}
