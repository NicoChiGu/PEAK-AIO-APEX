using System;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public class WobbleSpinBounce : MonoBehaviour
{
	// Token: 0x06000F1A RID: 3866 RVA: 0x00049EAC File Offset: 0x000480AC
	private void Start()
	{
		if (this.target == null)
		{
			this.target = base.transform;
		}
		this.startPos = this.target.position;
		this.startRot = base.transform.eulerAngles;
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x00049EEC File Offset: 0x000480EC
	private void Update()
	{
		this.target.Rotate(this.rotateSpeed);
		if (this.bounceSize != Vector3.zero)
		{
			this.target.transform.position = this.startPos + new Vector3(Mathf.Sin(Time.time * this.bounceSpeed.x) * this.bounceSize.x, Mathf.Sin(Time.time * this.bounceSpeed.y) * this.bounceSize.y, Mathf.Sin(Time.time * this.bounceSpeed.z) * this.bounceSize.z);
		}
		if (this.wobbleAmount != Vector3.zero)
		{
			this.target.transform.eulerAngles = this.startRot + new Vector3(Mathf.Sin(Time.time * this.wobbleSpeed.x) * this.wobbleAmount.x, Mathf.Sin(Time.time * this.wobbleSpeed.y) * this.wobbleAmount.y, Mathf.Sin(Time.time * this.wobbleSpeed.z) * this.wobbleAmount.z);
		}
	}

	// Token: 0x04000D19 RID: 3353
	public Transform target;

	// Token: 0x04000D1A RID: 3354
	[Header("Rotate")]
	public Vector3 rotateSpeed;

	// Token: 0x04000D1B RID: 3355
	public Vector3 wobbleSpeed;

	// Token: 0x04000D1C RID: 3356
	public Vector3 wobbleAmount;

	// Token: 0x04000D1D RID: 3357
	[Header("Position")]
	public Vector3 bounceSize;

	// Token: 0x04000D1E RID: 3358
	public Vector3 bounceSpeed;

	// Token: 0x04000D1F RID: 3359
	private Vector3 startPos;

	// Token: 0x04000D20 RID: 3360
	private Vector3 startRot;
}
