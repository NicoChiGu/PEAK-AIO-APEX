using System;
using UnityEngine;

// Token: 0x02000205 RID: 517
public class AudioLoop : MonoBehaviour
{
	// Token: 0x06000F6F RID: 3951 RVA: 0x0004CB0C File Offset: 0x0004AD0C
	private void Update()
	{
		this.loop.volume = Mathf.Lerp(this.loop.volume, this.volume, 2f * Time.deltaTime);
		this.loop.pitch = Mathf.Lerp(this.loop.pitch, this.pitch, 2f * Time.deltaTime);
	}

	// Token: 0x04000DC2 RID: 3522
	public AudioSource loop;

	// Token: 0x04000DC3 RID: 3523
	public float volume;

	// Token: 0x04000DC4 RID: 3524
	public float pitch = 1f;
}
