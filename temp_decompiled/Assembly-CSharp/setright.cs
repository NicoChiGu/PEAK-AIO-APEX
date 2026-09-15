using System;
using UnityEngine;

// Token: 0x02000328 RID: 808
public class setright : MonoBehaviour
{
	// Token: 0x060014C4 RID: 5316 RVA: 0x00069A83 File Offset: 0x00067C83
	private void Start()
	{
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x00069A85 File Offset: 0x00067C85
	private void Update()
	{
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x00069A87 File Offset: 0x00067C87
	public void go()
	{
		base.transform.right = this.right;
		base.transform.up = this.up;
	}

	// Token: 0x0400133B RID: 4923
	public Vector3 right;

	// Token: 0x0400133C RID: 4924
	public Vector3 up;
}
