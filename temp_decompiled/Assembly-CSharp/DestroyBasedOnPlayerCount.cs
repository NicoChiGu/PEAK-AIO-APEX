using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000247 RID: 583
public class DestroyBasedOnPlayerCount : MonoBehaviourPun
{
	// Token: 0x06001101 RID: 4353 RVA: 0x00055DDF File Offset: 0x00053FDF
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			yield break;
		}
		if (PhotonNetwork.PlayerList.Length < this.destroyIfPlayerCountIsLessThan)
		{
			Debug.Log(string.Format("Item was told to destroy if player count <{0} and it is {1}", this.destroyIfPlayerCountIsLessThan, PhotonNetwork.PlayerList.Length));
			PhotonNetwork.Destroy(base.photonView);
		}
		yield break;
	}

	// Token: 0x04000F75 RID: 3957
	public int destroyIfPlayerCountIsLessThan;
}
