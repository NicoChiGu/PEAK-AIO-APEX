using System;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003B7 RID: 951
	public class DisableWhenUsingSteam : MonoBehaviour
	{
		// Token: 0x06001872 RID: 6258 RVA: 0x0007C3CD File Offset: 0x0007A5CD
		private void Start()
		{
			Debug.Log("Disabling self because Steam is on.");
			base.gameObject.SetActive(false);
		}
	}
}
