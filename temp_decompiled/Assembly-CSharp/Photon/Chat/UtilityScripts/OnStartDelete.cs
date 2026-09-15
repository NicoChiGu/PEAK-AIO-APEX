using System;
using UnityEngine;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020003A0 RID: 928
	public class OnStartDelete : MonoBehaviour
	{
		// Token: 0x06001824 RID: 6180 RVA: 0x0007A2E8 File Offset: 0x000784E8
		private void Start()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
