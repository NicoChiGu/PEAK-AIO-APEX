using System;
using UnityEngine;

// Token: 0x0200027B RID: 635
public class ItemVFX : MonoBehaviour
{
	// Token: 0x060011B1 RID: 4529 RVA: 0x000593BE File Offset: 0x000575BE
	protected virtual void Start()
	{
		this.item = base.GetComponent<Item>();
		if (this.item.holderCharacter == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x000593E6 File Offset: 0x000575E6
	protected virtual void Update()
	{
		this.Shake();
		this.shakeSFX.volume = this.item.castProgress / 2f;
		this.shakeSFX.pitch = 1f + this.item.castProgress;
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x00059428 File Offset: 0x00057628
	protected virtual void Shake()
	{
		if (!this.item.finishedCast)
		{
			GamefeelHandler.instance.AddPerlinShake(this.item.castProgress * this.shakeAmount * Time.deltaTime * 60f, 0.2f, 15f);
		}
		if (this.item.finishedCast)
		{
			for (int i = 0; i < this.doneSFX.Length; i++)
			{
				this.doneSFX[i].Play(base.transform.position);
			}
		}
		this.castProgress = this.item.castProgress;
	}

	// Token: 0x04001036 RID: 4150
	protected Item item;

	// Token: 0x04001037 RID: 4151
	public bool shake;

	// Token: 0x04001038 RID: 4152
	public float shakeAmount = 1f;

	// Token: 0x04001039 RID: 4153
	public float castProgress;

	// Token: 0x0400103A RID: 4154
	public AudioSource shakeSFX;

	// Token: 0x0400103B RID: 4155
	public SFX_Instance[] doneSFX;
}
