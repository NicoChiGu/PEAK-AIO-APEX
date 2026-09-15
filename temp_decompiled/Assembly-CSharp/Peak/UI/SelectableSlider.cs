using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Zorro.ControllerSupport;

namespace Peak.UI
{
	// Token: 0x020003BB RID: 955
	public class SelectableSlider : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ISelectHandler, IDeselectHandler
	{
		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06001881 RID: 6273 RVA: 0x0007C5B0 File Offset: 0x0007A7B0
		private bool ChildIsSelected
		{
			get
			{
				return EventSystem.current.currentSelectedGameObject == this._childSlider.gameObject;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06001882 RID: 6274 RVA: 0x0007C5CC File Offset: 0x0007A7CC
		public Selectable MySelectable
		{
			get
			{
				return base.GetComponent<Selectable>();
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06001883 RID: 6275 RVA: 0x0007C5D4 File Offset: 0x0007A7D4
		public Slider ChildSlider
		{
			get
			{
				return this._childSlider;
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06001884 RID: 6276 RVA: 0x0007C5DC File Offset: 0x0007A7DC
		public KickButton KickButton
		{
			get
			{
				return this._kickButton;
			}
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x0007C5E4 File Offset: 0x0007A7E4
		private void Update()
		{
			this.MySelectable.interactable = InputHandler.GetCurrentUsedInputScheme() > 0;
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x0007C5FC File Offset: 0x0007A7FC
		public void Init()
		{
			if (this._init)
			{
				return;
			}
			this._init = true;
			this._childSlider = base.GetComponentInChildren<Slider>(true);
			this._childSlider.onValueChanged.AddListener(new UnityAction<float>(this.EnsureReselected));
			this._events = ExtensionMethods.GetOrAddComponent<SelectableEvents>(this._childSlider.gameObject);
			this._events.Submitted += new Action<BaseEventData>(this.SelectSelf);
			this._kickButton = base.GetComponentInChildren<KickButton>(true);
			this._kickButton.Init();
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x0007C687 File Offset: 0x0007A887
		private void SelectSelf(BaseEventData _)
		{
			this.MySelectable.Select();
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x0007C694 File Offset: 0x0007A894
		private void EnsureReselected(float _)
		{
			if (InputHandler.GetCurrentUsedInputScheme() != null)
			{
				this._childSlider.Select();
			}
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x0007C6A8 File Offset: 0x0007A8A8
		public void OnSubmit(BaseEventData eventData)
		{
			this._childSlider.Select();
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x0007C6B5 File Offset: 0x0007A8B5
		public void OnSelect(BaseEventData eventData)
		{
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x0007C6B7 File Offset: 0x0007A8B7
		public void OnDeselect(BaseEventData eventData)
		{
		}

		// Token: 0x0400169E RID: 5790
		private Color _defaultNameColor;

		// Token: 0x0400169F RID: 5791
		private Slider _childSlider;

		// Token: 0x040016A0 RID: 5792
		private SelectableEvents _events;

		// Token: 0x040016A1 RID: 5793
		private KickButton _kickButton;

		// Token: 0x040016A2 RID: 5794
		private bool _init;
	}
}
