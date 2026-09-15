using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000257 RID: 599
public class FadeIn : MonoBehaviour
{
	// Token: 0x0600112E RID: 4398 RVA: 0x000567D0 File Offset: 0x000549D0
	private void Awake()
	{
		Color color = this.fade.color;
		color.a = 1f;
		this.fade.color = color;
		TweenSettingsExtensions.OnComplete<TweenerCore<Color, Color, ColorOptions>>(DOTweenModuleUI.DOFade(this.fade, 0f, 2f), new TweenCallback(this.Disable));
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x00056828 File Offset: 0x00054A28
	private void Disable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000FA7 RID: 4007
	public Image fade;
}
