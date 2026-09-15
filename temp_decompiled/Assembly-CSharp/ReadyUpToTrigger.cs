using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class ReadyUpToTrigger : MonoBehaviourPunCallbacks
{
	// Token: 0x06000B3C RID: 2876 RVA: 0x0003BEFD File Offset: 0x0003A0FD
	public override void OnJoinedRoom()
	{
		this.readyUpStatusDict.Clear();
		this.PopulatePlayerDict();
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x0003BF10 File Offset: 0x0003A110
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.PopulatePlayerDict();
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0003BF18 File Offset: 0x0003A118
	public override void OnPlayerLeftRoom(Player leavingPlayer)
	{
		this.readyUpStatusDict.Remove(leavingPlayer);
		Debug.Log("Removing player from ready-up list: " + leavingPlayer.NickName);
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x0003BF3C File Offset: 0x0003A13C
	private void PopulatePlayerDict()
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			if (!this.readyUpStatusDict.ContainsKey(player))
			{
				this.readyUpStatusDict.Add(player, false);
				Debug.Log("Adding player to ready-up list: " + player.NickName);
			}
		}
	}

	// Token: 0x04000A7B RID: 2683
	public Dictionary<Player, bool> readyUpStatusDict = new Dictionary<Player, bool>();
}
