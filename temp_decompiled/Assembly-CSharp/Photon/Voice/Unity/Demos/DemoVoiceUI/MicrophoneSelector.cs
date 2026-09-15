using System;
using System.Collections.Generic;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x02000394 RID: 916
	public class MicrophoneSelector : VoiceComponent
	{
		// Token: 0x060017AA RID: 6058 RVA: 0x000786A0 File Offset: 0x000768A0
		protected override void Awake()
		{
			base.Awake();
			this.unityMicEnum = new AudioInEnumerator(base.Logger);
			this.photonMicEnum = Platform.CreateAudioInEnumerator(base.Logger);
			this.photonMicEnum.OnReady = delegate
			{
				this.SetupMicDropdown();
				this.SetCurrentValue();
			};
			this.refreshButton.GetComponentInChildren<Button>().onClick.AddListener(new UnityAction(this.RefreshMicrophones));
			this.fillArea = this.micLevelSlider.fillRect.GetComponent<Image>();
			this.defaultFillColor = this.fillArea.color;
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x00078734 File Offset: 0x00076934
		private void Update()
		{
			if (this.recorder != null)
			{
				this.micLevelSlider.value = this.recorder.LevelMeter.CurrentPeakAmp;
				this.fillArea.color = (this.recorder.IsCurrentlyTransmitting ? this.speakingFillColor : this.defaultFillColor);
			}
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x00078790 File Offset: 0x00076990
		private void OnEnable()
		{
			MicrophonePermission.MicrophonePermissionCallback += new Action<bool>(this.OnMicrophonePermissionCallback);
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x000787A3 File Offset: 0x000769A3
		private void OnMicrophonePermissionCallback(bool granted)
		{
			this.RefreshMicrophones();
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x000787AB File Offset: 0x000769AB
		private void OnDisable()
		{
			MicrophonePermission.MicrophonePermissionCallback -= new Action<bool>(this.OnMicrophonePermissionCallback);
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x000787C0 File Offset: 0x000769C0
		private void SetupMicDropdown()
		{
			this.micDropdown.ClearOptions();
			this.micOptions = new List<MicRef>();
			List<string> list = new List<string>();
			this.micOptions.Add(new MicRef(MicType.Unity, DeviceInfo.Default));
			list.Add(string.Format("[Unity]\u00a0[Default]", Array.Empty<object>()));
			foreach (DeviceInfo deviceInfo in this.unityMicEnum)
			{
				this.micOptions.Add(new MicRef(MicType.Unity, deviceInfo));
				list.Add(string.Format("[Unity]\u00a0{0}", deviceInfo));
			}
			this.micOptions.Add(new MicRef(MicType.Photon, DeviceInfo.Default));
			list.Add(string.Format("[Photon]\u00a0[Default]", Array.Empty<object>()));
			foreach (DeviceInfo deviceInfo2 in this.photonMicEnum)
			{
				this.micOptions.Add(new MicRef(MicType.Photon, deviceInfo2));
				list.Add(string.Format("[Photon]\u00a0{0}", deviceInfo2));
			}
			this.micDropdown.AddOptions(list);
			this.micDropdown.onValueChanged.RemoveAllListeners();
			this.micDropdown.onValueChanged.AddListener(delegate(int x)
			{
				this.SwitchToSelectedMic();
			});
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x00078938 File Offset: 0x00076B38
		public void SwitchToSelectedMic()
		{
			MicRef micRef = this.micOptions[this.micDropdown.value];
			MicType micType = micRef.MicType;
			if (micType != MicType.Unity)
			{
				if (micType == MicType.Photon)
				{
					this.recorder.SourceType = 0;
					this.recorder.MicrophoneType = 1;
					this.recorder.MicrophoneDevice = micRef.Device;
				}
			}
			else
			{
				this.recorder.SourceType = 0;
				this.recorder.MicrophoneType = 0;
				this.recorder.MicrophoneDevice = micRef.Device;
			}
			MicrophoneSelector.MicrophoneSelectorEvent microphoneSelectorEvent = this.onValueChanged;
			if (microphoneSelectorEvent == null)
			{
				return;
			}
			microphoneSelectorEvent.Invoke(micRef.MicType, micRef.Device);
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x000789DC File Offset: 0x00076BDC
		private void SetCurrentValue()
		{
			if (this.micOptions == null)
			{
				Debug.LogWarning("micOptions list is null");
				return;
			}
			this.micDropdown.gameObject.SetActive(true);
			this.refreshButton.SetActive(true);
			for (int i = 0; i < this.micOptions.Count; i++)
			{
				MicRef micRef = this.micOptions[i];
				if ((micRef.MicType == MicType.Unity && this.recorder.SourceType == null && this.recorder.MicrophoneType == null) || (micRef.MicType == MicType.Photon && this.recorder.SourceType == null && this.recorder.MicrophoneType == 1))
				{
					this.micDropdown.value = i;
					return;
				}
			}
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x00078A8F File Offset: 0x00076C8F
		public void RefreshMicrophones()
		{
			this.unityMicEnum.Refresh();
			this.photonMicEnum.Refresh();
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x00078AA7 File Offset: 0x00076CA7
		private void PhotonVoiceCreated()
		{
			this.RefreshMicrophones();
		}

		// Token: 0x04001615 RID: 5653
		public MicrophoneSelector.MicrophoneSelectorEvent onValueChanged = new MicrophoneSelector.MicrophoneSelectorEvent();

		// Token: 0x04001616 RID: 5654
		private List<MicRef> micOptions;

		// Token: 0x04001617 RID: 5655
		[SerializeField]
		private Dropdown micDropdown;

		// Token: 0x04001618 RID: 5656
		[SerializeField]
		private Slider micLevelSlider;

		// Token: 0x04001619 RID: 5657
		[SerializeField]
		private Recorder recorder;

		// Token: 0x0400161A RID: 5658
		[SerializeField]
		[FormerlySerializedAs("RefreshButton")]
		private GameObject refreshButton;

		// Token: 0x0400161B RID: 5659
		private Image fillArea;

		// Token: 0x0400161C RID: 5660
		private Color defaultFillColor = Color.white;

		// Token: 0x0400161D RID: 5661
		private Color speakingFillColor = Color.green;

		// Token: 0x0400161E RID: 5662
		private IDeviceEnumerator unityMicEnum;

		// Token: 0x0400161F RID: 5663
		private IDeviceEnumerator photonMicEnum;

		// Token: 0x02000531 RID: 1329
		public class MicrophoneSelectorEvent : UnityEvent<MicType, DeviceInfo>
		{
		}
	}
}
