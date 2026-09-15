using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200037E RID: 894
	public class Joystick : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler
	{
		// Token: 0x060016B5 RID: 5813 RVA: 0x00074B36 File Offset: 0x00072D36
		private void OnEnable()
		{
			this.CreateVirtualAxes();
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x00074B3E File Offset: 0x00072D3E
		private void Start()
		{
			this.m_StartPos = base.transform.position;
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x00074B54 File Offset: 0x00072D54
		private void UpdateVirtualAxes(Vector3 value)
		{
			Vector3 vector = this.m_StartPos - value;
			vector.y = -vector.y;
			vector /= (float)this.MovementRange;
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis.Update(-vector.x);
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis.Update(vector.y);
			}
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x00074BC0 File Offset: 0x00072DC0
		private void CreateVirtualAxes()
		{
			this.m_UseX = this.axesToUse == Joystick.AxisOption.Both || this.axesToUse == Joystick.AxisOption.OnlyHorizontal;
			this.m_UseY = this.axesToUse == Joystick.AxisOption.Both || this.axesToUse == Joystick.AxisOption.OnlyVertical;
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

		// Token: 0x060016B9 RID: 5817 RVA: 0x00074C4C File Offset: 0x00072E4C
		public void OnDrag(PointerEventData data)
		{
			Vector3 zero = Vector3.zero;
			if (this.m_UseX)
			{
				int num = (int)(data.position.x - this.m_StartPos.x);
				num = Mathf.Clamp(num, -this.MovementRange, this.MovementRange);
				zero.x = (float)num;
			}
			if (this.m_UseY)
			{
				int num2 = (int)(data.position.y - this.m_StartPos.y);
				num2 = Mathf.Clamp(num2, -this.MovementRange, this.MovementRange);
				zero.y = (float)num2;
			}
			base.transform.position = new Vector3(this.m_StartPos.x + zero.x, this.m_StartPos.y + zero.y, this.m_StartPos.z + zero.z);
			this.UpdateVirtualAxes(base.transform.position);
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x00074D32 File Offset: 0x00072F32
		public void OnPointerUp(PointerEventData data)
		{
			base.transform.position = this.m_StartPos;
			this.UpdateVirtualAxes(this.m_StartPos);
		}

		// Token: 0x060016BB RID: 5819 RVA: 0x00074D51 File Offset: 0x00072F51
		public void OnPointerDown(PointerEventData data)
		{
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x00074D53 File Offset: 0x00072F53
		private void OnDisable()
		{
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis.Remove();
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis.Remove();
			}
		}

		// Token: 0x0400156A RID: 5482
		public int MovementRange = 100;

		// Token: 0x0400156B RID: 5483
		public Joystick.AxisOption axesToUse;

		// Token: 0x0400156C RID: 5484
		public string horizontalAxisName = "Horizontal";

		// Token: 0x0400156D RID: 5485
		public string verticalAxisName = "Vertical";

		// Token: 0x0400156E RID: 5486
		private Vector3 m_StartPos;

		// Token: 0x0400156F RID: 5487
		private bool m_UseX;

		// Token: 0x04001570 RID: 5488
		private bool m_UseY;

		// Token: 0x04001571 RID: 5489
		private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

		// Token: 0x04001572 RID: 5490
		private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

		// Token: 0x02000527 RID: 1319
		public enum AxisOption
		{
			// Token: 0x04001BD4 RID: 7124
			Both,
			// Token: 0x04001BD5 RID: 7125
			OnlyHorizontal,
			// Token: 0x04001BD6 RID: 7126
			OnlyVertical
		}
	}
}
