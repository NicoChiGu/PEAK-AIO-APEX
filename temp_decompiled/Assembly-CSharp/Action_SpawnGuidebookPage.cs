using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000F7 RID: 247
public class Action_SpawnGuidebookPage : ItemAction
{
	// Token: 0x0600085A RID: 2138 RVA: 0x0002DE80 File Offset: 0x0002C080
	public override void RunAction()
	{
		if (base.character)
		{
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			int num;
			GuidebookSpawnData guidebookSpawnData = this.PickGuidebookPage(out num);
			base.character.StartCoroutine(this.SpawnPageDelayed(guidebookSpawnData, num));
		}
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x0002DECF File Offset: 0x0002C0CF
	public IEnumerator SpawnPageDelayed(GuidebookSpawnData itemToSpawn, int index)
	{
		Item itemToGrab = itemToSpawn.GetComponent<Item>();
		Character c = base.character;
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
		Singleton<AchievementManager>.Instance.TriggerSeenGuidebookPage(index);
		GameUtils.instance.InstantiateAndGrab(itemToGrab, c, 0);
		yield break;
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x0002DEEC File Offset: 0x0002C0EC
	public GuidebookSpawnData PickGuidebookPage(out int indexChosen)
	{
		int nextPage = Singleton<AchievementManager>.Instance.GetNextPage();
		if (nextPage < 8)
		{
			indexChosen = nextPage;
			return this.possiblePages[indexChosen];
		}
		indexChosen = Random.Range(0, this.possiblePages.Count - 1);
		return this.possiblePages[indexChosen];
	}

	// Token: 0x04000804 RID: 2052
	public List<GuidebookSpawnData> possiblePages;
}
