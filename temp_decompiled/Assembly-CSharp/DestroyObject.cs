using System;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class DestroyObject : MonoBehaviour
{
	// Token: 0x06001103 RID: 4355 RVA: 0x00055DF6 File Offset: 0x00053FF6
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}
