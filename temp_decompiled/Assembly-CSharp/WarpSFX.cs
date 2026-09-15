using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200036F RID: 879
public class WarpSFX : MonoBehaviour
{
	// Token: 0x0600165D RID: 5725 RVA: 0x00073DDB File Offset: 0x00071FDB
	private void Update()
	{
		this.warpSFX.volume = this.vol.weight / 2f;
		this.warpSFX.pitch = 1f + this.vol.weight * 2f;
	}

	// Token: 0x0400153E RID: 5438
	public Volume vol;

	// Token: 0x0400153F RID: 5439
	public AudioSource warpSFX;
}
