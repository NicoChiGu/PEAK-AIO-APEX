using System;
using UnityEngine;

// Token: 0x02000243 RID: 579
public class DebugStep : MonoBehaviour
{
	// Token: 0x060010F5 RID: 4341 RVA: 0x00055B8B File Offset: 0x00053D8B
	private void FixedUpdate()
	{
		if (this.stepType == DebugStep.StepType.FixedUpdate)
		{
			Debug.Break();
		}
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x00055B9B File Offset: 0x00053D9B
	private void Update()
	{
		if (this.stepType == DebugStep.StepType.Update)
		{
			Debug.Break();
		}
	}

	// Token: 0x04000F6E RID: 3950
	public DebugStep.StepType stepType;

	// Token: 0x020004D7 RID: 1239
	public enum StepType
	{
		// Token: 0x04001AB1 RID: 6833
		Update,
		// Token: 0x04001AB2 RID: 6834
		FixedUpdate
	}
}
