using System;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class VoiceClientHandler : MonoBehaviour
{
	// Token: 0x06000F12 RID: 3858 RVA: 0x00049C7C File Offset: 0x00047E7C
	private void Awake()
	{
		PunVoiceClient component = base.GetComponent<PunVoiceClient>();
		if (component == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (PunVoiceClient.Instance != component)
		{
			Debug.Log("Already Found VoiceClient, Destroying...");
			Object.Destroy(base.gameObject);
			return;
		}
		base.transform.SetParent(null);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x00049CE0 File Offset: 0x00047EE0
	private void Start()
	{
		VoiceClientHandler.m_VoiceConnection = base.GetComponent<VoiceConnection>();
		if (VoiceClientHandler.m_VoiceConnection.Client.State != 9)
		{
			VoiceClientHandler.m_VoiceConnection.Client.StateChanged += new Action<ClientState, ClientState>(this.OnStateChanged);
			return;
		}
		VoiceClientHandler.InitNetworkVoice();
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x00049D2C File Offset: 0x00047F2C
	private void OnStateChanged(ClientState state, ClientState toState)
	{
		if (toState == 9)
		{
			VoiceClientHandler.InitNetworkVoice();
		}
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x00049D38 File Offset: 0x00047F38
	public static void InitNetworkVoice()
	{
		if (VoiceClientHandler.m_LocalRecorder == null || VoiceClientHandler.m_VoiceConnection == null || VoiceClientHandler.m_VoiceConnection.Client.State != 9)
		{
			return;
		}
		VoiceClientHandler.m_VoiceConnection.Client.LoadBalancingPeer.OpChangeGroups(Array.Empty<byte>(), Array.Empty<byte>());
		VoiceClientHandler.m_LocalRecorder.InterestGroup = 0;
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x00049D9D File Offset: 0x00047F9D
	public static void LocalPlayerAssigned(Recorder r)
	{
		VoiceClientHandler.m_LocalRecorder = r;
		VoiceClientHandler.InitNetworkVoice();
	}

	// Token: 0x04000D15 RID: 3349
	private static VoiceConnection m_VoiceConnection;

	// Token: 0x04000D16 RID: 3350
	private static Recorder m_LocalRecorder;
}
