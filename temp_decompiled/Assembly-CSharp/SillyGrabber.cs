using System;
using UnityEngine;

// Token: 0x02000332 RID: 818
[ExecuteAlways]
public class SillyGrabber : MonoBehaviour
{
	// Token: 0x0600151B RID: 5403 RVA: 0x0006BBD8 File Offset: 0x00069DD8
	private void Update()
	{
		this.grabberArm.localScale = new Vector3(this.distance * this.mult, 1f, this.lengthToWidth.Evaluate(this.distance));
		this.grabberClaw.localScale = new Vector3(this.clawMult / this.distance, 1f, 1f / this.lengthToWidth.Evaluate(this.distance));
		this.grabberClaw2.localScale = new Vector3(this.clawMult / this.distance, 1f, 1f / this.lengthToWidth.Evaluate(this.distance));
		this.grabberClaw.localPosition = new Vector3(-22.3f, 0f, -this.distance * 0.1f);
		this.grabberClaw2.localPosition = new Vector3(-22.3f, 0f, this.distance * 0.1f);
	}

	// Token: 0x040013A7 RID: 5031
	public float distance = 1f;

	// Token: 0x040013A8 RID: 5032
	public float mult = 1f;

	// Token: 0x040013A9 RID: 5033
	public float clawMult = 1f;

	// Token: 0x040013AA RID: 5034
	public AnimationCurve lengthToWidth;

	// Token: 0x040013AB RID: 5035
	public Transform grabberArm;

	// Token: 0x040013AC RID: 5036
	public Transform grabberClaw;

	// Token: 0x040013AD RID: 5037
	public Transform grabberClaw2;
}
