using System;
using UnityEngine;

// Token: 0x02000106 RID: 262
public class BugleEventProc : MonoBehaviour
{
	// Token: 0x06000896 RID: 2198 RVA: 0x0002EDE3 File Offset: 0x0002CFE3
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.ThrowBugleEvent));
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0002EE18 File Offset: 0x0002D018
	private void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Remove(item.OnPrimaryStarted, new Action(this.ThrowBugleEvent));
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x0002EE41 File Offset: 0x0002D041
	private void ThrowBugleEvent()
	{
		GlobalEvents.TriggerBugleTooted(this.item);
	}

	// Token: 0x04000842 RID: 2114
	private Item item;
}
