using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class Action_WarpToRandomPlayer : ItemAction
{
	// Token: 0x06000871 RID: 2161 RVA: 0x0002E298 File Offset: 0x0002C498
	public override void RunAction()
	{
		for (int i = 0; i < this.warpSFX.Length; i++)
		{
			this.warpSFX[i].Play(default(Vector3));
		}
		List<Character> list = new List<Character>();
		foreach (Character character in Character.AllCharacters)
		{
			if (!(character == base.character) && !character.data.dead && Vector3.Distance(base.character.Center, character.Center) > this.minimumDistance)
			{
				list.Add(character);
			}
		}
		if (list.Count == 0 && this.restoreUsesOnFailure)
		{
			this.item.photonView.RPC("IncreaseUsesRPC", 0, Array.Empty<object>());
			return;
		}
		Vector3 center = list.RandomSelection((Character c) => 1).Center;
		base.character.photonView.RPC("WarpPlayerRPC", 0, new object[] { center, true });
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0002E3E0 File Offset: 0x0002C5E0
	[PunRPC]
	public void IncreaseUsesRPC()
	{
		OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (data.HasData && data.Value != -1)
		{
			data.Value++;
			if (this.item.totalUses > 0)
			{
				this.item.SetUseRemainingPercentage((float)data.Value / (float)this.item.totalUses);
			}
		}
	}

	// Token: 0x0400081A RID: 2074
	public float minimumDistance = 12f;

	// Token: 0x0400081B RID: 2075
	public bool restoreUsesOnFailure = true;

	// Token: 0x0400081C RID: 2076
	public SFX_Instance[] warpSFX;
}
