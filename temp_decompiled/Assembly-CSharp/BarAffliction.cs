using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BD RID: 445
public class BarAffliction : MonoBehaviour
{
	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000DBA RID: 3514 RVA: 0x00044CD1 File Offset: 0x00042ED1
	// (set) Token: 0x06000DBB RID: 3515 RVA: 0x00044CE3 File Offset: 0x00042EE3
	public float width
	{
		get
		{
			return this.rtf.sizeDelta.x;
		}
		set
		{
			this.rtf.sizeDelta = new Vector2(value, this.rtf.sizeDelta.y);
		}
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x00044D06 File Offset: 0x00042F06
	public void OnEnable()
	{
		this.icon.transform.localScale = Vector3.zero;
		TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(this.icon.transform, 1f, 0.5f), 24);
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x00044D40 File Offset: 0x00042F40
	public void ChangeAffliction(StaminaBar bar)
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		float currentStatus = Character.observedCharacter.refs.afflictions.GetCurrentStatus(this.afflictionType);
		this.size = bar.fullBar.sizeDelta.x * currentStatus;
		if (currentStatus > 0.01f)
		{
			if (this.size < bar.minAfflictionWidth)
			{
				this.size = bar.minAfflictionWidth;
			}
			base.gameObject.SetActive(true);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x00044DC9 File Offset: 0x00042FC9
	public void UpdateAffliction(StaminaBar bar)
	{
		this.width = Mathf.Lerp(this.width, this.size, Mathf.Min(Time.deltaTime * 10f, 0.1f));
	}

	// Token: 0x04000BD0 RID: 3024
	public RectTransform rtf;

	// Token: 0x04000BD1 RID: 3025
	public Image icon;

	// Token: 0x04000BD2 RID: 3026
	public float size;

	// Token: 0x04000BD3 RID: 3027
	public CharacterAfflictions.STATUSTYPE afflictionType;
}
