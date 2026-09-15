using System;
using UnityEngine;

// Token: 0x02000202 RID: 514
public class AntlionSandRumbleSFX : MonoBehaviour
{
	// Token: 0x06000F5D RID: 3933 RVA: 0x0004BEBD File Offset: 0x0004A0BD
	private void Start()
	{
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x0004BECC File Offset: 0x0004A0CC
	private void Update()
	{
		if (this.refObj)
		{
			if (this.refObj.active)
			{
				this.source.volume = Mathf.Lerp(this.source.volume, this.vol, 5f * Time.deltaTime);
				return;
			}
			this.source.volume = Mathf.Lerp(this.source.volume, 0f, 5f * Time.deltaTime);
		}
	}

	// Token: 0x04000DA2 RID: 3490
	public GameObject refObj;

	// Token: 0x04000DA3 RID: 3491
	public float vol = 0.3f;

	// Token: 0x04000DA4 RID: 3492
	private AudioSource source;
}
