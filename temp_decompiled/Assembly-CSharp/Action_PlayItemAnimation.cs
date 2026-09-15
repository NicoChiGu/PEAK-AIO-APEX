using System;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class Action_PlayItemAnimation : ItemAction
{
	// Token: 0x0600084A RID: 2122 RVA: 0x0002DAA6 File Offset: 0x0002BCA6
	public override void RunAction()
	{
		this.anim.Play(this.animationName, 0, 0f);
	}

	// Token: 0x040007F4 RID: 2036
	public Animator anim;

	// Token: 0x040007F5 RID: 2037
	public string animationName;
}
