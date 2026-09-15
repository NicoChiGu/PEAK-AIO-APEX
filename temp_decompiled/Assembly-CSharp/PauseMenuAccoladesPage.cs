using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001D8 RID: 472
public class PauseMenuAccoladesPage : UIPage, IHaveParentPage, INavigationPage
{
	// Token: 0x06000E76 RID: 3702 RVA: 0x00047436 File Offset: 0x00045636
	private void Start()
	{
		this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x00047454 File Offset: 0x00045654
	private void BackClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuMainPage>();
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x00047462 File Offset: 0x00045662
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<PauseMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x00047479 File Offset: 0x00045679
	public GameObject GetFirstSelectedGameObject()
	{
		return this.firstBadge.gameObject;
	}

	// Token: 0x04000C75 RID: 3189
	public Button firstBadge;

	// Token: 0x04000C76 RID: 3190
	public Button backButton;
}
