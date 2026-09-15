using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x02000380 RID: 896
	[RequireComponent(typeof(Image))]
	public class TouchPad : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x060016C3 RID: 5827 RVA: 0x00074E50 File Offset: 0x00073050
		private void OnEnable()
		{
			this.CreateVirtualAxes();
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x00074E58 File Offset: 0x00073058
		private void Start()
		{
			this.m_Image = base.GetComponent<Image>();
			this.m_Center = this.m_Image.transform.position;
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x00074E7C File Offset: 0x0007307C
		private void CreateVirtualAxes()
		{
			this.m_UseX = this.axesToUse == TouchPad.AxisOption.Both || this.axesToUse == TouchPad.AxisOption.OnlyHorizontal;
			this.m_UseY = this.axesToUse == TouchPad.AxisOption.Both || this.axesToUse == TouchPad.AxisOption.OnlyVertical;
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_HorizontalVirtualAxis);
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_VerticalVirtualAxis);
			}
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x00074F05 File Offset: 0x00073105
		private void UpdateVirtualAxes(Vector3 value)
		{
			value = value.normalized;
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis.Update(value.x);
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis.Update(value.y);
			}
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x00074F42 File Offset: 0x00073142
		public void OnPointerDown(PointerEventData data)
		{
			this.m_Dragging = true;
			this.m_Id = data.pointerId;
			if (this.controlStyle != TouchPad.ControlStyle.Absolute)
			{
				this.m_Center = data.position;
			}
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x00074F70 File Offset: 0x00073170
		private void Update()
		{
			if (!this.m_Dragging)
			{
				return;
			}
			if (Input.touchCount >= this.m_Id + 1 && this.m_Id != -1)
			{
				if (this.controlStyle == TouchPad.ControlStyle.Swipe)
				{
					this.m_Center = this.m_PreviousTouchPos;
					this.m_PreviousTouchPos = Input.touches[this.m_Id].position;
				}
				Vector2 normalized = new Vector2(Input.touches[this.m_Id].position.x - this.m_Center.x, Input.touches[this.m_Id].position.y - this.m_Center.y).normalized;
				normalized.x *= this.Xsensitivity;
				normalized.y *= this.Ysensitivity;
				this.UpdateVirtualAxes(new Vector3(normalized.x, normalized.y, 0f));
			}
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x00075071 File Offset: 0x00073271
		public void OnPointerUp(PointerEventData data)
		{
			this.m_Dragging = false;
			this.m_Id = -1;
			this.UpdateVirtualAxes(Vector3.zero);
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x0007508C File Offset: 0x0007328C
		private void OnDisable()
		{
			if (CrossPlatformInputManager.AxisExists(this.horizontalAxisName))
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.horizontalAxisName);
			}
			if (CrossPlatformInputManager.AxisExists(this.verticalAxisName))
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.verticalAxisName);
			}
		}

		// Token: 0x04001573 RID: 5491
		public TouchPad.AxisOption axesToUse;

		// Token: 0x04001574 RID: 5492
		public TouchPad.ControlStyle controlStyle;

		// Token: 0x04001575 RID: 5493
		public string horizontalAxisName = "Horizontal";

		// Token: 0x04001576 RID: 5494
		public string verticalAxisName = "Vertical";

		// Token: 0x04001577 RID: 5495
		public float Xsensitivity = 1f;

		// Token: 0x04001578 RID: 5496
		public float Ysensitivity = 1f;

		// Token: 0x04001579 RID: 5497
		private Vector3 m_StartPos;

		// Token: 0x0400157A RID: 5498
		private Vector2 m_PreviousDelta;

		// Token: 0x0400157B RID: 5499
		private Vector3 m_JoytickOutput;

		// Token: 0x0400157C RID: 5500
		private bool m_UseX;

		// Token: 0x0400157D RID: 5501
		private bool m_UseY;

		// Token: 0x0400157E RID: 5502
		private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

		// Token: 0x0400157F RID: 5503
		private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

		// Token: 0x04001580 RID: 5504
		private bool m_Dragging;

		// Token: 0x04001581 RID: 5505
		private int m_Id = -1;

		// Token: 0x04001582 RID: 5506
		private Vector2 m_PreviousTouchPos;

		// Token: 0x04001583 RID: 5507
		private Vector3 m_Center;

		// Token: 0x04001584 RID: 5508
		private Image m_Image;

		// Token: 0x02000528 RID: 1320
		public enum AxisOption
		{
			// Token: 0x04001BD8 RID: 7128
			Both,
			// Token: 0x04001BD9 RID: 7129
			OnlyHorizontal,
			// Token: 0x04001BDA RID: 7130
			OnlyVertical
		}

		// Token: 0x02000529 RID: 1321
		public enum ControlStyle
		{
			// Token: 0x04001BDC RID: 7132
			Absolute,
			// Token: 0x04001BDD RID: 7133
			Relative,
			// Token: 0x04001BDE RID: 7134
			Swipe
		}
	}
}
