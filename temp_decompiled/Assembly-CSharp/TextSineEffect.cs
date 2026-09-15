using System;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class TextSineEffect : DialogueEffect
{
	// Token: 0x06000EEB RID: 3819 RVA: 0x0004946C File Offset: 0x0004766C
	public override void UpdateCharacter(int index)
	{
		float num = this.offset * (float)index;
		Vector3 vector = Vector3.up * (Mathf.Sin((Time.time + num) / this.period) * this.amplitude);
		if (this.abs)
		{
			vector..ctor(vector.x, Mathf.Abs(vector.y), vector.z);
		}
		this.DTanimator.SetCharOffset(index, vector);
	}

	// Token: 0x04000CF3 RID: 3315
	public bool abs;

	// Token: 0x04000CF4 RID: 3316
	public float amplitude = 3f;

	// Token: 0x04000CF5 RID: 3317
	public float period = 0.15f;

	// Token: 0x04000CF6 RID: 3318
	public float offset = 0.1f;
}
