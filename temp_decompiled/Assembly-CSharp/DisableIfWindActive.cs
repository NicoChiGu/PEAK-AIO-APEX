using System;
using UnityEngine;

// Token: 0x0200024A RID: 586
public class DisableIfWindActive : MonoBehaviour
{
	// Token: 0x06001107 RID: 4359 RVA: 0x00055E54 File Offset: 0x00054054
	private void FixedUpdate()
	{
		if (RootsWind.instance && this.gameObjectToDisable)
		{
			if (RootsWind.instance.windZone.windActive)
			{
				this.gameObjectToDisable.SetActive(false);
				return;
			}
			if (!this.disablePermanently)
			{
				this.gameObjectToDisable.SetActive(true);
			}
		}
	}

	// Token: 0x04000F78 RID: 3960
	public GameObject gameObjectToDisable;

	// Token: 0x04000F79 RID: 3961
	public bool disablePermanently;
}
