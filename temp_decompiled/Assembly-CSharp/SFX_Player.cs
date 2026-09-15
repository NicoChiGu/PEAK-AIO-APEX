using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020001A6 RID: 422
public class SFX_Player : MonoBehaviour
{
	// Token: 0x06000D2A RID: 3370 RVA: 0x000424CC File Offset: 0x000406CC
	private void Start()
	{
		this.defaultSource = base.GetComponentInChildren<AudioSource>().gameObject;
		SFX_Player.instance = this;
		for (int i = 0; i < 20; i++)
		{
			this.CreateNewSource();
		}
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x00042504 File Offset: 0x00040704
	public SFX_Player.SoundEffectHandle PlaySFX(SFX_Instance SFX, Vector3 position, Transform followTransform = null, SFX_Settings overrideSettings = null, float volumeMultiplier = 1f, bool loop = false)
	{
		if (SFX == null)
		{
			return null;
		}
		if (SFX.clips.Length == 0)
		{
			return null;
		}
		if (!SFX.ReadyToPlay())
		{
			return null;
		}
		if (SFX.settings.spatialBlend > 0f && Vector3.Distance(MainCamera.instance.transform.position, position) > SFX.settings.range / 2f)
		{
			return null;
		}
		if (this.nrOfSoundsPlayed + 1 >= AudioSettings.GetConfiguration().numRealVoices)
		{
			this.StopOldest();
		}
		SFX.OnPlayed();
		SFX_Player.SoundEffectHandle soundEffectHandle = new SFX_Player.SoundEffectHandle();
		soundEffectHandle.Init(base.StartCoroutine(this.IPlaySFX(SFX, position, followTransform, overrideSettings, volumeMultiplier, loop, soundEffectHandle)));
		return soundEffectHandle;
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x000425B0 File Offset: 0x000407B0
	private void StopOldest()
	{
		this.currentlyPlayed[0].source.StopPlaying();
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x000425C8 File Offset: 0x000407C8
	private IEnumerator IPlaySFX(SFX_Instance SFX, Vector3 position, Transform followTransform, SFX_Settings overrideSettings, float volumeMultiplier, bool loop, SFX_Player.SoundEffectHandle handle)
	{
		SFX_Player.SFX_Source source = this.GetAvailibleSource();
		AudioClip clip = SFX.GetClip();
		if (clip == null)
		{
			Debug.LogError("Trying to play null sound >:I");
			yield break;
		}
		SFX_Settings settings = SFX.settings;
		if (overrideSettings != null)
		{
			settings = overrideSettings;
		}
		float c = 0f;
		float t = clip.length;
		source.source.clip = clip;
		source.source.transform.position = position;
		source.source.volume = settings.volume * Random.Range(1f - settings.volume_Variation, 1f) * volumeMultiplier;
		source.source.pitch = settings.pitch + Random.Range(-settings.pitch_Variation * 0.5f, settings.pitch_Variation * 0.5f);
		source.source.maxDistance = settings.range;
		source.source.spatialBlend = settings.spatialBlend;
		source.source.dopplerLevel = settings.dopplerLevel;
		source.source.loop = loop;
		source.source.outputAudioMixerGroup = this.defaultMixerGroup;
		Vector3 relativePos = Vector3.zero;
		if (followTransform)
		{
			relativePos = followTransform.InverseTransformPoint(position);
		}
		source.StartPlaying(handle);
		while (c < t || loop)
		{
			c += Time.deltaTime * settings.pitch;
			if (followTransform)
			{
				source.source.transform.position = followTransform.TransformPoint(relativePos);
			}
			yield return null;
		}
		source.StopPlaying();
		yield break;
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00042618 File Offset: 0x00040818
	private SFX_Player.SFX_Source GetAvailibleSource()
	{
		for (int i = 0; i < this.sources.Count; i++)
		{
			if (!this.sources[i].isPlaying)
			{
				return this.sources[i];
			}
		}
		return this.CreateNewSource();
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00042664 File Offset: 0x00040864
	private SFX_Player.SFX_Source CreateNewSource()
	{
		SFX_Player.SFX_Source sfx_Source = new SFX_Player.SFX_Source();
		GameObject gameObject = Object.Instantiate<GameObject>(this.defaultSource, base.transform.position, base.transform.rotation, base.transform);
		sfx_Source.source = gameObject.GetComponent<AudioSource>();
		sfx_Source.player = this;
		this.sources.Add(sfx_Source);
		return sfx_Source;
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x000426BF File Offset: 0x000408BF
	private void OnPlayed(SFX_Player.SoundEffectHandle handle)
	{
		this.nrOfSoundsPlayed++;
		this.currentlyPlayed.Add(handle);
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x000426DB File Offset: 0x000408DB
	private void OnStopped(SFX_Player.SoundEffectHandle handle)
	{
		this.nrOfSoundsPlayed--;
		this.currentlyPlayed.Remove(handle);
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x000426F8 File Offset: 0x000408F8
	public static void StopPlaying(SFX_Player.SoundEffectHandle handle, float fadeTime = 0f)
	{
		SFX_Player.SFX_Source sfxsourceFromHandle = SFX_Player.GetSFXSourceFromHandle(handle);
		if (sfxsourceFromHandle != null)
		{
			if (fadeTime == 0f)
			{
				sfxsourceFromHandle.StopPlaying();
				return;
			}
			SFX_Player.instance.StartCoroutine(SFX_Player.FadeOut(sfxsourceFromHandle, fadeTime));
		}
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00042730 File Offset: 0x00040930
	private static IEnumerator FadeOut(SFX_Player.SFX_Source source, float fadeTime)
	{
		float c = 0f;
		float startVolume = source.source.volume;
		while (c < fadeTime)
		{
			c += Time.deltaTime;
			source.source.volume = Mathf.Lerp(startVolume, 0f, c / fadeTime);
			yield return null;
		}
		source.StopPlaying();
		yield break;
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x00042748 File Offset: 0x00040948
	private static SFX_Player.SFX_Source GetSFXSourceFromHandle(SFX_Player.SoundEffectHandle handle)
	{
		foreach (SFX_Player.SFX_Source sfx_Source in SFX_Player.instance.sources)
		{
			if (sfx_Source.handle == handle)
			{
				return sfx_Source;
			}
		}
		return null;
	}

	// Token: 0x04000B65 RID: 2917
	public AudioMixerGroup defaultMixerGroup;

	// Token: 0x04000B66 RID: 2918
	private GameObject defaultSource;

	// Token: 0x04000B67 RID: 2919
	public List<SFX_Player.SFX_Source> sources = new List<SFX_Player.SFX_Source>();

	// Token: 0x04000B68 RID: 2920
	private List<SFX_Player.SoundEffectHandle> currentlyPlayed = new List<SFX_Player.SoundEffectHandle>();

	// Token: 0x04000B69 RID: 2921
	public static SFX_Player instance;

	// Token: 0x04000B6A RID: 2922
	private int nrOfSoundsPlayed;

	// Token: 0x0200049B RID: 1179
	[Serializable]
	public class SFX_Source
	{
		// Token: 0x06001BAC RID: 7084 RVA: 0x00082D10 File Offset: 0x00080F10
		public void StopPlaying()
		{
			if (!this.isPlaying)
			{
				return;
			}
			if (this.handle.corutine != null)
			{
				this.player.StopCoroutine(this.handle.corutine);
			}
			this.player.OnStopped(this.handle);
			this.source.Stop();
			this.isPlaying = false;
			this.handle.source = null;
			this.handle = null;
		}

		// Token: 0x06001BAD RID: 7085 RVA: 0x00082D7F File Offset: 0x00080F7F
		public void StartPlaying(SFX_Player.SoundEffectHandle setHandle)
		{
			if (this.isPlaying)
			{
				return;
			}
			this.player.OnPlayed(setHandle);
			this.source.Play();
			this.isPlaying = true;
			this.handle = setHandle;
			this.handle.source = this;
		}

		// Token: 0x040019B8 RID: 6584
		public AudioSource source;

		// Token: 0x040019B9 RID: 6585
		public bool isPlaying;

		// Token: 0x040019BA RID: 6586
		public SFX_Player.SoundEffectHandle handle;

		// Token: 0x040019BB RID: 6587
		public SFX_Player player;
	}

	// Token: 0x0200049C RID: 1180
	public class SoundEffectHandle
	{
		// Token: 0x06001BAF RID: 7087 RVA: 0x00082DC3 File Offset: 0x00080FC3
		public void Init(Coroutine c)
		{
			this.corutine = c;
		}

		// Token: 0x040019BC RID: 6588
		public Coroutine corutine;

		// Token: 0x040019BD RID: 6589
		public SFX_Player.SFX_Source source;
	}
}
