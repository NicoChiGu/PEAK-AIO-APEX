using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x02000396 RID: 918
	public class RemoteSpeakerUI : MonoBehaviour, IInRoomCallbacks
	{
		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060017C9 RID: 6089 RVA: 0x00078D94 File Offset: 0x00076F94
		protected Player Actor
		{
			get
			{
				if (this.loadBalancingClient == null || this.loadBalancingClient.CurrentRoom == null)
				{
					return null;
				}
				return this.loadBalancingClient.CurrentRoom.GetPlayer(this.speaker.RemoteVoice.PlayerId, false);
			}
		}

		// Token: 0x060017CA RID: 6090 RVA: 0x00078DD0 File Offset: 0x00076FD0
		protected virtual void Start()
		{
			this.speaker = base.GetComponent<Speaker>();
			this.audioSource = base.GetComponent<AudioSource>();
			this.playDelayInputField.text = this.speaker.PlayDelay.ToString();
			this.playDelayInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.OnPlayDelayChanged));
			this.SetNickname();
			this.SetMutedState();
			this.SetProperties();
			this.volumeSlider.minValue = 0f;
			this.volumeSlider.maxValue = 1f;
			this.volumeSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnVolumeChanged));
			this.volumeSlider.value = 1f;
			this.OnVolumeChanged(1f);
		}

		// Token: 0x060017CB RID: 6091 RVA: 0x00078E8E File Offset: 0x0007708E
		private void OnVolumeChanged(float newValue)
		{
			this.audioSource.volume = newValue;
		}

		// Token: 0x060017CC RID: 6092 RVA: 0x00078E9C File Offset: 0x0007709C
		private void OnPlayDelayChanged(string str)
		{
			int num;
			if (int.TryParse(str, ref num))
			{
				this.speaker.PlayDelay = num;
				return;
			}
			Debug.LogErrorFormat("Failed to parse {0}", new object[] { str });
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x00078ED4 File Offset: 0x000770D4
		private void Update()
		{
			this.remoteIsTalking.enabled = this.speaker.IsPlaying;
			if (this.speaker.IsPlaying)
			{
				int lag = this.speaker.Lag;
				this.smoothedLag = (lag + this.smoothedLag * 99) / 100;
				this.bufferLagText.text = string.Concat(new object[] { "Buffer Lag: ", this.smoothedLag, "/", lag });
				return;
			}
			this.bufferLagText.text = "Buffer Lag: " + this.smoothedLag + "/-";
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x00078F87 File Offset: 0x00077187
		private void OnDestroy()
		{
			if (this.loadBalancingClient != null)
			{
				this.loadBalancingClient.RemoveCallbackTarget(this);
			}
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x00078FA0 File Offset: 0x000771A0
		private void SetNickname()
		{
			string text = this.speaker.name;
			if (this.Actor != null)
			{
				text = this.Actor.NickName;
				if (string.IsNullOrEmpty(text))
				{
					text = "user " + this.Actor.ActorNumber;
				}
			}
			this.nameText.text = text;
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x00078FFC File Offset: 0x000771FC
		private void SetMutedState()
		{
			this.SetMutedState(this.Actor.IsMuted());
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x00079010 File Offset: 0x00077210
		private void SetProperties()
		{
			this.photonVad.enabled = this.Actor.HasPhotonVAD();
			this.webrtcVad.enabled = this.Actor.HasWebRTCVAD();
			this.aec.enabled = this.Actor.HasAEC();
			this.agc.enabled = this.Actor.HasAGC();
			this.agc.text = "AGC Gain: " + this.Actor.GetAGCGain().ToString() + " Level: " + this.Actor.GetAGCLevel().ToString();
			Recorder.MicType? micType = this.Actor.GetMic();
			this.mic.enabled = micType != null;
			Text text = this.mic;
			string text2;
			if (micType == null)
			{
				text2 = "";
			}
			else
			{
				Recorder.MicType? micType2 = micType;
				Recorder.MicType micType3 = 0;
				text2 = (((micType2.GetValueOrDefault() == micType3) & (micType2 != null)) ? "Unity MIC" : "Photon MIC");
			}
			text.text = text2;
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x00079112 File Offset: 0x00077312
		protected virtual void SetMutedState(bool isMuted)
		{
			this.remoteIsMuting.enabled = isMuted;
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x00079120 File Offset: 0x00077320
		protected virtual void OnActorPropertiesChanged(Player targetPlayer, Hashtable changedProps)
		{
			if (this.speaker != null && this.speaker.RemoteVoice != null && targetPlayer.ActorNumber == this.speaker.RemoteVoice.PlayerId)
			{
				this.SetMutedState();
				this.SetNickname();
				this.SetProperties();
			}
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x00079172 File Offset: 0x00077372
		public virtual void Init(VoiceConnection vC)
		{
			this.voiceConnection = vC;
			this.loadBalancingClient = this.voiceConnection.Client;
			this.loadBalancingClient.AddCallbackTarget(this);
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x00079198 File Offset: 0x00077398
		void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
		{
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x0007919A File Offset: 0x0007739A
		void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
		{
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x0007919C File Offset: 0x0007739C
		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x0007919E File Offset: 0x0007739E
		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			this.OnActorPropertiesChanged(targetPlayer, changedProps);
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x000791A8 File Offset: 0x000773A8
		void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
		{
		}

		// Token: 0x04001626 RID: 5670
		[SerializeField]
		private Text nameText;

		// Token: 0x04001627 RID: 5671
		[SerializeField]
		protected Image remoteIsMuting;

		// Token: 0x04001628 RID: 5672
		[SerializeField]
		private Image remoteIsTalking;

		// Token: 0x04001629 RID: 5673
		[SerializeField]
		private InputField playDelayInputField;

		// Token: 0x0400162A RID: 5674
		[SerializeField]
		private Text bufferLagText;

		// Token: 0x0400162B RID: 5675
		[SerializeField]
		private Slider volumeSlider;

		// Token: 0x0400162C RID: 5676
		[SerializeField]
		private Text photonVad;

		// Token: 0x0400162D RID: 5677
		[SerializeField]
		private Text webrtcVad;

		// Token: 0x0400162E RID: 5678
		[SerializeField]
		private Text aec;

		// Token: 0x0400162F RID: 5679
		[SerializeField]
		private Text agc;

		// Token: 0x04001630 RID: 5680
		[SerializeField]
		private Text mic;

		// Token: 0x04001631 RID: 5681
		protected Speaker speaker;

		// Token: 0x04001632 RID: 5682
		private AudioSource audioSource;

		// Token: 0x04001633 RID: 5683
		protected VoiceConnection voiceConnection;

		// Token: 0x04001634 RID: 5684
		protected LoadBalancingClient loadBalancingClient;

		// Token: 0x04001635 RID: 5685
		private int smoothedLag;
	}
}
