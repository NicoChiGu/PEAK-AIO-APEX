using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001DF RID: 479
public class PauseMenuSettingsMenuPage : UIPage, IHaveParentPage, INavigationPage
{
	// Token: 0x06000EAF RID: 3759 RVA: 0x000480EE File Offset: 0x000462EE
	private void Start()
	{
		this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x0004810C File Offset: 0x0004630C
	private void BackClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuMainPage>();
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0004811A File Offset: 0x0004631A
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<PauseMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x00048134 File Offset: 0x00046334
	public GameObject GetFirstSelectedGameObject()
	{
		GameObject defaultSelection = this.sharedSettingsMenu.GetDefaultSelection();
		if (defaultSelection == null)
		{
			return this.backButton.gameObject;
		}
		return defaultSelection;
	}

	// Token: 0x04000C9D RID: 3229
	public Button backButton;

	// Token: 0x04000C9E RID: 3230
	public SharedSettingsMenu sharedSettingsMenu;
}
