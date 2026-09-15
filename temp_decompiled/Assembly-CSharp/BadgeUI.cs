using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001BC RID: 444
public class BadgeUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
	// Token: 0x06000DB2 RID: 3506 RVA: 0x00044BF0 File Offset: 0x00042DF0
	public void Init(BadgeData data)
	{
		this.data = data;
		if (data)
		{
			base.gameObject.SetActive(true);
			this.icon.texture = data.icon;
			this.icon.color = new Color(1f, 1f, 1f, (float)(data.IsLocked ? 0 : 1));
			this.icon.enabled = true;
			this.blank.enabled = false;
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x00044C7A File Offset: 0x00042E7A
	public void Hover()
	{
		this.manager.selectedBadge = this;
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x00044C88 File Offset: 0x00042E88
	public void Dehover()
	{
		if (this.manager.selectedBadge == this)
		{
			this.manager.selectedBadge = null;
		}
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00044CA9 File Offset: 0x00042EA9
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x00044CB1 File Offset: 0x00042EB1
	public void OnPointerExit(PointerEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x00044CB9 File Offset: 0x00042EB9
	public void OnSelect(BaseEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x00044CC1 File Offset: 0x00042EC1
	public void OnDeselect(BaseEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x04000BCB RID: 3019
	public BadgeManager manager;

	// Token: 0x04000BCC RID: 3020
	public RawImage icon;

	// Token: 0x04000BCD RID: 3021
	public RawImage blank;

	// Token: 0x04000BCE RID: 3022
	public BadgeData data;

	// Token: 0x04000BCF RID: 3023
	public CanvasGroup canvasGroup;
}
