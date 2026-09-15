using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200033C RID: 828
public class SpiderManager : MonoBehaviour
{
	// Token: 0x0600155A RID: 5466 RVA: 0x0006D491 File Offset: 0x0006B691
	private void Awake()
	{
		SpiderManager.instance = this;
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x0006D49C File Offset: 0x0006B69C
	private void Update()
	{
		if (this.spiders.Count == 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			this.spiders[this.currentIndex].Scan();
			this.currentIndex = (this.currentIndex + 1) % this.spiders.Count;
		}
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x0006D4F3 File Offset: 0x0006B6F3
	public void Register(Spider spider)
	{
		if (!this.spiders.Contains(spider))
		{
			this.spiders.Add(spider);
		}
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x0006D50F File Offset: 0x0006B70F
	public void Unregister(Spider spider)
	{
		this.spiders.Remove(spider);
		if (this.currentIndex >= this.spiders.Count)
		{
			this.currentIndex = 0;
		}
	}

	// Token: 0x04001404 RID: 5124
	public static SpiderManager instance;

	// Token: 0x04001405 RID: 5125
	public List<Spider> spiders = new List<Spider>();

	// Token: 0x04001406 RID: 5126
	private int currentIndex;

	// Token: 0x04001407 RID: 5127
	private const int spidersPerFrame = 3;
}
