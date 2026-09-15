using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001C2 RID: 450
public class EmoteWheelSlice : UIWheelSlice, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06000DDC RID: 3548 RVA: 0x00045278 File Offset: 0x00043478
	public void Init(EmoteWheelData data, EmoteWheel wheel)
	{
		this.emoteWheel = wheel;
		this.emoteData = data;
		if (data == null)
		{
			this.image.enabled = false;
			this.button.interactable = false;
			return;
		}
		this.image.enabled = true;
		this.image.sprite = data.emoteSprite;
		this.button.interactable = true;
		if (data.requireGrounded)
		{
			this.button.interactable = Character.localCharacter.data.isGrounded;
		}
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x00045300 File Offset: 0x00043500
	public void Hover()
	{
		if (this.button.interactable)
		{
			this.emoteWheel.Hover(this.emoteData);
		}
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x00045320 File Offset: 0x00043520
	public void Dehover()
	{
		if (this.button.interactable)
		{
			this.emoteWheel.Dehover(this.emoteData);
		}
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x00045340 File Offset: 0x00043540
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x00045348 File Offset: 0x00043548
	public void OnPointerExit(PointerEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x04000BEB RID: 3051
	private EmoteWheel emoteWheel;

	// Token: 0x04000BEC RID: 3052
	private EmoteWheelData emoteData;

	// Token: 0x04000BED RID: 3053
	public Image image;
}
