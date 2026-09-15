using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class CookingBehavior_ModifyAudioSourcePitch : AdditionalCookingBehavior
{
	// Token: 0x0600042F RID: 1071 RVA: 0x0001A494 File Offset: 0x00018694
	protected override void TriggerBehaviour(int cookedAmount)
	{
		for (int i = 0; i < cookedAmount; i++)
		{
			this.source.pitch += this.changePerCooking;
		}
	}

	// Token: 0x040004B4 RID: 1204
	public float changePerCooking;

	// Token: 0x040004B5 RID: 1205
	public AudioSource source;
}
