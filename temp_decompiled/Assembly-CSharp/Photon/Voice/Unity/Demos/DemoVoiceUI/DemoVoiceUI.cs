using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x02000391 RID: 913
	[RequireComponent(typeof(UnityVoiceClient), typeof(ConnectAndJoin))]
	public class DemoVoiceUI : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
	{
		// Token: 0x06001776 RID: 6006 RVA: 0x00077584 File Offset: 0x00075784
		private void Start()
		{
			this.connectAndJoin = base.GetComponent<ConnectAndJoin>();
			this.voiceConnection = base.GetComponent<UnityVoiceClient>();
			this.voiceAudioPreprocessor = this.voiceConnection.PrimaryRecorder.GetComponent<WebRtcAudioDsp>();
			this.compressionGainGameObject = this.agcCompressionGainSlider.transform.parent.gameObject;
			this.compressionGainText = this.compressionGainGameObject.GetComponentInChildren<Text>();
			this.targetLevelGameObject = this.agcTargetLevelSlider.transform.parent.gameObject;
			this.targetLevelText = this.targetLevelGameObject.GetComponentInChildren<Text>();
			this.aecOptionsGameObject = this.aecHighPassToggle.transform.parent.gameObject;
			this.SetDefaults();
			this.InitUiCallbacks();
			this.GetSavedNickname();
			this.voiceConnection.PrimaryRecorder.InputFactory = this.toneInputFactory;
			this.voiceConnection.SpeakerLinked += new Action<Speaker>(this.OnSpeakerCreated);
			this.voiceConnection.Client.AddCallbackTarget(this);
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x00077683 File Offset: 0x00075883
		protected virtual void SetDefaults()
		{
			this.muteToggle.isOn = !this.defaultTransmitEnabled;
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x00077699 File Offset: 0x00075899
		private void OnDestroy()
		{
			this.voiceConnection.SpeakerLinked -= new Action<Speaker>(this.OnSpeakerCreated);
			this.voiceConnection.Client.RemoveCallbackTarget(this);
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x000776C4 File Offset: 0x000758C4
		private void GetSavedNickname()
		{
			string @string = PlayerPrefs.GetString("vNick");
			if (!string.IsNullOrEmpty(@string))
			{
				this.localNicknameText.text = @string;
				this.voiceConnection.Client.NickName = @string;
			}
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x00077704 File Offset: 0x00075904
		protected virtual void OnSpeakerCreated(Speaker speaker)
		{
			speaker.gameObject.transform.SetParent(this.RemoteVoicesPanel, false);
			speaker.GetComponent<RemoteSpeakerUI>().Init(this.voiceConnection);
			speaker.OnRemoteVoiceRemoveAction = (Action<Speaker>)Delegate.Combine(speaker.OnRemoteVoiceRemoveAction, new Action<Speaker>(this.OnRemoteVoiceRemove));
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x0007775B File Offset: 0x0007595B
		private void OnRemoteVoiceRemove(Speaker speaker)
		{
			if (speaker != null)
			{
				Object.Destroy(speaker.gameObject);
			}
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x00077774 File Offset: 0x00075974
		private void ToggleMute(bool isOn)
		{
			this.muteToggle.targetGraphic.enabled = !isOn;
			if (isOn)
			{
				this.voiceConnection.Client.LocalPlayer.Mute();
				return;
			}
			this.voiceConnection.Client.LocalPlayer.Unmute();
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x000777C5 File Offset: 0x000759C5
		protected virtual void ToggleIsRecording(bool isRecording)
		{
			this.voiceConnection.PrimaryRecorder.RecordingEnabled = isRecording;
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x000777D8 File Offset: 0x000759D8
		private void ToggleDebugEcho(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.DebugEchoMode = isOn;
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x000777EB File Offset: 0x000759EB
		private void ToggleReliable(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.ReliableMode = isOn;
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x000777FE File Offset: 0x000759FE
		private void ToggleEncryption(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.Encrypt = isOn;
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x00077811 File Offset: 0x00075A11
		private void ToggleAEC(bool isOn)
		{
			this.voiceAudioPreprocessor.AEC = isOn;
			this.aecOptionsGameObject.SetActive(isOn);
			this.voiceConnection.Client.LocalPlayer.SetAEC(isOn);
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x00077842 File Offset: 0x00075A42
		private void ToggleNoiseSuppression(bool isOn)
		{
			this.voiceAudioPreprocessor.NoiseSuppression = isOn;
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x00077850 File Offset: 0x00075A50
		private void ToggleAGC(bool isOn)
		{
			this.voiceAudioPreprocessor.AGC = isOn;
			this.compressionGainGameObject.SetActive(isOn);
			this.targetLevelGameObject.SetActive(isOn);
			this.voiceConnection.Client.LocalPlayer.SetAGC(isOn, this.voiceAudioPreprocessor.AgcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x000778AE File Offset: 0x00075AAE
		private void ToggleVAD(bool isOn)
		{
			this.voiceAudioPreprocessor.VAD = isOn;
			this.voiceConnection.Client.LocalPlayer.SetWebRTCVAD(isOn);
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x000778D3 File Offset: 0x00075AD3
		private void ToggleHighPass(bool isOn)
		{
			this.voiceAudioPreprocessor.HighPass = isOn;
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x000778E4 File Offset: 0x00075AE4
		private void ToggleDsp(bool isOn)
		{
			this.voiceAudioPreprocessor.enabled = isOn;
			this.voiceConnection.PrimaryRecorder.RestartRecording();
			this.webRtcDspGameObject.SetActive(isOn);
			this.voiceConnection.Client.LocalPlayer.SetWebRTCVAD(this.voiceAudioPreprocessor.VAD);
			this.voiceConnection.Client.LocalPlayer.SetAEC(this.voiceAudioPreprocessor.AEC);
			this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, this.voiceAudioPreprocessor.AgcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x00077993 File Offset: 0x00075B93
		private void ToggleAudioClipStreaming(bool isOn)
		{
			if (isOn)
			{
				this.audioToneToggle.SetValue(false);
				this.voiceConnection.PrimaryRecorder.SourceType = 1;
				return;
			}
			if (!this.audioToneToggle.isOn)
			{
				this.microphoneSelector.SwitchToSelectedMic();
			}
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x000779D0 File Offset: 0x00075BD0
		private void ToggleAudioToneFactory(bool isOn)
		{
			if (isOn)
			{
				this.streamAudioClipToggle.SetValue(false);
				this.voiceConnection.PrimaryRecorder.SourceType = 2;
				this.voiceConnection.PrimaryRecorder.InputFactory = this.toneInputFactory;
				return;
			}
			if (!this.streamAudioClipToggle.isOn)
			{
				this.microphoneSelector.SwitchToSelectedMic();
			}
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x00077A2C File Offset: 0x00075C2C
		private void TogglePhotonVAD(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.VoiceDetection = isOn;
			this.voiceConnection.Client.LocalPlayer.SetPhotonVAD(isOn);
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x00077A56 File Offset: 0x00075C56
		private void ToggleAecHighPass(bool isOn)
		{
			this.voiceAudioPreprocessor.AecHighPass = isOn;
			this.voiceConnection.Client.LocalPlayer.SetAEC(isOn);
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x00077A7C File Offset: 0x00075C7C
		private void OnAgcCompressionGainChanged(float agcCompressionGain)
		{
			this.voiceAudioPreprocessor.AgcCompressionGain = (int)agcCompressionGain;
			this.compressionGainText.text = "Compression Gain: " + agcCompressionGain;
			this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, (int)agcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
		}

		// Token: 0x0600178C RID: 6028 RVA: 0x00077AE0 File Offset: 0x00075CE0
		private void OnAgcTargetLevelChanged(float agcTargetLevel)
		{
			this.voiceAudioPreprocessor.AgcTargetLevel = (int)agcTargetLevel;
			this.targetLevelText.text = "Target Level: " + agcTargetLevel;
			this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, this.voiceAudioPreprocessor.AgcCompressionGain, (int)agcTargetLevel);
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x00077B44 File Offset: 0x00075D44
		private void OnReverseStreamDelayChanged(string newReverseStreamString)
		{
			int num;
			if (int.TryParse(newReverseStreamString, ref num) && num > 0)
			{
				this.voiceAudioPreprocessor.ReverseStreamDelayMs = num;
				return;
			}
			this.reverseStreamDelayInputField.text = this.voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x00077B8A File Offset: 0x00075D8A
		private void OnMicrophoneChanged(Recorder.MicType micType, DeviceInfo deviceInfo)
		{
			this.voiceConnection.Client.LocalPlayer.SetMic(micType);
			this.androidMicSettingGameObject.SetActive(micType == 1);
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x00077BB2 File Offset: 0x00075DB2
		private void OnAndroidMicSettingsChanged(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.SetAndroidNativeMicrophoneSettings(this.androidAecToggle.isOn, this.androidAgcToggle.isOn, this.androidNsToggle.isOn);
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x00077BE6 File Offset: 0x00075DE6
		private void UpdateSyncedNickname(string nickname)
		{
			nickname = nickname.Trim();
			this.voiceConnection.Client.LocalPlayer.NickName = nickname;
			PlayerPrefs.SetString("vNick", nickname);
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x00077C14 File Offset: 0x00075E14
		private void JoinOrCreateRoom(string roomName)
		{
			if (string.IsNullOrEmpty(roomName))
			{
				this.connectAndJoin.RoomName = string.Empty;
				this.connectAndJoin.RandomRoom = true;
			}
			else
			{
				this.connectAndJoin.RoomName = roomName.Trim();
				this.connectAndJoin.RandomRoom = false;
			}
			if (this.voiceConnection.Client.InRoom)
			{
				this.voiceConnection.Client.OpLeaveRoom(false, false);
				return;
			}
			if (!this.voiceConnection.Client.IsConnected)
			{
				this.voiceConnection.ConnectUsingSettings(null);
			}
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x00077CA9 File Offset: 0x00075EA9
		private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
		{
			this.InitUiValues();
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x00077CB4 File Offset: 0x00075EB4
		protected virtual void Update()
		{
			this.connectionStatusText.text = this.voiceConnection.Client.State.ToString();
			this.serverStatusText.text = string.Format("{0}/{1}", this.voiceConnection.Client.CloudRegion, this.voiceConnection.Client.CurrentServerAddress);
			if (this.voiceConnection.PrimaryRecorder.IsCurrentlyTransmitting)
			{
				float num = this.voiceConnection.PrimaryRecorder.LevelMeter.CurrentAvgAmp;
				if (num > 1f)
				{
					num /= 32768f;
				}
				if ((double)num > 0.1)
				{
					this.inputWarningText.text = "Input too loud!";
					this.inputWarningText.color = this.warningColor;
				}
				else
				{
					this.inputWarningText.text = string.Empty;
					this.ResetTextColor(this.inputWarningText);
				}
			}
			if (this.voiceConnection.FramesReceivedPerSecond > 0f)
			{
				this.packetLossWarningText.text = string.Format("{0:0.##}% Packet Loss", this.voiceConnection.FramesLostPercent);
				this.packetLossWarningText.color = ((this.voiceConnection.FramesLostPercent > 1f) ? this.warningColor : this.okColor);
			}
			else
			{
				this.packetLossWarningText.text = string.Empty;
				this.ResetTextColor(this.packetLossWarningText);
			}
			this.rttText.text = "RTT:" + this.voiceConnection.Client.LoadBalancingPeer.RoundTripTime;
			this.SetTextColor(this.voiceConnection.Client.LoadBalancingPeer.RoundTripTime, this.rttText, this.rttYellowThreshold, this.rttRedThreshold);
			this.rttVariationText.text = "VAR:" + this.voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance;
			this.SetTextColor(this.voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance, this.rttVariationText, this.rttVariationYellowThreshold, this.rttVariationRedThreshold);
		}

		// Token: 0x06001794 RID: 6036 RVA: 0x00077EDB File Offset: 0x000760DB
		private void SetTextColor(int textValue, Text text, int yellowThreshold, int redThreshold)
		{
			if (textValue > redThreshold)
			{
				text.color = this.redColor;
				return;
			}
			if (textValue > yellowThreshold)
			{
				text.color = this.warningColor;
				return;
			}
			text.color = this.okColor;
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x00077F0C File Offset: 0x0007610C
		private void ResetTextColor(Text text)
		{
			text.color = this.defaultColor;
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x00077F1C File Offset: 0x0007611C
		private void InitUiCallbacks()
		{
			this.muteToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleMute));
			this.debugEchoToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleDebugEcho));
			this.reliableTransmissionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleReliable));
			this.encryptionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleEncryption));
			this.streamAudioClipToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAudioClipStreaming));
			this.audioToneToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAudioToneFactory));
			this.photonVadToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.TogglePhotonVAD));
			this.vadToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleVAD));
			this.aecToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAEC));
			this.agcToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAGC));
			this.dspToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleDsp));
			this.highPassToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleHighPass));
			this.aecHighPassToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAecHighPass));
			this.noiseSuppressionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleNoiseSuppression));
			this.agcCompressionGainSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnAgcCompressionGainChanged));
			this.agcTargetLevelSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnAgcTargetLevelChanged));
			this.localNicknameText.SetSingleOnEndEditCallback(new UnityAction<string>(this.UpdateSyncedNickname));
			this.roomNameInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.JoinOrCreateRoom));
			this.reverseStreamDelayInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.OnReverseStreamDelayChanged));
			this.androidAgcToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
			this.androidAecToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
			this.androidNsToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x00078124 File Offset: 0x00076324
		private void InitUiValues()
		{
			this.muteToggle.SetValue(this.voiceConnection.Client.LocalPlayer.IsMuted());
			this.debugEchoToggle.SetValue(this.voiceConnection.PrimaryRecorder.DebugEchoMode);
			this.reliableTransmissionToggle.SetValue(this.voiceConnection.PrimaryRecorder.ReliableMode);
			this.encryptionToggle.SetValue(this.voiceConnection.PrimaryRecorder.Encrypt);
			this.streamAudioClipToggle.SetValue(this.voiceConnection.PrimaryRecorder.SourceType == 1);
			this.audioToneToggle.SetValue(this.voiceConnection.PrimaryRecorder.SourceType == 2 && this.voiceConnection.PrimaryRecorder.InputFactory == this.toneInputFactory);
			this.photonVadToggle.SetValue(this.voiceConnection.PrimaryRecorder.VoiceDetection);
			this.androidAgcToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneAGC);
			this.androidAecToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneAEC);
			this.androidNsToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneNS);
			if (this.webRtcDspGameObject != null)
			{
				this.dspToggle.gameObject.SetActive(true);
				this.dspToggle.SetValue(this.voiceAudioPreprocessor.enabled);
				this.webRtcDspGameObject.SetActive(this.dspToggle.isOn);
				this.aecToggle.SetValue(this.voiceAudioPreprocessor.AEC);
				this.aecHighPassToggle.SetValue(this.voiceAudioPreprocessor.AecHighPass);
				this.reverseStreamDelayInputField.text = this.voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
				this.aecOptionsGameObject.SetActive(this.voiceAudioPreprocessor.AEC);
				this.noiseSuppressionToggle.isOn = this.voiceAudioPreprocessor.NoiseSuppression;
				this.agcToggle.SetValue(this.voiceAudioPreprocessor.AGC);
				this.agcCompressionGainSlider.SetValue((float)this.voiceAudioPreprocessor.AgcCompressionGain);
				this.agcTargetLevelSlider.SetValue((float)this.voiceAudioPreprocessor.AgcTargetLevel);
				this.compressionGainGameObject.SetActive(this.voiceAudioPreprocessor.AGC);
				this.targetLevelGameObject.SetActive(this.voiceAudioPreprocessor.AGC);
				this.vadToggle.SetValue(this.voiceAudioPreprocessor.VAD);
				this.highPassToggle.SetValue(this.voiceAudioPreprocessor.HighPass);
				return;
			}
			this.dspToggle.gameObject.SetActive(false);
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x000783DC File Offset: 0x000765DC
		private void SetRoomDebugText()
		{
			string text = string.Empty;
			if (this.voiceConnection.Client.InRoom)
			{
				foreach (Player player in this.voiceConnection.Client.CurrentRoom.Players.Values)
				{
					text += player.ToStringFull();
				}
				this.roomStatusText.text = string.Format("{0} {1}", this.voiceConnection.Client.CurrentRoom.Name, text);
			}
			else
			{
				this.roomStatusText.text = string.Empty;
			}
			this.roomStatusText.text = ((this.voiceConnection.Client.CurrentRoom == null) ? string.Empty : string.Format("{0} {1}", this.voiceConnection.Client.CurrentRoom.Name, text));
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x000784E4 File Offset: 0x000766E4
		protected virtual void OnActorPropertiesChanged(Player targetPlayer, Hashtable changedProps)
		{
			if (targetPlayer.IsLocal)
			{
				bool flag = targetPlayer.IsMuted();
				this.voiceConnection.PrimaryRecorder.TransmitEnabled = !flag;
				this.muteToggle.SetValue(flag);
			}
			this.SetRoomDebugText();
		}

		// Token: 0x0600179A RID: 6042 RVA: 0x00078526 File Offset: 0x00076726
		protected void OnApplicationQuit()
		{
			this.voiceConnection.Client.RemoveCallbackTarget(this);
		}

		// Token: 0x0600179B RID: 6043 RVA: 0x00078539 File Offset: 0x00076739
		void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
		{
			this.SetRoomDebugText();
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x00078541 File Offset: 0x00076741
		void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
		{
			this.SetRoomDebugText();
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x00078549 File Offset: 0x00076749
		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x0007854B File Offset: 0x0007674B
		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			this.OnActorPropertiesChanged(targetPlayer, changedProps);
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x00078555 File Offset: 0x00076755
		void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
		{
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x00078557 File Offset: 0x00076757
		void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x00078559 File Offset: 0x00076759
		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x0007855B File Offset: 0x0007675B
		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060017A3 RID: 6051 RVA: 0x0007855D File Offset: 0x0007675D
		void IMatchmakingCallbacks.OnJoinedRoom()
		{
			this.SetRoomDebugText();
			this.voiceConnection.Client.LocalPlayer.SetMic(this.voiceConnection.PrimaryRecorder.MicrophoneType);
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x0007858B File Offset: 0x0007678B
		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x0007858D File Offset: 0x0007678D
		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060017A6 RID: 6054 RVA: 0x0007858F File Offset: 0x0007678F
		void IMatchmakingCallbacks.OnLeftRoom()
		{
			this.SetRoomDebugText();
			this.SetDefaults();
		}

		// Token: 0x040015DB RID: 5595
		[SerializeField]
		private Text connectionStatusText;

		// Token: 0x040015DC RID: 5596
		[SerializeField]
		private Text serverStatusText;

		// Token: 0x040015DD RID: 5597
		[SerializeField]
		private Text roomStatusText;

		// Token: 0x040015DE RID: 5598
		[SerializeField]
		private Text inputWarningText;

		// Token: 0x040015DF RID: 5599
		[SerializeField]
		private Text rttText;

		// Token: 0x040015E0 RID: 5600
		[SerializeField]
		private Text rttVariationText;

		// Token: 0x040015E1 RID: 5601
		[SerializeField]
		private Text packetLossWarningText;

		// Token: 0x040015E2 RID: 5602
		[SerializeField]
		private InputField localNicknameText;

		// Token: 0x040015E3 RID: 5603
		[SerializeField]
		private Toggle debugEchoToggle;

		// Token: 0x040015E4 RID: 5604
		[SerializeField]
		private Toggle reliableTransmissionToggle;

		// Token: 0x040015E5 RID: 5605
		[SerializeField]
		private Toggle encryptionToggle;

		// Token: 0x040015E6 RID: 5606
		[SerializeField]
		private GameObject webRtcDspGameObject;

		// Token: 0x040015E7 RID: 5607
		[SerializeField]
		private Toggle aecToggle;

		// Token: 0x040015E8 RID: 5608
		[SerializeField]
		private Toggle aecHighPassToggle;

		// Token: 0x040015E9 RID: 5609
		[SerializeField]
		private InputField reverseStreamDelayInputField;

		// Token: 0x040015EA RID: 5610
		[SerializeField]
		private Toggle noiseSuppressionToggle;

		// Token: 0x040015EB RID: 5611
		[SerializeField]
		private Toggle agcToggle;

		// Token: 0x040015EC RID: 5612
		[SerializeField]
		private Slider agcCompressionGainSlider;

		// Token: 0x040015ED RID: 5613
		[SerializeField]
		private Slider agcTargetLevelSlider;

		// Token: 0x040015EE RID: 5614
		[SerializeField]
		private Toggle vadToggle;

		// Token: 0x040015EF RID: 5615
		[SerializeField]
		private Toggle muteToggle;

		// Token: 0x040015F0 RID: 5616
		[SerializeField]
		private Toggle streamAudioClipToggle;

		// Token: 0x040015F1 RID: 5617
		[SerializeField]
		private Toggle audioToneToggle;

		// Token: 0x040015F2 RID: 5618
		[SerializeField]
		private Toggle dspToggle;

		// Token: 0x040015F3 RID: 5619
		[SerializeField]
		private Toggle highPassToggle;

		// Token: 0x040015F4 RID: 5620
		[SerializeField]
		private Toggle photonVadToggle;

		// Token: 0x040015F5 RID: 5621
		[SerializeField]
		private MicrophoneSelector microphoneSelector;

		// Token: 0x040015F6 RID: 5622
		[SerializeField]
		private GameObject androidMicSettingGameObject;

		// Token: 0x040015F7 RID: 5623
		[SerializeField]
		private Toggle androidAgcToggle;

		// Token: 0x040015F8 RID: 5624
		[SerializeField]
		private Toggle androidAecToggle;

		// Token: 0x040015F9 RID: 5625
		[SerializeField]
		private Toggle androidNsToggle;

		// Token: 0x040015FA RID: 5626
		[SerializeField]
		private bool defaultTransmitEnabled;

		// Token: 0x040015FB RID: 5627
		[SerializeField]
		private bool fullScreen;

		// Token: 0x040015FC RID: 5628
		[SerializeField]
		private InputField roomNameInputField;

		// Token: 0x040015FD RID: 5629
		[SerializeField]
		private int rttYellowThreshold = 100;

		// Token: 0x040015FE RID: 5630
		[SerializeField]
		private int rttRedThreshold = 160;

		// Token: 0x040015FF RID: 5631
		[SerializeField]
		private int rttVariationYellowThreshold = 25;

		// Token: 0x04001600 RID: 5632
		[SerializeField]
		private int rttVariationRedThreshold = 50;

		// Token: 0x04001601 RID: 5633
		private GameObject compressionGainGameObject;

		// Token: 0x04001602 RID: 5634
		private GameObject targetLevelGameObject;

		// Token: 0x04001603 RID: 5635
		private Text compressionGainText;

		// Token: 0x04001604 RID: 5636
		private Text targetLevelText;

		// Token: 0x04001605 RID: 5637
		private GameObject aecOptionsGameObject;

		// Token: 0x04001606 RID: 5638
		public Transform RemoteVoicesPanel;

		// Token: 0x04001607 RID: 5639
		protected UnityVoiceClient voiceConnection;

		// Token: 0x04001608 RID: 5640
		private WebRtcAudioDsp voiceAudioPreprocessor;

		// Token: 0x04001609 RID: 5641
		private ConnectAndJoin connectAndJoin;

		// Token: 0x0400160A RID: 5642
		private readonly Color warningColor = new Color(0.9f, 0.5f, 0f, 1f);

		// Token: 0x0400160B RID: 5643
		private readonly Color okColor = new Color(0f, 0.6f, 0.2f, 1f);

		// Token: 0x0400160C RID: 5644
		private readonly Color redColor = new Color(1f, 0f, 0f, 1f);

		// Token: 0x0400160D RID: 5645
		private readonly Color defaultColor = new Color(0f, 0f, 0f, 1f);

		// Token: 0x0400160E RID: 5646
		private Func<IAudioDesc> toneInputFactory = () => new AudioUtil.ToneAudioReader<float>(null, 440.0, 48000, 2);
	}
}
