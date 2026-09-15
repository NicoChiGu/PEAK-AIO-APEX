using System;
using UnityEngine;

// Token: 0x02000342 RID: 834
public class StepSoundCollection : MonoBehaviour
{
	// Token: 0x06001571 RID: 5489 RVA: 0x0006DCE0 File Offset: 0x0006BEE0
	public void PlayStep(Vector3 pos, int index, AudioSource sourceOverride = null)
	{
		if (index == 0)
		{
			for (int i = 0; i < this.stepDefault.Length; i++)
			{
				if (sourceOverride)
				{
					this.stepDefault[i].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.stepDefault[i].Play(pos);
				}
			}
		}
		if (index == 1)
		{
			for (int j = 0; j < this.beachSand.Length; j++)
			{
				if (sourceOverride)
				{
					this.beachSand[j].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.beachSand[j].Play(pos);
				}
			}
		}
		if (index == 2)
		{
			for (int k = 0; k < this.beachRock.Length; k++)
			{
				if (sourceOverride)
				{
					this.beachRock[k].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.beachRock[k].Play(pos);
				}
			}
		}
		if (index == 3)
		{
			for (int l = 0; l < this.jungleGrass.Length; l++)
			{
				if (sourceOverride)
				{
					this.jungleGrass[l].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.jungleGrass[l].Play(pos);
				}
			}
		}
		if (index == 4)
		{
			for (int m = 0; m < this.jungleRock.Length; m++)
			{
				if (sourceOverride)
				{
					this.jungleRock[m].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.jungleRock[m].Play(pos);
				}
			}
		}
		if (index == 5)
		{
			for (int n = 0; n < this.iceSnow.Length; n++)
			{
				if (sourceOverride)
				{
					this.iceSnow[n].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.iceSnow[n].Play(pos);
				}
			}
		}
		if (index == 6)
		{
			for (int num = 0; num < this.iceRock.Length; num++)
			{
				if (sourceOverride)
				{
					this.iceRock[num].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.iceRock[num].Play(pos);
				}
			}
		}
		if (index == 7)
		{
			for (int num2 = 0; num2 < this.metal.Length; num2++)
			{
				if (sourceOverride)
				{
					this.metal[num2].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.metal[num2].Play(pos);
				}
			}
		}
		if (index == 8)
		{
			for (int num3 = 0; num3 < this.wood.Length; num3++)
			{
				if (sourceOverride)
				{
					this.wood[num3].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.wood[num3].Play(pos);
				}
			}
		}
		if (index == 9)
		{
			for (int num4 = 0; num4 < this.volcanoRock.Length; num4++)
			{
				if (sourceOverride)
				{
					this.volcanoRock[num4].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.volcanoRock[num4].Play(pos);
				}
			}
		}
	}

	// Token: 0x0400142F RID: 5167
	public SFX_Instance[] stepDefault;

	// Token: 0x04001430 RID: 5168
	public SFX_Instance[] beachSand;

	// Token: 0x04001431 RID: 5169
	public SFX_Instance[] beachRock;

	// Token: 0x04001432 RID: 5170
	public SFX_Instance[] jungleGrass;

	// Token: 0x04001433 RID: 5171
	public SFX_Instance[] jungleRock;

	// Token: 0x04001434 RID: 5172
	public SFX_Instance[] iceSnow;

	// Token: 0x04001435 RID: 5173
	public SFX_Instance[] iceRock;

	// Token: 0x04001436 RID: 5174
	public SFX_Instance[] metal;

	// Token: 0x04001437 RID: 5175
	public SFX_Instance[] wood;

	// Token: 0x04001438 RID: 5176
	public SFX_Instance[] volcanoRock;
}
