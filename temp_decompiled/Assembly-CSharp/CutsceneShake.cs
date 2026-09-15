using System;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class CutsceneShake : MonoBehaviour
{
	// Token: 0x060010D9 RID: 4313 RVA: 0x00054D54 File Offset: 0x00052F54
	private void Update()
	{
		base.transform.position = Vector3.Lerp(base.transform.position, this.follow.position, this.smooth * Time.deltaTime);
		base.transform.Translate(Vector3.right * (Random.Range(-this.shake, this.shake) * Time.deltaTime));
		base.transform.Translate(Vector3.up * (Random.Range(-this.shake, this.shake) * Time.deltaTime));
		base.transform.Translate(Vector3.forward * (Random.Range(-this.shake, this.shake) * Time.deltaTime));
	}

	// Token: 0x04000F3A RID: 3898
	public Transform follow;

	// Token: 0x04000F3B RID: 3899
	public float smooth = 0.5f;

	// Token: 0x04000F3C RID: 3900
	public float shake;
}
