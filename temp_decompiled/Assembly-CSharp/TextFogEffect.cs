using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public class TextFogEffect : MonoBehaviour
{
	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000EE0 RID: 3808 RVA: 0x0004928D File Offset: 0x0004748D
	public virtual float colorSpeedMult
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x00049294 File Offset: 0x00047494
	private void Awake()
	{
		this.m_TextComponent = base.GetComponent<TMP_Text>();
		this.DTanimator = new DOTweenTMPAnimator(this.m_TextComponent);
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x000492B3 File Offset: 0x000474B3
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x000492BB File Offset: 0x000474BB
	private void OnEnable()
	{
		base.StartCoroutine(this.TextEffectRoutine());
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x000492CA File Offset: 0x000474CA
	private IEnumerator TextEffectRoutine()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
		for (;;)
		{
			this.UpdateCharacter(Random.Range(0, characterCount));
			yield return new WaitForSeconds(this.period);
		}
		yield break;
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x000492D9 File Offset: 0x000474D9
	public virtual void Init()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x000492F8 File Offset: 0x000474F8
	private void TryDestroy()
	{
		this.destroyed = true;
		Object.Destroy(this);
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x00049307 File Offset: 0x00047507
	private void LateUpdate()
	{
		bool flag = this.destroyed;
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x00049310 File Offset: 0x00047510
	protected virtual void EffectRoutine()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x00049330 File Offset: 0x00047530
	public void UpdateCharacter(int index)
	{
		if (this.period == 0f)
		{
			return;
		}
		float num = this.offset * (float)index;
		float num2 = Mathf.Sin((Time.time + num) / this.period);
		float num3 = 1f + num2 * this.amplitude;
		if (this.roundSin)
		{
			num3 = Mathf.Round(num3 * this.chunkiness) / this.chunkiness;
		}
		num3 = this.amplitude;
		TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(this.DTanimator.DOOffsetChar(index, Random.insideUnitSphere * num3, this.shiftTime), 10);
		float num4 = (Mathf.Sin((Time.time + num) / (this.period / this.colorSpeedMult)) + 1f) * 0.5f;
		this.DTanimator.SetCharColor(index, this.colorGradient.Evaluate(num4));
	}

	// Token: 0x04000CE2 RID: 3298
	public bool abs;

	// Token: 0x04000CE3 RID: 3299
	public float amplitude = 0.2f;

	// Token: 0x04000CE4 RID: 3300
	public float period = 0.5f;

	// Token: 0x04000CE5 RID: 3301
	public float offset = 0.1f;

	// Token: 0x04000CE6 RID: 3302
	public Gradient colorGradient;

	// Token: 0x04000CE7 RID: 3303
	public bool skewXtop = true;

	// Token: 0x04000CE8 RID: 3304
	public float skewX;

	// Token: 0x04000CE9 RID: 3305
	public bool skewYtop = true;

	// Token: 0x04000CEA RID: 3306
	public float skewY;

	// Token: 0x04000CEB RID: 3307
	public bool roundSin;

	// Token: 0x04000CEC RID: 3308
	public float chunkiness = 1f;

	// Token: 0x04000CED RID: 3309
	public float updateChance = 0.1f;

	// Token: 0x04000CEE RID: 3310
	public float shiftTime = 0.5f;

	// Token: 0x04000CEF RID: 3311
	protected TMP_Text m_TextComponent;

	// Token: 0x04000CF0 RID: 3312
	protected TMP_TextInfo textInfo;

	// Token: 0x04000CF1 RID: 3313
	public DOTweenTMPAnimator DTanimator;

	// Token: 0x04000CF2 RID: 3314
	private bool destroyed;
}
