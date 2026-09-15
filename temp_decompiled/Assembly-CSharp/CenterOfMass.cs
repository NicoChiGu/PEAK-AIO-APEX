using System;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class CenterOfMass : MonoBehaviour
{
	// Token: 0x0600101A RID: 4122 RVA: 0x0004FE8C File Offset: 0x0004E08C
	private void Start()
	{
		if (this.onlyOnGround)
		{
			this.item = base.GetComponent<Item>();
			if (this.item.itemState != ItemState.Ground)
			{
				return;
			}
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.centerOfMass = this.localCenterOfMass;
		this.rb.angularDamping = this.angularDamping;
		if (this.centerOfMassTransform)
		{
			this.rb.centerOfMass = this.centerOfMassTransform.localPosition;
		}
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x0004FF0C File Offset: 0x0004E10C
	private void FixedUpdate()
	{
		if (this.onlyOnGround && this.item.itemState != ItemState.Ground)
		{
			return;
		}
		if (this.centerOfMassTransform)
		{
			this.rb.centerOfMass = this.centerOfMassTransform.localPosition;
		}
		else
		{
			this.rb.centerOfMass = this.localCenterOfMass;
		}
		this.rb.angularDamping = this.angularDamping;
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x0004FF76 File Offset: 0x0004E176
	private void OnDrawGizmosSelected()
	{
		if (this.rb)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(this.rb.worldCenterOfMass, 0.5f);
		}
	}

	// Token: 0x04000E85 RID: 3717
	public bool onlyOnGround;

	// Token: 0x04000E86 RID: 3718
	private Item item;

	// Token: 0x04000E87 RID: 3719
	public Transform centerOfMassTransform;

	// Token: 0x04000E88 RID: 3720
	public Vector3 localCenterOfMass;

	// Token: 0x04000E89 RID: 3721
	public float angularDamping = 3f;

	// Token: 0x04000E8A RID: 3722
	private Rigidbody rb;
}
