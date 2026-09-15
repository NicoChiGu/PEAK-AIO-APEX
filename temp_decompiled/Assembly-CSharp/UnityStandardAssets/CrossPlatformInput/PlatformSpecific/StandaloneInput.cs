using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput.PlatformSpecific
{
	// Token: 0x02000383 RID: 899
	public class StandaloneInput : VirtualInput
	{
		// Token: 0x060016F2 RID: 5874 RVA: 0x000754E3 File Offset: 0x000736E3
		public override float GetAxis(string name, bool raw)
		{
			if (!raw)
			{
				return Input.GetAxis(name);
			}
			return Input.GetAxisRaw(name);
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x000754F5 File Offset: 0x000736F5
		public override bool GetButton(string name)
		{
			return Input.GetButton(name);
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x000754FD File Offset: 0x000736FD
		public override bool GetButtonDown(string name)
		{
			return Input.GetButtonDown(name);
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x00075505 File Offset: 0x00073705
		public override bool GetButtonUp(string name)
		{
			return Input.GetButtonUp(name);
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x0007550D File Offset: 0x0007370D
		public override void SetButtonDown(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x060016F7 RID: 5879 RVA: 0x00075519 File Offset: 0x00073719
		public override void SetButtonUp(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x060016F8 RID: 5880 RVA: 0x00075525 File Offset: 0x00073725
		public override void SetAxisPositive(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x060016F9 RID: 5881 RVA: 0x00075531 File Offset: 0x00073731
		public override void SetAxisNegative(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x060016FA RID: 5882 RVA: 0x0007553D File Offset: 0x0007373D
		public override void SetAxisZero(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x060016FB RID: 5883 RVA: 0x00075549 File Offset: 0x00073749
		public override void SetAxis(string name, float value)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x00075555 File Offset: 0x00073755
		public override Vector3 MousePosition()
		{
			return Input.mousePosition;
		}
	}
}
