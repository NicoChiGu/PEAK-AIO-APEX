using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000351 RID: 849
public class TodaysBiomes : MonoBehaviour
{
	// Token: 0x060015C8 RID: 5576 RVA: 0x000704F4 File Offset: 0x0006E6F4
	private void Start()
	{
		this.shoreIcon.SetActive(false);
		this.tropicsIcon.SetActive(false);
		this.alpineIcon.SetActive(false);
		this.mesaIcon.SetActive(false);
		this.kilnIcon.SetActive(false);
		base.StartCoroutine(this.<Start>g__TrySetBiomes|5_0());
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x0007054C File Offset: 0x0006E74C
	private void SetBiomes(string biomes)
	{
		if (biomes.Contains('S'))
		{
			this.shoreIcon.SetActive(true);
		}
		if (biomes.Contains('T'))
		{
			this.tropicsIcon.SetActive(true);
		}
		if (biomes.Contains('A'))
		{
			this.alpineIcon.SetActive(true);
		}
		if (biomes.Contains('M'))
		{
			this.mesaIcon.SetActive(true);
		}
		if (biomes.Contains('K'))
		{
			this.kilnIcon.SetActive(true);
		}
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x000705CF File Offset: 0x0006E7CF
	[CompilerGenerated]
	private IEnumerator <Start>g__TrySetBiomes|5_0()
	{
		bool set = false;
		while (!set)
		{
			NextLevelService service = GameHandler.GetService<NextLevelService>();
			if (service.Data.IsSome)
			{
				string biomeID = SingletonAsset<MapBaker>.Instance.GetBiomeID(service.Data.Value.CurrentLevelIndex + NextLevelService.debugLevelIndexOffset);
				this.SetBiomes(biomeID);
				set = true;
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
		yield break;
	}

	// Token: 0x040014A2 RID: 5282
	public GameObject shoreIcon;

	// Token: 0x040014A3 RID: 5283
	public GameObject tropicsIcon;

	// Token: 0x040014A4 RID: 5284
	public GameObject alpineIcon;

	// Token: 0x040014A5 RID: 5285
	public GameObject mesaIcon;

	// Token: 0x040014A6 RID: 5286
	public GameObject kilnIcon;
}
