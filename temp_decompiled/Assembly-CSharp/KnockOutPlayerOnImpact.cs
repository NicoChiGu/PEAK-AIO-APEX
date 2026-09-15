using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class KnockOutPlayerOnImpact : ItemComponent
{
	// Token: 0x0600091D RID: 2333 RVA: 0x00030A89 File Offset: 0x0002EC89
	private void Start()
	{
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00030A8B File Offset: 0x0002EC8B
	private void FixedUpdate()
	{
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00030A8D File Offset: 0x0002EC8D
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00030A8F File Offset: 0x0002EC8F
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x0400088C RID: 2188
	public float knockoutVelocity;

	// Token: 0x0400088D RID: 2189
	public float damage;

	// Token: 0x0400088E RID: 2190
	public float forceMult;
}
