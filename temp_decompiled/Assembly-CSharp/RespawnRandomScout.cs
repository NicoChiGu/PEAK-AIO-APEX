using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000128 RID: 296
public class RespawnRandomScout : MonoBehaviour
{
	// Token: 0x06000969 RID: 2409 RVA: 0x00031EBC File Offset: 0x000300BC
	private void Start()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			List<Character> list = new List<Character>();
			foreach (Character character in Character.AllCharacters)
			{
				if (character.data.dead || character.data.fullyPassedOut)
				{
					list.Add(character);
				}
			}
			list.RandomSelection((Character c) => 1).photonView.RPC("RPCA_ReviveAtPosition", 0, new object[]
			{
				base.transform.position,
				false,
				-1
			});
		}
		Object.Destroy(base.gameObject);
	}
}
