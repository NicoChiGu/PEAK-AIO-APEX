using System;
using UnityEngine;

// Token: 0x0200026A RID: 618
[DefaultExecutionOrder(-9999)]
[SelectionBase]
public class HandVisual : MonoBehaviour
{
	// Token: 0x06001183 RID: 4483 RVA: 0x00058070 File Offset: 0x00056270
	private void Awake()
	{
		base.transform.GetChild(0).gameObject.SetActive(false);
	}
}
