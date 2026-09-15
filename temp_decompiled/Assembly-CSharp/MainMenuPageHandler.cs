using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.UI;
using Zorro.UI.Modal;

// Token: 0x020001CD RID: 461
public class MainMenuPageHandler : UIPageHandler
{
	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000E29 RID: 3625 RVA: 0x00046902 File Offset: 0x00044B02
	private static bool CanSkipVersionCheck
	{
		get
		{
			return Debug.isDebugBuild;
		}
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000E2A RID: 3626 RVA: 0x00046909 File Offset: 0x00044B09
	private static ModalButtonsOption.Option CloseGameOption
	{
		get
		{
			if (!MainMenuPageHandler.CanSkipVersionCheck)
			{
				return new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null);
			}
			return new ModalButtonsOption.Option("Nah", null);
		}
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x0004692F File Offset: 0x00044B2F
	protected override void Start()
	{
		base.Start();
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<DefaultConnectionState>(false);
		CloudAPI.CheckVersion(delegate(LoginResponse response)
		{
			GameHandler.GetService<NextLevelService>().NewData(response);
			if (!response.VersionOkay)
			{
				HeaderModalOption headerModalOption = new DefaultHeaderModalOption(LocalizedText.GetText("VERSIONOUTOFDATE", true), LocalizedText.GetText("CLOSETHEGAME", true));
				ModalContentOption modalContentOption = new ModalButtonsOption(new ModalButtonsOption.Option[] { MainMenuPageHandler.CloseGameOption });
				Action action;
				if (!MainMenuPageHandler.CanSkipVersionCheck)
				{
					action = new Action(Application.Quit);
				}
				else
				{
					action = delegate
					{
					};
				}
				Modal.OpenModal(headerModalOption, modalContentOption, action);
			}
		});
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x0004696C File Offset: 0x00044B6C
	private void Update()
	{
		if (this.BackReference.action.WasPerformedThisFrame())
		{
			IHaveParentPage haveParentPage = this.currentPage as IHaveParentPage;
			if (haveParentPage != null)
			{
				ValueTuple<UIPage, PageTransistion> parentPage = haveParentPage.GetParentPage();
				UIPage item = parentPage.Item1;
				PageTransistion item2 = parentPage.Item2;
				base.TransistionToPage(item, item2);
			}
		}
		if ((PhotonNetwork.NetworkClientState == 15 && !(this.currentPage is MainMenuFirstTimeSetupPage)) || PhotonNetwork.OfflineMode)
		{
			this.IntroAnimation.SetBool(MainMenuPageHandler.Connected, true);
		}
		IsDisconnectingForOfflineMode isDisconnectingForOfflineMode;
		if (!this.disconnected && PhotonNetwork.NetworkClientState == 14 && !PhotonNetwork.OfflineMode && !GameHandler.TryGetStatus<IsDisconnectingForOfflineMode>(out isDisconnectingForOfflineMode))
		{
			this.disconnected = true;
			Debug.Log("Opening disconnected modal");
			HeaderModalOption headerModalOption = new DefaultHeaderModalOption(LocalizedText.GetText("FAILEDTOCONNECT", true), LocalizedText.GetText("TRYCONNECTAGAIN", true));
			ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[2];
			array[0] = new ModalButtonsOption.Option(LocalizedText.GetText("TRYAGAIN", true), delegate
			{
				PhotonNetwork.OfflineMode = false;
				NetworkingUtilities.ConnectToNetwork();
				base.StartCoroutine(this.<Update>g__Timeout|10_2());
			});
			array[1] = new ModalButtonsOption.Option(LocalizedText.GetText("PLAYOFFLINE", true), delegate
			{
				PhotonNetwork.OfflineMode = true;
			});
			Modal.OpenModal(headerModalOption, new ModalButtonsOption(array), null);
		}
		this.ConnectingInfoText.text = this.GetPrettyStateName();
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x00046AB0 File Offset: 0x00044CB0
	private string GetPrettyStateName()
	{
		ClientState networkClientState = PhotonNetwork.NetworkClientState;
		if (networkClientState != 1)
		{
			switch (networkClientState)
			{
			case 12:
			case 16:
			case 17:
				return LocalizedText.GetText("CONNECTING", true);
			case 15:
				return "";
			}
			return networkClientState.ToString();
		}
		return LocalizedText.GetText("AUTHENTICATING", true);
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x00046B49 File Offset: 0x00044D49
	[CompilerGenerated]
	private IEnumerator <Update>g__Timeout|10_2()
	{
		yield return new WaitForSecondsRealtime(5f);
		this.disconnected = false;
		yield break;
	}

	// Token: 0x04000C50 RID: 3152
	private static readonly int Connected = Animator.StringToHash("Connected");

	// Token: 0x04000C51 RID: 3153
	public InputActionReference BackReference;

	// Token: 0x04000C52 RID: 3154
	public Animator IntroAnimation;

	// Token: 0x04000C53 RID: 3155
	public TextMeshProUGUI ConnectingInfoText;

	// Token: 0x04000C54 RID: 3156
	private bool disconnected;
}
