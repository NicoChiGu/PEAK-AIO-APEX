using System;
using UnityEngine;

// Token: 0x02000251 RID: 593
public class EnableMusic : MonoBehaviour
{
	// Token: 0x0600111A RID: 4378 RVA: 0x000561D2 File Offset: 0x000543D2
	private void Update()
	{
		if (this.enable)
		{
			this.music.SetActive(true);
		}
	}

	// Token: 0x04000F88 RID: 3976
	public bool enable;

	// Token: 0x04000F89 RID: 3977
	public GameObject music;
}
