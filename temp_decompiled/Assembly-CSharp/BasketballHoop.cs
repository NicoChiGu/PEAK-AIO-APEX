using System;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class BasketballHoop : MonoBehaviour
{
	// Token: 0x06000F8E RID: 3982 RVA: 0x0004D414 File Offset: 0x0004B614
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			Item component = other.attachedRigidbody.GetComponent<Item>();
			if (component != null && component.transform.position.y > base.transform.position.y)
			{
				this.ballRb = other.attachedRigidbody;
			}
		}
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x0004D474 File Offset: 0x0004B674
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody != null && other.attachedRigidbody == this.ballRb && other.attachedRigidbody.linearVelocity.y < 0f && this.ballRb.transform.position.y < base.transform.position.y && other.attachedRigidbody.GetComponent<Item>() != null && Time.time > this.lastScoredTime + 2f)
		{
			this.ballRb = null;
			this.confetti.Play();
			this.success.Play();
			this.anim.SetTrigger("Score");
			this.lastScoredTime = Time.time;
		}
	}

	// Token: 0x04000DE9 RID: 3561
	public Animator anim;

	// Token: 0x04000DEA RID: 3562
	public ParticleSystem confetti;

	// Token: 0x04000DEB RID: 3563
	public SFX_PlayOneShot success;

	// Token: 0x04000DEC RID: 3564
	private float lastScoredTime;

	// Token: 0x04000DED RID: 3565
	private Rigidbody ballRb;
}
