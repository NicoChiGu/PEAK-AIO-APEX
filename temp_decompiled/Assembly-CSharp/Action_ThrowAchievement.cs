using System;
using Zorro.Core;

// Token: 0x020000F9 RID: 249
public class Action_ThrowAchievement : ItemAction
{
	// Token: 0x06000860 RID: 2144 RVA: 0x0002DF64 File Offset: 0x0002C164
	public override void RunAction()
	{
		if (this.item.holderCharacter && this.item.holderCharacter.IsLocal)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(this.achievementType);
		}
	}

	// Token: 0x04000806 RID: 2054
	public ACHIEVEMENTTYPE achievementType;
}
