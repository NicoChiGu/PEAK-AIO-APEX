using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000363 RID: 867
public class UI_UseItemProgress : MonoBehaviour
{
	// Token: 0x1700014B RID: 331
	// (get) Token: 0x0600161B RID: 5659 RVA: 0x00072253 File Offset: 0x00070453
	private bool constantUseInteractableExists
	{
		get
		{
			return Interaction.instance.currentHeldInteractible != null;
		}
	}

	// Token: 0x0600161C RID: 5660 RVA: 0x00072264 File Offset: 0x00070464
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		Character.localCharacter.data.currentItem != null;
		bool flag = this.UpdateFillAmount();
		if (!this.fill.enabled && flag)
		{
			ShortcutExtensions.DOKill(base.transform, false);
			base.transform.localScale = Vector3.zero;
			TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(base.transform, 1f, 0.25f), 27);
		}
		this.fill.enabled = flag;
		this.empty.enabled = this.fill.enabled;
	}

	// Token: 0x0600161D RID: 5661 RVA: 0x0007230C File Offset: 0x0007050C
	private bool UpdateFillAmount()
	{
		bool flag = Character.localCharacter.data.currentItem != null;
		if (Character.localCharacter.refs.items.climbingSpikeCastProgress > 0f)
		{
			this.fill.fillAmount = Character.localCharacter.refs.items.climbingSpikeCastProgress;
			return true;
		}
		if (flag && Character.localCharacter.data.currentItem.shouldShowCastProgress)
		{
			float progress = Character.localCharacter.data.currentItem.progress;
			if (progress > 0f)
			{
				this.fill.fillAmount = progress;
				return true;
			}
		}
		else if (this.constantUseInteractableExists && Interaction.instance.constantInteractableProgress > 0f)
		{
			this.fill.fillAmount = Interaction.instance.constantInteractableProgress;
			return true;
		}
		return false;
	}

	// Token: 0x04001505 RID: 5381
	public Image fill;

	// Token: 0x04001506 RID: 5382
	public Image empty;
}
