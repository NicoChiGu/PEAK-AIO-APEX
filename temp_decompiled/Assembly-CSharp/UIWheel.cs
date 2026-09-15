using System;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001EB RID: 491
public class UIWheel : MonoBehaviour
{
	// Token: 0x06000EF0 RID: 3824 RVA: 0x000495FB File Offset: 0x000477FB
	protected virtual Vector2 GetCursorOrigin()
	{
		return new Vector2(base.transform.position.x, base.transform.position.y);
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x00049622 File Offset: 0x00047822
	protected virtual void Update()
	{
		if (InputHandler.GetCurrentUsedInputScheme() == 1)
		{
			this.TestGamepadInput();
		}
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x00049634 File Offset: 0x00047834
	protected void TestGamepadInput()
	{
		Vector2 wheelNavigationVector = Singleton<UIInputHandler>.Instance.wheelNavigationVector;
		this.TestSelectSliceGamepad(wheelNavigationVector);
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x00049653 File Offset: 0x00047853
	protected virtual void TestSelectSliceGamepad(Vector2 gamepadVector)
	{
	}

	// Token: 0x04000CFD RID: 3325
	public float maxCursorDistance;
}
