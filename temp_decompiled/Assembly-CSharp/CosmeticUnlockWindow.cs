using System;
using UnityEngine.UI;

// Token: 0x020001D2 RID: 466
public class CosmeticUnlockWindow : MenuWindow
{
	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000E41 RID: 3649 RVA: 0x00046DD7 File Offset: 0x00044FD7
	public new virtual Selectable objectToSelectOnOpen
	{
		get
		{
			return this.continueButton;
		}
	}

	// Token: 0x04000C5E RID: 3166
	public Button continueButton;
}
