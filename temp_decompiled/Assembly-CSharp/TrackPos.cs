using System;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class TrackPos : MonoBehaviour
{
	// Token: 0x06000D88 RID: 3464 RVA: 0x00043C72 File Offset: 0x00041E72
	private void Start()
	{
		this.startPos = base.transform.position;
		this.startRot = base.transform.rotation;
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x00043C98 File Offset: 0x00041E98
	private void Update()
	{
		if (this.trackPos)
		{
			base.transform.position = this.trackTransform.position + this.startPos;
		}
		if (this.trackRot)
		{
			base.transform.rotation = this.trackTransform.rotation * this.startRot;
		}
	}

	// Token: 0x04000BA8 RID: 2984
	public Transform trackTransform;

	// Token: 0x04000BA9 RID: 2985
	private Vector3 startPos;

	// Token: 0x04000BAA RID: 2986
	private Quaternion startRot;

	// Token: 0x04000BAB RID: 2987
	public bool trackPos;

	// Token: 0x04000BAC RID: 2988
	public bool trackRot;
}
