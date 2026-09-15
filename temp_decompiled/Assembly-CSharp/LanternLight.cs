using System;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class LanternLight : MonoBehaviour
{
	// Token: 0x0600098D RID: 2445 RVA: 0x00032B4F File Offset: 0x00030D4F
	private void Start()
	{
		this.startIntensity = this.light.intensity;
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x00032B64 File Offset: 0x00030D64
	private void Update()
	{
		if (!this.light.enabled)
		{
			return;
		}
		float num = this.flickerCurve.Evaluate(Time.deltaTime * this.flickerSpeed) * this.flickerCurve.Evaluate(Time.deltaTime * this.flickerSpeed * 0.38374f);
		this.light.intensity = this.startIntensity + num * this.flickerAmount;
	}

	// Token: 0x04000901 RID: 2305
	public Light light;

	// Token: 0x04000902 RID: 2306
	public float flickerSpeed;

	// Token: 0x04000903 RID: 2307
	public float flickerAmount;

	// Token: 0x04000904 RID: 2308
	public AnimationCurve flickerCurve;

	// Token: 0x04000905 RID: 2309
	private float startIntensity;
}
