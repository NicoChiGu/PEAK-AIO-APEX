using System;
using UnityEngine;
using Zorro.UI;

// Token: 0x020001E4 RID: 484
public class SettingsTABSButton : TAB_Button
{
	// Token: 0x06000EC7 RID: 3783 RVA: 0x00048628 File Offset: 0x00046828
	private void Update()
	{
		Color color = (base.Selected ? Color.black : Color.white);
		this.text.color = Color.Lerp(this.text.color, color, Time.unscaledDeltaTime * 7f);
		this.SelectedGraphic.gameObject.SetActive(base.Selected);
	}

	// Token: 0x04000CB1 RID: 3249
	public SettingsCategory category;

	// Token: 0x04000CB2 RID: 3250
	public GameObject SelectedGraphic;
}
