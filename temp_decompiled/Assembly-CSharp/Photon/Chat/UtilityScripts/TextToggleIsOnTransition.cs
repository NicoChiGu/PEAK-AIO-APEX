using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020003A2 RID: 930
	[RequireComponent(typeof(Text))]
	public class TextToggleIsOnTransition : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x0600182C RID: 6188 RVA: 0x0007A3AB File Offset: 0x000785AB
		public void OnEnable()
		{
			this._text = base.GetComponent<Text>();
			this.OnValueChanged(this.toggle.isOn);
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x0007A3E6 File Offset: 0x000785E6
		public void OnDisable()
		{
			this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnValueChanged));
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x0007A404 File Offset: 0x00078604
		public void OnValueChanged(bool isOn)
		{
			this._text.color = (isOn ? (this.isHover ? this.HoverOnColor : this.HoverOnColor) : (this.isHover ? this.NormalOffColor : this.NormalOffColor));
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x0007A442 File Offset: 0x00078642
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.isHover = true;
			this._text.color = (this.toggle.isOn ? this.HoverOnColor : this.HoverOffColor);
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x0007A471 File Offset: 0x00078671
		public void OnPointerExit(PointerEventData eventData)
		{
			this.isHover = false;
			this._text.color = (this.toggle.isOn ? this.NormalOnColor : this.NormalOffColor);
		}

		// Token: 0x0400165F RID: 5727
		public Toggle toggle;

		// Token: 0x04001660 RID: 5728
		private Text _text;

		// Token: 0x04001661 RID: 5729
		public Color NormalOnColor = Color.white;

		// Token: 0x04001662 RID: 5730
		public Color NormalOffColor = Color.black;

		// Token: 0x04001663 RID: 5731
		public Color HoverOnColor = Color.black;

		// Token: 0x04001664 RID: 5732
		public Color HoverOffColor = Color.black;

		// Token: 0x04001665 RID: 5733
		private bool isHover;
	}
}
