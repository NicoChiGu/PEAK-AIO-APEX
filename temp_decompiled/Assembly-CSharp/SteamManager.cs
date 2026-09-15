using System;
using System.Text;
using AOT;
using Steamworks;
using UnityEngine;

// Token: 0x020001B2 RID: 434
[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000D61 RID: 3425 RVA: 0x000433F2 File Offset: 0x000415F2
	public static SteamManager Instance
	{
		get
		{
			return SteamManager.s_instance;
		}
	}

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000D62 RID: 3426 RVA: 0x000433F9 File Offset: 0x000415F9
	public static bool Initialized
	{
		get
		{
			return SteamManager.s_EverInitialized && SteamManager.Instance.m_bInitialized;
		}
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x0004340E File Offset: 0x0004160E
	[MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x00043416 File Offset: 0x00041616
	[RuntimeInitializeOnLoadMethod(4)]
	private static void InitOnPlayMode()
	{
		SteamManager.s_EverInitialized = false;
		SteamManager.s_instance = null;
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x00043424 File Offset: 0x00041624
	protected virtual void Awake()
	{
		if (SteamManager.s_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SteamManager.s_instance = this;
		if (SteamManager.s_EverInitialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(new AppId_t(3527290U)))
			{
				Debug.Log("[Steamworks.NET] Shutting down because RestartAppIfNecessary returned true. Steam will restart the application.");
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			string text = "[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n";
			DllNotFoundException ex2 = ex;
			Debug.LogError(text + ((ex2 != null) ? ex2.ToString() : null), this);
			Application.Quit();
			return;
		}
		this.m_bInitialized = SteamAPI.Init();
		if (!this.m_bInitialized)
		{
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		SteamManager.s_EverInitialized = true;
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x00043510 File Offset: 0x00041710
	protected virtual void OnEnable()
	{
		if (SteamManager.s_instance == null)
		{
			SteamManager.s_instance = this;
		}
		if (!this.m_bInitialized)
		{
			return;
		}
		if (this.m_SteamAPIWarningMessageHook == null)
		{
			this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
		}
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x0004355E File Offset: 0x0004175E
	protected virtual void OnDestroy()
	{
		if (SteamManager.s_instance != this)
		{
			return;
		}
		SteamManager.s_instance = null;
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.Shutdown();
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00043582 File Offset: 0x00041782
	protected virtual void Update()
	{
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.RunCallbacks();
	}

	// Token: 0x04000B92 RID: 2962
	protected static bool s_EverInitialized;

	// Token: 0x04000B93 RID: 2963
	protected static SteamManager s_instance;

	// Token: 0x04000B94 RID: 2964
	protected bool m_bInitialized;

	// Token: 0x04000B95 RID: 2965
	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
}
