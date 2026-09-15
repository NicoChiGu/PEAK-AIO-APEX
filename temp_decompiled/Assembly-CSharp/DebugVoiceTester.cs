using System;
using UnityEngine;

// Token: 0x02000244 RID: 580
public class DebugVoiceTester : MonoBehaviour
{
	// Token: 0x060010F8 RID: 4344 RVA: 0x00055BB4 File Offset: 0x00053DB4
	private void Start()
	{
		this.audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
		this.audioSource.loop = true;
		while (Microphone.GetPosition(null) <= 0)
		{
		}
		this.audioSource.Play();
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00055C00 File Offset: 0x00053E00
	private void Update()
	{
	}

	// Token: 0x04000F6F RID: 3951
	public AudioSource audioSource;
}
