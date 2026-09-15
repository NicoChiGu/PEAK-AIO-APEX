using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.UI;

// Token: 0x020001DD RID: 477
public class PauseMenuRebindButton : MonoBehaviour
{
	// Token: 0x06000E9D RID: 3741 RVA: 0x00047AE8 File Offset: 0x00045CE8
	private void Awake()
	{
		this.inputAction = InputSystem.actions.FindAction(this.inputActionName, false);
		this.rebindButton.onClick.AddListener(new UnityAction(this.OnRebindClicked));
		this.resetButton.onClick.AddListener(new UnityAction(this.OnResetClicked));
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x00047B44 File Offset: 0x00045D44
	internal void UpdateBindingVisuals(PauseMenuRebindButton[] allButtons, InputScheme scheme)
	{
		bool flag;
		this.currentBindingPath = InputSpriteData.GetBindingPath(this.inputActionName, scheme, out flag);
		bool flag2 = false;
		foreach (PauseMenuRebindButton pauseMenuRebindButton in allButtons)
		{
			bool flag3;
			if (!(pauseMenuRebindButton == this) && pauseMenuRebindButton.gameObject.activeInHierarchy && InputSpriteData.GetBindingPath(pauseMenuRebindButton.inputActionName, scheme, out flag3) == this.currentBindingPath)
			{
				flag2 = true;
			}
		}
		this.warning.SetActive(flag2);
		if (flag)
		{
			this.inputDescriptionText.tmp.color = this.overriddenTextColor;
			return;
		}
		this.inputDescriptionText.tmp.color = this.defaultTextColor;
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x00047BEE File Offset: 0x00045DEE
	public void Init(UIPageHandler pageHandler)
	{
		this.pageHandler = pageHandler;
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x00047BF7 File Offset: 0x00045DF7
	private void OnRebindClicked()
	{
		PauseMenuRebindKeyPage.inputAction = this.inputAction;
		PauseMenuRebindKeyPage.inputLocIndex = this.inputDescriptionText.index;
		PauseMenuRebindKeyPage.forcedInputScheme = InputHandler.GetCurrentUsedInputScheme();
		this.pageHandler.TransistionToPage<PauseMenuRebindKeyPage>();
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x00047C2A File Offset: 0x00045E2A
	private void OnResetClicked()
	{
		InputActionRebindingExtensions.RemoveAllBindingOverrides(this.inputAction);
		Rebinding.SaveRebindingsToFile(null);
		Action<InputScheme> inputSchemeChanged = RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged;
		if (inputSchemeChanged == null)
		{
			return;
		}
		inputSchemeChanged.Invoke(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x00047C56 File Offset: 0x00045E56
	public InputAction GetInputAction()
	{
		if (this.inputAction == null)
		{
			this.inputAction = InputSystem.actions.FindAction(this.inputActionName, false);
		}
		return this.inputAction;
	}

	// Token: 0x04000C8A RID: 3210
	private InputAction inputAction;

	// Token: 0x04000C8B RID: 3211
	public string inputActionName;

	// Token: 0x04000C8C RID: 3212
	public LocalizedText inputDescriptionText;

	// Token: 0x04000C8D RID: 3213
	public string currentBindingPath;

	// Token: 0x04000C8E RID: 3214
	public Button rebindButton;

	// Token: 0x04000C8F RID: 3215
	public Button resetButton;

	// Token: 0x04000C90 RID: 3216
	[SerializeField]
	private UIPageHandler pageHandler;

	// Token: 0x04000C91 RID: 3217
	public Color defaultTextColor;

	// Token: 0x04000C92 RID: 3218
	public Color overriddenTextColor;

	// Token: 0x04000C93 RID: 3219
	public GameObject warning;

	// Token: 0x04000C94 RID: 3220
	public bool allowAxisBinding;

	// Token: 0x04000C95 RID: 3221
	private bool initialized;
}
