using System;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class PreventUnpause : MonoBehaviour
{
	// Token: 0x06000EB4 RID: 3764 RVA: 0x0004816B File Offset: 0x0004636B
	private void OnEnable()
	{
		PreventUnpause.UnpausePreventionActive = true;
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x00048173 File Offset: 0x00046373
	private void OnDisable()
	{
		PreventUnpause.UnpausePreventionActive = false;
	}

	// Token: 0x04000C9F RID: 3231
	public static bool UnpausePreventionActive;
}
