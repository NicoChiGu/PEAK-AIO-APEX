using System;
using UnityEngine;

// Token: 0x02000249 RID: 585
public class DisableIfPhotosensitive : MonoBehaviour
{
	// Token: 0x06001105 RID: 4357 RVA: 0x00055E0B File Offset: 0x0005400B
	private void Start()
	{
		if (GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>().Value == 1)
		{
			this.objectToDisable.SetActive(false);
			if (this.objectToReplace)
			{
				this.objectToReplace.SetActive(true);
			}
		}
	}

	// Token: 0x04000F76 RID: 3958
	public GameObject objectToDisable;

	// Token: 0x04000F77 RID: 3959
	public GameObject objectToReplace;
}
