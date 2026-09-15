using System;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class BackPackAudio : MonoBehaviour
{
	// Token: 0x06000F75 RID: 3957 RVA: 0x0004CC5A File Offset: 0x0004AE5A
	private void Start()
	{
		this.item = base.GetComponent<Backpack>();
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x0004CC68 File Offset: 0x0004AE68
	private void Update()
	{
		if (this.item)
		{
			if (this.item.holderCharacter)
			{
				if (!this.hT)
				{
					for (int i = 0; i < this.holdSFX.Length; i++)
					{
						this.holdSFX[i].Play(base.transform.position);
					}
					this.hT = true;
				}
			}
			else
			{
				this.hT = false;
			}
			if (this.item.rig.useGravity)
			{
				if (!this.dT)
				{
					for (int j = 0; j < this.dropSFX.Length; j++)
					{
						this.dropSFX[j].Play(base.transform.position);
					}
				}
				this.dT = true;
				return;
			}
			this.dT = false;
		}
	}

	// Token: 0x04000DC9 RID: 3529
	private Backpack item;

	// Token: 0x04000DCA RID: 3530
	public SFX_Instance[] holdSFX;

	// Token: 0x04000DCB RID: 3531
	private bool hT;

	// Token: 0x04000DCC RID: 3532
	public SFX_Instance[] dropSFX;

	// Token: 0x04000DCD RID: 3533
	private bool dT;
}
