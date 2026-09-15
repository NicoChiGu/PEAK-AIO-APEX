using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.UI.Modal;

// Token: 0x0200007B RID: 123
public class KickedState : ConnectionState
{
	// Token: 0x0600054E RID: 1358 RVA: 0x0001F420 File Offset: 0x0001D620
	public static void DisplayModal()
	{
		LoadingScreenHandler.KillCurrentLoadingScreen();
		HeaderModalOption headerModalOption = new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_KICKED_TITLE", true), LocalizedText.GetText("MODAL_KICKED_BODY", true));
		ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[1];
		array[0] = new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), delegate
		{
		});
		Modal.OpenModal(headerModalOption, new ModalButtonsOption(array), null);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001F490 File Offset: 0x0001D690
	public override void Enter()
	{
		base.Enter();
		Debug.Log("Getting booted!!");
		Player.LeaveCurrentGame();
		SceneManager.LoadScene("Title");
	}

	// Token: 0x04000598 RID: 1432
	private const string ModalTitleKey = "MODAL_KICKED_TITLE";

	// Token: 0x04000599 RID: 1433
	private const string ModalBodyKey = "MODAL_KICKED_BODY";
}
