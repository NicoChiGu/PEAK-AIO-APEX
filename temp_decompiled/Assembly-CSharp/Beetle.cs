using System;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class Beetle : Mob
{
	// Token: 0x06000A30 RID: 2608 RVA: 0x00035F88 File Offset: 0x00034188
	protected override void InflictAttack(Character character)
	{
		if (character.IsLocal)
		{
			character.Fall(this.ragdollTime, 0f);
			character.AddForceAtPosition(base.transform.forward * this.bonkForce + base.transform.up * this.bonkForceUp, this.bonkPoint.position, this.bonkRange);
		}
	}

	// Token: 0x04000990 RID: 2448
	public float bonkForce = 100f;

	// Token: 0x04000991 RID: 2449
	public float bonkForceUp = 100f;

	// Token: 0x04000992 RID: 2450
	public float bonkRange = 4f;

	// Token: 0x04000993 RID: 2451
	public float ragdollTime = 2f;

	// Token: 0x04000994 RID: 2452
	public Transform bonkPoint;
}
