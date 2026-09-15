using System;
using UnityEngine;

// Token: 0x020002B0 RID: 688
public class NoParent : MonoBehaviour
{
	// Token: 0x060012C2 RID: 4802 RVA: 0x0005FA50 File Offset: 0x0005DC50
	private void Start()
	{
		base.transform.parent = null;
	}
}
