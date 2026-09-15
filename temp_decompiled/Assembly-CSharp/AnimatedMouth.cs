using System;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x02000057 RID: 87
public class AnimatedMouth : MonoBehaviour
{
	// Token: 0x06000459 RID: 1113 RVA: 0x0001A9D0 File Offset: 0x00018BD0
	private void Start()
	{
		this.amplitudePeakLimiter = this.minAmplitudeThreshold;
		this.character = base.GetComponent<Character>();
		if (!this.isGhost && this.character != null && this.character.IsLocal)
		{
			Singleton<MicrophoneRelay>.Instance.RegisterMicListener(new Action<float[]>(this.OnGetMic));
		}
		this.pushToTalkSetting = GameHandler.Instance.SettingsHandler.GetSetting<PushToTalkSetting>();
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x0001AA44 File Offset: 0x00018C44
	private void OnDestroy()
	{
		if (!this.isGhost && this.character != null && this.character.IsLocal && Singleton<MicrophoneRelay>.Instance)
		{
			Singleton<MicrophoneRelay>.Instance.UnregisterMicListener(new Action<float[]>(this.OnGetMic));
		}
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0001AA96 File Offset: 0x00018C96
	public void OnGetMic(float[] buffer)
	{
		this.m_lastSentLocalBuffer = buffer;
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x0001AAA0 File Offset: 0x00018CA0
	private void Update()
	{
		float[] array = new float[256];
		if (this.audioSource != null)
		{
			this.audioSource.GetSpectrumData(array, 0, 0);
		}
		if (this.m_lastSentLocalBuffer != null)
		{
			array = this.m_lastSentLocalBuffer;
		}
		this.ProcessMicData(array);
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x0001AAEC File Offset: 0x00018CEC
	public static float MicrophoneLevelMax(float[] data)
	{
		int num = 128;
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			float num3 = data[i] * data[i];
			if (num2 < num3)
			{
				num2 = num3;
			}
		}
		return num2;
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x0001AB20 File Offset: 0x00018D20
	public static float MicrophoneLevelMaxDecibels(float level)
	{
		return 20f * Mathf.Log10(Mathf.Abs(level));
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x0001AB34 File Offset: 0x00018D34
	private void ProcessMicData(float[] buffer)
	{
		if (!this.audioSource)
		{
			return;
		}
		if (!this.isGhost && this.character != null && (this.character.data.dead || this.character.data.passedOut))
		{
			return;
		}
		float num = AnimatedMouth.MicrophoneLevelMaxDecibels(AnimatedMouth.MicrophoneLevelMax(buffer));
		if (this.character != null && this.character.IsLocal && ((this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToTalk && !this.character.input.pushToTalkPressed) || (this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToMute && this.character.input.pushToTalkPressed)))
		{
			num = -80f;
		}
		float num2 = this.decibelToAmountCurve.Evaluate(num);
		if (num2 > this.amplitudePeakLimiter)
		{
			this.amplitudePeakLimiter = num2;
		}
		if (this.amplitudePeakLimiter > this.minAmplitudeThreshold)
		{
			this.amplitudePeakLimiter -= this.amplitudeHighestDecay * Time.deltaTime;
		}
		this.volume = num2 / this.amplitudePeakLimiter;
		if (this.volume > this.volumePeak)
		{
			this.volumePeak = this.volume;
		}
		this.volumePeak = Mathf.Lerp(this.volumePeak, 0f, Time.deltaTime * this.amplitudeSmoothing);
		if (this.volumePeak > this.talkThreshold)
		{
			this.mouthRenderer.material.SetInt("_UseTalkSprites", 1);
			this.isSpeaking = true;
		}
		else
		{
			this.isSpeaking = false;
			this.mouthRenderer.material.SetInt("_UseTalkSprites", 0);
		}
		this.amplitudeIndex = (int)(Mathf.Clamp01(this.volumePeak * this.amplitudeMult) * (float)(this.mouthTextures.Length - 1));
		this.mouthRenderer.material.SetTexture("_TalkSprite", this.mouthTextures[this.amplitudeIndex]);
	}

	// Token: 0x040004D6 RID: 1238
	public AnimationCurve decibelToAmountCurve = AnimationCurve.EaseInOut(-80f, 0f, 12f, 1f);

	// Token: 0x040004D7 RID: 1239
	public bool isSpeaking;

	// Token: 0x040004D8 RID: 1240
	public AudioSource audioSource;

	// Token: 0x040004D9 RID: 1241
	public Vector2 BandPassFilter;

	// Token: 0x040004DA RID: 1242
	[FormerlySerializedAs("amplitude")]
	[Range(0f, 1f)]
	public float volume;

	// Token: 0x040004DB RID: 1243
	[FormerlySerializedAs("amplitudeHighest")]
	public float amplitudePeakLimiter;

	// Token: 0x040004DC RID: 1244
	public float minAmplitudeThreshold = 0.5f;

	// Token: 0x040004DD RID: 1245
	public float amplitudeHighestDecay = 0.01f;

	// Token: 0x040004DE RID: 1246
	public float amplitudeSmoothing = 0.2f;

	// Token: 0x040004DF RID: 1247
	public float talkThreshold = 0.1f;

	// Token: 0x040004E0 RID: 1248
	public float amplitudeMult;

	// Token: 0x040004E1 RID: 1249
	[HideInInspector]
	public int amplitudeIndex;

	// Token: 0x040004E2 RID: 1250
	[FormerlySerializedAs("textures")]
	[Header("Mouth Cards")]
	public Texture2D[] mouthTextures;

	// Token: 0x040004E3 RID: 1251
	public Renderer mouthRenderer;

	// Token: 0x040004E4 RID: 1252
	public Character character;

	// Token: 0x040004E5 RID: 1253
	public bool isGhost;

	// Token: 0x040004E6 RID: 1254
	private float volumePeak;

	// Token: 0x040004E7 RID: 1255
	private PushToTalkSetting pushToTalkSetting;

	// Token: 0x040004E8 RID: 1256
	private float[] m_lastSentLocalBuffer;
}
