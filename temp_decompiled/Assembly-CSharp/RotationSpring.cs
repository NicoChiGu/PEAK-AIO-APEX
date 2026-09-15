using System;
using UnityEngine;

// Token: 0x02000319 RID: 793
public class RotationSpring : MonoBehaviour
{
	// Token: 0x06001470 RID: 5232 RVA: 0x00067E98 File Offset: 0x00066098
	private void Update()
	{
		Transform parent = base.transform.parent;
		Vector3 forward = parent.forward;
		Vector3 up = parent.up;
		Vector3 vector = Vector3.Cross(base.transform.forward, forward).normalized * Vector3.Angle(base.transform.forward, forward);
		vector += Vector3.Cross(base.transform.up, up).normalized * Vector3.Angle(base.transform.up, up);
		this.vel = FRILerp.Lerp(this.vel, vector * this.spring, this.drag, true);
		base.transform.Rotate(this.vel * Time.deltaTime, 0);
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x00067F63 File Offset: 0x00066163
	public void AddForce(Vector3 force)
	{
		this.vel += force;
	}

	// Token: 0x04001309 RID: 4873
	public float spring;

	// Token: 0x0400130A RID: 4874
	public float drag;

	// Token: 0x0400130B RID: 4875
	private Vector3 vel;
}
