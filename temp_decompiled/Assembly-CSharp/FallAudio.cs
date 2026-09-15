using System;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class FallAudio : MonoBehaviour
{
	// Token: 0x06001135 RID: 4405 RVA: 0x000568A4 File Offset: 0x00054AA4
	private void Update()
	{
		this.yVel = base.transform.position.y - this.prevY;
		this.prevY = base.transform.position.y;
		this.au.volume = Mathf.Lerp(this.au.volume, Mathf.Abs(this.yVel) / 10f, Time.deltaTime * 10f);
		if (this.au.volume > 0.5f)
		{
			this.au.volume = 0.5f;
		}
	}

	// Token: 0x04000FAA RID: 4010
	public AudioSource au;

	// Token: 0x04000FAB RID: 4011
	private float yVel;

	// Token: 0x04000FAC RID: 4012
	private float prevY;
}
