using System;

// Token: 0x020000E6 RID: 230
public class Action_GuidebookScroll : ItemActionBase
{
	// Token: 0x0600082C RID: 2092 RVA: 0x0002D5A0 File Offset: 0x0002B7A0
	private void Awake()
	{
		this.guidebook = base.GetComponent<Guidebook>();
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0002D5B0 File Offset: 0x0002B7B0
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Combine(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollBackwardPressed = (Action)Delegate.Combine(item2.OnScrollBackwardPressed, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollForwardPressed = (Action)Delegate.Combine(item3.OnScrollForwardPressed, new Action(this.ScrollRight));
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0002D634 File Offset: 0x0002B834
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Remove(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollBackwardPressed = (Action)Delegate.Remove(item2.OnScrollBackwardPressed, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollForwardPressed = (Action)Delegate.Remove(item3.OnScrollForwardPressed, new Action(this.ScrollRight));
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0002D6B6 File Offset: 0x0002B8B6
	private void ScrollLeft()
	{
		this.Scrolled(-1f);
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0002D6C3 File Offset: 0x0002B8C3
	private void ScrollRight()
	{
		this.Scrolled(1f);
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0002D6D0 File Offset: 0x0002B8D0
	private void Scrolled(float value)
	{
		if (this.guidebook && this.guidebook.isOpen)
		{
			if (value < 0f)
			{
				this.guidebook.FlipPageLeft();
				return;
			}
			if (value > 0f)
			{
				this.guidebook.FlipPageRight();
			}
		}
	}

	// Token: 0x040007E2 RID: 2018
	private Guidebook guidebook;
}
