using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public class Action_ApplyAffliction : ItemAction
{
	// Token: 0x06000803 RID: 2051 RVA: 0x0002CF4C File Offset: 0x0002B14C
	public override void RunAction()
	{
		if (this.affliction == null)
		{
			Debug.LogError("Your affliction is null bro");
			return;
		}
		base.character.refs.afflictions.AddAffliction(this.affliction, false);
		if (this.extraAfflictions != null)
		{
			foreach (Affliction affliction in this.extraAfflictions)
			{
				base.character.refs.afflictions.AddAffliction(affliction, false);
			}
		}
	}

	// Token: 0x040007CF RID: 1999
	[SerializeReference]
	public Affliction affliction;

	// Token: 0x040007D0 RID: 2000
	[SerializeReference]
	public Affliction[] extraAfflictions;
}
