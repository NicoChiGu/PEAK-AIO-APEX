using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class Action_ConsumeAndSpawn : ItemAction
{
	// Token: 0x06000820 RID: 2080 RVA: 0x0002D488 File Offset: 0x0002B688
	public override void RunAction()
	{
		if (base.character)
		{
			int num = 0;
			IntItemData intItemData;
			if (this.item.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
			{
				num = intItemData.Value;
			}
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			base.character.StartCoroutine(this.SpawnItemDelayed(num));
		}
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0002D4EB File Offset: 0x0002B6EB
	public IEnumerator SpawnItemDelayed(int cookedAmount)
	{
		Character c = base.character;
		Item item = this.itemToSpawn;
		float timeout = 2f;
		while (this != null)
		{
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				yield break;
			}
			yield return null;
		}
		GameUtils.instance.InstantiateAndGrab(item, c, cookedAmount);
		yield break;
	}

	// Token: 0x040007DC RID: 2012
	public Item itemToSpawn;
}
