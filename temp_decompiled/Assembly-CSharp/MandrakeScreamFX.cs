using System;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class MandrakeScreamFX : MonoBehaviour
{
	// Token: 0x06000952 RID: 2386 RVA: 0x000316AE File Offset: 0x0002F8AE
	private void Awake()
	{
		this.tracker = base.GetComponent<TrackNetworkedObject>();
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x000316BC File Offset: 0x0002F8BC
	private void Update()
	{
		if (this.tracker.trackedObject)
		{
			if (this.mandrake == null)
			{
				this.mandrake = this.tracker.trackedObject.GetComponent<Mandrake>();
			}
			if (this.mandrake == null || this.mandrake.item.cooking.timesCookedLocal > 0)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			if (this.screaming)
			{
				if (!this.mandrake.screaming)
				{
					this.EndScreamFX();
					return;
				}
			}
			else if (this.mandrake.screaming)
			{
				this.StartScreamFX();
			}
		}
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x00031763 File Offset: 0x0002F963
	public void StartScreamFX()
	{
		this.screaming = true;
		this.vfx.Play();
		this.sfxScream.PlayFromSource(base.transform.position, this.source);
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x00031793 File Offset: 0x0002F993
	public void EndScreamFX()
	{
		this.screaming = false;
		this.vfx.Stop();
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x000317A7 File Offset: 0x0002F9A7
	private void OnDestroy()
	{
		if (this.handle != null)
		{
			this.source.Stop();
		}
	}

	// Token: 0x040008B0 RID: 2224
	public ParticleSystem vfx;

	// Token: 0x040008B1 RID: 2225
	public SFX_Instance sfxScream;

	// Token: 0x040008B2 RID: 2226
	public AudioSource source;

	// Token: 0x040008B3 RID: 2227
	private TrackNetworkedObject tracker;

	// Token: 0x040008B4 RID: 2228
	private Mandrake mandrake;

	// Token: 0x040008B5 RID: 2229
	private SFX_Player.SoundEffectHandle handle;

	// Token: 0x040008B6 RID: 2230
	private bool screaming;
}
