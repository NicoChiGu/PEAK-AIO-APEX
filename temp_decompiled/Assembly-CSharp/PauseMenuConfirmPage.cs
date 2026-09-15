using System;
using UnityEngine;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001D9 RID: 473
public class PauseMenuConfirmPage : UIPage, INavigationPage
{
	// Token: 0x06000E7B RID: 3707 RVA: 0x0004748E File Offset: 0x0004568E
	public GameObject GetFirstSelectedGameObject()
	{
		return this.firstButton.gameObject;
	}

	// Token: 0x04000C77 RID: 3191
	public Button firstButton;
}
