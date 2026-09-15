using System;
using UnityEngine;

// Token: 0x0200031D RID: 797
public class ScaleIn : MonoBehaviour
{
	// Token: 0x0600147C RID: 5244 RVA: 0x00068111 File Offset: 0x00066311
	private void Start()
	{
		this.targetScale = base.transform.localScale.x;
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x0600147D RID: 5245 RVA: 0x0006813C File Offset: 0x0006633C
	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(this.targetScale, this.targetScale, this.targetScale), Time.deltaTime * 5f);
		if (Mathf.Abs(this.targetScale - base.transform.localScale.x) < 0.05f)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x0400130E RID: 4878
	private float targetScale = 1f;
}
