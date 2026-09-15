using System;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class ActionManager : MonoBehaviour
{
	// Token: 0x0600041B RID: 1051 RVA: 0x0001A0EC File Offset: 0x000182EC
	private void Start()
	{
		if (base.GetComponent<Animator>())
		{
			this.anim = base.GetComponent<Animator>();
		}
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x0001A108 File Offset: 0x00018308
	private void Update()
	{
		if (this.anim)
		{
			this.anim.SetBool("Jump Cancel", this.jumpCancel);
			this.anim.SetBool("Attack Cancel", this.attackCancel);
			this.anim.SetBool("Continuable", this.continuable);
			this.anim.SetBool("Fall Cancel", this.fallCancel);
			this.anim.SetBool("Dash Cancel", this.dashCancel);
			this.anim.SetBool("Crouch Cancel", this.crouchCancel);
			this.anim.SetBool("Special State", this.specialState);
			if (this.actionTimer <= 0f)
			{
				this.anim.SetBool("Action", false);
			}
			if (this.actionTimer > 0f)
			{
				this.anim.SetBool("Action", true);
			}
			if (this.edgeCaseTimer <= 0f)
			{
				this.anim.SetBool("Edge Case", false);
			}
			if (this.edgeCaseTimer > 0f)
			{
				this.anim.SetBool("Edge Case", true);
			}
		}
		this.actionTimer -= Time.deltaTime;
		this.edgeCaseTimer -= Time.deltaTime;
		if (this.actionTimer <= 0f)
		{
			this.actionTimer = 0f;
		}
		if (this.edgeCaseTimer <= 0f)
		{
			this.edgeCaseTimer = 0f;
		}
	}

	// Token: 0x0400049D RID: 1181
	public float actionTimer;

	// Token: 0x0400049E RID: 1182
	public float edgeCaseTimer;

	// Token: 0x0400049F RID: 1183
	public Animator anim;

	// Token: 0x040004A0 RID: 1184
	public bool fallCancel = true;

	// Token: 0x040004A1 RID: 1185
	public bool jumpCancel = true;

	// Token: 0x040004A2 RID: 1186
	public bool attackCancel = true;

	// Token: 0x040004A3 RID: 1187
	public bool dashCancel = true;

	// Token: 0x040004A4 RID: 1188
	public bool crouchCancel = true;

	// Token: 0x040004A5 RID: 1189
	public bool continuable;

	// Token: 0x040004A6 RID: 1190
	public bool specialState;
}
