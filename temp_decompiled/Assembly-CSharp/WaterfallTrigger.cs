using System;
using UnityEngine;

// Token: 0x02000372 RID: 882
public class WaterfallTrigger : MonoBehaviour
{
	// Token: 0x06001663 RID: 5731 RVA: 0x00073FDC File Offset: 0x000721DC
	private void OnTriggerStay(Collider other)
	{
		Character componentInParent = other.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (this.ragdoll)
		{
			componentInParent.Fall(this.ragdollLength, 0f);
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (!attachedRigidbody)
		{
			return;
		}
		Vector3 vector = other.transform.position - base.transform.position;
		attachedRigidbody.AddForce(vector * this.force, 1);
		attachedRigidbody.AddForce(base.transform.forward * this.force / 4f, 5);
		attachedRigidbody.linearVelocity *= this.drag;
	}

	// Token: 0x0400154A RID: 5450
	public float force = 5f;

	// Token: 0x0400154B RID: 5451
	public float drag = 0.9f;

	// Token: 0x0400154C RID: 5452
	public bool ragdoll;

	// Token: 0x0400154D RID: 5453
	public float ragdollLength = 1f;
}
