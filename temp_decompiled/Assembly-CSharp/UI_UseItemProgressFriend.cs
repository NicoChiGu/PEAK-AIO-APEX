using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001ED RID: 493
public class UI_UseItemProgressFriend : MonoBehaviour
{
	// Token: 0x06000EF7 RID: 3831 RVA: 0x00049698 File Offset: 0x00047898
	public void Init(FeedData feedData)
	{
		this.giverID = feedData.giverID;
		this._maxTime = feedData.totalItemTime;
		Item item;
		if (ItemDatabase.TryGetItem(feedData.itemID, out item))
		{
			this.icon.texture = item.UIData.GetIcon();
		}
		Vector2 sizeDelta = this.rect.sizeDelta;
		this.rect.sizeDelta = Vector2.zero;
		TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOSizeDelta(this.rect, sizeDelta, 0.5f, false), 27);
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x00049718 File Offset: 0x00047918
	private void Update()
	{
		if (!this._dead)
		{
			this._currentTime += Time.deltaTime;
			this.fill.fillAmount = this._currentTime / this._maxTime;
		}
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0004974C File Offset: 0x0004794C
	public void Kill()
	{
		this._dead = true;
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04000D00 RID: 3328
	public RectTransform rect;

	// Token: 0x04000D01 RID: 3329
	public Image fill;

	// Token: 0x04000D02 RID: 3330
	public RawImage icon;

	// Token: 0x04000D03 RID: 3331
	public int giverID;

	// Token: 0x04000D04 RID: 3332
	private float _maxTime;

	// Token: 0x04000D05 RID: 3333
	private float _currentTime;

	// Token: 0x04000D06 RID: 3334
	private bool _dead;
}
