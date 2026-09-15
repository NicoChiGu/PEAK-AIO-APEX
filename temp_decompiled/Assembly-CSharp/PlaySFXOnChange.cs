using System;
using UnityEngine;

// Token: 0x020002C8 RID: 712
public class PlaySFXOnChange : MonoBehaviour
{
	// Token: 0x06001342 RID: 4930 RVA: 0x000620E8 File Offset: 0x000602E8
	private void Update()
	{
		if (this.refObj.active && !this.t)
		{
			this.t = true;
			for (int i = 0; i < this.sfxOn.Length; i++)
			{
				this.sfxOn[i].Play(default(Vector3));
			}
		}
		if (!this.refObj.active && this.t)
		{
			this.t = false;
			for (int j = 0; j < this.sfxOff.Length; j++)
			{
				this.sfxOff[j].Play(default(Vector3));
			}
		}
	}

	// Token: 0x040011E9 RID: 4585
	public SFX_Instance[] sfxOn;

	// Token: 0x040011EA RID: 4586
	public SFX_Instance[] sfxOff;

	// Token: 0x040011EB RID: 4587
	private bool t;

	// Token: 0x040011EC RID: 4588
	public GameObject refObj;
}
