using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Settings;

// Token: 0x020001E5 RID: 485
public class SettingsUICell : MonoBehaviour
{
	// Token: 0x06000EC9 RID: 3785 RVA: 0x00048690 File Offset: 0x00046890
	public void Setup<T>(T setting) where T : Setting
	{
		this.m_canvasGroup = base.GetComponent<CanvasGroup>();
		this.m_canvasGroup.alpha = 0f;
		IExposedSetting exposedSetting = setting as IExposedSetting;
		if (exposedSetting != null)
		{
			this.localizedText.SetIndex(exposedSetting.GetDisplayName());
		}
		SettingInputUICell component = Object.Instantiate<GameObject>(setting.GetSettingUICell(), this.m_settingsContentParent).GetComponent<SettingInputUICell>();
		component.disable = this.disable;
		if (this.disable)
		{
			this.onlyOnMainMenu.SetActive(true);
		}
		component.Setup(setting, GameHandler.Instance.SettingsHandler);
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x0004872C File Offset: 0x0004692C
	public void FadeIn()
	{
		this.m_fadeIn = true;
		if (this.fadeInSFX)
		{
			this.fadeInSFX.Play(default(Vector3));
		}
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x00048761 File Offset: 0x00046961
	private void Update()
	{
		if (this.m_fadeIn)
		{
			this.m_canvasGroup.alpha = Mathf.Lerp(this.m_canvasGroup.alpha, 1f, Time.unscaledDeltaTime * 10f);
		}
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x00048798 File Offset: 0x00046998
	public GameObject GetSelectable()
	{
		Button componentInChildren = this.m_settingsContentParent.GetComponentInChildren<Button>();
		if (componentInChildren != null)
		{
			return componentInChildren.gameObject;
		}
		Slider componentInChildren2 = this.m_settingsContentParent.GetComponentInChildren<Slider>();
		if (componentInChildren2 != null)
		{
			return componentInChildren2.gameObject;
		}
		TMP_Dropdown componentInChildren3 = this.m_settingsContentParent.GetComponentInChildren<TMP_Dropdown>();
		if (componentInChildren3 != null)
		{
			return componentInChildren3.gameObject;
		}
		return null;
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x000487FA File Offset: 0x000469FA
	public void ShouldntShow()
	{
		this.disable = true;
	}

	// Token: 0x04000CB3 RID: 3251
	public Transform m_settingsContentParent;

	// Token: 0x04000CB4 RID: 3252
	public TextMeshProUGUI m_text;

	// Token: 0x04000CB5 RID: 3253
	public LocalizedText localizedText;

	// Token: 0x04000CB6 RID: 3254
	public GameObject onlyOnMainMenu;

	// Token: 0x04000CB7 RID: 3255
	private bool m_fadeIn;

	// Token: 0x04000CB8 RID: 3256
	private CanvasGroup m_canvasGroup;

	// Token: 0x04000CB9 RID: 3257
	public SFX_Instance fadeInSFX;

	// Token: 0x04000CBA RID: 3258
	private bool disable;
}
