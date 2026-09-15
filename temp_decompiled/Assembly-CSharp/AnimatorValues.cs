using System;
using UnityEngine;

// Token: 0x02000200 RID: 512
public class AnimatorValues : MonoBehaviour
{
	// Token: 0x06000F4C RID: 3916 RVA: 0x0004B7E4 File Offset: 0x000499E4
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.cD = base.GetComponentInParent<CharacterData>();
		this.cI = base.GetComponentInParent<CharacterInput>();
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x0004B80C File Offset: 0x00049A0C
	private void Update()
	{
		this.anim.SetFloat("Input X", this.cI.movementInput.x);
		this.anim.SetFloat("Input Y", this.cI.movementInput.y);
		this.anim.SetBool("Is Grounded", this.cD.isGrounded);
		if (this.cI.sprintIsPressed)
		{
			this.anim.SetFloat("Sprint", 1f, 0.125f, Time.deltaTime);
		}
		if (!this.cI.sprintIsPressed)
		{
			this.anim.SetFloat("Sprint", 0f, 0.125f, Time.deltaTime);
		}
		this.anim.SetFloat("Velocity Y", this.cD.avarageVelocity.y);
	}

	// Token: 0x04000D8F RID: 3471
	private Animator anim;

	// Token: 0x04000D90 RID: 3472
	private CharacterData cD;

	// Token: 0x04000D91 RID: 3473
	private CharacterInput cI;
}
