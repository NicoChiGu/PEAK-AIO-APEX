using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020003A1 RID: 929
	[RequireComponent(typeof(Text))]
	public class TextButtonTransition : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06001826 RID: 6182 RVA: 0x0007A2FD File Offset: 0x000784FD
		public void Awake()
		{
			this._text = base.GetComponent<Text>();
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x0007A30B File Offset: 0x0007850B
		public void OnEnable()
		{
			this._text.color = this.NormalColor;
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x0007A31E File Offset: 0x0007851E
		public void OnDisable()
		{
			this._text.color = this.NormalColor;
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x0007A331 File Offset: 0x00078531
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.Selectable == null || this.Selectable.IsInteractable())
			{
				this._text.color = this.HoverColor;
			}
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x0007A35F File Offset: 0x0007855F
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.Selectable == null || this.Selectable.IsInteractable())
			{
				this._text.color = this.NormalColor;
			}
		}

		// Token: 0x0400165B RID: 5723
		private Text _text;

		// Token: 0x0400165C RID: 5724
		public Selectable Selectable;

		// Token: 0x0400165D RID: 5725
		public Color NormalColor = Color.white;

		// Token: 0x0400165E RID: 5726
		public Color HoverColor = Color.black;
	}
}
