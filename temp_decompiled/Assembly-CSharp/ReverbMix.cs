using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000164 RID: 356
public class ReverbMix : MonoBehaviour
{
	// Token: 0x06000B5A RID: 2906 RVA: 0x0003C8AC File Offset: 0x0003AAAC
	private void Start()
	{
		this.audioMixerGroup.audioMixer.GetFloat("EffectsStrength", ref this.startReverbStrength);
		this.audioMixerGroup.audioMixer.SetFloat("EffectsStrength", this.reverbStrength);
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x0003C8E6 File Offset: 0x0003AAE6
	private void Update()
	{
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x0003C8E8 File Offset: 0x0003AAE8
	private void OnDisable()
	{
		this.audioMixerGroup.audioMixer.SetFloat("EffectsStrength", this.startReverbStrength);
	}

	// Token: 0x04000A82 RID: 2690
	public AudioMixerGroup audioMixerGroup;

	// Token: 0x04000A83 RID: 2691
	private float startReverbStrength;

	// Token: 0x04000A84 RID: 2692
	public float reverbStrength;
}
