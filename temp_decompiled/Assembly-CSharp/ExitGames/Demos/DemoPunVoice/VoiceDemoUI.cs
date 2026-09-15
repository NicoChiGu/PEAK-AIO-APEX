using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x0200038C RID: 908
	public class VoiceDemoUI : MonoBehaviour
	{
		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06001740 RID: 5952 RVA: 0x000763C9 File Offset: 0x000745C9
		// (set) Token: 0x06001741 RID: 5953 RVA: 0x000763D4 File Offset: 0x000745D4
		public bool DebugMode
		{
			get
			{
				return this.debugMode;
			}
			set
			{
				this.debugMode = value;
				this.debugGO.SetActive(this.debugMode);
				this.voiceDebugText.text = string.Empty;
				if (this.debugMode)
				{
					this.previousDebugLevel = this.punVoiceClient.Client.LoadBalancingPeer.DebugOut;
					this.punVoiceClient.Client.LoadBalancingPeer.DebugOut = 5;
				}
				else
				{
					this.punVoiceClient.Client.LoadBalancingPeer.DebugOut = this.previousDebugLevel;
				}
				if (VoiceDemoUI.DebugToggled != null)
				{
					VoiceDemoUI.DebugToggled(this.debugMode);
				}
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06001742 RID: 5954 RVA: 0x00076478 File Offset: 0x00074678
		// (remove) Token: 0x06001743 RID: 5955 RVA: 0x000764AC File Offset: 0x000746AC
		public static event VoiceDemoUI.OnDebugToggle DebugToggled;

		// Token: 0x06001744 RID: 5956 RVA: 0x000764DF File Offset: 0x000746DF
		private void Awake()
		{
			this.punVoiceClient = PunVoiceClient.Instance;
			Debug.LogWarning("VoiceDemoUI selected a punVoiceClient.Instance", this.punVoiceClient);
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x000764FC File Offset: 0x000746FC
		private void OnDestroy()
		{
			ChangePOV.CameraChanged -= this.OnCameraChanged;
			BetterToggle.ToggleValueChanged -= this.BetterToggle_ToggleValueChanged;
			CharacterInstantiation.CharacterInstantiated -= this.CharacterInstantiation_CharacterInstantiated;
			this.punVoiceClient.Client.StateChanged -= new Action<ClientState, ClientState>(this.VoiceClientStateChanged);
			PhotonNetwork.NetworkingClient.StateChanged -= new Action<ClientState, ClientState>(this.PunClientStateChanged);
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x00076570 File Offset: 0x00074770
		private void CharacterInstantiation_CharacterInstantiated(GameObject character)
		{
			PhotonVoiceView component = character.GetComponent<PhotonVoiceView>();
			if (component != null)
			{
				this.recorder = component;
			}
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x00076594 File Offset: 0x00074794
		private void InitToggles(Toggle[] toggles)
		{
			if (toggles == null)
			{
				return;
			}
			foreach (Toggle toggle in toggles)
			{
				string name = toggle.name;
				if (!(name == "Mute"))
				{
					if (!(name == "VoiceDetection"))
					{
						if (!(name == "DebugVoice"))
						{
							if (!(name == "Transmit"))
							{
								if (!(name == "DebugEcho"))
								{
									if (name == "AutoConnectAndJoin")
									{
										toggle.isOn = this.punVoiceClient.AutoConnectAndJoin;
									}
								}
								else if (this.recorder != null && this.recorder.RecorderInUse != null)
								{
									toggle.isOn = this.recorder.RecorderInUse.DebugEchoMode;
								}
							}
							else if (this.recorder != null && this.recorder.RecorderInUse != null)
							{
								toggle.isOn = this.recorder.RecorderInUse.TransmitEnabled;
							}
						}
						else
						{
							toggle.isOn = this.DebugMode;
						}
					}
					else if (this.recorder != null && this.recorder.RecorderInUse != null)
					{
						toggle.isOn = this.recorder.RecorderInUse.VoiceDetection;
					}
				}
				else
				{
					toggle.isOn = AudioListener.volume <= 0.001f;
				}
			}
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x00076714 File Offset: 0x00074914
		private void BetterToggle_ToggleValueChanged(Toggle toggle)
		{
			string name = toggle.name;
			if (!(name == "Mute"))
			{
				if (!(name == "Transmit"))
				{
					if (!(name == "VoiceDetection"))
					{
						if (!(name == "DebugEcho"))
						{
							if (name == "DebugVoice")
							{
								this.DebugMode = toggle.isOn;
								return;
							}
							if (!(name == "AutoConnectAndJoin"))
							{
								return;
							}
							this.punVoiceClient.AutoConnectAndJoin = toggle.isOn;
						}
						else if (this.recorder.RecorderInUse)
						{
							this.recorder.RecorderInUse.DebugEchoMode = toggle.isOn;
							return;
						}
					}
					else if (this.recorder.RecorderInUse)
					{
						this.recorder.RecorderInUse.VoiceDetection = toggle.isOn;
						return;
					}
				}
				else if (this.recorder.RecorderInUse)
				{
					this.recorder.RecorderInUse.TransmitEnabled = toggle.isOn;
					return;
				}
				return;
			}
			if (toggle.isOn)
			{
				this.volumeBeforeMute = AudioListener.volume;
				AudioListener.volume = 0f;
				return;
			}
			AudioListener.volume = this.volumeBeforeMute;
			this.volumeBeforeMute = 0f;
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x00076854 File Offset: 0x00074A54
		private void OnCameraChanged(Camera newCamera)
		{
			this.canvas.worldCamera = newCamera;
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x00076864 File Offset: 0x00074A64
		private void Start()
		{
			ChangePOV.CameraChanged += this.OnCameraChanged;
			BetterToggle.ToggleValueChanged += this.BetterToggle_ToggleValueChanged;
			CharacterInstantiation.CharacterInstantiated += this.CharacterInstantiation_CharacterInstantiated;
			this.punVoiceClient.Client.StateChanged += new Action<ClientState, ClientState>(this.VoiceClientStateChanged);
			PhotonNetwork.NetworkingClient.StateChanged += new Action<ClientState, ClientState>(this.PunClientStateChanged);
			this.canvas = base.GetComponentInChildren<Canvas>();
			if (this.punSwitch != null)
			{
				this.punSwitchText = this.punSwitch.GetComponentInChildren<Text>();
				this.punSwitch.onClick.AddListener(new UnityAction(this.PunSwitchOnClick));
			}
			if (this.voiceSwitch != null)
			{
				this.voiceSwitchText = this.voiceSwitch.GetComponentInChildren<Text>();
				this.voiceSwitch.onClick.AddListener(new UnityAction(this.VoiceSwitchOnClick));
			}
			if (this.calibrateButton != null)
			{
				this.calibrateButton.onClick.AddListener(new UnityAction(this.CalibrateButtonOnClick));
				this.calibrateText = this.calibrateButton.GetComponentInChildren<Text>();
			}
			if (this.punState != null)
			{
				this.debugGO = this.punState.transform.parent.gameObject;
			}
			this.volumeBeforeMute = AudioListener.volume;
			this.previousDebugLevel = this.punVoiceClient.Client.LoadBalancingPeer.DebugOut;
			if (this.globalSettings != null)
			{
				this.globalSettings.SetActive(true);
				this.InitToggles(this.globalSettings.GetComponentsInChildren<Toggle>());
			}
			if (this.devicesInfoText != null)
			{
				using (AudioInEnumerator audioInEnumerator = new AudioInEnumerator(this.punVoiceClient.Logger))
				{
					using (IDeviceEnumerator deviceEnumerator = Platform.CreateAudioInEnumerator(this.punVoiceClient.Logger))
					{
						if (Enumerable.Count<DeviceInfo>(audioInEnumerator) + Enumerable.Count<DeviceInfo>(deviceEnumerator) == 0)
						{
							this.devicesInfoText.enabled = true;
							this.devicesInfoText.color = Color.red;
							this.devicesInfoText.text = "No microphone device detected!";
						}
						else
						{
							this.devicesInfoText.text = "Mic Unity: " + string.Join(", ", Enumerable.Select<DeviceInfo, string>(audioInEnumerator, (DeviceInfo x) => x.ToString()));
							Text text = this.devicesInfoText;
							text.text = text.text + "\nMic Photon: " + string.Join(", ", Enumerable.Select<DeviceInfo, string>(deviceEnumerator, (DeviceInfo x) => x.ToString()));
						}
					}
				}
			}
			this.VoiceClientStateChanged(0, this.punVoiceClient.ClientState);
			this.PunClientStateChanged(0, PhotonNetwork.NetworkingClient.State);
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x00076B64 File Offset: 0x00074D64
		private void PunSwitchOnClick()
		{
			if (PhotonNetwork.NetworkClientState == 9)
			{
				PhotonNetwork.Disconnect();
				return;
			}
			if (PhotonNetwork.NetworkClientState == 14 || PhotonNetwork.NetworkClientState == null)
			{
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x00076B8C File Offset: 0x00074D8C
		private void VoiceSwitchOnClick()
		{
			if (this.punVoiceClient.ClientState == 9)
			{
				this.punVoiceClient.Disconnect();
				return;
			}
			if (this.punVoiceClient.ClientState == null || this.punVoiceClient.ClientState == 14)
			{
				this.punVoiceClient.ConnectAndJoinRoom();
			}
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x00076BDC File Offset: 0x00074DDC
		private void CalibrateButtonOnClick()
		{
			if (this.recorder.RecorderInUse && !this.recorder.RecorderInUse.VoiceDetectorCalibrating)
			{
				this.recorder.RecorderInUse.VoiceDetectorCalibrate(this.calibrationMilliSeconds, null);
			}
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x00076C1C File Offset: 0x00074E1C
		private void Update()
		{
			if (this.recorder != null && this.recorder.RecorderInUse != null && this.recorder.RecorderInUse.LevelMeter != null)
			{
				this.voiceDebugText.text = string.Format("Amp: avg. {0:0.000000}, peak {1:0.000000}", this.recorder.RecorderInUse.LevelMeter.CurrentAvgAmp, this.recorder.RecorderInUse.LevelMeter.CurrentPeakAmp);
			}
		}

		// Token: 0x0600174F RID: 5967 RVA: 0x00076CA8 File Offset: 0x00074EA8
		private void PunClientStateChanged(ClientState fromState, ClientState toState)
		{
			this.punState.text = string.Format("PUN: {0}", toState);
			if (toState != null)
			{
				if (toState == 9)
				{
					this.punSwitch.interactable = true;
					this.punSwitchText.text = "PUN Disconnect";
					goto IL_0080;
				}
				if (toState != 14)
				{
					this.punSwitch.interactable = false;
					this.punSwitchText.text = "PUN busy";
					goto IL_0080;
				}
			}
			this.punSwitch.interactable = true;
			this.punSwitchText.text = "PUN Connect";
			IL_0080:
			this.UpdateUiBasedOnVoiceState(this.punVoiceClient.ClientState);
		}

		// Token: 0x06001750 RID: 5968 RVA: 0x00076D46 File Offset: 0x00074F46
		private void VoiceClientStateChanged(ClientState fromState, ClientState toState)
		{
			this.UpdateUiBasedOnVoiceState(toState);
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x00076D50 File Offset: 0x00074F50
		private void UpdateUiBasedOnVoiceState(ClientState voiceClientState)
		{
			this.voiceState.text = string.Format("PhotonVoice: {0}", voiceClientState);
			if (voiceClientState != null)
			{
				if (voiceClientState != 9)
				{
					if (voiceClientState != 14)
					{
						this.voiceSwitch.interactable = false;
						this.voiceSwitchText.text = "Voice busy";
						return;
					}
				}
				else
				{
					this.voiceSwitch.interactable = true;
					this.inGameSettings.SetActive(true);
					this.voiceSwitchText.text = "Voice Disconnect";
					this.InitToggles(this.inGameSettings.GetComponentsInChildren<Toggle>());
					if (this.recorder != null && this.recorder.RecorderInUse != null)
					{
						this.calibrateButton.interactable = !this.recorder.RecorderInUse.VoiceDetectorCalibrating;
						this.calibrateText.text = (this.recorder.RecorderInUse.VoiceDetectorCalibrating ? "Calibrating" : string.Format("Calibrate ({0}s)", this.calibrationMilliSeconds / 1000));
						return;
					}
					this.calibrateButton.interactable = false;
					this.calibrateText.text = "Unavailable";
					return;
				}
			}
			if (PhotonNetwork.InRoom)
			{
				this.voiceSwitch.interactable = true;
				this.voiceSwitchText.text = "Voice Connect";
				this.voiceDebugText.text = string.Empty;
			}
			else
			{
				this.voiceSwitch.interactable = false;
				this.voiceSwitchText.text = "Voice N/A";
				this.voiceDebugText.text = string.Empty;
			}
			this.calibrateButton.interactable = false;
			this.voiceSwitchText.text = "Voice Connect";
			this.calibrateText.text = "Unavailable";
			this.inGameSettings.SetActive(false);
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x00076F1B File Offset: 0x0007511B
		protected void OnApplicationQuit()
		{
			this.punVoiceClient.Client.StateChanged -= new Action<ClientState, ClientState>(this.VoiceClientStateChanged);
			PhotonNetwork.NetworkingClient.StateChanged -= new Action<ClientState, ClientState>(this.PunClientStateChanged);
		}

		// Token: 0x040015B8 RID: 5560
		[SerializeField]
		private Text punState;

		// Token: 0x040015B9 RID: 5561
		[SerializeField]
		private Text voiceState;

		// Token: 0x040015BA RID: 5562
		private PunVoiceClient punVoiceClient;

		// Token: 0x040015BB RID: 5563
		private Canvas canvas;

		// Token: 0x040015BC RID: 5564
		[SerializeField]
		private Button punSwitch;

		// Token: 0x040015BD RID: 5565
		private Text punSwitchText;

		// Token: 0x040015BE RID: 5566
		[SerializeField]
		private Button voiceSwitch;

		// Token: 0x040015BF RID: 5567
		private Text voiceSwitchText;

		// Token: 0x040015C0 RID: 5568
		[SerializeField]
		private Button calibrateButton;

		// Token: 0x040015C1 RID: 5569
		private Text calibrateText;

		// Token: 0x040015C2 RID: 5570
		[SerializeField]
		private Text voiceDebugText;

		// Token: 0x040015C3 RID: 5571
		private PhotonVoiceView recorder;

		// Token: 0x040015C4 RID: 5572
		[SerializeField]
		private GameObject inGameSettings;

		// Token: 0x040015C5 RID: 5573
		[SerializeField]
		private GameObject globalSettings;

		// Token: 0x040015C6 RID: 5574
		[SerializeField]
		private Text devicesInfoText;

		// Token: 0x040015C7 RID: 5575
		private GameObject debugGO;

		// Token: 0x040015C8 RID: 5576
		private bool debugMode;

		// Token: 0x040015C9 RID: 5577
		private float volumeBeforeMute;

		// Token: 0x040015CA RID: 5578
		private DebugLevel previousDebugLevel;

		// Token: 0x040015CC RID: 5580
		[SerializeField]
		private int calibrationMilliSeconds = 2000;

		// Token: 0x0200052E RID: 1326
		// (Invoke) Token: 0x06001DDA RID: 7642
		public delegate void OnDebugToggle(bool debugMode);
	}
}
