using System;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class BingBongAudioSwitch : MonoBehaviour
{
	// Token: 0x06000F98 RID: 3992 RVA: 0x0004D7D4 File Offset: 0x0004B9D4
	public void Init()
	{
		if (this.refObject.activeSelf)
		{
			for (int i = 0; i < this.enableLoop.Length; i++)
			{
				this.enableLoop[i].SetActive(true);
			}
			for (int j = 0; j < this.disableLoop.Length; j++)
			{
				this.disableLoop[j].SetActive(false);
			}
			for (int k = 0; k < this.clipOriginal.Length; k++)
			{
				this.clipOriginal[k].enabled = false;
			}
			for (int l = 0; l < this.clipReplace.Length; l++)
			{
				this.clipReplace[l].enabled = true;
			}
		}
		if (!this.refObject.activeSelf)
		{
			for (int m = 0; m < this.enableLoop.Length; m++)
			{
				this.enableLoop[m].SetActive(false);
			}
			for (int n = 0; n < this.disableLoop.Length; n++)
			{
				this.disableLoop[n].SetActive(true);
			}
			for (int num = 0; num < this.clipOriginal.Length; num++)
			{
				this.clipOriginal[num].enabled = true;
			}
			for (int num2 = 0; num2 < this.clipReplace.Length; num2++)
			{
				this.clipReplace[num2].enabled = false;
			}
		}
	}

	// Token: 0x04000DF4 RID: 3572
	public GameObject refObject;

	// Token: 0x04000DF5 RID: 3573
	public SFX_PlayOneShot[] clipOriginal;

	// Token: 0x04000DF6 RID: 3574
	public SFX_PlayOneShot[] clipReplace;

	// Token: 0x04000DF7 RID: 3575
	public GameObject[] enableLoop;

	// Token: 0x04000DF8 RID: 3576
	public GameObject[] disableLoop;
}
