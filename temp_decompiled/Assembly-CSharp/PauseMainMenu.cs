using System;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020002B8 RID: 696
public class PauseMainMenu : MenuWindow
{
	// Token: 0x17000127 RID: 295
	// (get) Token: 0x060012FA RID: 4858 RVA: 0x000605EB File Offset: 0x0005E7EB
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x060012FB RID: 4859 RVA: 0x000605EE File Offset: 0x0005E7EE
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x060012FC RID: 4860 RVA: 0x000605F1 File Offset: 0x0005E7F1
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x060012FD RID: 4861 RVA: 0x000605F4 File Offset: 0x0005E7F4
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x000605F7 File Offset: 0x0005E7F7
	protected override void Initialize()
	{
		this.backButton.onClick.AddListener(new UnityAction(base.Close));
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x00060615 File Offset: 0x0005E815
	protected override void OnClose()
	{
		this.mainMenu.Open();
	}

	// Token: 0x0400119F RID: 4511
	public MenuWindow mainMenu;

	// Token: 0x040011A0 RID: 4512
	public Button backButton;
}
