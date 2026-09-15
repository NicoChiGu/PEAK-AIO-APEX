using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x02000136 RID: 310
[ExecuteInEditMode]
public class LogoCombineSequence : MonoBehaviour
{
	// Token: 0x060009DE RID: 2526 RVA: 0x0003465C File Offset: 0x0003285C
	private void Start()
	{
		if (this.volume.profile.TryGet<ChromaticAberration>(ref this.chromaticAberration))
		{
			this.chromaticAberration.intensity.value = 0f;
		}
		if (this.volume.profile.TryGet<Bloom>(ref this.bloom))
		{
			this.bloom.intensity.value = 0f;
		}
		if (this.volume.profile.TryGet<LensDistortion>(ref this.lensDistortion))
		{
			this.lensDistortion.intensity.value = 0f;
		}
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x000346F0 File Offset: 0x000328F0
	private void Update()
	{
		if (this.chromaticAberration != null)
		{
			this.chromaticAberration.intensity.value = this.chromaticAmplitude;
		}
		if (this.bloom != null)
		{
			this.bloom.intensity.value = this.bloomIntensity;
		}
		if (this.lensDistortion != null)
		{
			this.lensDistortion.intensity.value = this.lensIntensity;
			this.lensDistortion.scale.value = this.lensScale;
		}
		this.material.SetFloat("_StreakAmount", this.streakAmount);
		this.material.SetFloat("_StretchAmount", this.stretchAmount);
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x000347AC File Offset: 0x000329AC
	private void OnValidate()
	{
		if (this.bloom != null)
		{
			this.bloom.intensity.value = this.bloomIntensity;
		}
		if (this.chromaticAberration != null)
		{
			this.chromaticAberration.intensity.value = this.chromaticAmplitude;
		}
		if (this.lensDistortion != null)
		{
			this.lensDistortion.intensity.value = this.lensIntensity;
			this.lensDistortion.scale.value = this.lensScale;
		}
		this.material.SetFloat("_StreakAmount", this.streakAmount);
		this.material.SetFloat("_StretchAmount", this.stretchAmount);
	}

	// Token: 0x04000949 RID: 2377
	public float streakAmount;

	// Token: 0x0400094A RID: 2378
	public float stretchAmount;

	// Token: 0x0400094B RID: 2379
	public float chromaticAmplitude;

	// Token: 0x0400094C RID: 2380
	public float lensScale;

	// Token: 0x0400094D RID: 2381
	public float lensIntensity;

	// Token: 0x0400094E RID: 2382
	public float bloomIntensity;

	// Token: 0x0400094F RID: 2383
	public Material material;

	// Token: 0x04000950 RID: 2384
	public Volume volume;

	// Token: 0x04000951 RID: 2385
	private ChromaticAberration chromaticAberration;

	// Token: 0x04000952 RID: 2386
	private Bloom bloom;

	// Token: 0x04000953 RID: 2387
	private LensDistortion lensDistortion;
}
