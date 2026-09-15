using System;
using UnityEngine;

// Token: 0x02000317 RID: 791
public class RopeAudio : MonoBehaviour
{
	// Token: 0x0600146B RID: 5227 RVA: 0x00067BFA File Offset: 0x00065DFA
	private void Start()
	{
		this.prev = this.ropeSpool.segments;
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x00067C10 File Offset: 0x00065E10
	private void Update()
	{
		this.startT -= Time.deltaTime;
		this.prev = Mathf.Lerp(this.prev, this.ropeSpool.segments, Time.deltaTime * 20f);
		if (this.startT <= 0f)
		{
			this.loop1.volume = Mathf.Lerp(this.loop1.volume, Mathf.Abs(this.prev - this.ropeSpool.segments) / 6f, 20f * Time.deltaTime);
			this.loop1.pitch = Mathf.Lerp(this.loop1.pitch, 1f + Mathf.Abs(this.prev - this.ropeSpool.segments) / 2f, 20f * Time.deltaTime);
			this.loop2.volume = Mathf.Lerp(this.loop2.volume, Mathf.Abs(this.prev - this.ropeSpool.segments) / 3f, 10f * Time.deltaTime);
			this.loop2.pitch = Mathf.Lerp(this.loop2.pitch, 0.25f + Mathf.Abs(this.prev - this.ropeSpool.segments) / 2f, 10f * Time.deltaTime);
			if (this.loop1.volume > 0.075f)
			{
				this.loop1.volume = 0.075f;
			}
			if (this.loop2.volume > 0.075f)
			{
				this.loop2.volume = 0.075f;
			}
			if (!this.t && this.ropeSpool.segments == 40f)
			{
				for (int i = 0; i < this.min.Length; i++)
				{
					this.min[i].Play(base.transform.position);
				}
				this.t = true;
			}
			if (this.t && this.ropeSpool.segments == 3f)
			{
				for (int j = 0; j < this.max.Length; j++)
				{
					this.max[j].Play(base.transform.position);
				}
				this.t = false;
			}
		}
	}

	// Token: 0x040012FF RID: 4863
	public RopeSpool ropeSpool;

	// Token: 0x04001300 RID: 4864
	public AudioSource loop1;

	// Token: 0x04001301 RID: 4865
	public AudioSource loop2;

	// Token: 0x04001302 RID: 4866
	private float prev;

	// Token: 0x04001303 RID: 4867
	public SFX_Instance[] min;

	// Token: 0x04001304 RID: 4868
	public SFX_Instance[] max;

	// Token: 0x04001305 RID: 4869
	private bool t;

	// Token: 0x04001306 RID: 4870
	private float startT = 0.5f;
}
