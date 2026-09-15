using System;
using UnityEngine;

// Token: 0x0200023A RID: 570
public class CursedSkullVFX : ItemVFX
{
	// Token: 0x060010CE RID: 4302 RVA: 0x00054AB7 File Offset: 0x00052CB7
	protected override void Start()
	{
		base.Start();
		this.curseParticles.Play();
		this.animator.enabled = true;
		if (this.item.itemState == ItemState.InBackpack)
		{
			this.disableInBackpack.SetActive(false);
		}
	}

	// Token: 0x04000F1B RID: 3867
	public ParticleSystem curseParticles;

	// Token: 0x04000F1C RID: 3868
	public Animator animator;

	// Token: 0x04000F1D RID: 3869
	public GameObject disableInBackpack;
}
