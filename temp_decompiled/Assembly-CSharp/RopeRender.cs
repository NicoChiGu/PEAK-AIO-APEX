using System;
using UnityEngine;

// Token: 0x02000310 RID: 784
[Serializable]
public class RopeRender
{
	// Token: 0x0600144A RID: 5194 RVA: 0x000673AC File Offset: 0x000655AC
	public void DisplayRope(Vector3 from, Vector3 to, float time, LineRenderer line)
	{
		line.enabled = true;
		float num = Mathf.Lerp(1f, 0f, time);
		for (int i = 0; i < line.positionCount; i++)
		{
			float num2 = (float)i / ((float)line.positionCount - 1f);
			Vector3 vector = Vector3.Lerp(from, to, num2);
			float num3 = (float)i * this.scale;
			float num4 = time * this.scrollSpeed;
			float num5 = Mathf.Cos(num3 + num4);
			vector += num5 * line.transform.up * num * this.wobbleCurve.Evaluate(time) * this.wobbleOverLineCurve.Evaluate(num2);
			line.SetPosition(i, vector);
		}
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x0006746D File Offset: 0x0006566D
	internal void StopRend(LineRenderer line)
	{
		line.enabled = false;
	}

	// Token: 0x040012E3 RID: 4835
	public float wobble = 1f;

	// Token: 0x040012E4 RID: 4836
	public float scrollSpeed = 1f;

	// Token: 0x040012E5 RID: 4837
	public float scale = 0.3f;

	// Token: 0x040012E6 RID: 4838
	public AnimationCurve wobbleCurve;

	// Token: 0x040012E7 RID: 4839
	public AnimationCurve wobbleOverLineCurve;
}
