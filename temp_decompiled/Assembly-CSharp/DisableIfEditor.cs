using System;
using UnityEngine;

// Token: 0x0200009A RID: 154
public class DisableIfEditor : MonoBehaviour
{
	// Token: 0x060005E0 RID: 1504 RVA: 0x00021625 File Offset: 0x0001F825
	private void Start()
	{
		if (Application.isEditor)
		{
			base.gameObject.SetActive(false);
		}
	}
}
