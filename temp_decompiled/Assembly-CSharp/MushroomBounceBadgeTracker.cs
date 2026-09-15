using System;
using System.Collections;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002AA RID: 682
public class MushroomBounceBadgeTracker : MonoBehaviour, ICollisionModifierEvent
{
	// Token: 0x060012A7 RID: 4775 RVA: 0x0005F040 File Offset: 0x0005D240
	public void OnBouncedCharacter(Character character)
	{
		if (character.IsLocal && !this.checkingPlayerHeight)
		{
			this.checkingPlayerHeight = true;
			GameUtils.instance.StartCoroutine(this.TestAchievementRoutine(character, character.Center.y));
		}
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x0005F076 File Offset: 0x0005D276
	private IEnumerator TestAchievementRoutine(Character character, float startingHeight)
	{
		yield return new WaitForSeconds(0.6f);
		float timeout = 1f;
		while (character)
		{
			if (!character.data.isGrounded)
			{
				break;
			}
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				this.checkingPlayerHeight = false;
				yield break;
			}
			yield return null;
		}
		while (character != null && !character.data.isGrounded)
		{
			yield return null;
			if ((character.Center.y - startingHeight) * CharacterStats.unitsToMeters >= 40f)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.MycoacrobaticsBadge);
				this.checkingPlayerHeight = false;
				yield break;
			}
		}
		this.checkingPlayerHeight = false;
		yield break;
	}

	// Token: 0x04001162 RID: 4450
	public const float HEIGHT_REQUIREMENT_IN_METERS = 40f;

	// Token: 0x04001163 RID: 4451
	private bool checkingPlayerHeight;
}
