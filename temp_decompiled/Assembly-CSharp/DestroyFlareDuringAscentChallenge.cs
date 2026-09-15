using System;
using System.Collections;
using Photon.Pun;

// Token: 0x02000099 RID: 153
public class DestroyFlareDuringAscentChallenge : MonoBehaviourPun
{
	// Token: 0x060005DE RID: 1502 RVA: 0x0002160E File Offset: 0x0001F80E
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		if (!Ascents.shouldSpawnFlare)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		yield break;
	}
}
