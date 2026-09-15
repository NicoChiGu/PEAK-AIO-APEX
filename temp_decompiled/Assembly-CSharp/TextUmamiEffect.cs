using System;
using UnityEngine;

// Token: 0x020001EA RID: 490
public class TextUmamiEffect : DialogueEffect
{
	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000EED RID: 3821 RVA: 0x00049504 File Offset: 0x00047704
	public virtual float colorSpeedMult
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x0004950C File Offset: 0x0004770C
	public override void UpdateCharacter(int index)
	{
		float num = this.offset * (float)index;
		float num2 = Mathf.Sin((Time.time + num) / this.period);
		float num3 = 1f + num2 * this.amplitude;
		Vector3 vector = Vector3.one * num3;
		this.DTanimator.SetCharScale(index, vector);
		this.DTanimator.SetCharOffset(index, Vector3.up * num3 * this.charOffset);
		float num4 = (Mathf.Sin((Time.time + num) / (this.period / this.colorSpeedMult)) + 1f) * 0.5f;
		this.DTanimator.SetCharColor(index, this.colorGradient.Evaluate(num4));
	}

	// Token: 0x04000CF7 RID: 3319
	public bool abs;

	// Token: 0x04000CF8 RID: 3320
	public float amplitude = 0.2f;

	// Token: 0x04000CF9 RID: 3321
	public float period = 0.5f;

	// Token: 0x04000CFA RID: 3322
	public float offset = 0.1f;

	// Token: 0x04000CFB RID: 3323
	public float charOffset = 10f;

	// Token: 0x04000CFC RID: 3324
	public Gradient colorGradient;
}
