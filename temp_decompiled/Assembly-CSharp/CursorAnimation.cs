using System;
using UnityEngine;

// Token: 0x0200023B RID: 571
public class CursorAnimation : MonoBehaviour
{
	// Token: 0x060010D0 RID: 4304 RVA: 0x00054AF8 File Offset: 0x00052CF8
	private void Start()
	{
		Cursor.SetCursor(this.cursorOpen, this.cursorHotspot, 0);
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x00054B0C File Offset: 0x00052D0C
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Cursor.SetCursor(this.curserClosed, this.cursorHotspot, 0);
			return;
		}
		if (Input.GetMouseButtonUp(0))
		{
			Cursor.SetCursor(this.cursorOpen, this.cursorHotspot, 0);
		}
	}

	// Token: 0x04000F1E RID: 3870
	public Texture2D cursorOpen;

	// Token: 0x04000F1F RID: 3871
	public Texture2D curserClosed;

	// Token: 0x04000F20 RID: 3872
	private Vector2 cursorHotspot = new Vector2(32f, 32f);
}
