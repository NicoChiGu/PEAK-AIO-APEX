using System;
using UnityEngine;

// Token: 0x02000279 RID: 633
public class ItemMoveZone : MonoBehaviour
{
	// Token: 0x060011AE RID: 4526 RVA: 0x00059348 File Offset: 0x00057548
	private void OnTriggerStay(Collider other)
	{
		if (other.attachedRigidbody.GetComponent<Item>() != null)
		{
			other.attachedRigidbody.MovePosition(other.attachedRigidbody.position + base.transform.forward * this.forceMultiplier * Time.fixedDeltaTime);
		}
	}

	// Token: 0x04001032 RID: 4146
	public float forceMultiplier = 1f;
}
