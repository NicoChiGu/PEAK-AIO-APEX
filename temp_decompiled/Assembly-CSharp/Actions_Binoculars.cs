using System;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class Actions_Binoculars : ItemActionBase
{
	// Token: 0x060007F4 RID: 2036 RVA: 0x0002CB90 File Offset: 0x0002AD90
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Combine(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollForwardHeld = (Action)Delegate.Combine(item2.OnScrollForwardHeld, new Action(this.ScrollForwardHeld));
		Item item3 = this.item;
		item3.OnScrollBackwardHeld = (Action)Delegate.Combine(item3.OnScrollBackwardHeld, new Action(this.ScrollBackwardHeld));
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0002CC14 File Offset: 0x0002AE14
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Remove(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollForwardHeld = (Action)Delegate.Remove(item2.OnScrollForwardHeld, new Action(this.ScrollForwardHeld));
		Item item3 = this.item;
		item3.OnScrollBackwardHeld = (Action)Delegate.Remove(item3.OnScrollBackwardHeld, new Action(this.ScrollBackwardHeld));
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0002CC96 File Offset: 0x0002AE96
	private void ScrollForwardHeld()
	{
		if (this.binocOverlay.binocularsActive)
		{
			this.cameraOverride.AdjustFOV(-this.scrollSpeedButton * Time.deltaTime);
		}
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0002CCBD File Offset: 0x0002AEBD
	private void ScrollBackwardHeld()
	{
		if (this.binocOverlay.binocularsActive)
		{
			this.cameraOverride.AdjustFOV(this.scrollSpeedButton * Time.deltaTime);
		}
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0002CCE3 File Offset: 0x0002AEE3
	private void Scrolled(float value)
	{
		if (this.binocOverlay.binocularsActive)
		{
			this.cameraOverride.AdjustFOV(-value * this.scrollSpeed);
		}
	}

	// Token: 0x040007C5 RID: 1989
	public Action_ShowBinocularOverlay binocOverlay;

	// Token: 0x040007C6 RID: 1990
	public CameraOverride_Binoculars cameraOverride;

	// Token: 0x040007C7 RID: 1991
	public float scrollSpeed = 2f;

	// Token: 0x040007C8 RID: 1992
	public float scrollSpeedButton = 2f;
}
