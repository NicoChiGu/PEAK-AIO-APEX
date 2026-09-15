using System;
using UnityEngine.InputSystem;
using Zorro.UI;

// Token: 0x020001E3 RID: 483
public class SettingsTABS : TABS<SettingsTABSButton>
{
	// Token: 0x06000EC4 RID: 3780 RVA: 0x000485D7 File Offset: 0x000467D7
	public override void OnSelected(SettingsTABSButton button)
	{
		this.SettingsMenu.ShowSettings(button.category);
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x000485EA File Offset: 0x000467EA
	private void Update()
	{
		if (this.RightAction.action.WasPressedThisFrame())
		{
			base.SelectNext();
			return;
		}
		if (this.LeftAction.action.WasPressedThisFrame())
		{
			base.SelectPrevious();
		}
	}

	// Token: 0x04000CAE RID: 3246
	public SharedSettingsMenu SettingsMenu;

	// Token: 0x04000CAF RID: 3247
	public InputActionReference RightAction;

	// Token: 0x04000CB0 RID: 3248
	public InputActionReference LeftAction;
}
