using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200037D RID: 893
	public class InputAxisScrollbar : MonoBehaviour
	{
		// Token: 0x060016B2 RID: 5810 RVA: 0x00074B12 File Offset: 0x00072D12
		private void Update()
		{
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x00074B14 File Offset: 0x00072D14
		public void HandleInput(float value)
		{
			CrossPlatformInputManager.SetAxis(this.axis, value * 2f - 1f);
		}

		// Token: 0x04001569 RID: 5481
		public string axis;
	}
}
