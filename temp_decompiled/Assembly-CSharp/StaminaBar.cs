using System;
using System.Collections;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001E7 RID: 487
public class StaminaBar : MonoBehaviour
{
	// Token: 0x06000ED5 RID: 3797 RVA: 0x00048A4C File Offset: 0x00046C4C
	private void Start()
	{
		this.afflictions = base.GetComponentsInChildren<BarAffliction>();
		this.TAU = 6.2831855f;
		BarAffliction[] array = this.afflictions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x00048A94 File Offset: 0x00046C94
	public void ChangeBar()
	{
		for (int i = 0; i < this.afflictions.Length; i++)
		{
			this.afflictions[i].ChangeAffliction(this);
		}
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x00048AC4 File Offset: 0x00046CC4
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		for (int i = 0; i < this.afflictions.Length; i++)
		{
			this.afflictions[i].UpdateAffliction(this);
		}
		this.desiredStaminaSize = Mathf.Max(0f, Character.observedCharacter.data.currentStamina * this.fullBar.sizeDelta.x + this.staminaBarOffset);
		if (Character.observedCharacter.data.currentStamina <= 0.005f)
		{
			if (!this.outOfStamina)
			{
				this.outOfStamina = true;
				this.OutOfStaminaPulse();
			}
		}
		else
		{
			this.outOfStamina = false;
		}
		this.staminaBar.sizeDelta = new Vector2(Mathf.Lerp(this.staminaBar.sizeDelta.x, this.desiredStaminaSize, Time.deltaTime * 10f), this.staminaBar.sizeDelta.y);
		Color color = this.staminaGlow.color;
		float num = Mathf.Clamp01((this.staminaBar.sizeDelta.x - this.desiredStaminaSize) * 0.5f);
		this.sinTime += Time.deltaTime * 10f * num;
		color.a = num * 0.4f - Mathf.Abs(Mathf.Sin(this.sinTime)) * 0.2f;
		this.staminaGlow.color = color;
		this.desiredMaxStaminaSize = Mathf.Max(0f, Character.observedCharacter.GetMaxStamina() * this.fullBar.sizeDelta.x + this.staminaBarOffset);
		this.maxStaminaBar.sizeDelta = new Vector2(Mathf.Lerp(this.maxStaminaBar.sizeDelta.x, this.desiredMaxStaminaSize, Time.deltaTime * 10f), this.maxStaminaBar.sizeDelta.y);
		float statusSum = Character.observedCharacter.refs.afflictions.statusSum;
		this.staminaBarOutline.sizeDelta = new Vector2(14f + Mathf.Max(1f, statusSum) * this.fullBar.sizeDelta.x, this.staminaBarOutline.sizeDelta.y);
		this.staminaBarOutlineOverflowBar.gameObject.SetActive((double)statusSum > 1.005);
		this.staminaBar.gameObject.SetActive(this.staminaBar.sizeDelta.x > this.minStaminaBarWidth);
		this.maxStaminaBar.gameObject.SetActive(this.maxStaminaBar.sizeDelta.x > this.minStaminaBarWidth);
		bool flag = Character.observedCharacter.data.extraStamina > 0f;
		if (!this.extraBar.gameObject.activeSelf && flag)
		{
			this.extraBar.sizeDelta = Vector2.zero;
			ShortcutExtensions.DOKill(this.extraBar, false);
			TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOSizeDelta(this.extraBar, new Vector2(45f, 45f), 0.25f, false), 9);
			this.extraBar.gameObject.SetActive(true);
			this.desiredExtraStaminaSize = Mathf.Max(0f, Character.observedCharacter.data.extraStamina * this.fullBar.sizeDelta.x);
			this.extraBarOutline.sizeDelta = new Vector2(Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), this.extraBarOutline.sizeDelta.y);
			this.extraBarStamina.sizeDelta = new Vector2(this.desiredExtraStaminaSize, this.extraBarStamina.sizeDelta.y);
			this.cachedExtraStam = this.desiredExtraStaminaSize;
		}
		if (this.extraBar.gameObject.activeSelf)
		{
			this.desiredExtraStaminaSize = Mathf.Max(0f, Character.observedCharacter.data.extraStamina * this.fullBar.sizeDelta.x);
			this.cachedExtraStam = Mathf.Lerp(this.cachedExtraStam, this.desiredExtraStaminaSize, Time.deltaTime * 10f);
			if (Mathf.Abs(this.desiredExtraStaminaSize - this.cachedExtraStam) < 0.05f)
			{
				this.extraBarOutline.sizeDelta = new Vector2(Mathf.Lerp(this.extraBarOutline.sizeDelta.x, Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), Time.deltaTime * 10f), this.extraBarOutline.sizeDelta.y);
			}
			else if (this.desiredExtraStaminaSize + 12f > this.extraBarOutline.sizeDelta.x)
			{
				this.extraBarOutline.sizeDelta = new Vector2(Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), this.extraBarOutline.sizeDelta.y);
			}
			Color color2 = this.extraStaminaGlow.color;
			float num2 = Mathf.Clamp01((this.extraBar.sizeDelta.x - this.desiredExtraStaminaSize) * 0.5f);
			this.sinTime += Time.deltaTime * 10f * num2;
			color2.a = num2 * 0.4f - Mathf.Abs(Mathf.Sin(this.sinTime)) * 0.2f;
			this.extraStaminaGlow.color = color2;
			this.extraBarStamina.sizeDelta = new Vector2(Mathf.Max(6f, this.cachedExtraStam), this.extraBarStamina.sizeDelta.y);
			if (!flag && !this.sequencingExtraBar)
			{
				this.sequencingExtraBar = true;
				Sequence sequence = DOTween.Sequence();
				TweenSettingsExtensions.Append(sequence, DOTweenModuleUI.DOSizeDelta(this.extraBar, new Vector2(this.extraBar.sizeDelta.x, 0f), 0.2f, false));
				TweenSettingsExtensions.OnComplete<Sequence>(sequence, new TweenCallback(this.DisableExtraBar));
			}
		}
		this.shield.gameObject.SetActive(Character.observedCharacter.data.isInvincible);
		if (this.sinTime > this.TAU)
		{
			this.sinTime -= this.TAU;
		}
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x00049104 File Offset: 0x00047304
	public void OutOfStaminaPulse()
	{
		this.backing.color = this.outOfStaminaBackingColor;
		DOTweenModuleUI.DOColor(this.backing, this.defaultBackingColor, 0.5f);
		this.noStaminaSFX.Play(default(Vector3));
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0004914D File Offset: 0x0004734D
	private void DisableExtraBar()
	{
		this.extraBar.gameObject.SetActive(false);
		this.sequencingExtraBar = false;
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x00049168 File Offset: 0x00047368
	public void AddRainbow()
	{
		if (this.rainbowRoutine != null)
		{
			base.StopCoroutine(this.rainbowRoutine);
		}
		this.rainbowStamina.enabled = true;
		this.rainbowStamina.color = (GUIManager.instance.photosensitivity ? new Color(0.5f, 0.5f, 0.5f, 0f) : new Color(1f, 1f, 1f, 0f));
		DOTweenModuleUI.DOFade(this.rainbowStamina, 1f, 0.5f);
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x000491F6 File Offset: 0x000473F6
	public void RemoveRainbow()
	{
		DOTweenModuleUI.DOFade(this.rainbowStamina, 0f, 0.5f);
		this.rainbowRoutine = base.StartCoroutine(this.<RemoveRainbow>g__RemoveRainbowRoutine|38_0());
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x00049220 File Offset: 0x00047420
	public void PlayMoraleBoost(int scoutCount)
	{
		this.moraleBoostText.enabled = true;
		this.moraleBoostText.text = LocalizedText.GetText("MORALEBOOST", true);
		base.StartCoroutine(this.MoraleBoostRoutine());
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x00049251 File Offset: 0x00047451
	private IEnumerator MoraleBoostRoutine()
	{
		if (this.animator == null)
		{
			this.animator = new DOTweenTMPAnimator(this.moraleBoostText);
		}
		this.animator.Refresh();
		this.moraleBoostAnimator.Play("Boost", 0, 0f);
		for (int j = 0; j < this.animator.textInfo.characterCount; j++)
		{
			this.animator.SetCharScale(j, Vector3.zero);
		}
		yield return null;
		int num;
		for (int i = 0; i < this.animator.textInfo.characterCount; i = num + 1)
		{
			TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(this.animator.DOScaleChar(i, Vector3.one, 0.2f), 27);
			yield return new WaitForSeconds(0.033f);
			num = i;
		}
		yield return new WaitForSeconds(2f);
		yield return new WaitForSeconds(0.5f);
		this.moraleBoostText.enabled = false;
		yield break;
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x0004927E File Offset: 0x0004747E
	[CompilerGenerated]
	private IEnumerator <RemoveRainbow>g__RemoveRainbowRoutine|38_0()
	{
		yield return new WaitForSeconds(0.5f);
		this.rainbowStamina.enabled = false;
		yield break;
	}

	// Token: 0x04000CC1 RID: 3265
	public Image backing;

	// Token: 0x04000CC2 RID: 3266
	public RectTransform fullBar;

	// Token: 0x04000CC3 RID: 3267
	public RectTransform staminaBar;

	// Token: 0x04000CC4 RID: 3268
	public Image staminaGlow;

	// Token: 0x04000CC5 RID: 3269
	public Image extraStaminaGlow;

	// Token: 0x04000CC6 RID: 3270
	public RectTransform maxStaminaBar;

	// Token: 0x04000CC7 RID: 3271
	public RectTransform staminaBarOutline;

	// Token: 0x04000CC8 RID: 3272
	public RectTransform staminaBarOutlineOverflowBar;

	// Token: 0x04000CC9 RID: 3273
	public RectTransform extraBar;

	// Token: 0x04000CCA RID: 3274
	public RectTransform extraBarStamina;

	// Token: 0x04000CCB RID: 3275
	public RectTransform extraBarOutline;

	// Token: 0x04000CCC RID: 3276
	public Image rainbowStamina;

	// Token: 0x04000CCD RID: 3277
	[HideInInspector]
	public BarAffliction[] afflictions;

	// Token: 0x04000CCE RID: 3278
	public float staminaBarOffset;

	// Token: 0x04000CCF RID: 3279
	private float desiredStaminaSize;

	// Token: 0x04000CD0 RID: 3280
	private float desiredMaxStaminaSize;

	// Token: 0x04000CD1 RID: 3281
	private float desiredExtraStaminaSize;

	// Token: 0x04000CD2 RID: 3282
	public float minAfflictionWidth = 60f;

	// Token: 0x04000CD3 RID: 3283
	public float minStaminaBarWidth = 20f;

	// Token: 0x04000CD4 RID: 3284
	public TextMeshProUGUI moraleBoostText;

	// Token: 0x04000CD5 RID: 3285
	public Animator moraleBoostAnimator;

	// Token: 0x04000CD6 RID: 3286
	public GameObject shield;

	// Token: 0x04000CD7 RID: 3287
	public Color defaultBackingColor;

	// Token: 0x04000CD8 RID: 3288
	public Color outOfStaminaBackingColor;

	// Token: 0x04000CD9 RID: 3289
	private float TAU;

	// Token: 0x04000CDA RID: 3290
	public SFX_Instance noStaminaSFX;

	// Token: 0x04000CDB RID: 3291
	private float cachedExtraStam;

	// Token: 0x04000CDC RID: 3292
	private float allAfflictionSizes;

	// Token: 0x04000CDD RID: 3293
	private bool outOfStamina;

	// Token: 0x04000CDE RID: 3294
	private float sinTime;

	// Token: 0x04000CDF RID: 3295
	private bool sequencingExtraBar;

	// Token: 0x04000CE0 RID: 3296
	private Coroutine rainbowRoutine;

	// Token: 0x04000CE1 RID: 3297
	private DOTweenTMPAnimator animator;
}
