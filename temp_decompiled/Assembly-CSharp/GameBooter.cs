using System;
using Peak;
using Peak.Network;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Token: 0x02000082 RID: 130
public static class GameBooter
{
	// Token: 0x06000568 RID: 1384 RVA: 0x0001F6A4 File Offset: 0x0001D8A4
	[RuntimeInitializeOnLoadMethod(3)]
	public static void Initialize()
	{
		IntegratedGraphicsHelper.DisableGPUCullingIfNecessary();
		Application.quitting += new Action(GameBooter.CleanUp);
		SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(GameBooter.OnSceneLoaded);
		SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(GameBooter.AutoBoot);
		NetCode.Matchmaking.InviteReceived += new Action(GameBooter.CheckForInviteAndJoinWhenReady);
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0001F6FF File Offset: 0x0001D8FF
	private static void CleanUp()
	{
		Application.quitting -= new Action(GameBooter.CleanUp);
		SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(GameBooter.OnSceneLoaded);
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0001F723 File Offset: 0x0001D923
	private static void OnSceneLoaded(Scene sceneLoaded, LoadSceneMode _)
	{
		if (sceneLoaded.name == "Title")
		{
			GameBooter.CheckForInviteAndJoinWhenReady();
		}
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0001F73D File Offset: 0x0001D93D
	private static void JoinLobbyAfterRelayReady()
	{
		NetCode.Session.ConnectedAndReady -= new Action(GameBooter.JoinLobbyAfterRelayReady);
		GameBooter.CheckForInviteAndJoinWhenReady();
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0001F75A File Offset: 0x0001D95A
	private static void CheckForInviteAndJoinWhenReady()
	{
		if (!NetCode.Session.IsConnectedAndReady)
		{
			NetCode.ConnectionEvents.ConnectedAndReady += new Action(GameBooter.JoinLobbyAfterRelayReady);
			return;
		}
		if (NetCode.Matchmaking.HasPendingInvite)
		{
			NetCode.Matchmaking.ConsumePendingJoin();
		}
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0001F795 File Offset: 0x0001D995
	private static void AutoBoot(Scene _, LoadSceneMode __)
	{
		SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(GameBooter.AutoBoot);
		GameObject gameObject = new GameObject("Game");
		gameObject.AddComponent<GameHandler>().Initialize();
		gameObject.AddComponent<UIInputHandler>().Initialize();
	}
}
