using System;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.UI.Modal;

// Token: 0x0200023C RID: 572
public class CursorHandler : Singleton<CursorHandler>
{
	// Token: 0x060010D3 RID: 4307 RVA: 0x00054B60 File Offset: 0x00052D60
	private void Update()
	{
		if (InputHandler.GetCurrentUsedInputScheme() != null || (!this.isMenuScene && !DebugUIHandler.IsOpen && (!(GUIManager.instance != null) || (!GUIManager.instance.windowShowingCursor && !GUIManager.instance.wheelActive)) && !Modal.IsOpen))
		{
			Cursor.lockState = 1;
			Cursor.visible = false;
			return;
		}
		Cursor.lockState = 0;
		Cursor.visible = true;
	}

	// Token: 0x04000F21 RID: 3873
	public bool isMenuScene;
}
