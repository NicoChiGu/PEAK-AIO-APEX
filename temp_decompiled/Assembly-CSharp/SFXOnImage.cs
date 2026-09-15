using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200032B RID: 811
public class SFXOnImage : MonoBehaviour
{
	// Token: 0x060014CD RID: 5325 RVA: 0x00069B7C File Offset: 0x00067D7C
	private void Update()
	{
		if (this.image.texture != this.tex)
		{
			for (int i = 0; i < this.equipSound.Length; i++)
			{
				this.equipSound[i].Play(default(Vector3));
			}
		}
		this.tex = this.image.texture;
	}

	// Token: 0x04001342 RID: 4930
	public RawImage image;

	// Token: 0x04001343 RID: 4931
	private Texture tex;

	// Token: 0x04001344 RID: 4932
	public SFX_Instance[] equipSound;
}
