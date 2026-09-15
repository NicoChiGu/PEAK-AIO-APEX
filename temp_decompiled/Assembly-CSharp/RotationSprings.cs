using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031A RID: 794
public class RotationSprings : MonoBehaviour
{
	// Token: 0x06001473 RID: 5235 RVA: 0x00067F80 File Offset: 0x00066180
	private void Update()
	{
		Transform parent = base.transform.parent;
		Vector3 forward = parent.forward;
		Vector3 up = parent.up;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < this.springs.Count; i++)
		{
			this.springs[i].DoUpdate(forward, up);
		}
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x00067FD4 File Offset: 0x000661D4
	public void AddForce(Vector3 force, float spring, float drag)
	{
		RotationSprings.RotationSpringInstance rotationSpringInstance = new RotationSprings.RotationSpringInstance();
		rotationSpringInstance.spring = spring;
		rotationSpringInstance.drag = drag;
		rotationSpringInstance.forward = base.transform.parent.forward;
		rotationSpringInstance.up = base.transform.parent.up;
	}

	// Token: 0x0400130C RID: 4876
	public List<RotationSprings.RotationSpringInstance> springs = new List<RotationSprings.RotationSpringInstance>();

	// Token: 0x02000507 RID: 1287
	[Serializable]
	public class RotationSpringInstance
	{
		// Token: 0x06001D57 RID: 7511 RVA: 0x00087B7C File Offset: 0x00085D7C
		public void DoUpdate(Vector3 targetForward, Vector3 targetUp)
		{
			Vector3 vector = Vector3.Cross(this.forward, targetForward) * Vector3.Angle(this.forward, targetForward);
			vector += Vector3.Cross(this.up, targetUp) * Vector3.Angle(this.up, targetUp);
			this.vel = FRILerp.Lerp(this.vel, vector * this.spring, this.drag, true);
			this.forward = Quaternion.AngleAxis(Time.deltaTime * this.vel.magnitude, this.vel) * this.forward;
			this.up = Quaternion.AngleAxis(Time.deltaTime * this.vel.magnitude, this.vel) * this.up;
		}

		// Token: 0x04001B4F RID: 6991
		public float spring;

		// Token: 0x04001B50 RID: 6992
		public float drag;

		// Token: 0x04001B51 RID: 6993
		public Vector3 vel;

		// Token: 0x04001B52 RID: 6994
		public Vector3 forward;

		// Token: 0x04001B53 RID: 6995
		public Vector3 up;
	}
}
