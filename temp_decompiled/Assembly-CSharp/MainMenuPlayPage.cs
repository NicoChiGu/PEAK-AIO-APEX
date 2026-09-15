using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.UI;

// Token: 0x020001CF RID: 463
public class MainMenuPlayPage : UIPage, IHaveParentPage
{
	// Token: 0x06000E34 RID: 3636 RVA: 0x00046B94 File Offset: 0x00044D94
	private void Start()
	{
		this.m_playButton.onClick.AddListener(new UnityAction(this.PlayClicked));
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x00046BB2 File Offset: 0x00044DB2
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<MainMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x00046BCC File Offset: 0x00044DCC
	public void PlayClicked()
	{
		if (string.IsNullOrEmpty(this.m_usernameField.text))
		{
			Debug.LogError("Failed to get username field...");
			return;
		}
		if (string.IsNullOrEmpty(this.m_roomField.text))
		{
			Debug.LogError("Failed to get room name field...");
			return;
		}
		JoinSpecificRoomState joinSpecificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false);
		joinSpecificRoomState.RoomName = this.m_roomField.text.ToLower();
		joinSpecificRoomState.RegionToJoin = "eu";
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f) });
	}

	// Token: 0x04000C57 RID: 3159
	[SerializeField]
	private Button m_playButton;

	// Token: 0x04000C58 RID: 3160
	[SerializeField]
	private TMP_InputField m_usernameField;

	// Token: 0x04000C59 RID: 3161
	[SerializeField]
	private TMP_InputField m_roomField;
}
