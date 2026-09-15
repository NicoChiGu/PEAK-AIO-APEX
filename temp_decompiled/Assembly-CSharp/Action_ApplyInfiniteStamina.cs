using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class Action_ApplyInfiniteStamina : ItemAction
{
	// Token: 0x06000808 RID: 2056 RVA: 0x0002CFD4 File Offset: 0x0002B1D4
	public override void RunAction()
	{
		Debug.Log("Adding infinite stamina buff");
		base.character.refs.afflictions.AddAffliction(new Affliction_InfiniteStamina(this.buffTime), false);
	}

	// Token: 0x040007D1 RID: 2001
	public float buffTime;

	// Token: 0x040007D2 RID: 2002
	public float drowsyAmount = 0.25f;
}
