using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001F9 RID: 505
public class Action_Spawn : ItemAction
{
	// Token: 0x06000F37 RID: 3895 RVA: 0x0004A7A8 File Offset: 0x000489A8
	public override void RunAction()
	{
		if (this.spawnPoint == null)
		{
			this.spawnPoint = base.transform;
		}
		this.item.photonView.RPC("RPCSpawn", 0, new object[]
		{
			this.spawnPoint.transform.position,
			this.spawnPoint.transform.rotation
		});
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x0004A81B File Offset: 0x00048A1B
	[PunRPC]
	public void RPCSpawn(Vector3 position, Quaternion rotation)
	{
		Object.Instantiate<GameObject>(this.objectToSpawn, position, rotation);
	}

	// Token: 0x04000D42 RID: 3394
	public GameObject objectToSpawn;

	// Token: 0x04000D43 RID: 3395
	public Transform spawnPoint;
}
