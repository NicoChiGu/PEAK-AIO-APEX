using System;
using UnityEngine;

// Token: 0x02000131 RID: 305
public class LazyGizmo : MonoBehaviour
{
	// Token: 0x06000990 RID: 2448 RVA: 0x00032BD8 File Offset: 0x00030DD8
	private void DrawGizmos()
	{
		Gizmos.color = this.color;
		if (this.useTop)
		{
			Gizmos.DrawSphere(base.transform.position - Vector3.up * this.radius, this.radius);
			return;
		}
		Gizmos.DrawSphere(base.transform.position, this.radius);
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x00032C3A File Offset: 0x00030E3A
	private void OnDrawGizmos()
	{
		if (!this.onSelected)
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x00032C4C File Offset: 0x00030E4C
	private void OnDrawGizmosSelected()
	{
		if (this.onSelected)
		{
			this.DrawGizmos();
		}
		if (this.showArrows)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.up * (this.radius + 1f));
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * (this.radius + 1f));
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.right * (this.radius + 1f));
		}
	}

	// Token: 0x04000906 RID: 2310
	public bool onSelected = true;

	// Token: 0x04000907 RID: 2311
	public bool useTop;

	// Token: 0x04000908 RID: 2312
	public Color color;

	// Token: 0x04000909 RID: 2313
	public float radius;

	// Token: 0x0400090A RID: 2314
	public bool showArrows;
}
