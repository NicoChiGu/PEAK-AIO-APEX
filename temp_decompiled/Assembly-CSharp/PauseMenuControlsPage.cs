using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.UI;

// Token: 0x020001DA RID: 474
public class PauseMenuControlsPage : UIPage, IHaveParentPage, INavigationPage
{
	// Token: 0x06000E7D RID: 3709 RVA: 0x000474A3 File Offset: 0x000456A3
	private void Awake()
	{
		Rebinding.LoadRebindingsFromFile(null);
		this.restoreAllButton.onClick.AddListener(new UnityAction(this.OnResetClicked));
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x000474C7 File Offset: 0x000456C7
	private void OnResetClicked()
	{
		InputActionRebindingExtensions.RemoveAllBindingOverrides(InputSystem.actions);
		Action<InputScheme> inputSchemeChanged = RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged;
		if (inputSchemeChanged != null)
		{
			inputSchemeChanged.Invoke(InputHandler.GetCurrentUsedInputScheme());
		}
		Rebinding.SaveRebindingsToFile(null);
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x000474F3 File Offset: 0x000456F3
	private void OnEnable()
	{
		this.InitButtons();
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
		this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0004752C File Offset: 0x0004572C
	private void OnDisable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x00047554 File Offset: 0x00045754
	private void OnDeviceChange(InputScheme scheme)
	{
		if (scheme == null)
		{
			GameObject[] array = this.keyboardOnlyObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			array = this.controllerOnlyObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
		}
		else if (scheme == 1)
		{
			GameObject[] array = this.keyboardOnlyObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array = this.controllerOnlyObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
		}
		this.InitButtonBindingVisuals(scheme);
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x000475F9 File Offset: 0x000457F9
	private void InitButtons()
	{
		if (this.pageHandler == null)
		{
			this.pageHandler = base.GetComponentInParent<UIPageHandler>();
		}
		if (!this.initializedButtons)
		{
			this.controlsMenuButtons = this.controlsMenuButtonsParent.GetComponentsInChildren<PauseMenuRebindButton>(true);
			this.initializedButtons = true;
		}
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x00047638 File Offset: 0x00045838
	private void InitButtonBindingVisuals(InputScheme scheme)
	{
		for (int i = 0; i < this.controlsMenuButtons.Length; i++)
		{
			this.controlsMenuButtons[i].Init(this.pageHandler);
			this.controlsMenuButtons[i].UpdateBindingVisuals(this.controlsMenuButtons, scheme);
		}
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x0004767F File Offset: 0x0004587F
	private void Start()
	{
		this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x0004769D File Offset: 0x0004589D
	private void BackClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuMainPage>();
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x000476AB File Offset: 0x000458AB
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<PauseMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x000476C4 File Offset: 0x000458C4
	public GameObject GetFirstSelectedGameObject()
	{
		if (this.controlsMenuButtons.Length != 0)
		{
			for (int i = 0; i < this.controlsMenuButtons.Length; i++)
			{
				if (this.controlsMenuButtons[i].gameObject.activeInHierarchy)
				{
					return this.controlsMenuButtons[i].gameObject;
				}
			}
		}
		return this.backButton.gameObject;
	}

	// Token: 0x04000C78 RID: 3192
	public Button backButton;

	// Token: 0x04000C79 RID: 3193
	public Button restoreAllButton;

	// Token: 0x04000C7A RID: 3194
	private PauseMenuRebindButton[] controlsMenuButtons;

	// Token: 0x04000C7B RID: 3195
	public Transform controlsMenuButtonsParent;

	// Token: 0x04000C7C RID: 3196
	private bool initializedButtons;

	// Token: 0x04000C7D RID: 3197
	public GameObject[] keyboardOnlyObjects;

	// Token: 0x04000C7E RID: 3198
	public GameObject[] controllerOnlyObjects;
}
