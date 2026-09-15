using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class Arrow : MonoBehaviour
{
	// Token: 0x06000461 RID: 1121 RVA: 0x0001AD76 File Offset: 0x00018F76
	private void Start()
	{
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x0001AD78 File Offset: 0x00018F78
	public void stuckArrow(bool stuck)
	{
		this.isStuck = stuck;
		this.arrowRB.isKinematic = stuck;
		this.arrowCollider.enabled = !stuck;
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x0001AD9C File Offset: 0x00018F9C
	private void Update()
	{
		if (base.transform.parent == null && this.isStuck)
		{
			this.stuckArrow(false);
		}
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x0001ADC0 File Offset: 0x00018FC0
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x040004E9 RID: 1257
	public bool isStuck = true;

	// Token: 0x040004EA RID: 1258
	public Rigidbody arrowRB;

	// Token: 0x040004EB RID: 1259
	public Collider arrowCollider;
}
