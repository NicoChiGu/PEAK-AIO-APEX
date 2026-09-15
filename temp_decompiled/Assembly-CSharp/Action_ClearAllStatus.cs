using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DE RID: 222
public class Action_ClearAllStatus : ItemAction
{
	// Token: 0x06000816 RID: 2070 RVA: 0x0002D228 File Offset: 0x0002B428
	public override void RunAction()
	{
		int num = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length;
		for (int i = 0; i < num; i++)
		{
			CharacterAfflictions.STATUSTYPE statustype = (CharacterAfflictions.STATUSTYPE)i;
			if ((!this.excludeCurse || statustype != CharacterAfflictions.STATUSTYPE.Curse) && !this.otherExclusions.Contains(statustype))
			{
				base.character.refs.afflictions.SubtractStatus(statustype, (float)Mathf.Abs(5), false, false);
			}
		}
	}

	// Token: 0x040007D7 RID: 2007
	public bool excludeCurse = true;

	// Token: 0x040007D8 RID: 2008
	public List<CharacterAfflictions.STATUSTYPE> otherExclusions = new List<CharacterAfflictions.STATUSTYPE>();
}
