using System;
using Peak.Network;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001CC RID: 460
public class MainMenuMainPage : UIPage, INavigationPage
{
	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000E21 RID: 3617 RVA: 0x0004683F File Offset: 0x00044A3F
	public Button PlayButton
	{
		get
		{
			return this.m_playButton;
		}
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x00046848 File Offset: 0x00044A48
	private void Start()
	{
		this.m_playButton.onClick.AddListener(new UnityAction(this.PlayClicked));
		this.m_settingsButton.onClick.AddListener(new UnityAction(this.SettingsClicked));
		NetworkingUtilities.ConnectToNetwork();
		GameHandler.GetService<RichPresenceService>().SetState(RichPresenceState.Status_MainMenu);
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x0004689E File Offset: 0x00044A9E
	private void SettingsClicked()
	{
		this.pageHandler.TransistionToPage<MainMenuSettingsPage>(new SetActivePageTransistion());
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x000468B1 File Offset: 0x00044AB1
	private void OnDestroy()
	{
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x000468B3 File Offset: 0x00044AB3
	private void PlayClicked()
	{
		NetCode.Matchmaking.CreateLobby(GameHandler.Instance.SettingsHandler.GetSetting<LobbyTypeSetting>().Value);
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x000468D3 File Offset: 0x00044AD3
	private void Update()
	{
		this.m_playButton.gameObject.SetActive(!PhotonNetwork.OfflineMode);
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x000468ED File Offset: 0x00044AED
	public GameObject GetFirstSelectedGameObject()
	{
		return this.m_playButton.gameObject;
	}

	// Token: 0x04000C4E RID: 3150
	[SerializeField]
	private Button m_playButton;

	// Token: 0x04000C4F RID: 3151
	[SerializeField]
	private Button m_settingsButton;
}
