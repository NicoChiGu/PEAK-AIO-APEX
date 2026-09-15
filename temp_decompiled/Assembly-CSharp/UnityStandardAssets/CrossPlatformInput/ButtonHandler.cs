using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200037B RID: 891
	public class ButtonHandler : MonoBehaviour
	{
		// Token: 0x06001691 RID: 5777 RVA: 0x00074954 File Offset: 0x00072B54
		private void OnEnable()
		{
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x00074956 File Offset: 0x00072B56
		public void SetDownState()
		{
			CrossPlatformInputManager.SetButtonDown(this.Name);
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x00074963 File Offset: 0x00072B63
		public void SetUpState()
		{
			CrossPlatformInputManager.SetButtonUp(this.Name);
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x00074970 File Offset: 0x00072B70
		public void SetAxisPositiveState()
		{
			CrossPlatformInputManager.SetAxisPositive(this.Name);
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x0007497D File Offset: 0x00072B7D
		public void SetAxisNeutralState()
		{
			CrossPlatformInputManager.SetAxisZero(this.Name);
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x0007498A File Offset: 0x00072B8A
		public void SetAxisNegativeState()
		{
			CrossPlatformInputManager.SetAxisNegative(this.Name);
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x00074997 File Offset: 0x00072B97
		public void Update()
		{
		}

		// Token: 0x04001565 RID: 5477
		public string Name;
	}
}
