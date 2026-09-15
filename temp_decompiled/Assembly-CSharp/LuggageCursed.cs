using System;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class LuggageCursed : Luggage
{
	// Token: 0x0600093A RID: 2362 RVA: 0x00030DAC File Offset: 0x0002EFAC
	public override void Interact_CastFinished(Character interactor)
	{
		if (!interactor.IsLocal)
		{
			return;
		}
		float num = (float)Random.Range(this.minCurse, this.maxCurse + 1) * 0.025f;
		if (num > 0f)
		{
			interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, num, false, true, true);
		}
		if (interactor.data.isSkeleton)
		{
			interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, this.injuryAmt * 0.125f, false, true, true);
		}
		else
		{
			interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, this.injuryAmt, false, true, true);
		}
		base.Interact_CastFinished(interactor);
	}

	// Token: 0x04000897 RID: 2199
	public int minCurse;

	// Token: 0x04000898 RID: 2200
	public int maxCurse;

	// Token: 0x04000899 RID: 2201
	public float injuryAmt;
}
