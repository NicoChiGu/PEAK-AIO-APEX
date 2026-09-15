using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.UI;

// Token: 0x020001DB RID: 475
public class PauseMenuHandler : UIPageHandler
{
	// Token: 0x06000E89 RID: 3721 RVA: 0x00047724 File Offset: 0x00045924
	private void OnEnable()
	{
		if (PhotonNetwork.OfflineMode)
		{
			Time.timeScale = 0f;
		}
		if (!(this.currentPage is PauseMenuMainPage))
		{
			base.TransistionToPage<PauseMenuMainPage>();
			return;
		}
		this.currentPage.gameObject.SetActive(true);
		this.currentPage.OnPageEnter();
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x00047773 File Offset: 0x00045973
	private void OnDisable()
	{
		if (PhotonNetwork.OfflineMode)
		{
			Time.timeScale = 1f;
		}
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x00047788 File Offset: 0x00045988
	private void Update()
	{
		if (Character.localCharacter && (Character.localCharacter.input.pauseWasPressed || this.backButton.action.WasPressedThisFrame()))
		{
			IHaveParentPage haveParentPage = this.currentPage as IHaveParentPage;
			if (haveParentPage != null)
			{
				ValueTuple<UIPage, PageTransistion> parentPage = haveParentPage.GetParentPage();
				UIPage item = parentPage.Item1;
				PageTransistion item2 = parentPage.Item2;
				base.TransistionToPage(item, item2);
			}
			else if (!PreventUnpause.UnpausePreventionActive)
			{
				base.gameObject.SetActive(false);
			}
			Character.localCharacter.input.pauseWasPressed = false;
		}
	}

	// Token: 0x04000C7F RID: 3199
	public InputActionReference backButton;
}
