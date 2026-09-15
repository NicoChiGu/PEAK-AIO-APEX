using System;
using UnityEngine;
using UnityEngine.Networking;
using Zorro.Core;

// Token: 0x02000074 RID: 116
public static class CloudAPI
{
	// Token: 0x0600053D RID: 1341 RVA: 0x0001F010 File Offset: 0x0001D210
	public static void CheckVersion(Action<LoginResponse> response)
	{
		GameHandler.AddStatus<QueryingGameTimeStatus>(new QueryingGameTimeStatus());
		BuildVersion buildVersion;
		buildVersion..ctor(Application.version, "???");
		string text = "https://peaklogin3.azurewebsites.net/api/VersionCheck?version=" + buildVersion.ToMatchmaking();
		if (buildVersion.BuildName == "beta")
		{
			text = "https://peaklogin-beta.azurewebsites.net/api/VersionCheck?version=" + buildVersion.ToMatchmaking();
		}
		Debug.Log("Sending GET Request to: " + text);
		UnityWebRequest request = UnityWebRequest.Get(text);
		request.SendWebRequest().completed += delegate(AsyncOperation _)
		{
			GameHandler.ClearStatus<QueryingGameTimeStatus>();
			if (request.result != 1)
			{
				Debug.Log("Got error: " + request.error);
				if (request.result != 2)
				{
					Action<LoginResponse> response2 = response;
					if (response2 == null)
					{
						return;
					}
					response2.Invoke(new LoginResponse
					{
						VersionOkay = false
					});
				}
				return;
			}
			string text2 = request.downloadHandler.text;
			LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(text2);
			Debug.Log("Got message: " + text2);
			Action<LoginResponse> response3 = response;
			if (response3 == null)
			{
				return;
			}
			response3.Invoke(loginResponse);
		};
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0001F0B4 File Offset: 0x0001D2B4
	public static void VerifyLobby(ulong lobbyID, Action<string> response)
	{
		string text = "https://peaklogin.azurewebsites.net/api/VerifyLobby?lobby=" + lobbyID.ToString();
		Debug.Log("Sending GET Request to: " + text);
		UnityWebRequest request = UnityWebRequest.Get(text);
		request.SendWebRequest().completed += delegate(AsyncOperation _)
		{
			GameHandler.ClearStatus<QueryingGameTimeStatus>();
			if (request.result != 1)
			{
				Debug.Log("Failed to verify lobby: " + request.error);
				return;
			}
			string text2 = request.downloadHandler.text;
			Debug.Log("Got message: " + text2);
			Action<string> response2 = response;
			if (response2 == null)
			{
				return;
			}
			response2.Invoke(text2);
		};
	}
}
