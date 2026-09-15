using System;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class BugPhobia : MonoBehaviour
{
	// Token: 0x0600100B RID: 4107 RVA: 0x0004FC10 File Offset: 0x0004DE10
	private void Start()
	{
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<BugPhobiaSetting>();
		if (this.setting != null)
		{
			for (int i = 0; i < this.bugPhobiaGameObjects.Length; i++)
			{
				this.bugPhobiaGameObjects[i].SetActive(this.setting.Value == 1);
			}
			for (int j = 0; j < this.defaultGameObjects.Length; j++)
			{
				this.defaultGameObjects[j].SetActive(this.setting.Value != 1);
			}
			if (this.bbas)
			{
				this.bbas.Init();
			}
		}
	}

	// Token: 0x04000E79 RID: 3705
	public GameObject[] defaultGameObjects;

	// Token: 0x04000E7A RID: 3706
	public GameObject[] bugPhobiaGameObjects;

	// Token: 0x04000E7B RID: 3707
	private BugPhobiaSetting setting;

	// Token: 0x04000E7C RID: 3708
	public BingBongAudioSwitch bbas;
}
