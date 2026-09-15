using System;
using UnityEngine;

// Token: 0x02000095 RID: 149
public class DebugDisableBasedOnSetting<T> : MonoBehaviour where T : OffOnSetting
{
	// Token: 0x060005CC RID: 1484 RVA: 0x000211A0 File Offset: 0x0001F3A0
	private void Update()
	{
		if (this.settings == null && GameHandler.Instance != null)
		{
			this.settings = GameHandler.Instance.SettingsHandler.GetSetting<T>();
		}
		if (this.settings != null)
		{
			this.target.SetActive(this.settings.Value == 0);
		}
	}

	// Token: 0x040005F2 RID: 1522
	public GameObject target;

	// Token: 0x040005F3 RID: 1523
	private T settings;
}
