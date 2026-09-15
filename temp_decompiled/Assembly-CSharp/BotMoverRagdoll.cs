using System;
using UnityEngine;

// Token: 0x02000069 RID: 105
public class BotMoverRagdoll : MonoBehaviour
{
	// Token: 0x060004E6 RID: 1254 RVA: 0x0001D02A File Offset: 0x0001B22A
	private void Awake()
	{
		this.bot = base.GetComponent<Bot>();
		this.rig_g = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0001D044 File Offset: 0x0001B244
	private void Start()
	{
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0001D048 File Offset: 0x0001B248
	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		this.rig_g.AddForce(base.transform.forward * (this.bot.MovementInput.y * (this.movementSpeed * fixedDeltaTime)), 5);
		Vector3 up = Vector3.up;
		Vector3 lookDirection = this.bot.LookDirection;
		Vector3 vector = Vector3.Cross(base.transform.up, up).normalized * Vector3.Angle(base.transform.up, up);
		Vector3 vector2 = Vector3.Cross(base.transform.forward, lookDirection).normalized * Vector3.Angle(base.transform.forward, lookDirection);
		this.rig_g.angularVelocity = FRILerp.PLerp(this.rig_g.angularVelocity, (vector2 + vector) * this.rotSpring, this.rotDamp, fixedDeltaTime);
	}

	// Token: 0x04000551 RID: 1361
	private Bot bot;

	// Token: 0x04000552 RID: 1362
	public float movementSpeed;

	// Token: 0x04000553 RID: 1363
	private Rigidbody rig_g;

	// Token: 0x04000554 RID: 1364
	private Vector3 angularVel;

	// Token: 0x04000555 RID: 1365
	public float rotSpring = 15f;

	// Token: 0x04000556 RID: 1366
	public float rotDamp = 35f;
}
