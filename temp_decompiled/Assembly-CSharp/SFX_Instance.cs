using System;
using UnityEngine;

// Token: 0x020001A4 RID: 420
[CreateAssetMenu(fileName = "SoundEffectInstance", menuName = "Landfall/SoundEffectInstance")]
public class SFX_Instance : ScriptableObject
{
	// Token: 0x06000D20 RID: 3360 RVA: 0x000422FE File Offset: 0x000404FE
	public AudioClip GetClip()
	{
		return this.clips[Random.Range(0, this.clips.Length)];
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x00042315 File Offset: 0x00040515
	public void Play(Vector3 pos = default(Vector3))
	{
		SFX_Player.instance.PlaySFX(this, pos, null, null, 1f, false);
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x0004232C File Offset: 0x0004052C
	public void Play(Vector3 position, Transform follow)
	{
		SFX_Player.instance.PlaySFX(this, position, follow, null, 1f, false);
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00042344 File Offset: 0x00040544
	public void PlayFromSource(Vector3 position, AudioSource source)
	{
		if (source.isPlaying)
		{
			source.Stop();
		}
		source.clip = this.GetClip();
		source.volume = this.GetVolume();
		source.volume = this.GetPitch();
		source.spatialBlend = this.settings.spatialBlend;
		source.dopplerLevel = this.settings.dopplerLevel;
		source.Play();
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x000423AB File Offset: 0x000405AB
	internal void OnPlayed()
	{
		this.lastTimePlayed = Time.unscaledTime;
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x000423B8 File Offset: 0x000405B8
	internal bool ReadyToPlay()
	{
		return this.lastTimePlayed > Time.unscaledTime + this.settings.cooldown || this.lastTimePlayed + this.settings.cooldown < Time.unscaledTime;
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x000423EE File Offset: 0x000405EE
	private float GetVolume()
	{
		return this.settings.volume * (1f - Random.Range(0f, this.settings.volume_Variation));
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x00042417 File Offset: 0x00040617
	private float GetPitch()
	{
		return this.settings.pitch + Random.Range(-this.settings.pitch_Variation * 0.5f, this.settings.pitch_Variation * 0.5f);
	}

	// Token: 0x04000B59 RID: 2905
	public AudioClip[] clips;

	// Token: 0x04000B5A RID: 2906
	public SFX_Settings settings;

	// Token: 0x04000B5B RID: 2907
	internal float lastTimePlayed;
}
