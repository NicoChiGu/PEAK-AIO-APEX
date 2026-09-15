using System;
using UnityEngine;

// Token: 0x02000282 RID: 642
public class LandImpactAudio : MonoBehaviour
{
	// Token: 0x060011E9 RID: 4585 RVA: 0x0005A88C File Offset: 0x00058A8C
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x0005A8A4 File Offset: 0x00058AA4
	private void Update()
	{
		this.yVel = base.transform.position.y - this.prevY;
		this.prevY = base.transform.position.y;
		if (this.yVel < -0.025f)
		{
			this.storeYVel = this.yVel;
		}
		if (this.yVel > 0.025f)
		{
			this.storeYVel = 0f;
		}
		this.impactVelocity = this.storeYVel;
		if (!this.t && this.character.data.isGrounded)
		{
			if (this.impactVelocity < -0.3f && !this.t)
			{
				if (this.impactGiant)
				{
					Object.Instantiate<GameObject>(this.impactGiant, base.transform.position, base.transform.rotation);
				}
				this.t = true;
			}
			if (this.impactVelocity < -0.2f && !this.t)
			{
				this.impactHeavy.SetActive(true);
				this.t = true;
			}
			if (this.impactVelocity < -0.1f && !this.t)
			{
				this.impactMedium.SetActive(true);
				this.t = true;
			}
			if (this.impactVelocity < -0.05f && !this.t)
			{
				this.impactSmall.SetActive(true);
				this.t = true;
			}
			this.storeYVel = 0f;
		}
		if (this.character.data.isGrounded)
		{
			this.storeYVel = 0f;
		}
		if (!this.character.data.isGrounded)
		{
			this.t = false;
			this.impactHeavy.SetActive(false);
			this.impactMedium.SetActive(false);
			this.impactSmall.SetActive(false);
		}
	}

	// Token: 0x04001064 RID: 4196
	private Character character;

	// Token: 0x04001065 RID: 4197
	public float impactVelocity;

	// Token: 0x04001066 RID: 4198
	private float yVel;

	// Token: 0x04001067 RID: 4199
	private float storeYVel;

	// Token: 0x04001068 RID: 4200
	private float prevY;

	// Token: 0x04001069 RID: 4201
	private bool t;

	// Token: 0x0400106A RID: 4202
	public GameObject impactSmall;

	// Token: 0x0400106B RID: 4203
	public GameObject impactMedium;

	// Token: 0x0400106C RID: 4204
	public GameObject impactHeavy;

	// Token: 0x0400106D RID: 4205
	public GameObject impactGiant;
}
