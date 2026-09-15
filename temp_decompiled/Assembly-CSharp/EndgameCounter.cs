using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class EndgameCounter : MonoBehaviour
{
	// Token: 0x0600111E RID: 4382 RVA: 0x00056210 File Offset: 0x00054410
	public void UpdateCounter(int value)
	{
		this.counterGroup.gameObject.SetActive(true);
		DOTweenModuleUI.DOFade(this.counterGroup, 1f, 0.25f);
		this.counter.text = value.ToString() ?? "";
		this.counter.transform.localScale = Vector3.one * 2f;
		this.counter.alpha = 0f;
		TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensionsTMPText.DOScale(this.counter, 1f, 0.25f), 9);
		TweenSettingsExtensions.SetEase<TweenerCore<Color, Color, ColorOptions>>(ShortcutExtensionsTMPText.DOFade(this.counter, 1f, 0.25f), 9);
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x000562C8 File Offset: 0x000544C8
	public void Win()
	{
		this.winGroup.gameObject.SetActive(true);
		this.winGroup.alpha = 0f;
		DOTweenModuleUI.DOFade(this.winGroup, 1f, 1f);
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x00056301 File Offset: 0x00054501
	public void Lose()
	{
		this.loseGroup.gameObject.SetActive(true);
		this.loseGroup.alpha = 0f;
		DOTweenModuleUI.DOFade(this.loseGroup, 1f, 1f);
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x0005633A File Offset: 0x0005453A
	public void Disable()
	{
		this.counterGroup.gameObject.SetActive(false);
	}

	// Token: 0x04000F8B RID: 3979
	public CanvasGroup counterGroup;

	// Token: 0x04000F8C RID: 3980
	public CanvasGroup winGroup;

	// Token: 0x04000F8D RID: 3981
	public CanvasGroup loseGroup;

	// Token: 0x04000F8E RID: 3982
	public TextMeshProUGUI counter;
}
