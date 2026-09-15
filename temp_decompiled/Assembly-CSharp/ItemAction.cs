using System;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class ItemAction : ItemActionBase
{
	// Token: 0x060008FF RID: 2303 RVA: 0x00030410 File Offset: 0x0002E610
	protected override void Subscribe()
	{
		if (this.OnPressed)
		{
			Item item = this.item;
			item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.RunAction));
		}
		if (this.OnHeld)
		{
			Item item2 = this.item;
			item2.OnPrimaryHeld = (Action)Delegate.Combine(item2.OnPrimaryHeld, new Action(this.RunAction));
		}
		if (this.OnCastFinished)
		{
			Item item3 = this.item;
			item3.OnPrimaryFinishedCast = (Action)Delegate.Combine(item3.OnPrimaryFinishedCast, new Action(this.RunAction));
		}
		if (this.OnCancelled)
		{
			Item item4 = this.item;
			item4.OnPrimaryCancelled = (Action)Delegate.Combine(item4.OnPrimaryCancelled, new Action(this.RunAction));
		}
		if (this.OnConsumed)
		{
			Item item5 = this.item;
			item5.OnConsumed = (Action)Delegate.Combine(item5.OnConsumed, new Action(this.RunAction));
		}
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00030510 File Offset: 0x0002E710
	protected override void Unsubscribe()
	{
		if (this.OnPressed)
		{
			Item item = this.item;
			item.OnPrimaryStarted = (Action)Delegate.Remove(item.OnPrimaryStarted, new Action(this.RunAction));
		}
		if (this.OnHeld)
		{
			Item item2 = this.item;
			item2.OnPrimaryHeld = (Action)Delegate.Remove(item2.OnPrimaryHeld, new Action(this.RunAction));
		}
		if (this.OnCastFinished)
		{
			Item item3 = this.item;
			item3.OnPrimaryFinishedCast = (Action)Delegate.Remove(item3.OnPrimaryFinishedCast, new Action(this.RunAction));
		}
		if (this.OnCancelled)
		{
			Item item4 = this.item;
			item4.OnPrimaryCancelled = (Action)Delegate.Remove(item4.OnPrimaryCancelled, new Action(this.RunAction));
		}
		if (this.OnConsumed)
		{
			Item item5 = this.item;
			item5.OnConsumed = (Action)Delegate.Remove(item5.OnConsumed, new Action(this.RunAction));
		}
	}

	// Token: 0x0400087B RID: 2171
	[SerializeField]
	public bool OnPressed;

	// Token: 0x0400087C RID: 2172
	[SerializeField]
	public bool OnHeld;

	// Token: 0x0400087D RID: 2173
	[SerializeField]
	public bool OnReleased;

	// Token: 0x0400087E RID: 2174
	[SerializeField]
	public bool OnCastFinished;

	// Token: 0x0400087F RID: 2175
	[SerializeField]
	public bool OnCancelled;

	// Token: 0x04000880 RID: 2176
	public bool OnConsumed;
}
