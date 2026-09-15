using System;
using UnityEngine;

// Token: 0x0200030E RID: 782
public class RemoveObject : MonoBehaviour
{
	// Token: 0x06001438 RID: 5176 RVA: 0x00066646 File Offset: 0x00064846
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}
