using System;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class CoverageToVolume : MonoBehaviour
{
	// Token: 0x060010CC RID: 4300 RVA: 0x000549D4 File Offset: 0x00052BD4
	private void Update()
	{
		if (this.aM && this.sound)
		{
			if (this.aM.obstruction <= 0.6f)
			{
				this.vol = this.max;
			}
			if (this.aM.obstruction > 0.6f)
			{
				this.vol = this.mid;
			}
			if (this.aM.obstruction >= 0.8f)
			{
				this.vol = this.min;
			}
			this.sound.volume = Mathf.Lerp(this.sound.volume, this.vol * this.mod, 0.5f * Time.deltaTime);
		}
	}

	// Token: 0x04000F14 RID: 3860
	public float mod;

	// Token: 0x04000F15 RID: 3861
	public AudioSource sound;

	// Token: 0x04000F16 RID: 3862
	public AmbienceAudio aM;

	// Token: 0x04000F17 RID: 3863
	public float max = 0.1f;

	// Token: 0x04000F18 RID: 3864
	public float mid = 0.05f;

	// Token: 0x04000F19 RID: 3865
	public float min = 0.025f;

	// Token: 0x04000F1A RID: 3866
	private float vol;
}
