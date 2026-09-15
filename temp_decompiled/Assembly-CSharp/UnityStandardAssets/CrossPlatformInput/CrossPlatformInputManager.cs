using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput.PlatformSpecific;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200037C RID: 892
	public static class CrossPlatformInputManager
	{
		// Token: 0x0600169A RID: 5786 RVA: 0x000749C1 File Offset: 0x00072BC1
		public static void SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod activeInputMethod)
		{
			if (activeInputMethod == CrossPlatformInputManager.ActiveInputMethod.Hardware)
			{
				CrossPlatformInputManager.activeInput = CrossPlatformInputManager.s_HardwareInput;
				return;
			}
			if (activeInputMethod != CrossPlatformInputManager.ActiveInputMethod.Touch)
			{
				return;
			}
			CrossPlatformInputManager.activeInput = CrossPlatformInputManager.s_TouchInput;
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x000749E0 File Offset: 0x00072BE0
		public static bool AxisExists(string name)
		{
			return CrossPlatformInputManager.activeInput.AxisExists(name);
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x000749ED File Offset: 0x00072BED
		public static bool ButtonExists(string name)
		{
			return CrossPlatformInputManager.activeInput.ButtonExists(name);
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x000749FA File Offset: 0x00072BFA
		public static void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
		{
			CrossPlatformInputManager.activeInput.RegisterVirtualAxis(axis);
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x00074A07 File Offset: 0x00072C07
		public static void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
		{
			CrossPlatformInputManager.activeInput.RegisterVirtualButton(button);
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x00074A14 File Offset: 0x00072C14
		public static void UnRegisterVirtualAxis(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			CrossPlatformInputManager.activeInput.UnRegisterVirtualAxis(name);
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x00074A2F File Offset: 0x00072C2F
		public static void UnRegisterVirtualButton(string name)
		{
			CrossPlatformInputManager.activeInput.UnRegisterVirtualButton(name);
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x00074A3C File Offset: 0x00072C3C
		public static CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			return CrossPlatformInputManager.activeInput.VirtualAxisReference(name);
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x00074A49 File Offset: 0x00072C49
		public static float GetAxis(string name)
		{
			return CrossPlatformInputManager.GetAxis(name, false);
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x00074A52 File Offset: 0x00072C52
		public static float GetAxisRaw(string name)
		{
			return CrossPlatformInputManager.GetAxis(name, true);
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x00074A5B File Offset: 0x00072C5B
		private static float GetAxis(string name, bool raw)
		{
			return CrossPlatformInputManager.activeInput.GetAxis(name, raw);
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x00074A69 File Offset: 0x00072C69
		public static bool GetButton(string name)
		{
			return CrossPlatformInputManager.activeInput.GetButton(name);
		}

		// Token: 0x060016A6 RID: 5798 RVA: 0x00074A76 File Offset: 0x00072C76
		public static bool GetButtonDown(string name)
		{
			return CrossPlatformInputManager.activeInput.GetButtonDown(name);
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x00074A83 File Offset: 0x00072C83
		public static bool GetButtonUp(string name)
		{
			return CrossPlatformInputManager.activeInput.GetButtonUp(name);
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x00074A90 File Offset: 0x00072C90
		public static void SetButtonDown(string name)
		{
			CrossPlatformInputManager.activeInput.SetButtonDown(name);
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x00074A9D File Offset: 0x00072C9D
		public static void SetButtonUp(string name)
		{
			CrossPlatformInputManager.activeInput.SetButtonUp(name);
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x00074AAA File Offset: 0x00072CAA
		public static void SetAxisPositive(string name)
		{
			CrossPlatformInputManager.activeInput.SetAxisPositive(name);
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x00074AB7 File Offset: 0x00072CB7
		public static void SetAxisNegative(string name)
		{
			CrossPlatformInputManager.activeInput.SetAxisNegative(name);
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x00074AC4 File Offset: 0x00072CC4
		public static void SetAxisZero(string name)
		{
			CrossPlatformInputManager.activeInput.SetAxisZero(name);
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x00074AD1 File Offset: 0x00072CD1
		public static void SetAxis(string name, float value)
		{
			CrossPlatformInputManager.activeInput.SetAxis(name, value);
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060016AE RID: 5806 RVA: 0x00074ADF File Offset: 0x00072CDF
		public static Vector3 mousePosition
		{
			get
			{
				return CrossPlatformInputManager.activeInput.MousePosition();
			}
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x00074AEB File Offset: 0x00072CEB
		public static void SetVirtualMousePositionX(float f)
		{
			CrossPlatformInputManager.activeInput.SetVirtualMousePositionX(f);
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x00074AF8 File Offset: 0x00072CF8
		public static void SetVirtualMousePositionY(float f)
		{
			CrossPlatformInputManager.activeInput.SetVirtualMousePositionY(f);
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x00074B05 File Offset: 0x00072D05
		public static void SetVirtualMousePositionZ(float f)
		{
			CrossPlatformInputManager.activeInput.SetVirtualMousePositionZ(f);
		}

		// Token: 0x04001566 RID: 5478
		private static VirtualInput activeInput = CrossPlatformInputManager.s_HardwareInput;

		// Token: 0x04001567 RID: 5479
		private static VirtualInput s_TouchInput = new MobileInput();

		// Token: 0x04001568 RID: 5480
		private static VirtualInput s_HardwareInput = new StandaloneInput();

		// Token: 0x02000524 RID: 1316
		public enum ActiveInputMethod
		{
			// Token: 0x04001BC9 RID: 7113
			Hardware,
			// Token: 0x04001BCA RID: 7114
			Touch
		}

		// Token: 0x02000525 RID: 1317
		public class VirtualAxis
		{
			// Token: 0x17000274 RID: 628
			// (get) Token: 0x06001DB7 RID: 7607 RVA: 0x00088BD6 File Offset: 0x00086DD6
			// (set) Token: 0x06001DB8 RID: 7608 RVA: 0x00088BDE File Offset: 0x00086DDE
			public string name { get; private set; }

			// Token: 0x17000275 RID: 629
			// (get) Token: 0x06001DB9 RID: 7609 RVA: 0x00088BE7 File Offset: 0x00086DE7
			// (set) Token: 0x06001DBA RID: 7610 RVA: 0x00088BEF File Offset: 0x00086DEF
			public bool matchWithInputManager { get; private set; }

			// Token: 0x06001DBB RID: 7611 RVA: 0x00088BF8 File Offset: 0x00086DF8
			public VirtualAxis(string name)
				: this(name, true)
			{
			}

			// Token: 0x06001DBC RID: 7612 RVA: 0x00088C02 File Offset: 0x00086E02
			public VirtualAxis(string name, bool matchToInputSettings)
			{
				this.name = name;
				this.matchWithInputManager = matchToInputSettings;
			}

			// Token: 0x06001DBD RID: 7613 RVA: 0x00088C18 File Offset: 0x00086E18
			public void Remove()
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.name);
			}

			// Token: 0x06001DBE RID: 7614 RVA: 0x00088C25 File Offset: 0x00086E25
			public void Update(float value)
			{
				this.m_Value = value;
			}

			// Token: 0x17000276 RID: 630
			// (get) Token: 0x06001DBF RID: 7615 RVA: 0x00088C2E File Offset: 0x00086E2E
			public float GetValue
			{
				get
				{
					return this.m_Value;
				}
			}

			// Token: 0x17000277 RID: 631
			// (get) Token: 0x06001DC0 RID: 7616 RVA: 0x00088C36 File Offset: 0x00086E36
			public float GetValueRaw
			{
				get
				{
					return this.m_Value;
				}
			}

			// Token: 0x04001BCC RID: 7116
			private float m_Value;
		}

		// Token: 0x02000526 RID: 1318
		public class VirtualButton
		{
			// Token: 0x17000278 RID: 632
			// (get) Token: 0x06001DC1 RID: 7617 RVA: 0x00088C3E File Offset: 0x00086E3E
			// (set) Token: 0x06001DC2 RID: 7618 RVA: 0x00088C46 File Offset: 0x00086E46
			public string name { get; private set; }

			// Token: 0x17000279 RID: 633
			// (get) Token: 0x06001DC3 RID: 7619 RVA: 0x00088C4F File Offset: 0x00086E4F
			// (set) Token: 0x06001DC4 RID: 7620 RVA: 0x00088C57 File Offset: 0x00086E57
			public bool matchWithInputManager { get; private set; }

			// Token: 0x06001DC5 RID: 7621 RVA: 0x00088C60 File Offset: 0x00086E60
			public VirtualButton(string name)
				: this(name, true)
			{
			}

			// Token: 0x06001DC6 RID: 7622 RVA: 0x00088C6A File Offset: 0x00086E6A
			public VirtualButton(string name, bool matchToInputSettings)
			{
				this.name = name;
				this.matchWithInputManager = matchToInputSettings;
			}

			// Token: 0x06001DC7 RID: 7623 RVA: 0x00088C90 File Offset: 0x00086E90
			public void Pressed()
			{
				if (this.m_Pressed)
				{
					return;
				}
				this.m_Pressed = true;
				this.m_LastPressedFrame = Time.frameCount;
			}

			// Token: 0x06001DC8 RID: 7624 RVA: 0x00088CAD File Offset: 0x00086EAD
			public void Released()
			{
				this.m_Pressed = false;
				this.m_ReleasedFrame = Time.frameCount;
			}

			// Token: 0x06001DC9 RID: 7625 RVA: 0x00088CC1 File Offset: 0x00086EC1
			public void Remove()
			{
				CrossPlatformInputManager.UnRegisterVirtualButton(this.name);
			}

			// Token: 0x1700027A RID: 634
			// (get) Token: 0x06001DCA RID: 7626 RVA: 0x00088CCE File Offset: 0x00086ECE
			public bool GetButton
			{
				get
				{
					return this.m_Pressed;
				}
			}

			// Token: 0x1700027B RID: 635
			// (get) Token: 0x06001DCB RID: 7627 RVA: 0x00088CD6 File Offset: 0x00086ED6
			public bool GetButtonDown
			{
				get
				{
					return this.m_LastPressedFrame - Time.frameCount == -1;
				}
			}

			// Token: 0x1700027C RID: 636
			// (get) Token: 0x06001DCC RID: 7628 RVA: 0x00088CE7 File Offset: 0x00086EE7
			public bool GetButtonUp
			{
				get
				{
					return this.m_ReleasedFrame == Time.frameCount - 1;
				}
			}

			// Token: 0x04001BD0 RID: 7120
			private int m_LastPressedFrame = -5;

			// Token: 0x04001BD1 RID: 7121
			private int m_ReleasedFrame = -5;

			// Token: 0x04001BD2 RID: 7122
			private bool m_Pressed;
		}
	}
}
