using System;

// Token: 0x02000126 RID: 294
public class OnItemStateChangedAction : ItemActionBase
{
	// Token: 0x0600095D RID: 2397 RVA: 0x00031944 File Offset: 0x0002FB44
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Combine(item.OnStateChange, new Action<ItemState>(this.RunAction));
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x0003196E File Offset: 0x0002FB6E
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Remove(item.OnStateChange, new Action<ItemState>(this.RunAction));
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x00031998 File Offset: 0x0002FB98
	public virtual void RunAction(ItemState state)
	{
	}
}
