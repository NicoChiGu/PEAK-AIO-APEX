using System;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class Action_ConstructableScoutCannonScroll : ItemActionBase
{
	// Token: 0x06000818 RID: 2072 RVA: 0x0002D2A8 File Offset: 0x0002B4A8
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Combine(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollBackwardPressed = (Action)Delegate.Combine(item2.OnScrollBackwardPressed, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollForwardPressed = (Action)Delegate.Combine(item3.OnScrollForwardPressed, new Action(this.ScrollRight));
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x0002D32C File Offset: 0x0002B52C
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Remove(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollBackwardPressed = (Action)Delegate.Remove(item2.OnScrollBackwardPressed, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollForwardPressed = (Action)Delegate.Remove(item3.OnScrollForwardPressed, new Action(this.ScrollRight));
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x0002D3AE File Offset: 0x0002B5AE
	private void ScrollLeft()
	{
		this.Scrolled(-1f);
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0002D3BB File Offset: 0x0002B5BB
	private void ScrollRight()
	{
		this.Scrolled(1f);
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0002D3C8 File Offset: 0x0002B5C8
	private void Scrolled(float value)
	{
		if (this.constructable != null && this.constructable.currentPreview != null)
		{
			this.constructable.angleOffset += value * this.angleAmount;
			this.constructable.angleOffset = Mathf.Clamp(this.constructable.angleOffset, -this.maxAngle, this.maxAngle);
			this.constructable.UpdateAngle();
		}
	}

	// Token: 0x040007D9 RID: 2009
	public Constructable constructable;

	// Token: 0x040007DA RID: 2010
	public float angleAmount = 5f;

	// Token: 0x040007DB RID: 2011
	public float maxAngle;
}
