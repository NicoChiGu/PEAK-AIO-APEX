using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020002A2 RID: 674
public class MainMenu : MonoBehaviour
{
	// Token: 0x06001273 RID: 4723 RVA: 0x0005DF00 File Offset: 0x0005C100
	private void Start()
	{
		AudioLevels.ResetSliders();
		this.playSoloButton.onClick.AddListener(new UnityAction(this.PlaySoloClicked));
		this.creditsButton.onClick.AddListener(new UnityAction(this.ToggleCredits));
		this.quitButton.onClick.AddListener(new UnityAction(this.Quit));
		this.discordButton.onClick.AddListener(new UnityAction(this.OpenDiscord));
		this.landfallButton.onClick.AddListener(new UnityAction(this.OpenLandfallWebsite));
		this.aggrocrabButton.onClick.AddListener(new UnityAction(this.OpenAggrocrabWebsite));
		Time.timeScale = 1f;
		if (NetworkConnector.CurrentConnectionState is KickedState)
		{
			KickedState.DisplayModal();
		}
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x0005DFD5 File Offset: 0x0005C1D5
	public void ToggleCredits()
	{
		this.credits.SetActive(!this.credits.activeSelf);
		if (this.credits.activeSelf)
		{
			this.RandomizeMainGuys();
		}
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x0005E003 File Offset: 0x0005C203
	public void OpenDiscord()
	{
		Application.OpenURL("https://discord.gg/peakgame");
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x0005E00F File Offset: 0x0005C20F
	public void OpenLandfallWebsite()
	{
		Application.OpenURL("https://landfall.se/");
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x0005E01B File Offset: 0x0005C21B
	public void OpenAggrocrabWebsite()
	{
		Application.OpenURL("https://aggrocrab.com/");
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x0005E028 File Offset: 0x0005C228
	public void RandomizeMainGuys()
	{
		Transform transform = this.mainGuysHolder;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			list.Add(transform.GetChild(i));
		}
		for (int j = list.Count - 1; j > 0; j--)
		{
			int num = Random.Range(0, j + 1);
			List<Transform> list2 = list;
			int num2 = j;
			List<Transform> list3 = list;
			int num3 = num;
			Transform transform2 = list[num];
			Transform transform3 = list[j];
			list2[num2] = transform2;
			list3[num3] = transform3;
		}
		for (int k = 0; k < list.Count; k++)
		{
			list[k].SetSiblingIndex(k);
		}
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x0005E0DA File Offset: 0x0005C2DA
	public void Quit()
	{
		Application.Quit();
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x0005E0E1 File Offset: 0x0005C2E1
	private void PlaySoloClicked()
	{
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { this.StartOfflineModeRoutine() });
	}

	// Token: 0x0600127B RID: 4731 RVA: 0x0005E0FE File Offset: 0x0005C2FE
	private IEnumerator StartOfflineModeRoutine()
	{
		PhotonNetwork.IsMessageQueueRunning = true;
		GameHandler.AddStatus<IsDisconnectingForOfflineMode>(new IsDisconnectingForOfflineMode());
		PhotonNetwork.Disconnect();
		while (PhotonNetwork.IsConnected)
		{
			Debug.Log("We are still connected.. waiting for disconnect");
			yield return null;
		}
		PhotonNetwork.OfflineMode = true;
		GameHandler.ClearStatus<IsDisconnectingForOfflineMode>();
		yield return RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f);
		yield break;
	}

	// Token: 0x04001121 RID: 4385
	public GameObject credits;

	// Token: 0x04001122 RID: 4386
	public Transform mainGuysHolder;

	// Token: 0x04001123 RID: 4387
	public const string SceneName = "Title";

	// Token: 0x04001124 RID: 4388
	public Button playSoloButton;

	// Token: 0x04001125 RID: 4389
	public Button creditsButton;

	// Token: 0x04001126 RID: 4390
	public Button quitButton;

	// Token: 0x04001127 RID: 4391
	public Button discordButton;

	// Token: 0x04001128 RID: 4392
	public Button landfallButton;

	// Token: 0x04001129 RID: 4393
	public Button aggrocrabButton;
}
