using System;
using UnityEngine;

// Token: 0x02000371 RID: 881
public class WaterfallPusher : MonoBehaviour
{
	// Token: 0x06001661 RID: 5729 RVA: 0x00073EB8 File Offset: 0x000720B8
	private void OnTriggerEnter(Collider other)
	{
		Character componentInParent = other.GetComponentInParent<Character>();
		if (componentInParent && componentInParent.IsLocal && Time.time > this.cooldown + this.cooldownTimer)
		{
			if (this.fallTime > 0f)
			{
				componentInParent.Fall(this.fallTime, 0f);
			}
			this.cooldownTimer = Time.time;
			this.sfx.Play();
			GamefeelHandler.instance.AddPerlinShake(30f, 0.8f, 20f);
			Vector3 normalized = (componentInParent.Center - base.transform.position).normalized;
			normalized.y = 0f;
			Vector3 vector = normalized * this.knockback;
			Vector3 vector2 = Vector3.down * this.downwardKnockback;
			componentInParent.AddForce(vector + vector2, 0.7f, 1.3f);
		}
	}

	// Token: 0x04001544 RID: 5444
	public float fallTime = 0.5f;

	// Token: 0x04001545 RID: 5445
	public float knockback = 25f;

	// Token: 0x04001546 RID: 5446
	public float downwardKnockback = 25f;

	// Token: 0x04001547 RID: 5447
	public float cooldown = 1f;

	// Token: 0x04001548 RID: 5448
	private float cooldownTimer;

	// Token: 0x04001549 RID: 5449
	public SFX_PlayOneShot sfx;
}
