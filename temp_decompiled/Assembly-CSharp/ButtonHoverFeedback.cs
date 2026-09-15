using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000220 RID: 544
public class ButtonHoverFeedback : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x0600100D RID: 4109 RVA: 0x0004FCB9 File Offset: 0x0004DEB9
	private void Start()
	{
		Button component = base.GetComponent<Button>();
		if (component == null)
		{
			return;
		}
		component.onClick.AddListener(new UnityAction(this.OnClick));
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x0004FCDC File Offset: 0x0004DEDC
	private void OnClick()
	{
		this.vel += 15f;
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x0004FCF0 File Offset: 0x0004DEF0
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.targetScale = 1.15f;
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x0004FCFD File Offset: 0x0004DEFD
	public void OnPointerExit(PointerEventData eventData)
	{
		this.targetScale = 1f;
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x0004FD0A File Offset: 0x0004DF0A
	private void OnEnable()
	{
		base.transform.localScale = Vector3.one;
		this.scale = 1f;
		this.vel = 0f;
		this.targetScale = 1f;
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x0004FD40 File Offset: 0x0004DF40
	private void Update()
	{
		this.vel = FRILerp.Lerp(this.vel, (this.targetScale - this.scale) * 25f, 20f, true);
		this.scale += this.vel * Time.deltaTime;
		base.transform.localScale = Vector3.one * this.scale;
	}

	// Token: 0x04000E7D RID: 3709
	private float scale = 1f;

	// Token: 0x04000E7E RID: 3710
	private float vel;

	// Token: 0x04000E7F RID: 3711
	private float targetScale = 1f;
}
