using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200034D RID: 845
public class TickTrigger : MonoBehaviour
{
	// Token: 0x060015B6 RID: 5558 RVA: 0x00070138 File Offset: 0x0006E338
	private void Start()
	{
		if (Random.value > this.tickChance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x00070154 File Offset: 0x0006E354
	private void OnTriggerEnter(Collider other)
	{
		Character componentInParent = other.GetComponentInParent<Character>();
		if (componentInParent && componentInParent.IsLocal)
		{
			foreach (KeyValuePair<Bugfix, Character> keyValuePair in Bugfix.AllAttachedBugs)
			{
				if (keyValuePair.Value == componentInParent)
				{
					return;
				}
			}
			PhotonNetwork.Instantiate("BugfixOnYou", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PhotonView>().RPC("AttachBug", 0, new object[] { componentInParent.photonView.ViewID });
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0400148E RID: 5262
	public float tickChance = 0.01f;
}
