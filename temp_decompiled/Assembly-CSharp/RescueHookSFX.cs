using System;
using UnityEngine;

// Token: 0x02000311 RID: 785
public class RescueHookSFX : MonoBehaviour
{
	// Token: 0x0600144D RID: 5197 RVA: 0x000674A0 File Offset: 0x000656A0
	private void Update()
	{
		if (this.hookPoint.localPosition.x != 0f)
		{
			this.t = true;
		}
		if (this.t && this.hookPoint.localPosition.x == 0f)
		{
			this.t = false;
			for (int i = 0; i < this.reAttatch.Length; i++)
			{
				this.reAttatch[i].Play(base.transform.position);
			}
		}
		if (this.lR.enabled)
		{
			this.loop.volume = 0.45f;
			this.loop.pitch += Time.deltaTime / 2f;
			if (this.loop.pitch > 2f)
			{
				this.loop.pitch = 2f;
				return;
			}
		}
		else
		{
			this.loop.volume = 0f;
			this.loop.pitch = 1f;
		}
	}

	// Token: 0x040012E8 RID: 4840
	public LineRenderer lR;

	// Token: 0x040012E9 RID: 4841
	public Transform hookPoint;

	// Token: 0x040012EA RID: 4842
	public SFX_Instance[] reAttatch;

	// Token: 0x040012EB RID: 4843
	public AudioSource loop;

	// Token: 0x040012EC RID: 4844
	public bool t;
}
