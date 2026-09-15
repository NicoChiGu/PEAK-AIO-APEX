using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class CookingBehavior_MessUpAudio : AdditionalCookingBehavior
{
	// Token: 0x06000427 RID: 1063 RVA: 0x0001A3B2 File Offset: 0x000185B2
	protected override void TriggerBehaviour(int cookedAmount)
	{
		this.source.pitch -= this.pitchReductionPerCooking * (float)cookedAmount;
		this.source.volume -= this.volumeReductionPerCooking * (float)cookedAmount;
	}

	// Token: 0x040004AE RID: 1198
	public AudioSource source;

	// Token: 0x040004AF RID: 1199
	public float pitchReductionPerCooking;

	// Token: 0x040004B0 RID: 1200
	public float volumeReductionPerCooking;
}
