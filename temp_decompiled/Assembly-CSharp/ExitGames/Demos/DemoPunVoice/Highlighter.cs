using System;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000389 RID: 905
	[RequireComponent(typeof(Canvas))]
	public class Highlighter : MonoBehaviour
	{
		// Token: 0x06001731 RID: 5937 RVA: 0x000760F5 File Offset: 0x000742F5
		private void OnEnable()
		{
			ChangePOV.CameraChanged += this.ChangePOV_CameraChanged;
			VoiceDemoUI.DebugToggled += this.VoiceDemoUI_DebugToggled;
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x00076119 File Offset: 0x00074319
		private void OnDisable()
		{
			ChangePOV.CameraChanged -= this.ChangePOV_CameraChanged;
			VoiceDemoUI.DebugToggled -= this.VoiceDemoUI_DebugToggled;
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x0007613D File Offset: 0x0007433D
		private void VoiceDemoUI_DebugToggled(bool debugMode)
		{
			this.showSpeakerLag = debugMode;
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x00076146 File Offset: 0x00074346
		private void ChangePOV_CameraChanged(Camera camera)
		{
			this.canvas.worldCamera = camera;
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x00076154 File Offset: 0x00074354
		private void Awake()
		{
			this.canvas = base.GetComponent<Canvas>();
			if (this.canvas != null && this.canvas.worldCamera == null)
			{
				this.canvas.worldCamera = Camera.main;
			}
			this.photonVoiceView = base.GetComponentInParent<PhotonVoiceView>();
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x000761AC File Offset: 0x000743AC
		private void Update()
		{
			this.recorderSprite.enabled = this.photonVoiceView.IsRecording;
			this.speakerSprite.enabled = this.photonVoiceView.IsSpeaking;
			this.bufferLagText.enabled = this.showSpeakerLag && this.photonVoiceView.IsSpeaking;
			if (this.bufferLagText.enabled)
			{
				this.bufferLagText.text = string.Format("{0}", this.photonVoiceView.SpeakerInUse.Lag);
			}
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x00076240 File Offset: 0x00074440
		private void LateUpdate()
		{
			if (this.canvas == null || this.canvas.worldCamera == null)
			{
				return;
			}
			base.transform.rotation = Quaternion.Euler(0f, this.canvas.worldCamera.transform.eulerAngles.y, 0f);
		}

		// Token: 0x040015AF RID: 5551
		private Canvas canvas;

		// Token: 0x040015B0 RID: 5552
		private PhotonVoiceView photonVoiceView;

		// Token: 0x040015B1 RID: 5553
		[SerializeField]
		private Image recorderSprite;

		// Token: 0x040015B2 RID: 5554
		[SerializeField]
		private Image speakerSprite;

		// Token: 0x040015B3 RID: 5555
		[SerializeField]
		private Text bufferLagText;

		// Token: 0x040015B4 RID: 5556
		private bool showSpeakerLag;
	}
}
