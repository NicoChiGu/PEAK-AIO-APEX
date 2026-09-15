using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D4 RID: 468
public class MenuWindowTabbed : MenuWindow
{
	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000E67 RID: 3687 RVA: 0x000470D0 File Offset: 0x000452D0
	public virtual int startOnTab
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x000470D3 File Offset: 0x000452D3
	internal override void Open()
	{
		this.InitTabs();
		base.Open();
		this.SelectTab(this.startOnTab);
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x000470ED File Offset: 0x000452ED
	protected virtual void InitTabs()
	{
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x000470F0 File Offset: 0x000452F0
	public void SelectTab(int index)
	{
		if (this.tabs.Count <= index || index < 0)
		{
			Debug.LogError(string.Format("{0} tried to select out of range tab: {1}", base.gameObject.name, index));
			return;
		}
		for (int i = 0; i < this.tabs.Count; i++)
		{
			if (i == index)
			{
				this.tabs[i].Open();
			}
			else
			{
				this.tabs[i].Close();
			}
		}
		this.currentTab = index;
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x00047178 File Offset: 0x00045378
	public void SelectNextTab(bool forward)
	{
		this.currentTab += (forward ? 1 : (-1));
		if (this.currentTab >= this.tabs.Count)
		{
			this.currentTab = 0;
		}
		else if (this.currentTab < 0)
		{
			this.currentTab = this.tabs.Count - 1;
		}
		this.SelectTab(this.currentTab);
	}

	// Token: 0x04000C63 RID: 3171
	protected List<MenuWindow> tabs = new List<MenuWindow>();

	// Token: 0x04000C64 RID: 3172
	private int currentTab;
}
