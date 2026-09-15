using System;
using UnityEngine;

// Token: 0x0200030B RID: 779
public class RandomAnimatorSpeed : MonoBehaviour
{
	// Token: 0x0600142F RID: 5167 RVA: 0x000664AF File Offset: 0x000646AF
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.anim.speed = Random.Range(this.minSpeed, this.maxSpeed);
	}

	// Token: 0x040012B1 RID: 4785
	private Animator anim;

	// Token: 0x040012B2 RID: 4786
	public float minSpeed = 0.5f;

	// Token: 0x040012B3 RID: 4787
	public float maxSpeed = 2f;
}
