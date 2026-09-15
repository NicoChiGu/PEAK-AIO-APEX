using System;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001DC RID: 476
public class PauseMenuMainPage : UIPage, INavigationPage
{
	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000E8D RID: 3725 RVA: 0x0004781C File Offset: 0x00045A1C
	public Button resumeButton
	{
		get
		{
			return this.m_resumeButton;
		}
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x00047824 File Offset: 0x00045A24
	private void Start()
	{
		this.m_quitButton.onClick.AddListener(new UnityAction(this.OnQuitClicked));
		this.m_settingsButton.onClick.AddListener(new UnityAction(this.OnSettingsClicked));
		this.m_resumeButton.onClick.AddListener(new UnityAction(this.OnResumeClicked));
		this.m_accoladesButton.onClick.AddListener(new UnityAction(this.OnAccoladesClicked));
		this.m_controllsButton.onClick.AddListener(new UnityAction(this.OnControlsClicked));
		this.m_inviteButton.onClick.AddListener(new UnityAction(this.InviteFriendsClicked));
		this.m_confirmCancelButton.onClick.AddListener(new UnityAction(this.ConfirmCancel));
		if (PhotonNetwork.OfflineMode)
		{
			Navigation navigation = this.resumeButton.navigation;
			navigation.selectOnDown = this.m_accoladesButton;
			this.resumeButton.navigation = navigation;
		}
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x00047921 File Offset: 0x00045B21
	private void OnEnable()
	{
		this.m_inviteButton.gameObject.SetActive(!PhotonNetwork.OfflineMode);
	}

	// Token: 0x06000E90 RID: 3728 RVA: 0x0004793B File Offset: 0x00045B3B
	private void OnDisable()
	{
		if (this.confirmWindow.isOpen)
		{
			this.confirmWindow.Close();
		}
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x00047958 File Offset: 0x00045B58
	private void InviteFriendsClicked()
	{
		CSteamID csteamID;
		if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out csteamID))
		{
			SteamFriends.ActivateGameOverlayInviteDialog(csteamID);
		}
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x00047979 File Offset: 0x00045B79
	private void OnControlsClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuControlsPage>();
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x00047987 File Offset: 0x00045B87
	private void OnAccoladesClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuAccoladesPage>();
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x00047995 File Offset: 0x00045B95
	private void OnResumeClicked()
	{
		this.pageHandler.gameObject.SetActive(false);
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x000479A8 File Offset: 0x00045BA8
	private void OnSettingsClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuSettingsMenuPage>();
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x000479B6 File Offset: 0x00045BB6
	private void OnQuitClicked()
	{
		this.OpenQuitConfirmWindow();
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x000479C0 File Offset: 0x00045BC0
	private void OpenQuitConfirmWindow()
	{
		this.confirmWindow.Open();
		this.confirmText.SetTextLocalized("LEAVE_GAME_CONFIRM");
		this.m_confirmOkButton.onClick.RemoveAllListeners();
		this.m_confirmOkButton.onClick.AddListener(new UnityAction(this.Quit));
		this.m_confirmCancelButton.Select();
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x00047A20 File Offset: 0x00045C20
	public Button OpenKickConfirmWindow(string playerName)
	{
		this.confirmWindow.Open();
		this.confirmText.SetText(LocalizedText.GetText("MODAL_KICK_PROMPT", true).Replace("#", playerName));
		this.confirmWindow.SetInputActive(true);
		this.m_confirmOkButton.onClick.RemoveAllListeners();
		this.m_confirmOkButton.onClick.AddListener(new UnityAction(this.ConfirmCancel));
		this.m_confirmCancelButton.Select();
		return this.m_confirmOkButton;
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x00047AA2 File Offset: 0x00045CA2
	private void ConfirmCancel()
	{
		this.confirmWindow.Close();
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x00047AAF File Offset: 0x00045CAF
	private void Quit()
	{
		this.confirmWindow.Close();
		this.pageHandler.gameObject.SetActive(false);
		Player.LeaveCurrentGame();
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x00047AD2 File Offset: 0x00045CD2
	public GameObject GetFirstSelectedGameObject()
	{
		return this.m_resumeButton.gameObject;
	}

	// Token: 0x04000C80 RID: 3200
	[SerializeField]
	private Button m_quitButton;

	// Token: 0x04000C81 RID: 3201
	[SerializeField]
	private Button m_settingsButton;

	// Token: 0x04000C82 RID: 3202
	[SerializeField]
	private Button m_resumeButton;

	// Token: 0x04000C83 RID: 3203
	[SerializeField]
	private Button m_accoladesButton;

	// Token: 0x04000C84 RID: 3204
	[SerializeField]
	private Button m_controllsButton;

	// Token: 0x04000C85 RID: 3205
	[SerializeField]
	private Button m_inviteButton;

	// Token: 0x04000C86 RID: 3206
	public MenuWindow confirmWindow;

	// Token: 0x04000C87 RID: 3207
	[SerializeField]
	private Button m_confirmOkButton;

	// Token: 0x04000C88 RID: 3208
	[SerializeField]
	private Button m_confirmCancelButton;

	// Token: 0x04000C89 RID: 3209
	public LocalizedText confirmText;
}
