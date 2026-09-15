using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class AddScreenshake : MonoBehaviour
{
	// Token: 0x06000F3A RID: 3898 RVA: 0x0004A833 File Offset: 0x00048A33
	private IEnumerator Start()
	{
		while (GamefeelHandler.instance == null)
		{
			yield return null;
		}
		while (GamefeelHandler.instance.setting == null)
		{
			yield return null;
		}
		if (!this.auto)
		{
			yield break;
		}
		this.Shake();
		yield break;
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0004A844 File Offset: 0x00048A44
	public void Shake()
	{
		if (this.positional)
		{
			GamefeelHandler.instance.AddPerlinShakeProximity(base.transform.position, this.amount, this.duration, this.scale, this.range);
			return;
		}
		GamefeelHandler.instance.AddPerlinShake(this.amount, this.duration, this.scale);
	}

	// Token: 0x04000D44 RID: 3396
	public float amount = 5f;

	// Token: 0x04000D45 RID: 3397
	public float duration = 0.3f;

	// Token: 0x04000D46 RID: 3398
	public float scale = 12f;

	// Token: 0x04000D47 RID: 3399
	public bool auto;

	// Token: 0x04000D48 RID: 3400
	public bool positional;

	// Token: 0x04000D49 RID: 3401
	public float range = 15f;
}
