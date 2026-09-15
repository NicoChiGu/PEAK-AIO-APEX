using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;

// Token: 0x02000134 RID: 308
public class LoadingScreenHandler : RetrievableResourceSingleton<LoadingScreenHandler>
{
	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060009AA RID: 2474 RVA: 0x00033B8C File Offset: 0x00031D8C
	// (set) Token: 0x060009AB RID: 2475 RVA: 0x00033B93 File Offset: 0x00031D93
	public static bool loading
	{
		get
		{
			return LoadingScreenHandler._loading;
		}
		private set
		{
			if (!value && RetrievableResourceSingleton<LoadingScreenHandler>.Instance._lastActiveLoadingScreen != null)
			{
				Debug.Log("Stopped loading without first destroying loading screen");
			}
			LoadingScreenHandler._loading = value;
		}
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00033BBA File Offset: 0x00031DBA
	public static void KillCurrentLoadingScreen()
	{
		if (RetrievableResourceSingleton<LoadingScreenHandler>.Instance._lastActiveLoadingScreen)
		{
			Object.Destroy(RetrievableResourceSingleton<LoadingScreenHandler>.Instance._lastActiveLoadingScreen.gameObject);
		}
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00033BE1 File Offset: 0x00031DE1
	private void Awake()
	{
		Dictionary<LoadingScreen.LoadingScreenType, LoadingScreen> dictionary = new Dictionary<LoadingScreen.LoadingScreenType, LoadingScreen>();
		dictionary.Add(LoadingScreen.LoadingScreenType.Basic, this.loadingScreenPrefabBasic);
		dictionary.Add(LoadingScreen.LoadingScreenType.Plane, this.loadingScreenPrefabPlane);
		this.loadingScreens = dictionary;
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00033C0E File Offset: 0x00031E0E
	public LoadingScreen GetLoadingScreenPrefab(LoadingScreen.LoadingScreenType type)
	{
		return this.loadingScreens[type];
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x00033C1C File Offset: 0x00031E1C
	public void Load(LoadingScreen.LoadingScreenType type, Action runAfter, params IEnumerator[] processes)
	{
		GameHandler.ClearStatus<EndScreenStatus>();
		if (!LoadingScreenHandler.loading)
		{
			base.StartCoroutine(this.LoadingRoutine(type, runAfter, processes));
			return;
		}
		Debug.LogError("Tried to load while already loading! If this happens a lot it's likely an issue!");
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00033C45 File Offset: 0x00031E45
	private IEnumerator LoadingRoutine(LoadingScreen.LoadingScreenType type, Action runAfter, params IEnumerator[] processes)
	{
		PhotonNetwork.IsMessageQueueRunning = false;
		LoadingScreenHandler.loading = true;
		LoadingScreen loadingScreenPrefab = this.GetLoadingScreenPrefab(type);
		this._lastActiveLoadingScreen = Object.Instantiate<LoadingScreen>(loadingScreenPrefab, Vector3.zero, Quaternion.identity);
		yield return base.StartCoroutine(this._lastActiveLoadingScreen.LoadingRoutine(runAfter, processes));
		LoadingScreenHandler.loading = false;
		PhotonNetwork.IsMessageQueueRunning = true;
		yield break;
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00033C69 File Offset: 0x00031E69
	public IEnumerator LoadSceneProcess(string sceneName, bool networked, bool yieldForCharacterSpawn = false, float extraYieldTimeOnEnd = 3f)
	{
		if (networked)
		{
			yield return this.LoadSceneProcessNetworked(sceneName, yieldForCharacterSpawn, extraYieldTimeOnEnd);
		}
		else
		{
			yield return this.LoadSceneProcessOffline(sceneName, yieldForCharacterSpawn, extraYieldTimeOnEnd);
		}
		yield break;
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00033C95 File Offset: 0x00031E95
	private IEnumerator LoadSceneProcessNetworked(string sceneName, bool yieldForCharacterSpawn, float extraYieldTimeOnEnd)
	{
		PhotonNetwork.LoadLevel(sceneName);
		float timeout = 5f;
		while ((timeout > 0f && PhotonNetwork.LevelLoadingProgress == 0f) || PhotonNetwork.LevelLoadingProgress >= 1f)
		{
			timeout -= Time.unscaledDeltaTime;
			yield return null;
		}
		Debug.Log(string.Format("Waited {0} for level to start loading. Progress: {1}", 5f - timeout, PhotonNetwork.LevelLoadingProgress));
		if (DayNightManager.instance != null)
		{
			DayNightManager.instance.specialDaySunBlend = 0f;
			DayNightManager.instance.specialDaySkyBlend = 0f;
		}
		float tic = Time.realtimeSinceStartup;
		while (PhotonNetwork.LevelLoadingProgress < 1f)
		{
			yield return null;
		}
		Debug.Log(string.Format("Level took {0} seconds to start loading.", Time.realtimeSinceStartup - tic));
		tic = Time.realtimeSinceStartup;
		while (PhotonNetwork.NetworkClientState == 6)
		{
			yield return null;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (Mathf.Approximately(0f, realtimeSinceStartup - tic))
		{
			Debug.Log("We were connected to game server before level finished loading.");
		}
		Debug.Log(string.Format("Needed to wait {0} additional seconds for Photon to connect to game server.", realtimeSinceStartup - tic));
		if (yieldForCharacterSpawn)
		{
			yield return this.WaitForCharacterSpawn(120f);
		}
		yield return new WaitForSecondsRealtime(extraYieldTimeOnEnd);
		yield break;
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00033CB9 File Offset: 0x00031EB9
	private IEnumerator LoadSceneProcessOffline(string sceneName, bool yieldForCharacterSpawn, float extraYieldTimeOnEnd)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
		if (!operation.isDone)
		{
			Debug.Log("Waiting for scene loading...");
		}
		while (!operation.isDone)
		{
			yield return null;
		}
		if (PhotonNetwork.NetworkClientState == 6)
		{
			Debug.Log("Waiting while connecting...");
		}
		else
		{
			Debug.Log("Scene load complete and no need to wait for Photon connect.");
		}
		while (PhotonNetwork.NetworkClientState == 6)
		{
			yield return null;
		}
		if (yieldForCharacterSpawn)
		{
			yield return this.WaitForCharacterSpawn(120f);
		}
		yield return new WaitForSecondsRealtime(extraYieldTimeOnEnd);
		yield break;
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x00033CDD File Offset: 0x00031EDD
	private IEnumerator WaitForCharacterSpawn(float timeout = 120f)
	{
		if (!PhotonNetwork.IsMessageQueueRunning)
		{
			Debug.LogWarning("OOPS! Message queue was disabled. We'll need that to receive our spawn request");
			PhotonNetwork.IsMessageQueueRunning = true;
		}
		Debug.Log("Level loaded and Photon connected! Just waiting for Character to spawn...");
		float tic = Time.realtimeSinceStartup;
		while (!Character.localCharacter || !PhotonNetwork.InRoom)
		{
			if (Time.realtimeSinceStartup - tic > timeout)
			{
				Debug.LogError(string.Format("Waited to spawn for {0} seconds and it didn't happen. Giving up", timeout));
				yield break;
			}
			yield return null;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		Debug.Log(string.Format("It took {0} seconds for player's local character to spawn.", realtimeSinceStartup - tic));
		yield break;
	}

	// Token: 0x04000925 RID: 2341
	private LoadingScreen _lastActiveLoadingScreen;

	// Token: 0x04000926 RID: 2342
	public LoadingScreen loadingScreenPrefabBasic;

	// Token: 0x04000927 RID: 2343
	public LoadingScreen loadingScreenPrefabPlane;

	// Token: 0x04000928 RID: 2344
	private static bool _loading;

	// Token: 0x04000929 RID: 2345
	private Dictionary<LoadingScreen.LoadingScreenType, LoadingScreen> loadingScreens;
}
