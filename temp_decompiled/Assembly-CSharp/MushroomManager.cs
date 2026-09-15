using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class MushroomManager : LevelGenStep
{
	// Token: 0x06000958 RID: 2392 RVA: 0x000317C4 File Offset: 0x0002F9C4
	public void Awake()
	{
		MushroomManager.instance = this;
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x000317CC File Offset: 0x0002F9CC
	private void GenerateEffectList()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<int> list4 = new List<int>();
		List<int> list5 = new List<int>();
		int num = this.minGoodEffects;
		int num2 = this.minBadEffects;
		for (int i = 0; i < 10; i++)
		{
			list.Add(i);
			if (Extensions.Contains(Action_RandomMushroomEffect.GoodEffects, i))
			{
				list2.Add(i);
			}
			if (Extensions.Contains(Action_RandomMushroomEffect.BadEffects, i))
			{
				list3.Add(i);
			}
		}
		while (list.Count > 0)
		{
			int num4;
			if (num > 0)
			{
				int num3 = Random.Range(0, list2.Count);
				num4 = list2[num3];
				num--;
			}
			else if (num2 > 0)
			{
				int num3 = Random.Range(0, list3.Count);
				num4 = list3[num3];
				num2--;
			}
			else
			{
				int num3 = Random.Range(0, list.Count);
				num4 = list[num3];
			}
			list4.Add(num4);
			list5.Add(Random.Range(0, 4));
			list.Remove(num4);
			list2.Remove(num4);
			list3.Remove(num4);
		}
		this.mushroomEffects = list4.ToArray();
		this.mushroomStamAmt = list5.ToArray();
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x0003190A File Offset: 0x0002FB0A
	public override void Execute()
	{
		this.GenerateEffectList();
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x00031912 File Offset: 0x0002FB12
	public override void Clear()
	{
	}

	// Token: 0x040008B7 RID: 2231
	public static MushroomManager instance;

	// Token: 0x040008B8 RID: 2232
	public const int MAX_MUSHROOM_EFFECTS = 10;

	// Token: 0x040008B9 RID: 2233
	public int[] mushroomEffects = new int[10];

	// Token: 0x040008BA RID: 2234
	public int[] mushroomStamAmt = new int[10];

	// Token: 0x040008BB RID: 2235
	public int minGoodEffects = 1;

	// Token: 0x040008BC RID: 2236
	public int minBadEffects = 1;
}
