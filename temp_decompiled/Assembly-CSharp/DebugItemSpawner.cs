using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class DebugItemSpawner : MonoBehaviour
{
	// Token: 0x060008DB RID: 2267 RVA: 0x0002FC77 File Offset: 0x0002DE77
	private IEnumerator Start()
	{
		ISpawner spawner = base.GetComponent<ISpawner>();
		if (spawner == null || !PhotonNetwork.IsMasterClient)
		{
			Object.Destroy(this);
			yield break;
		}
		while (!PhotonNetwork.InRoom || !Character.localCharacter)
		{
			yield return null;
		}
		spawner.TrySpawnItems();
		yield break;
	}
}
