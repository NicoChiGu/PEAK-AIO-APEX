using System;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class SpiderTrigger : MonoBehaviour
{
	// Token: 0x0600155F RID: 5471 RVA: 0x0006D54C File Offset: 0x0006B74C
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		Character componentInParent = other.attachedRigidbody.GetComponentInParent<Character>();
		if (componentInParent && componentInParent.IsLocal && !componentInParent.refs.afflictions.isWebbed)
		{
			this.spider.GrabCharacter(componentInParent.photonView);
		}
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x0006D5A7 File Offset: 0x0006B7A7
	public void Bonk()
	{
		this.spider.Bonk();
	}

	// Token: 0x04001408 RID: 5128
	public Spider spider;
}
