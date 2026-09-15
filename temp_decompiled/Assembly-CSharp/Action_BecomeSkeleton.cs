using System;

// Token: 0x020000DC RID: 220
public class Action_BecomeSkeleton : ItemAction
{
	// Token: 0x06000812 RID: 2066 RVA: 0x0002D1B6 File Offset: 0x0002B3B6
	public override void RunAction()
	{
		if (base.character.IsLocal)
		{
			base.character.data.SetSkeleton(!base.character.data.isSkeleton);
		}
	}
}
