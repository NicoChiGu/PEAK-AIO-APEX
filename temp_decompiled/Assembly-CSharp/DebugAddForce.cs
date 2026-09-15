using System;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class DebugAddForce : ItemComponent
{
	// Token: 0x060005C9 RID: 1481 RVA: 0x00021136 File Offset: 0x0001F336
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00021138 File Offset: 0x0001F338
	private void FixedUpdate()
	{
		if (this.item.itemState == ItemState.Ground && this.item.photonView.IsMine && !this.item.rig.isKinematic)
		{
			this.item.rig.linearVelocity = Vector3.right * this.force;
		}
	}

	// Token: 0x040005F1 RID: 1521
	public float force;
}
