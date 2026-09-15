using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000369 RID: 873
public class VoiceObscuranceFilter : MonoBehaviour
{
	// Token: 0x06001638 RID: 5688 RVA: 0x00072F88 File Offset: 0x00071188
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		if (GameObject.Find("Airport"))
		{
			this.lowPass.enabled = false;
			this.echo.enabled = false;
			this.reverb.enabled = false;
		}
		if (!this.head)
		{
			this.head = MainCamera.instance.transform;
		}
		if (this.head)
		{
			this.reverbAddition = math.saturate(LightVolume.Instance().SamplePositionAlpha(base.transform.position));
			if (Physics.Linecast(base.transform.position, this.head.position, ref this.hit, this.layer))
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 1500f, 100f * Time.deltaTime);
			}
			else
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 7500f, 100f * Time.deltaTime);
			}
			if (Vector3.Distance(base.transform.position, this.head.position) > 60f)
			{
				if (this.anim != null)
				{
					this.anim.SetFloat("Obscurance", 1f, Time.deltaTime, 0.5f);
				}
				this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0.35f, 5f * Time.deltaTime);
				this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 0.5f, 5f * Time.deltaTime);
				this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0.3f, 5f * Time.deltaTime);
				this.echo.delay = Mathf.Lerp(this.echo.delay, 500f, 5f * Time.deltaTime);
				return;
			}
			if (this.anim != null)
			{
				this.anim.SetFloat("Obscurance", this.reverbAddition);
			}
			this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0f, 1f * Time.deltaTime);
			this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 1f, 1f * Time.deltaTime);
			this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0f, 1f * Time.deltaTime);
			this.echo.delay = Mathf.Lerp(this.echo.delay, 10f, 1f * Time.deltaTime);
		}
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x00073278 File Offset: 0x00071478
	private void Update()
	{
		if (!this.head)
		{
			this.head = MainCamera.instance.transform;
		}
		if (this.head)
		{
			this.reverbAddition = math.saturate(LightVolume.Instance().SamplePositionAlpha(base.transform.position));
			if (Physics.Linecast(base.transform.position, this.head.position, ref this.hit, this.layer))
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 1500f, 1f * Time.deltaTime);
			}
			else
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 7500f, 1f * Time.deltaTime);
			}
			if (Vector3.Distance(base.transform.position, this.head.position) > 60f)
			{
				if (this.anim != null)
				{
					this.anim.SetFloat("Obscurance", 1f, Time.deltaTime, 0.5f);
				}
				this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0.35f, 5f * Time.deltaTime);
				this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 0.5f, 5f * Time.deltaTime);
				this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0.3f, 5f * Time.deltaTime);
				this.echo.delay = Mathf.Lerp(this.echo.delay, 500f, 5f * Time.deltaTime);
				return;
			}
			if (this.anim != null)
			{
				this.anim.SetFloat("Obscurance", this.reverbAddition);
			}
			this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0f, 1f * Time.deltaTime);
			this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 1f, 1f * Time.deltaTime);
			this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0f, 1f * Time.deltaTime);
			this.echo.delay = Mathf.Lerp(this.echo.delay, 10f, 1f * Time.deltaTime);
		}
	}

	// Token: 0x04001523 RID: 5411
	public LayerMask layer;

	// Token: 0x04001524 RID: 5412
	private RaycastHit hit;

	// Token: 0x04001525 RID: 5413
	public Transform head;

	// Token: 0x04001526 RID: 5414
	public AudioLowPassFilter lowPass;

	// Token: 0x04001527 RID: 5415
	public AudioReverbFilter reverb;

	// Token: 0x04001528 RID: 5416
	public AudioEchoFilter echo;

	// Token: 0x04001529 RID: 5417
	public float reverbAddition;

	// Token: 0x0400152A RID: 5418
	private Animator anim;
}
