using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x02000381 RID: 897
	public abstract class VirtualInput
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060016CC RID: 5836 RVA: 0x000750F9 File Offset: 0x000732F9
		// (set) Token: 0x060016CD RID: 5837 RVA: 0x00075101 File Offset: 0x00073301
		public Vector3 virtualMousePosition { get; private set; }

		// Token: 0x060016CE RID: 5838 RVA: 0x0007510A File Offset: 0x0007330A
		public bool AxisExists(string name)
		{
			return this.m_VirtualAxes.ContainsKey(name);
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x00075118 File Offset: 0x00073318
		public bool ButtonExists(string name)
		{
			return this.m_VirtualButtons.ContainsKey(name);
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x00075128 File Offset: 0x00073328
		public void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
		{
			if (this.m_VirtualAxes.ContainsKey(axis.name))
			{
				Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
				return;
			}
			this.m_VirtualAxes.Add(axis.name, axis);
			if (!axis.matchWithInputManager)
			{
				this.m_AlwaysUseVirtual.Add(axis.name);
			}
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x00075190 File Offset: 0x00073390
		public void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
		{
			if (this.m_VirtualButtons.ContainsKey(button.name))
			{
				Debug.LogError("There is already a virtual button named " + button.name + " registered.");
				return;
			}
			this.m_VirtualButtons.Add(button.name, button);
			if (!button.matchWithInputManager)
			{
				this.m_AlwaysUseVirtual.Add(button.name);
			}
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x000751F6 File Offset: 0x000733F6
		public void UnRegisterVirtualAxis(string name)
		{
			if (this.m_VirtualAxes.ContainsKey(name))
			{
				this.m_VirtualAxes.Remove(name);
			}
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x00075213 File Offset: 0x00073413
		public void UnRegisterVirtualButton(string name)
		{
			if (this.m_VirtualButtons.ContainsKey(name))
			{
				this.m_VirtualButtons.Remove(name);
			}
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x00075230 File Offset: 0x00073430
		public CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			if (!this.m_VirtualAxes.ContainsKey(name))
			{
				return null;
			}
			return this.m_VirtualAxes[name];
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x0007524E File Offset: 0x0007344E
		public void SetVirtualMousePositionX(float f)
		{
			this.virtualMousePosition = new Vector3(f, this.virtualMousePosition.y, this.virtualMousePosition.z);
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x00075272 File Offset: 0x00073472
		public void SetVirtualMousePositionY(float f)
		{
			this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, f, this.virtualMousePosition.z);
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x00075296 File Offset: 0x00073496
		public void SetVirtualMousePositionZ(float f)
		{
			this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, this.virtualMousePosition.y, f);
		}

		// Token: 0x060016D8 RID: 5848
		public abstract float GetAxis(string name, bool raw);

		// Token: 0x060016D9 RID: 5849
		public abstract bool GetButton(string name);

		// Token: 0x060016DA RID: 5850
		public abstract bool GetButtonDown(string name);

		// Token: 0x060016DB RID: 5851
		public abstract bool GetButtonUp(string name);

		// Token: 0x060016DC RID: 5852
		public abstract void SetButtonDown(string name);

		// Token: 0x060016DD RID: 5853
		public abstract void SetButtonUp(string name);

		// Token: 0x060016DE RID: 5854
		public abstract void SetAxisPositive(string name);

		// Token: 0x060016DF RID: 5855
		public abstract void SetAxisNegative(string name);

		// Token: 0x060016E0 RID: 5856
		public abstract void SetAxisZero(string name);

		// Token: 0x060016E1 RID: 5857
		public abstract void SetAxis(string name, float value);

		// Token: 0x060016E2 RID: 5858
		public abstract Vector3 MousePosition();

		// Token: 0x04001586 RID: 5510
		protected Dictionary<string, CrossPlatformInputManager.VirtualAxis> m_VirtualAxes = new Dictionary<string, CrossPlatformInputManager.VirtualAxis>();

		// Token: 0x04001587 RID: 5511
		protected Dictionary<string, CrossPlatformInputManager.VirtualButton> m_VirtualButtons = new Dictionary<string, CrossPlatformInputManager.VirtualButton>();

		// Token: 0x04001588 RID: 5512
		protected List<string> m_AlwaysUseVirtual = new List<string>();
	}
}
