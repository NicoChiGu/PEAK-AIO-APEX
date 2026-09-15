using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200037A RID: 890
	public class AxisTouchButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x0600168B RID: 5771 RVA: 0x000747E8 File Offset: 0x000729E8
		private void OnEnable()
		{
			if (!CrossPlatformInputManager.AxisExists(this.axisName))
			{
				this.m_Axis = new CrossPlatformInputManager.VirtualAxis(this.axisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_Axis);
			}
			else
			{
				this.m_Axis = CrossPlatformInputManager.VirtualAxisReference(this.axisName);
			}
			this.FindPairedButton();
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x00074838 File Offset: 0x00072A38
		private void FindPairedButton()
		{
			AxisTouchButton[] array = Object.FindObjectsOfType(typeof(AxisTouchButton)) as AxisTouchButton[];
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].axisName == this.axisName && array[i] != this)
					{
						this.m_PairedWith = array[i];
					}
				}
			}
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x00074894 File Offset: 0x00072A94
		private void OnDisable()
		{
			this.m_Axis.Remove();
		}

		// Token: 0x0600168E RID: 5774 RVA: 0x000748A4 File Offset: 0x00072AA4
		public void OnPointerDown(PointerEventData data)
		{
			if (this.m_PairedWith == null)
			{
				this.FindPairedButton();
			}
			this.m_Axis.Update(Mathf.MoveTowards(this.m_Axis.GetValue, this.axisValue, this.responseSpeed * Time.deltaTime));
		}

		// Token: 0x0600168F RID: 5775 RVA: 0x000748F2 File Offset: 0x00072AF2
		public void OnPointerUp(PointerEventData data)
		{
			this.m_Axis.Update(Mathf.MoveTowards(this.m_Axis.GetValue, 0f, this.responseSpeed * Time.deltaTime));
		}

		// Token: 0x0400155F RID: 5471
		public string axisName = "Horizontal";

		// Token: 0x04001560 RID: 5472
		public float axisValue = 1f;

		// Token: 0x04001561 RID: 5473
		public float responseSpeed = 3f;

		// Token: 0x04001562 RID: 5474
		public float returnToCentreSpeed = 3f;

		// Token: 0x04001563 RID: 5475
		private AxisTouchButton m_PairedWith;

		// Token: 0x04001564 RID: 5476
		private CrossPlatformInputManager.VirtualAxis m_Axis;
	}
}
