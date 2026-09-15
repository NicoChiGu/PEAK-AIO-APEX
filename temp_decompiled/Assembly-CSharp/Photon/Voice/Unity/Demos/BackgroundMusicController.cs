using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	// Token: 0x0200038D RID: 909
	public class BackgroundMusicController : MonoBehaviour
	{
		// Token: 0x06001754 RID: 5972 RVA: 0x00076F64 File Offset: 0x00075164
		private void Awake()
		{
			this.volumeSlider.minValue = 0f;
			this.volumeSlider.maxValue = 1f;
			this.volumeSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnVolumeChanged));
			this.volumeSlider.value = this.initialVolume;
			this.OnVolumeChanged(this.initialVolume);
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x00076FC5 File Offset: 0x000751C5
		private void OnVolumeChanged(float newValue)
		{
			this.audioSource.volume = newValue;
		}

		// Token: 0x040015CD RID: 5581
		[SerializeField]
		private Text volumeText;

		// Token: 0x040015CE RID: 5582
		[SerializeField]
		private Slider volumeSlider;

		// Token: 0x040015CF RID: 5583
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040015D0 RID: 5584
		[SerializeField]
		private float initialVolume = 0.125f;
	}
}
