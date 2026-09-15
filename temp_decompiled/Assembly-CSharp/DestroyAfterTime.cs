using System;
using UnityEngine;

// Token: 0x02000246 RID: 582
public class DestroyAfterTime : MonoBehaviour
{
	// Token: 0x060010FF RID: 4351 RVA: 0x00055DB9 File Offset: 0x00053FB9
	private void Start()
	{
		Object.Destroy(base.gameObject, this.time);
	}

	// Token: 0x04000F74 RID: 3956
	public float time = 3f;
}
