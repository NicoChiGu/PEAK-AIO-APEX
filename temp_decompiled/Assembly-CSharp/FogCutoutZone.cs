using System;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class FogCutoutZone : MonoBehaviour
{
	// Token: 0x06001152 RID: 4434 RVA: 0x000570A4 File Offset: 0x000552A4
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 1f, 1f, this.amount);
		Gizmos.DrawWireSphere(base.transform.position, this.min);
		Gizmos.DrawWireSphere(base.transform.position, this.max);
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position + Vector3.forward * this.transitionPoint, new Vector3(300f, 9999f, 0.1f));
	}

	// Token: 0x04000FCA RID: 4042
	public float min = 10f;

	// Token: 0x04000FCB RID: 4043
	public float max = 100f;

	// Token: 0x04000FCC RID: 4044
	public float amount = 1f;

	// Token: 0x04000FCD RID: 4045
	public float transitionPoint;
}
