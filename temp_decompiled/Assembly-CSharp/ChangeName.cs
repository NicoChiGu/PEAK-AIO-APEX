using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002BE RID: 702
[RequireComponent(typeof(PhotonView))]
public class ChangeName : MonoBehaviour
{
	// Token: 0x0600131A RID: 4890 RVA: 0x0006100C File Offset: 0x0005F20C
	private void Start()
	{
		PhotonView component = base.GetComponent<PhotonView>();
		base.name = string.Format("ActorNumber {0}", component.OwnerActorNr);
	}
}
