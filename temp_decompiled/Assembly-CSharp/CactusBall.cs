using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000009 RID: 9
public class CactusBall : StickyItemComponent
{
	// Token: 0x0600003C RID: 60 RVA: 0x00002DD7 File Offset: 0x00000FD7
	private void Start()
	{
		CollisionModifier component = this.physicalCollider.GetComponent<CollisionModifier>();
		component.onCollide = (Action<Character, CollisionModifier, Collision, Bodypart>)Delegate.Combine(component.onCollide, new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide));
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00002E08 File Offset: 0x00001008
	private void OnCollide(Character character, CollisionModifier modifier, Collision collision, Bodypart bodypart)
	{
		if (!character.IsLocal)
		{
			return;
		}
		if (this.stuck)
		{
			return;
		}
		if (character.data.isInvincible)
		{
			return;
		}
		if (character.data.isSkeleton)
		{
			return;
		}
		if (this.item.itemState != ItemState.Ground)
		{
			return;
		}
		if (this.item.lastThrownCharacter == character && Time.time - this.item.lastThrownTime < 0.5f)
		{
			return;
		}
		this.StickToCharacterLocal(character, bodypart, base.transform.position - bodypart.transform.position);
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00002EA2 File Offset: 0x000010A2
	internal override void StickToCharacterLocal(Character character, Bodypart bodypart, Vector3 worldOffset)
	{
		base.StickToCharacterLocal(character, bodypart, worldOffset);
		this.TestNeedlepointAchievement(character);
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00002EB4 File Offset: 0x000010B4
	public void TestNeedlepointAchievement(Character character)
	{
		if (character.IsLocal)
		{
			int num = 0;
			foreach (StickyItemComponent stickyItemComponent in StickyItemComponent.ALL_STUCK_ITEMS)
			{
				if (stickyItemComponent.stuckToCharacter.IsLocal && stickyItemComponent is CactusBall)
				{
					num++;
				}
			}
			if (num >= 5)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.NeedlepointBadge);
			}
		}
	}

	// Token: 0x0400002D RID: 45
	private int framesSinceSpawned;
}
