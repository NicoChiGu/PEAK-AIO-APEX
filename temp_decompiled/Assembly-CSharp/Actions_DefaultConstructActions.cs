using System;
using UnityEngine;

// Token: 0x020000D4 RID: 212
[RequireComponent(typeof(Constructable))]
public class Actions_DefaultConstructActions : ItemActionBase
{
	// Token: 0x060007FA RID: 2042 RVA: 0x0002CD24 File Offset: 0x0002AF24
	private void Awake()
	{
		this.constructable = base.GetComponent<Constructable>();
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002CD34 File Offset: 0x0002AF34
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.StartConstruction));
		Item item2 = this.item;
		item2.OnPrimaryFinishedCast = (Action)Delegate.Combine(item2.OnPrimaryFinishedCast, new Action(this.RunAction));
		Item item3 = this.item;
		item3.OnPrimaryCancelled = (Action)Delegate.Combine(item3.OnPrimaryCancelled, new Action(this.CancelConstruction));
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0002CDBC File Offset: 0x0002AFBC
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Remove(item.OnPrimaryStarted, new Action(this.StartConstruction));
		Item item2 = this.item;
		item2.OnPrimaryFinishedCast = (Action)Delegate.Remove(item2.OnPrimaryFinishedCast, new Action(this.RunAction));
		Item item3 = this.item;
		item3.OnPrimaryCancelled = (Action)Delegate.Remove(item3.OnPrimaryCancelled, new Action(this.CancelConstruction));
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0002CE41 File Offset: 0x0002B041
	public virtual void StartConstruction()
	{
		this.constructable.StartConstruction();
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0002CE4E File Offset: 0x0002B04E
	public virtual void CancelConstruction()
	{
		this.constructable.DestroyPreview();
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0002CE5B File Offset: 0x0002B05B
	public override void RunAction()
	{
		this.constructable.FinishConstruction();
	}

	// Token: 0x040007C9 RID: 1993
	public Constructable constructable;
}
