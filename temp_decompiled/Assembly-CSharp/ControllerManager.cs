using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

// Token: 0x02000077 RID: 119
public class ControllerManager
{
	// Token: 0x06000543 RID: 1347 RVA: 0x0001F13D File Offset: 0x0001D33D
	public void Init()
	{
		InputSystem.onDeviceChange += new Action<InputDevice, InputDeviceChange>(this.OnDeviceChange);
		this.UpdateGamepadUsage();
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0001F156 File Offset: 0x0001D356
	public void Destroy()
	{
		InputSystem.onDeviceChange -= new Action<InputDevice, InputDeviceChange>(this.OnDeviceChange);
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001F169 File Offset: 0x0001D369
	private void OnDeviceChange(InputDevice device, InputDeviceChange change)
	{
		this.UpdateGamepadUsage();
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0001F174 File Offset: 0x0001D374
	private void UpdateGamepadUsage()
	{
		using (ReadOnlyArray<InputDevice>.Enumerator enumerator = InputSystem.devices.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is Gamepad)
				{
					this.gamepadAttached = true;
					return;
				}
			}
		}
		this.gamepadAttached = false;
	}

	// Token: 0x0400058F RID: 1423
	public bool gamepadAttached;
}
