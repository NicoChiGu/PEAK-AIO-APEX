using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class BingBongMouth : MonoBehaviour
{
	// Token: 0x060004BC RID: 1212 RVA: 0x0001C360 File Offset: 0x0001A560
	private void Start()
	{
		this.action.OnAsk += this.SampleAudioClip;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001C37C File Offset: 0x0001A57C
	public void SampleAudioClip(AudioClip clip)
	{
		float[] array = new float[clip.samples * clip.channels];
		clip.GetData(array, 0);
		List<float> list = new List<float>();
		int num = clip.frequency / 30;
		for (int i = 0; i < array.Length; i += num)
		{
			float num2 = 0f;
			int num3 = 0;
			while (num3 < num && i + num3 < array.Length)
			{
				num2 += Mathf.Abs(array[i + num3]);
				num3++;
			}
			list.Add(num2 / (float)num);
		}
		this.CreateCurveMap(list);
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001C408 File Offset: 0x0001A608
	public void CreateCurveMap(List<float> samples)
	{
		this.curveMap = new AnimationCurve();
		for (int i = 0; i < samples.Count; i++)
		{
			float num = (float)i / 30f;
			float num2 = samples[i];
			this.curveMap.AddKey(num, num2);
		}
		this.maxTime = (float)(samples.Count - 1) / 30f;
		this.canPlay = true;
		this.time = this.timeOffset;
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001C478 File Offset: 0x0001A678
	private void Update()
	{
		if (this.canPlay)
		{
			this.time += Time.deltaTime * this.timeMultiplier * this.audioSource.pitch;
			float num = this.curveMap.Evaluate(this.time) * this.maxMouthOpen;
			this.lerpedMouthVal = Mathf.Lerp(this.lerpedMouthVal, num, Time.deltaTime * 25f);
			this.animator.SetFloat(this.animValue, this.lerpedMouthVal);
			if (this.time > this.maxTime)
			{
				this.canPlay = false;
				this.time = this.timeOffset;
				this.lerpedMouthVal = 0f;
			}
		}
	}

	// Token: 0x04000526 RID: 1318
	public AudioSource audioSource;

	// Token: 0x04000527 RID: 1319
	public Animator animator;

	// Token: 0x04000528 RID: 1320
	public string animValue;

	// Token: 0x04000529 RID: 1321
	public AnimationCurve curveMap;

	// Token: 0x0400052A RID: 1322
	public float maxMouthOpen;

	// Token: 0x0400052B RID: 1323
	public bool isPlaying;

	// Token: 0x0400052C RID: 1324
	public bool canPlay;

	// Token: 0x0400052D RID: 1325
	public Action_AskBingBong action;

	// Token: 0x0400052E RID: 1326
	public float timeOffset = 0.25f;

	// Token: 0x0400052F RID: 1327
	private float maxTime;

	// Token: 0x04000530 RID: 1328
	private float time;

	// Token: 0x04000531 RID: 1329
	private float lerpedMouthVal;

	// Token: 0x04000532 RID: 1330
	public float timeMultiplier = 1.5f;
}
