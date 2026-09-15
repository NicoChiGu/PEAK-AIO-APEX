using System;
using Zorro.Core;

// Token: 0x020000E5 RID: 229
public class Action_Guidebook : ItemAction
{
	// Token: 0x06000829 RID: 2089 RVA: 0x0002D565 File Offset: 0x0002B765
	private void Awake()
	{
		this.guidebook = base.GetComponent<Guidebook>();
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0002D573 File Offset: 0x0002B773
	public override void RunAction()
	{
		this.guidebook.ToggleGuidebook();
		if (this.isSinglePage)
		{
			Singleton<AchievementManager>.Instance.TriggerSeenGuidebookPage(this.singlePageIndex);
		}
	}

	// Token: 0x040007DF RID: 2015
	private Guidebook guidebook;

	// Token: 0x040007E0 RID: 2016
	public bool isSinglePage;

	// Token: 0x040007E1 RID: 2017
	public int singlePageIndex;
}
