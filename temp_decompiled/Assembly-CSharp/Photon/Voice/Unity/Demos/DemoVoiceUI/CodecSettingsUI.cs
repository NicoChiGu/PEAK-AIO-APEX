using System;
using System.Collections.Generic;
using POpusCodec.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x02000390 RID: 912
	public class CodecSettingsUI : MonoBehaviour
	{
		// Token: 0x0600176C RID: 5996 RVA: 0x00077230 File Offset: 0x00075430
		private void Awake()
		{
			this.frameDurationDropdown.ClearOptions();
			this.frameDurationDropdown.AddOptions(CodecSettingsUI.frameDurationOptions);
			this.InitFrameDuration();
			this.frameDurationDropdown.SetSingleOnValueChangedCallback(new UnityAction<int>(this.OnFrameDurationChanged));
			this.samplingRateDropdown.ClearOptions();
			this.samplingRateDropdown.AddOptions(CodecSettingsUI.samplingRateOptions);
			this.InitSamplingRate();
			this.samplingRateDropdown.SetSingleOnValueChangedCallback(new UnityAction<int>(this.OnSamplingRateChanged));
			this.bitrateInputField.SetSingleOnValueChangedCallback(new UnityAction<string>(this.OnBitrateChanged));
			this.InitBitrate();
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x000772CA File Offset: 0x000754CA
		private void Update()
		{
			this.InitFrameDuration();
			this.InitSamplingRate();
			this.InitBitrate();
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x000772E0 File Offset: 0x000754E0
		private void OnBitrateChanged(string newBitrateString)
		{
			int num;
			if (int.TryParse(newBitrateString, ref num))
			{
				this.recorder.Bitrate = num;
			}
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x00077304 File Offset: 0x00075504
		private void OnFrameDurationChanged(int index)
		{
			OpusCodec.FrameDuration frameDuration = this.recorder.FrameDuration;
			switch (index)
			{
			case 0:
				frameDuration = 2500;
				break;
			case 1:
				frameDuration = 5000;
				break;
			case 2:
				frameDuration = 10000;
				break;
			case 3:
				frameDuration = 20000;
				break;
			case 4:
				frameDuration = 40000;
				break;
			case 5:
				frameDuration = 60000;
				break;
			}
			this.recorder.FrameDuration = frameDuration;
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x00077378 File Offset: 0x00075578
		private void OnSamplingRateChanged(int index)
		{
			SamplingRate samplingRate = this.recorder.SamplingRate;
			switch (index)
			{
			case 0:
				samplingRate = 8000;
				break;
			case 1:
				samplingRate = 12000;
				break;
			case 2:
				samplingRate = 16000;
				break;
			case 3:
				samplingRate = 24000;
				break;
			case 4:
				samplingRate = 48000;
				break;
			}
			this.recorder.SamplingRate = samplingRate;
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x000773E0 File Offset: 0x000755E0
		private void InitFrameDuration()
		{
			int num = 0;
			OpusCodec.FrameDuration frameDuration = this.recorder.FrameDuration;
			if (frameDuration <= 10000)
			{
				if (frameDuration != 5000)
				{
					if (frameDuration == 10000)
					{
						num = 2;
					}
				}
				else
				{
					num = 1;
				}
			}
			else if (frameDuration != 20000)
			{
				if (frameDuration != 40000)
				{
					if (frameDuration == 60000)
					{
						num = 5;
					}
				}
				else
				{
					num = 4;
				}
			}
			else
			{
				num = 3;
			}
			this.frameDurationDropdown.value = num;
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x00077450 File Offset: 0x00075650
		private void InitSamplingRate()
		{
			int num = 0;
			SamplingRate samplingRate = this.recorder.SamplingRate;
			if (samplingRate <= 16000)
			{
				if (samplingRate != 12000)
				{
					if (samplingRate == 16000)
					{
						num = 2;
					}
				}
				else
				{
					num = 1;
				}
			}
			else if (samplingRate != 24000)
			{
				if (samplingRate == 48000)
				{
					num = 4;
				}
			}
			else
			{
				num = 3;
			}
			this.samplingRateDropdown.value = num;
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x000774B4 File Offset: 0x000756B4
		private void InitBitrate()
		{
			this.bitrateInputField.text = this.recorder.Bitrate.ToString();
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x000774E8 File Offset: 0x000756E8
		// Note: this type is marked as 'beforefieldinit'.
		static CodecSettingsUI()
		{
			List<string> list = new List<string>();
			list.Add("2.5ms");
			list.Add("5ms");
			list.Add("10ms");
			list.Add("20ms");
			list.Add("40ms");
			list.Add("60ms");
			CodecSettingsUI.frameDurationOptions = list;
			List<string> list2 = new List<string>();
			list2.Add("8kHz");
			list2.Add("12kHz");
			list2.Add("16kHz");
			list2.Add("24kHz");
			list2.Add("48kHz");
			CodecSettingsUI.samplingRateOptions = list2;
		}

		// Token: 0x040015D5 RID: 5589
		[SerializeField]
		private Dropdown frameDurationDropdown;

		// Token: 0x040015D6 RID: 5590
		[SerializeField]
		private Dropdown samplingRateDropdown;

		// Token: 0x040015D7 RID: 5591
		[SerializeField]
		private InputField bitrateInputField;

		// Token: 0x040015D8 RID: 5592
		[SerializeField]
		private Recorder recorder;

		// Token: 0x040015D9 RID: 5593
		private static readonly List<string> frameDurationOptions;

		// Token: 0x040015DA RID: 5594
		private static readonly List<string> samplingRateOptions;
	}
}
