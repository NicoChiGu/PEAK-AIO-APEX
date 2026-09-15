using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zorro.Core;

// Token: 0x02000083 RID: 131
[DefaultExecutionOrder(-100)]
public class GameHandler : MonoBehaviour
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x0600056E RID: 1390 RVA: 0x0001F7C7 File Offset: 0x0001D9C7
	public static GameHandler Instance
	{
		get
		{
			return GameHandler._instance;
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x0600056F RID: 1391 RVA: 0x0001F7CE File Offset: 0x0001D9CE
	// (set) Token: 0x06000570 RID: 1392 RVA: 0x0001F7D6 File Offset: 0x0001D9D6
	public SettingsHandler SettingsHandler { get; private set; }

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x06000571 RID: 1393 RVA: 0x0001F7DF File Offset: 0x0001D9DF
	public static bool PlayersHaveLeftShore
	{
		get
		{
			return GameHandler.IsOnIsland && (Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.Beach || OrbFogHandler.IsFoggingCurrentSegment);
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x06000572 RID: 1394 RVA: 0x0001F7FD File Offset: 0x0001D9FD
	public static bool PlayersAreResting
	{
		get
		{
			return GameHandler.IsOnIsland && !MapHandler.CurrentBaseCampIsFogged;
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000573 RID: 1395 RVA: 0x0001F810 File Offset: 0x0001DA10
	public static bool IsOnIsland
	{
		get
		{
			return GameHandler.Instance != null && MapHandler.Exists;
		}
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000574 RID: 1396 RVA: 0x0001F826 File Offset: 0x0001DA26
	public static bool Initialized
	{
		get
		{
			return GameHandler.Instance != null && GameHandler.Instance.m_initialized;
		}
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0001F841 File Offset: 0x0001DA41
	public void Initialize()
	{
		Debug.Log("Game Handler Initialized");
		GameHandler._instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0001F860 File Offset: 0x0001DA60
	private void OnDestroy()
	{
		Debug.Log("Game Handler Destroying...");
		foreach (GameService gameService in this.m_gameServices.Values)
		{
			gameService.OnDestroy();
		}
		SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded);
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0001F8D0 File Offset: 0x0001DAD0
	private bool IsGameplayScene(Scene scene)
	{
		return scene.name.Contains("Island") || scene.name.Contains("Level_");
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0001F8F8 File Offset: 0x0001DAF8
	private void Awake()
	{
		GameHandler.<Awake>d__21 <Awake>d__;
		<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Awake>d__.<>4__this = this;
		<Awake>d__.<>1__state = -1;
		<Awake>d__.<>t__builder.Start<GameHandler.<Awake>d__21>(ref <Awake>d__);
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0001F92F File Offset: 0x0001DB2F
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (mode == null)
		{
			GameHandler.RestartService<PlayerHandler>(new PlayerHandler());
			return;
		}
		Debug.LogWarning("WHOA " + scene.name + " was loaded additively. I didn't think we did that!!");
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0001F95C File Offset: 0x0001DB5C
	private void RegisterService<T>(T service) where T : GameService
	{
		Type type = service.GetType();
		this.m_gameServices[type] = service;
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0001F987 File Offset: 0x0001DB87
	public static T GetService<T>() where T : GameService
	{
		GameHandler instance = GameHandler.Instance;
		return ((instance != null) ? instance.m_gameServices[typeof(T)] : null) as T;
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0001F9B4 File Offset: 0x0001DBB4
	public static Awaitable WaitForInitialization()
	{
		GameHandler.<WaitForInitialization>d__25 <WaitForInitialization>d__;
		<WaitForInitialization>d__.<>t__builder = Awaitable.AwaitableAsyncMethodBuilder.Create();
		<WaitForInitialization>d__.<>1__state = -1;
		<WaitForInitialization>d__.<>t__builder.Start<GameHandler.<WaitForInitialization>d__25>(ref <WaitForInitialization>d__);
		return <WaitForInitialization>d__.<>t__builder.Task;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0001F9F0 File Offset: 0x0001DBF0
	private static T RestartService<T>(T service) where T : GameService, IDisposable
	{
		Debug.Log("Restarting Service of type: " + typeof(T).Name);
		Type type = service.GetType();
		if (GameHandler.Instance.m_gameServices.ContainsKey(type))
		{
			((T)((object)GameHandler.Instance.m_gameServices[type])).Dispose();
		}
		GameHandler.Instance.m_gameServices[type] = service;
		return service;
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x0001FA70 File Offset: 0x0001DC70
	public static void AddStatus<T>(GameStatus status) where T : GameStatus
	{
		Type type = status.GetType();
		GameHandler.Instance.m_gameStatus[type] = status;
		Debug.Log(string.Format("Add status: {0}", type));
		SceneSwitchingStatus sceneSwitchingStatus = status as SceneSwitchingStatus;
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0001FAAC File Offset: 0x0001DCAC
	public static bool TryGetStatus<T>(out T status) where T : GameStatus
	{
		Type typeFromHandle = typeof(T);
		GameStatus gameStatus;
		bool flag = GameHandler.Instance.m_gameStatus.TryGetValue(typeFromHandle, ref gameStatus);
		status = default(T);
		if (flag)
		{
			status = gameStatus as T;
		}
		return flag;
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0001FAF4 File Offset: 0x0001DCF4
	public static void ClearStatus<T>() where T : GameStatus
	{
		Type typeFromHandle = typeof(T);
		if (GameHandler.Instance.m_gameStatus.ContainsKey(typeFromHandle))
		{
			GameHandler.Instance.m_gameStatus.Remove(typeFromHandle);
			Debug.Log(string.Format("Clear status: {0}", typeFromHandle));
		}
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x0001FB3F File Offset: 0x0001DD3F
	public static void ClearAllStatuses()
	{
		GameHandler.Instance.m_gameStatus.Clear();
		Debug.Log("Clearing all statuses!");
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0001FB5C File Offset: 0x0001DD5C
	private void Update()
	{
		this.SettingsHandler.Update();
		foreach (GameService gameService in this.m_gameServices.Values)
		{
			gameService.Update();
		}
		Debug.ClearDeveloperConsole();
	}

	// Token: 0x040005A9 RID: 1449
	private static GameHandler _instance;

	// Token: 0x040005AA RID: 1450
	private Dictionary<Type, GameService> m_gameServices;

	// Token: 0x040005AC RID: 1452
	private bool m_initialized;

	// Token: 0x040005AD RID: 1453
	private Dictionary<Type, GameStatus> m_gameStatus;
}
