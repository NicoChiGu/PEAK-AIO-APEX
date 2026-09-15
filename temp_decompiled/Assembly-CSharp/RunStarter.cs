using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class RunStarter : MonoBehaviour
{
	// Token: 0x06000C0A RID: 3082 RVA: 0x0004053A File Offset: 0x0003E73A
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom || !Character.localCharacter || LoadingScreenHandler.loading)
		{
			yield return null;
		}
		Debug.Log("RUN STARTED");
		this.StartRun();
		yield break;
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x00040549 File Offset: 0x0003E749
	private void StartRun()
	{
		RunManager.Instance.StartRun();
	}
}
