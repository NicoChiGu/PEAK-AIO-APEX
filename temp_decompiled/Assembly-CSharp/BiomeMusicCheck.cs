using System;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class BiomeMusicCheck : MonoBehaviour
{
	// Token: 0x06000FC7 RID: 4039 RVA: 0x0004E8A4 File Offset: 0x0004CAA4
	private void Update()
	{
		if (!this.tornado)
		{
			this.regularMusic.SetActive(true);
			this.mesaMusic.SetActive(false);
			return;
		}
		if (this.tornado.active)
		{
			this.regularMusic.SetActive(false);
			this.mesaMusic.SetActive(true);
			return;
		}
		this.regularMusic.SetActive(true);
		this.mesaMusic.SetActive(false);
	}

	// Token: 0x04000E22 RID: 3618
	public GameObject tornado;

	// Token: 0x04000E23 RID: 3619
	public GameObject regularMusic;

	// Token: 0x04000E24 RID: 3620
	public GameObject mesaMusic;
}
