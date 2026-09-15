using System;
using UnityEngine;

// Token: 0x0200032A RID: 810
public class SFXOnChildCount : MonoBehaviour
{
	// Token: 0x060014CA RID: 5322 RVA: 0x00069B02 File Offset: 0x00067D02
	private void Start()
	{
		this.index = base.transform.childCount;
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x00069B18 File Offset: 0x00067D18
	private void Update()
	{
		if (this.index != base.transform.childCount)
		{
			for (int i = 0; i < this.sfx.Length; i++)
			{
				this.sfx[i].Play(default(Vector3));
			}
		}
		this.index = base.transform.childCount;
	}

	// Token: 0x04001340 RID: 4928
	public SFX_Instance[] sfx;

	// Token: 0x04001341 RID: 4929
	private int index;
}
