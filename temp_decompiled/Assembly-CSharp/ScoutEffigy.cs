using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200012A RID: 298
public class ScoutEffigy : Constructable
{
	// Token: 0x0600096D RID: 2413 RVA: 0x00031FE0 File Offset: 0x000301E0
	protected override void Update()
	{
		if (this.item.holderCharacter)
		{
			if (!Character.PlayerIsDeadOrDown())
			{
				this.item.overrideUsability = Optionable<bool>.Some(false);
			}
			else
			{
				this.item.overrideUsability = Optionable<bool>.None;
			}
		}
		base.Update();
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x00032030 File Offset: 0x00030230
	public override GameObject FinishConstruction()
	{
		if (!this.constructing)
		{
			return null;
		}
		if (this.currentPreview == null)
		{
			return null;
		}
		List<Character> list = new List<Character>();
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.dead || character.data.fullyPassedOut)
			{
				list.Add(character);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.RandomSelection((Character c) => 1).photonView.RPC("RPCA_ReviveAtPosition", 0, new object[]
		{
			this.currentConstructHit.point + Vector3.up * 1f,
			false,
			-1
		});
		if (Singleton<AchievementManager>.Instance)
		{
			Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.ScoutsResurrected, 1);
		}
		return null;
	}
}
