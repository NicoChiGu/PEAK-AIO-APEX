using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Settings;

// Token: 0x020001E6 RID: 486
public class SharedSettingsMenu : MonoBehaviour
{
	// Token: 0x06000ECF RID: 3791 RVA: 0x0004880B File Offset: 0x00046A0B
	private void OnEnable()
	{
		this.RefreshSettings();
		if (this.m_tabs.selectedButton != null)
		{
			this.m_tabs.Select(this.m_tabs.selectedButton);
		}
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x0004883C File Offset: 0x00046A3C
	private void RefreshSettings()
	{
		if (GameHandler.Instance != null)
		{
			this.settings = ISettingsHandlerExtensions.GetSettingsThatImplements<IExposedSetting>(GameHandler.Instance.SettingsHandler);
		}
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x00048860 File Offset: 0x00046A60
	public void ShowSettings(SettingsCategory category)
	{
		if (this.m_fadeInCoroutine != null)
		{
			base.StopCoroutine(this.m_fadeInCoroutine);
			this.m_fadeInCoroutine = null;
		}
		foreach (SettingsUICell settingsUICell in this.m_spawnedCells)
		{
			Object.Destroy(settingsUICell.gameObject);
		}
		this.m_spawnedCells.Clear();
		this.RefreshSettings();
		foreach (IExposedSetting exposedSetting in Enumerable.Where<IExposedSetting>(Enumerable.Where<IExposedSetting>(this.settings, (IExposedSetting setting) => setting.GetCategory() == category.ToString()), delegate(IExposedSetting setting)
		{
			IConditionalSetting conditionalSetting2 = setting as IConditionalSetting;
			return true;
		}))
		{
			SettingsUICell component = Object.Instantiate<GameObject>(this.m_settingsCellPrefab, this.m_settingsContentParent).GetComponent<SettingsUICell>();
			IConditionalSetting conditionalSetting = exposedSetting as IConditionalSetting;
			if (conditionalSetting != null && !conditionalSetting.ShouldShow())
			{
				component.ShouldntShow();
			}
			this.m_spawnedCells.Add(component);
			component.Setup<Setting>(exposedSetting as Setting);
		}
		this.m_fadeInCoroutine = base.StartCoroutine(this.FadeInCells());
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x000489B8 File Offset: 0x00046BB8
	private IEnumerator FadeInCells()
	{
		int i = 0;
		foreach (SettingsUICell settingsUICell in this.m_spawnedCells)
		{
			settingsUICell.FadeIn();
			yield return new WaitForSecondsRealtime(0.05f);
			int num = i;
			i = num + 1;
		}
		List<SettingsUICell>.Enumerator enumerator = default(List<SettingsUICell>.Enumerator);
		this.m_fadeInCoroutine = null;
		yield break;
		yield break;
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x000489C8 File Offset: 0x00046BC8
	public GameObject GetDefaultSelection()
	{
		foreach (SettingsUICell settingsUICell in this.m_spawnedCells)
		{
			if (settingsUICell.gameObject.activeSelf)
			{
				GameObject selectable = settingsUICell.GetSelectable();
				if (selectable)
				{
					return selectable;
				}
			}
		}
		return null;
	}

	// Token: 0x04000CBB RID: 3259
	[SerializeField]
	private SettingsTABS m_tabs;

	// Token: 0x04000CBC RID: 3260
	public GameObject m_settingsCellPrefab;

	// Token: 0x04000CBD RID: 3261
	public Transform m_settingsContentParent;

	// Token: 0x04000CBE RID: 3262
	private List<IExposedSetting> settings;

	// Token: 0x04000CBF RID: 3263
	private readonly List<SettingsUICell> m_spawnedCells = new List<SettingsUICell>();

	// Token: 0x04000CC0 RID: 3264
	private Coroutine m_fadeInCoroutine;
}
