using System;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class FadeSFX : MonoBehaviour
{
	// Token: 0x06001131 RID: 4401 RVA: 0x0005683E File Offset: 0x00054A3E
	private void Update()
	{
		AudioListener.volume = this.f;
	}

	// Token: 0x04000FA8 RID: 4008
	public float f;
}
