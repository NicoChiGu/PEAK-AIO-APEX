using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class ItemParticles : MonoBehaviour
{
	// Token: 0x060002C8 RID: 712 RVA: 0x00013BA3 File Offset: 0x00011DA3
	public void EnableSmoke(bool active)
	{
		if (this.smoke)
		{
			if (active)
			{
				this.smoke.Play();
				return;
			}
			this.smoke.Stop();
		}
	}

	// Token: 0x0400029C RID: 668
	public ParticleSystem smoke;
}
