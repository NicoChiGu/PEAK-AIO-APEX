using System;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class MagicBeanVine : MonoBehaviour
{
	// Token: 0x06000946 RID: 2374 RVA: 0x000312D0 File Offset: 0x0002F4D0
	private void Awake()
	{
		this.currentLength = this.initialLength;
		float num = this.currentLength / this.maxLength;
		float num2 = this.xzScaleCurve.Evaluate(num) * this.maxWidth;
		this.vineOriginTransform.transform.localScale = new Vector3(num2, this.currentLength, num2);
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x00031328 File Offset: 0x0002F528
	private void FixedUpdate()
	{
		if (this.currentLength < this.maxLength)
		{
			this.currentLength = Mathf.MoveTowards(this.currentLength, this.maxLength, this.growingSpeed * Time.fixedDeltaTime);
			float num = this.currentLength / this.maxLength;
			float num2 = this.xzScaleCurve.Evaluate(num) * this.maxWidth;
			this.vineOriginTransform.transform.localScale = new Vector3(num2, this.currentLength, num2);
			this.vineOriginTransform.transform.Rotate(0f, this.rotationSpeed * this.rotationSpeedCurve.Evaluate(num), 0f);
		}
	}

	// Token: 0x040008A1 RID: 2209
	public Transform vineOriginTransform;

	// Token: 0x040008A2 RID: 2210
	public float maxWidth = 1.5f;

	// Token: 0x040008A3 RID: 2211
	public float maxLength = 20f;

	// Token: 0x040008A4 RID: 2212
	public float initialLength = 0.5f;

	// Token: 0x040008A5 RID: 2213
	private float currentLength = 0.01f;

	// Token: 0x040008A6 RID: 2214
	public float growingSpeed = 1f;

	// Token: 0x040008A7 RID: 2215
	public float rotationSpeed = 10f;

	// Token: 0x040008A8 RID: 2216
	public AnimationCurve xzScaleCurve;

	// Token: 0x040008A9 RID: 2217
	public AnimationCurve rotationSpeedCurve;
}
