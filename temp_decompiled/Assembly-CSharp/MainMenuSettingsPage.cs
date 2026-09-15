using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001D0 RID: 464
public class MainMenuSettingsPage : UIPage, IHaveParentPage, INavigationPage
{
	// Token: 0x06000E38 RID: 3640 RVA: 0x00046C76 File Offset: 0x00044E76
	private void Start()
	{
		this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x00046C94 File Offset: 0x00044E94
	private void BackClicked()
	{
		this.pageHandler.TransistionToPage<MainMenuMainPage>();
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x00046CA2 File Offset: 0x00044EA2
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<MainMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x00046CBC File Offset: 0x00044EBC
	public GameObject GetFirstSelectedGameObject()
	{
		GameObject defaultSelection = this.SharedSettingsMenu.GetDefaultSelection();
		if (defaultSelection)
		{
			return defaultSelection;
		}
		return this.backButton.gameObject;
	}

	// Token: 0x04000C5A RID: 3162
	public SharedSettingsMenu SharedSettingsMenu;

	// Token: 0x04000C5B RID: 3163
	public Button backButton;
}
