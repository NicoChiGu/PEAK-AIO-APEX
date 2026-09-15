using System;
using UnityEngine;

// Token: 0x02000344 RID: 836
public class StormAudio : MonoBehaviour
{
	// Token: 0x06001576 RID: 5494 RVA: 0x0006E068 File Offset: 0x0006C268
	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("Storm");
		GameObject gameObject2 = GameObject.FindGameObjectWithTag("Rain");
		GameObject gameObject3 = GameObject.FindGameObjectWithTag("Wind");
		if (gameObject)
		{
			this.stormVisual = gameObject.GetComponent<StormVisual>();
		}
		if (gameObject2)
		{
			this.rainVisual = gameObject2.GetComponent<StormVisual>();
		}
		if (gameObject3)
		{
			this.windVisual = gameObject3.GetComponent<StormVisual>();
		}
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x0006E0D4 File Offset: 0x0006C2D4
	private void Update()
	{
		if (this.stormVisual)
		{
			this.StormPlay(this.stormVisual, this.loopStorm, this.lPStorm, this.stormVolume);
		}
		if (this.windVisual)
		{
			this.StormPlay(this.windVisual, this.loopWind, this.lPStorm, this.rainVolume);
		}
		this.RainPlay();
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x0006E140 File Offset: 0x0006C340
	private void RainPlay()
	{
		this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0f, Time.deltaTime * 0.25f);
		this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0f, Time.deltaTime * 0.05f);
		if (this.rainVisual)
		{
			if (!this.rainVisual.observedPlayerInWindZone)
			{
				this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0f, Time.deltaTime * 0.25f);
				this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0f, Time.deltaTime * 0.05f);
			}
			if (this.rainVisual.observedPlayerInWindZone && this.aM)
			{
				if (this.aM.obstruction < 0.6f)
				{
					this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.25f, Time.deltaTime * 2f);
					this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0.005f, Time.deltaTime * 2f);
				}
				if (this.aM.obstruction >= 0.6f)
				{
					this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.15f, Time.deltaTime * 2f);
					this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, this.rainVolume, Time.deltaTime * 2f);
				}
			}
		}
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x0006E308 File Offset: 0x0006C508
	private void StormPlay(StormVisual sV, AudioLoop aL, AudioLowPassFilter lFilter, float volume)
	{
		if (sV && aL && lFilter)
		{
			if (!sV.observedPlayerInWindZone)
			{
				aL.volume = Mathf.Lerp(aL.volume, 0f, Time.deltaTime * 0.25f);
				aL.pitch = Mathf.Lerp(aL.pitch, 0.25f, Time.deltaTime * 0.25f);
				lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 8000f, Time.deltaTime * 1f);
				return;
			}
			aL.pitch = Mathf.Lerp(aL.pitch, 1f, Time.deltaTime * 0.5f);
			if (this.aM.obstruction >= 0.6f)
			{
				lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 500f, Time.deltaTime * 0.25f);
				aL.volume = Mathf.Lerp(aL.volume, 0.05f, Time.deltaTime * 0.5f);
				return;
			}
			lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 8000f, Time.deltaTime * 1f);
			aL.volume = Mathf.Lerp(aL.volume, volume, Time.deltaTime * 0.5f);
		}
	}

	// Token: 0x0400143A RID: 5178
	public AmbienceAudio aM;

	// Token: 0x0400143B RID: 5179
	public AudioLoop loopStorm;

	// Token: 0x0400143C RID: 5180
	public AudioLoop loopWind;

	// Token: 0x0400143D RID: 5181
	public AudioLowPassFilter lPStorm;

	// Token: 0x0400143E RID: 5182
	public AudioLoop loopRainHeavy;

	// Token: 0x0400143F RID: 5183
	public AudioLoop loopRainSoft;

	// Token: 0x04001440 RID: 5184
	public StormVisual stormVisual;

	// Token: 0x04001441 RID: 5185
	public StormVisual rainVisual;

	// Token: 0x04001442 RID: 5186
	public StormVisual windVisual;

	// Token: 0x04001443 RID: 5187
	public float stormVolume = 0.25f;

	// Token: 0x04001444 RID: 5188
	public float windVolume = 0.35f;

	// Token: 0x04001445 RID: 5189
	public float rainVolume = 0.25f;
}
