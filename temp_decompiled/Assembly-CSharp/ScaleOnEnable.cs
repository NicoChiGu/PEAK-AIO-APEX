using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x0200031E RID: 798
public class ScaleOnEnable : MonoBehaviour
{
	// Token: 0x0600147F RID: 5247 RVA: 0x000681C4 File Offset: 0x000663C4
	private void OnEnable()
	{
		base.transform.localScale = Vector3.zero;
		TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(base.transform, Vector3.one, this.time), this.easeType);
		if (this.canvasGroup)
		{
			this.canvasGroup.alpha = 0f;
			TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTweenModuleUI.DOFade(this.canvasGroup, 1f, this.time), this.easeType);
		}
	}

	// Token: 0x0400130F RID: 4879
	public float time = 0.25f;

	// Token: 0x04001310 RID: 4880
	public Ease easeType = 30;

	// Token: 0x04001311 RID: 4881
	public CanvasGroup canvasGroup;
}
