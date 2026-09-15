using System;
using Peak.Afflictions;

// Token: 0x020000ED RID: 237
public class Action_Numb : ItemAction
{
	// Token: 0x06000840 RID: 2112 RVA: 0x0002DA10 File Offset: 0x0002BC10
	public override void RunAction()
	{
		Affliction_Numb affliction_Numb = new Affliction_Numb
		{
			totalTime = this.numbAmount
		};
		base.character.refs.afflictions.AddAffliction(affliction_Numb, false);
	}

	// Token: 0x040007F0 RID: 2032
	public float numbAmount = 60f;
}
