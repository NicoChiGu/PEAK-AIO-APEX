using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000138 RID: 312
[ConsoleClassCustomizer("MapHandler")]
public class MapDebugUI : MonoBehaviour
{
	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060009E7 RID: 2535 RVA: 0x000348F1 File Offset: 0x00032AF1
	private static bool UiIsVisible
	{
		get
		{
			return GUIManager.instance == null || GUIManager.instance.hudCanvas.isActiveAndEnabled;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060009E8 RID: 2536 RVA: 0x00034911 File Offset: 0x00032B11
	private static bool CanvasIsVisible
	{
		get
		{
			return MapDebugUI.canvasEnabled && MapDebugUI.UiIsVisible;
		}
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x00034924 File Offset: 0x00032B24
	private void Start()
	{
		if (!Application.isEditor && !Debug.isDebugBuild)
		{
			Object.Destroy(this.canvasToDestroy);
			return;
		}
		this._sb = new StringBuilder();
		SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoad);
		base.StartCoroutine(this.StartInitialization());
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00034974 File Offset: 0x00032B74
	private void OnSceneLoad(Scene _, LoadSceneMode __)
	{
		this._ambience = Object.FindFirstObjectByType<AmbienceAudio>();
		this._guiManager = Object.FindFirstObjectByType<GUIManager>().GetComponent<AudioSource>();
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00034991 File Offset: 0x00032B91
	[ConsoleCommand]
	public static void IncrementLevel()
	{
		NextLevelService.debugLevelIndexOffset++;
		MapDebugUI.forceInitialization = true;
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x000349A5 File Offset: 0x00032BA5
	[ConsoleCommand]
	public static void ToggleDebugText()
	{
		MapDebugUI.canvasEnabled = !MapDebugUI.canvasEnabled;
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x000349B4 File Offset: 0x00032BB4
	private IEnumerator StartInitialization()
	{
		this.initialized = false;
		MapDebugUI.forceInitialization = false;
		this.audioInitialized = true;
		int loopstop = 10;
		NextLevelService service;
		for (;;)
		{
			int num = loopstop;
			loopstop = num - 1;
			if (num <= 0)
			{
				goto IL_00D9;
			}
			this._ambience = Object.FindFirstObjectByType<AmbienceAudio>();
			this._guiManager = Object.FindFirstObjectByType<GUIManager>().GetComponent<AudioSource>();
			service = GameHandler.GetService<NextLevelService>();
			if (service.Data.IsSome)
			{
				break;
			}
			yield return new WaitForSeconds(1f);
		}
		this.scene = SingletonAsset<MapBaker>.Instance.GetLevel(service.Data.Value.CurrentLevelIndex + NextLevelService.debugLevelIndexOffset);
		Debug.Log("Initialized.");
		IL_00D9:
		this.initialized = true;
		yield break;
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x000349C4 File Offset: 0x00032BC4
	private void Update()
	{
		if (MapDebugUI.forceInitialization)
		{
			base.StartCoroutine(this.StartInitialization());
			return;
		}
		if (!this.initialized)
		{
			return;
		}
		if (!this._foundAmbience && MapHandler.Exists)
		{
			this._ambience = Object.FindFirstObjectByType<AmbienceAudio>();
			this._foundAmbience = this._ambience;
		}
		this.text.enabled = MapDebugUI.CanvasIsVisible;
		if (Character.localCharacter && MainCamera.instance)
		{
			this.position = MainCamera.instance.transform.position.ToString();
		}
		else
		{
			this.position = "";
		}
		BuildVersion buildVersion;
		buildVersion..ctor(Application.version, "???");
		string text = buildVersion.ToString();
		string text2 = this.scene;
		this._sb.Clear();
		this._sb.AppendLine(string.Concat(new string[] { text, "\nMap: ", text2, "\nCameraPos: ", this.position }));
		if (this._ambience && this._ambience.isActiveAndEnabled)
		{
			this._sb.AppendLine("MainMusic: " + MapDebugUI.<Update>g__GetAudioString|21_0(this._ambience.mainMusic));
			this._sb.AppendLine("Stinger: " + MapDebugUI.<Update>g__GetAudioString|21_0(this._ambience.stingerSource));
			this._sb.AppendLine("GUIManager: " + MapDebugUI.<Update>g__GetAudioString|21_0(this._guiManager));
		}
		this.text.text = this._sb.ToString();
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00034B8C File Offset: 0x00032D8C
	[CompilerGenerated]
	internal static string <Update>g__GetAudioString|21_0(AudioSource source)
	{
		if (!source.clip)
		{
			return "<color=#d3d3d388>null</color>";
		}
		int num = Mathf.RoundToInt(source.volume * 100f);
		if (source.isPlaying && !source.isVirtual && source.volume > 0.1f)
		{
			return string.Format("<color={0}>{1} [{2}%]</color>", "#ffffff", source.clip.name, num);
		}
		return string.Format("<color={0}>{1} [{2}%]</color>", "#d3d3d388", source.clip.name, num);
	}

	// Token: 0x04000956 RID: 2390
	private bool initialized;

	// Token: 0x04000957 RID: 2391
	private bool audioInitialized;

	// Token: 0x04000958 RID: 2392
	private string scene;

	// Token: 0x04000959 RID: 2393
	public TextMeshProUGUI text;

	// Token: 0x0400095A RID: 2394
	public GameObject canvasToDestroy;

	// Token: 0x0400095B RID: 2395
	private bool _foundAmbience;

	// Token: 0x0400095C RID: 2396
	private AmbienceAudio _ambience;

	// Token: 0x0400095D RID: 2397
	private AudioSource _guiManager;

	// Token: 0x0400095E RID: 2398
	private StringBuilder _sb;

	// Token: 0x0400095F RID: 2399
	public static bool canvasEnabled = true;

	// Token: 0x04000960 RID: 2400
	public static bool forceInitialization;

	// Token: 0x04000961 RID: 2401
	private string position;
}
