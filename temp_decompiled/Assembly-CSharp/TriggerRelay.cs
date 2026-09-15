using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class TriggerRelay : MonoBehaviour
{
	// Token: 0x060015FF RID: 5631 RVA: 0x0007173A File Offset: 0x0006F93A
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x00071748 File Offset: 0x0006F948
	[PunRPC]
	public void RPCA_Trigger(int childID)
	{
		Transform child = base.transform.GetChild(childID);
		TriggerEvent triggerEvent;
		if (child && child.TryGetComponent<TriggerEvent>(ref triggerEvent))
		{
			triggerEvent.Trigger();
		}
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x0007177A File Offset: 0x0006F97A
	[PunRPC]
	public void RPCA_TriggerWithTarget(int childID, int targetID)
	{
		base.transform.GetChild(childID).GetComponent<SlipperyJellyfish>().Trigger(targetID);
	}

	// Token: 0x040014E0 RID: 5344
	internal PhotonView view;
}
