using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020002D0 RID: 720
[ModifierID("Round")]
public class RoundModifier : ProceduralImageModifier
{
	// Token: 0x06001369 RID: 4969 RVA: 0x000628ED File Offset: 0x00060AED
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		float num = Mathf.Min(imageRect.width, imageRect.height) * 0.5f;
		return new Vector4(num, num, num, num);
	}
}
