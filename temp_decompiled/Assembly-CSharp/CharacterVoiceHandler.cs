using System;
using Photon.Voice;
using Photon.Voice.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

// Token: 0x02000018 RID: 24
public class CharacterVoiceHandler : MonoBehaviour
{
	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000217 RID: 535 RVA: 0x00010258 File Offset: 0x0000E458
	// (set) Token: 0x06000218 RID: 536 RVA: 0x00010260 File Offset: 0x0000E460
	internal AudioSource audioSource { get; private set; }

	// Token: 0x06000219 RID: 537 RVA: 0x00010269 File Offset: 0x0000E469
	private void OnEnable()
	{
		GlobalEvents.OnCharacterAudioLevelsUpdated = (Action)Delegate.Combine(GlobalEvents.OnCharacterAudioLevelsUpdated, new Action(this.UpdateAudioLevel));
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0001028B File Offset: 0x0000E48B
	private void OnDisable()
	{
		GlobalEvents.OnCharacterAudioLevelsUpdated = (Action)Delegate.Remove(GlobalEvents.OnCharacterAudioLevelsUpdated, new Action(this.UpdateAudioLevel));
	}

	// Token: 0x0600021B RID: 539 RVA: 0x000102B0 File Offset: 0x0000E4B0
	private void UpdateAudioLevel()
	{
		if (AudioLevels.PlayerAudioLevels.ContainsKey(this.m_character.photonView.Owner.UserId))
		{
			float num = AudioLevels.PlayerAudioLevels[this.m_character.photonView.Owner.UserId];
			this.audioLevel = num;
			return;
		}
		this.audioLevel = 0.5f;
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00010314 File Offset: 0x0000E514
	private void Start()
	{
		this.m_Recorder = base.GetComponent<Recorder>();
		this.m_character = base.GetComponentInParent<Character>();
		this.microphoneSetting = GameHandler.Instance.SettingsHandler.GetSetting<MicrophoneSetting>();
		this.pushToTalkSetting = GameHandler.Instance.SettingsHandler.GetSetting<PushToTalkSetting>();
		this.audioSource = base.GetComponent<AudioSource>();
		this.m_source = base.GetComponent<AudioSource>();
		if (this.m_character.IsLocal)
		{
			return;
		}
		byte b = PlayerHandler.AssignMixerGroup(this.m_character);
		if (b != 255)
		{
			this.m_source.outputAudioMixerGroup = this.GetMixerGroup(b);
			this.m_parameter = this.GetMixerGroupParameter(b);
		}
		this.UpdateAudioLevel();
	}

	// Token: 0x0600021D RID: 541 RVA: 0x000103C4 File Offset: 0x0000E5C4
	private AudioMixerGroup GetMixerGroup(byte group)
	{
		AudioMixerGroup audioMixerGroup;
		switch (group)
		{
		case 0:
			audioMixerGroup = this.m_mixerGroup1;
			break;
		case 1:
			audioMixerGroup = this.m_mixerGroup2;
			break;
		case 2:
			audioMixerGroup = this.m_mixerGroup3;
			break;
		case 3:
			audioMixerGroup = this.m_mixerGroup4;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return audioMixerGroup;
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00010414 File Offset: 0x0000E614
	private string GetMixerGroupParameter(byte group)
	{
		return "Voice" + ((int)(group + 1)).ToString() + "Effects";
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0001043C File Offset: 0x0000E63C
	private void Update()
	{
		this.m_source.volume = ((this.m_character.data.fullyConscious || this.m_character.IsGhost) ? this.audioLevel : ((this.m_character.data.passedOut && !this.m_character.data.fullyPassedOut) ? (this.audioLevel * Mathf.Clamp01(1f - this.m_character.data.passOutValue)) : 0f));
		this.PushToTalk();
		if (this.m_character.IsLocal && !this.m_character.isBot)
		{
			string id = this.microphoneSetting.Value.id;
			if (id != this.m_setMicrophoneDevice && !string.IsNullOrEmpty(id))
			{
				this.m_setMicrophoneDevice = id;
				this.m_Recorder.MicrophoneDevice = new DeviceInfo(id, null);
				Debug.Log("Setting microphone to " + id);
			}
		}
	}

	// Token: 0x06000220 RID: 544 RVA: 0x00010538 File Offset: 0x0000E738
	private void PushToTalk()
	{
		bool flag = this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.VoiceActivation || (this.m_character.input.pushToTalkPressed && this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToTalk) || (!this.m_character.input.pushToTalkPressed && this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToMute);
		if (flag != this.m_currentlyTransmitting || this.firstTime)
		{
			this.firstTime = false;
			this.m_currentlyTransmitting = flag;
			this.m_Recorder.TransmitEnabled = flag;
		}
	}

	// Token: 0x06000221 RID: 545 RVA: 0x000105C4 File Offset: 0x0000E7C4
	private void LateUpdate()
	{
		bool flag = false;
		if (Singleton<PeakHandler>.Instance != null && Singleton<PeakHandler>.Instance.isPlayingCinematic)
		{
			flag = true;
		}
		this.m_source.spatialBlend = (float)(flag ? 0 : 1);
		if (this.m_character.IsLocal)
		{
			return;
		}
		Vector3 vector = this.m_character.refs.head.transform.position;
		if (this.m_character.Ghost != null)
		{
			vector = this.m_character.Ghost.transform.position;
		}
		base.transform.position = vector;
		float num = math.saturate(LightVolume.Instance().SamplePositionAlpha(vector));
		num = math.saturate(1f - math.remap(0f, 0.3f, 0f, 1f, num));
		if (flag)
		{
		}
	}

	// Token: 0x040001F0 RID: 496
	private Character m_character;

	// Token: 0x040001F1 RID: 497
	[SerializeField]
	private AudioMixer m_mixer;

	// Token: 0x040001F3 RID: 499
	[SerializeField]
	private AudioMixerGroup m_mixerGroup1;

	// Token: 0x040001F4 RID: 500
	[SerializeField]
	private AudioMixerGroup m_mixerGroup2;

	// Token: 0x040001F5 RID: 501
	[SerializeField]
	private AudioMixerGroup m_mixerGroup3;

	// Token: 0x040001F6 RID: 502
	[SerializeField]
	private AudioMixerGroup m_mixerGroup4;

	// Token: 0x040001F7 RID: 503
	private AudioSource m_source;

	// Token: 0x040001F8 RID: 504
	private string m_parameter;

	// Token: 0x040001F9 RID: 505
	private MicrophoneSetting microphoneSetting;

	// Token: 0x040001FA RID: 506
	private PushToTalkSetting pushToTalkSetting;

	// Token: 0x040001FB RID: 507
	private string m_setMicrophoneDevice;

	// Token: 0x040001FC RID: 508
	private Recorder m_Recorder;

	// Token: 0x040001FD RID: 509
	private bool m_currentlyTransmitting;

	// Token: 0x040001FE RID: 510
	private float audioLevel = 0.5f;

	// Token: 0x040001FF RID: 511
	private bool firstTime = true;

	// Token: 0x04000200 RID: 512
	public const float DEFAULT_VOICE_VOLUME = 0.5f;
}
