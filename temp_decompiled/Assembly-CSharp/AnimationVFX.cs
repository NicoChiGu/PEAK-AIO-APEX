using System;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class AnimationVFX : MonoBehaviour
{
	// Token: 0x06000F4A RID: 3914 RVA: 0x0004B7CD File Offset: 0x000499CD
	public void PlayVFX(int x)
	{
		this.vfx[x].Play();
	}

	// Token: 0x04000D8E RID: 3470
	public ParticleSystem[] vfx;
}
