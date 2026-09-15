using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000EB RID: 235
public class Action_ModifyStatus : ItemAction
{
	// Token: 0x0600083C RID: 2108 RVA: 0x0002D808 File Offset: 0x0002BA08
	public override void RunAction()
	{
		if (this.ifSkeleton && !base.character.data.isSkeleton)
		{
			return;
		}
		bool passedOut = base.character.data.passedOut;
		if (this.changeAmount < 0f)
		{
			if (this.statusType == CharacterAfflictions.STATUSTYPE.Poison)
			{
				base.character.refs.afflictions.ClearPoisonAfflictions();
				int num = Mathf.RoundToInt(Mathf.Min(base.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison), Mathf.Abs(this.changeAmount)) * 100f);
				Character character;
				if (this.item.TryGetFeeder(out character))
				{
					GameUtils.instance.IncrementFriendPoisonHealing(num, character.photonView.Owner);
				}
				else
				{
					Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PoisonHealed, num);
				}
			}
			Character character2;
			if (this.statusType == CharacterAfflictions.STATUSTYPE.Injury && this.item.TryGetFeeder(out character2))
			{
				int num2 = Mathf.RoundToInt(Mathf.Min(base.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Injury), Mathf.Abs(this.changeAmount)) * 100f);
				GameUtils.instance.IncrementFriendHealing(num2, character2.photonView.Owner);
			}
			base.character.refs.afflictions.SubtractStatus(this.statusType, Mathf.Abs(this.changeAmount), false, false);
		}
		else
		{
			base.character.refs.afflictions.AddStatus(this.statusType, Mathf.Abs(this.changeAmount), false, true, true);
		}
		float statusSum = base.character.refs.afflictions.statusSum;
		if (passedOut && statusSum <= 1f)
		{
			Debug.Log("LIFE WAS SAVED");
			Character character3;
			if (this.item.TryGetFeeder(out character3))
			{
				GameUtils.instance.ThrowEmergencyPreparednessAchievement(character3.photonView.Owner);
			}
		}
	}

	// Token: 0x040007EA RID: 2026
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x040007EB RID: 2027
	public float changeAmount;

	// Token: 0x040007EC RID: 2028
	public bool ifSkeleton;
}
