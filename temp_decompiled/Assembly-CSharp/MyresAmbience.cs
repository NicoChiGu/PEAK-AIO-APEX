using System;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class MyresAmbience : MonoBehaviour
{
	// Token: 0x060012AA RID: 4778 RVA: 0x0005F09C File Offset: 0x0005D29C
	private void Update()
	{
		if (this.anim)
		{
			if (this.anim.GetFloat("Myers Distance") > 60f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0f, 1f * Time.deltaTime);
			}
			if (this.anim.GetFloat("Myers Distance") < 50f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.25f, 1f * Time.deltaTime);
			}
			if (this.anim.GetFloat("Myers Distance") < 25f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.75f, 1f * Time.deltaTime);
			}
			if (this.anim.GetFloat("Myers Distance") == 0f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0f, 1f * Time.deltaTime);
			}
		}
	}

	// Token: 0x04001164 RID: 4452
	public Animator anim;

	// Token: 0x04001165 RID: 4453
	public AudioSource fearMusic;
}
