using System;
using UnityEngine;

// Token: 0x0200031B RID: 795
public class RotationTest : MonoBehaviour
{
	// Token: 0x06001476 RID: 5238 RVA: 0x00068027 File Offset: 0x00066227
	private void Update()
	{
		base.transform.Rotate(this.refVector.up, Time.deltaTime * 90f, 0);
	}

	// Token: 0x0400130D RID: 4877
	public Transform refVector;
}
