using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.UI;

// Token: 0x020001DE RID: 478
public class PauseMenuRebindKeyPage : UIPage, INavigationPage
{
	// Token: 0x06000EA4 RID: 3748 RVA: 0x00047C85 File Offset: 0x00045E85
	private void Awake()
	{
		this.action_pause = InputSystem.actions.FindAction("Pause", false);
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x00047CA0 File Offset: 0x00045EA0
	private void Update()
	{
		if (this.action_pause.WasPressedThisFrame() && this.rebindOperation != null && this.rebindOperation.started && !this.rebindOperation.completed)
		{
			Debug.Log("CANCEL REBINDING " + PauseMenuRebindKeyPage.inputAction.name);
			this.rebindOperation.Cancel();
			this.rebindOperation.Dispose();
		}
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x00047D0B File Offset: 0x00045F0B
	public GameObject GetFirstSelectedGameObject()
	{
		return this.dummyButton;
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x00047D14 File Offset: 0x00045F14
	public override void OnPageEnter()
	{
		base.OnPageEnter();
		if (PauseMenuRebindKeyPage.inputAction == null)
		{
			return;
		}
		if (PauseMenuRebindKeyPage.inputAction.name == "Pause")
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < PauseMenuRebindKeyPage.inputAction.bindings.Count; i++)
		{
			if (PauseMenuRebindKeyPage.inputAction.bindings[i].groups.Contains("Keyboard&Mouse") && PauseMenuRebindKeyPage.forcedInputScheme == null)
			{
				num = i;
				break;
			}
			if (PauseMenuRebindKeyPage.inputAction.bindings[i].groups.Contains("Gamepad") && PauseMenuRebindKeyPage.forcedInputScheme == 1)
			{
				num = i;
				break;
			}
		}
		PauseMenuRebindKeyPage.inputAction.Disable();
		this.rebindOperation = InputActionRebindingExtensions.PerformInteractiveRebinding(PauseMenuRebindKeyPage.inputAction, num).WithControlsExcluding("<Mouse>/position").WithControlsExcluding("<Mouse>/delta")
			.WithControlsExcluding("<Gamepad>/Start")
			.WithControlsExcluding("<Gamepad>/leftStick/left")
			.WithControlsExcluding("<Gamepad>/leftStick/right")
			.WithControlsExcluding("<Gamepad>/leftStick/up")
			.WithControlsExcluding("<Gamepad>/leftStick/down")
			.WithControlsExcluding("<Gamepad>/rightStick/left")
			.WithControlsExcluding("<Gamepad>/rightStick/right")
			.WithControlsExcluding("<Gamepad>/rightStick/up")
			.WithControlsExcluding("<Gamepad>/rightStick/down")
			.WithControlsExcluding("<Keyboard>/leftMeta")
			.WithControlsExcluding("<Keyboard>/rightMeta")
			.WithControlsExcluding("<Keyboard>/contextMenu")
			.WithControlsExcluding("<Keyboard>/anyKey")
			.WithCancelingThrough("<Keyboard>/escape")
			.WithCancelingThrough("<Gamepad>/Start")
			.WithControlsExcluding("<Keyboard>/escape")
			.OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				this.Completed();
			})
			.OnCancel(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				this.Cancelled();
			});
		if (PauseMenuRebindKeyPage.forcedInputScheme == 1)
		{
			this.rebindOperation = this.rebindOperation.WithControlsExcluding("<Keyboard>").WithControlsExcluding("<Mouse>");
		}
		else if (PauseMenuRebindKeyPage.forcedInputScheme == null)
		{
			this.rebindOperation = this.rebindOperation.WithControlsExcluding("<Gamepad>");
		}
		this.rebindOperation.Start();
		this.promptText.text = LocalizedText.GetText("PROMPT_REBIND", true).Replace("@", LocalizedText.GetText(PauseMenuRebindKeyPage.inputLocIndex, true));
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x00047F3C File Offset: 0x0004613C
	private void Completed()
	{
		Debug.Log(string.Format("FINISHED REBINDING {0} to {1}", PauseMenuRebindKeyPage.inputAction.name, this.rebindOperation.selectedControl));
		foreach (InputBinding inputBinding in PauseMenuRebindKeyPage.inputAction.bindings)
		{
			Debug.Log("Checking against " + inputBinding.path);
			if (InputSpriteData.GetPathEnd(inputBinding.path) == InputSpriteData.GetPathEnd(this.rebindOperation.selectedControl.path))
			{
				InputActionRebindingExtensions.RemoveAllBindingOverrides(this.rebindOperation.action);
			}
		}
		this.rebindOperation.Dispose();
		PauseMenuRebindKeyPage.inputAction.Enable();
		Rebinding.SaveRebindingsToFile(null);
		Action<InputScheme> inputSchemeChanged = RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged;
		if (inputSchemeChanged != null)
		{
			inputSchemeChanged.Invoke(InputHandler.GetCurrentUsedInputScheme());
		}
		base.StartCoroutine(this.ReturnRoutine());
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x00048044 File Offset: 0x00046244
	private void Cancelled()
	{
		this.rebindOperation.Dispose();
		PauseMenuRebindKeyPage.inputAction.Enable();
		base.StartCoroutine(this.ReturnRoutine());
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x00048068 File Offset: 0x00046268
	private IEnumerator ReturnRoutine()
	{
		yield return null;
		this.pageHandler.TransistionToPage<PauseMenuControlsPage>();
		yield break;
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x00048078 File Offset: 0x00046278
	public override void OnPageExit()
	{
		if (this.rebindOperation != null && this.rebindOperation.started && !this.rebindOperation.completed)
		{
			Debug.Log("CANCEL REBINDING " + PauseMenuRebindKeyPage.inputAction.name);
			this.rebindOperation.Cancel();
			this.rebindOperation.Dispose();
		}
	}

	// Token: 0x04000C96 RID: 3222
	private InputActionRebindingExtensions.RebindingOperation rebindOperation;

	// Token: 0x04000C97 RID: 3223
	public GameObject dummyButton;

	// Token: 0x04000C98 RID: 3224
	internal static InputAction inputAction;

	// Token: 0x04000C99 RID: 3225
	internal static string inputLocIndex;

	// Token: 0x04000C9A RID: 3226
	public TextMeshProUGUI promptText;

	// Token: 0x04000C9B RID: 3227
	internal static InputScheme forcedInputScheme;

	// Token: 0x04000C9C RID: 3228
	private InputAction action_pause;
}
