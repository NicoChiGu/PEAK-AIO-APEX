using System;
using Photon.Voice.PUN;
using UnityEngine;

// Token: 0x020002BF RID: 703
[RequireComponent(typeof(PhotonVoiceView))]
public class PointersController : MonoBehaviour
{
	// Token: 0x0600131C RID: 4892 RVA: 0x00061043 File Offset: 0x0005F243
	private void Awake()
	{
		this.photonVoiceView = base.GetComponent<PhotonVoiceView>();
		this.SetActiveSafe(this.pointerUp, false);
		this.SetActiveSafe(this.pointerDown, false);
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x0006106B File Offset: 0x0005F26B
	private void Update()
	{
		this.SetActiveSafe(this.pointerDown, this.photonVoiceView.IsSpeaking);
		this.SetActiveSafe(this.pointerUp, this.photonVoiceView.IsRecording);
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x0006109B File Offset: 0x0005F29B
	private void SetActiveSafe(GameObject go, bool active)
	{
		if (go != null && go.activeSelf != active)
		{
			go.SetActive(active);
		}
	}

	// Token: 0x040011C0 RID: 4544
	[SerializeField]
	private GameObject pointerDown;

	// Token: 0x040011C1 RID: 4545
	[SerializeField]
	private GameObject pointerUp;

	// Token: 0x040011C2 RID: 4546
	private PhotonVoiceView photonVoiceView;
}
