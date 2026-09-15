using System;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class PassportTab : MonoBehaviour
{
	// Token: 0x06000E72 RID: 3698 RVA: 0x000473DF File Offset: 0x000455DF
	public void SetTab()
	{
		if (!this.opened)
		{
			this.manager.OpenTab(this.type);
		}
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x000473FA File Offset: 0x000455FA
	public void Open()
	{
		this.anim.SetBool("Open", true);
		this.opened = true;
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x00047414 File Offset: 0x00045614
	public void Close()
	{
		this.anim.SetBool("Open", false);
		this.opened = false;
	}

	// Token: 0x04000C71 RID: 3185
	public PassportManager manager;

	// Token: 0x04000C72 RID: 3186
	public Customization.Type type;

	// Token: 0x04000C73 RID: 3187
	public Animator anim;

	// Token: 0x04000C74 RID: 3188
	private bool opened;
}
