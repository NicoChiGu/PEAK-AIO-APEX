using System;
using UnityEngine;

// Token: 0x02000278 RID: 632
public class ItemImpactSFX : MonoBehaviour
{
	// Token: 0x060011AA RID: 4522 RVA: 0x000591A5 File Offset: 0x000573A5
	private void Start()
	{
		this.rig = base.GetComponent<Rigidbody>();
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x000591C0 File Offset: 0x000573C0
	private void Update()
	{
		if (this.rig)
		{
			if (this.rig.isKinematic)
			{
				return;
			}
			this.vel = Mathf.Lerp(this.vel, Vector3.SqrMagnitude(this.rig.linearVelocity) * this.velMult, 10f * Time.deltaTime);
		}
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x0005921C File Offset: 0x0005741C
	private void OnCollisionEnter(Collision collision)
	{
		if (this.rig)
		{
			if (this.item)
			{
				if (!this.item.holderCharacter)
				{
					if (this.vel > 4f)
					{
						for (int i = 0; i < this.impact.Length; i++)
						{
							this.impact[i].Play(base.transform.position);
						}
					}
				}
				else if (this.vel > 36f && !this.disallowInHands)
				{
					for (int j = 0; j < this.impact.Length; j++)
					{
						this.impact[j].Play(base.transform.position);
					}
				}
			}
			if (!this.item && !collision.rigidbody && this.vel > 64f)
			{
				for (int k = 0; k < this.impact.Length; k++)
				{
					this.impact[k].Play(base.transform.position);
				}
			}
			this.vel = 0f;
		}
	}

	// Token: 0x0400102C RID: 4140
	public float vel;

	// Token: 0x0400102D RID: 4141
	private Rigidbody rig;

	// Token: 0x0400102E RID: 4142
	private Item item;

	// Token: 0x0400102F RID: 4143
	public float velMult = 1f;

	// Token: 0x04001030 RID: 4144
	public SFX_Instance[] impact;

	// Token: 0x04001031 RID: 4145
	public bool disallowInHands;
}
