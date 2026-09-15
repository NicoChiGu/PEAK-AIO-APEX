using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class SingleItemSpawner : MonoBehaviour, ISpawner
{
	// Token: 0x06000D47 RID: 3399 RVA: 0x00042AE0 File Offset: 0x00040CE0
	public List<PhotonView> TrySpawnItems()
	{
		if (this.playersInRoomRequirement > PhotonNetwork.PlayerList.Length)
		{
			Debug.LogError(string.Format("Not spawning: {0} because of players in room req: {1}", this.prefab, this.playersInRoomRequirement));
			return new List<PhotonView>();
		}
		if (this.belowAscentRequirement != -1 && Ascents.currentAscent >= this.belowAscentRequirement)
		{
			Debug.LogError(string.Format("Not spawning: {0} because ascent is too high: {1}", this.prefab, Ascents.currentAscent));
			return new List<PhotonView>();
		}
		PhotonView component = PhotonNetwork.InstantiateItemRoom(this.prefab.name, base.transform.position + Vector3.up * 0.1f, base.transform.rotation).GetComponent<PhotonView>();
		if (this.isKinematic)
		{
			component.GetComponent<PhotonView>().RPC("SetKinematicRPC", 3, new object[]
			{
				true,
				component.transform.position,
				component.transform.rotation
			});
		}
		List<PhotonView> list = new List<PhotonView>();
		list.Add(component);
		return list;
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00042BF7 File Offset: 0x00040DF7
	private void OnDrawGizmos()
	{
		if (this.prefab != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(base.transform.position, 0.25f);
		}
	}

	// Token: 0x04000B7D RID: 2941
	public bool isKinematic;

	// Token: 0x04000B7E RID: 2942
	public GameObject prefab;

	// Token: 0x04000B7F RID: 2943
	public int playersInRoomRequirement;

	// Token: 0x04000B80 RID: 2944
	public int belowAscentRequirement = -1;
}
