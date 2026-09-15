using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001EE RID: 494
[DefaultExecutionOrder(-1000)]
public class UIInputHandler : Singleton<UIInputHandler>
{
	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000EFB RID: 3835 RVA: 0x00049768 File Offset: 0x00047968
	// (set) Token: 0x06000EFC RID: 3836 RVA: 0x00049770 File Offset: 0x00047970
	public Vector2 wheelNavigationVector { get; private set; }

	// Token: 0x06000EFD RID: 3837 RVA: 0x0004977C File Offset: 0x0004797C
	public void Initialize()
	{
		UIInputHandler.action_confirm = InputSystem.actions.FindAction("UIConfirm", false);
		UIInputHandler.action_cancel = InputSystem.actions.FindAction("UICancel", false);
		UIInputHandler.action_tabLeft = InputSystem.actions.FindAction("UITabLeft", false);
		UIInputHandler.action_tabRight = InputSystem.actions.FindAction("UITabRight", false);
		UIInputHandler.action_navigateWheel = InputSystem.actions.FindAction("NavigateWheel", false);
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnInputSchemeChanged));
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x00049818 File Offset: 0x00047A18
	public override void OnDestroy()
	{
		base.OnDestroy();
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnInputSchemeChanged));
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x00049846 File Offset: 0x00047A46
	private void Update()
	{
		this.Sample();
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x00049850 File Offset: 0x00047A50
	private void Sample()
	{
		this.confirmWasPressed = UIInputHandler.action_confirm.WasPressedThisFrame();
		this.cancelWasPressed = UIInputHandler.action_cancel.WasPressedThisFrame();
		this.tabLeftWasPressed = UIInputHandler.action_tabLeft.WasPressedThisFrame();
		this.tabRightWasPressed = UIInputHandler.action_tabRight.WasPressedThisFrame();
		this.wheelNavigationVector = UIInputHandler.action_navigateWheel.ReadValue<Vector2>();
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x000498AD File Offset: 0x00047AAD
	private void OnInputSchemeChanged(InputScheme scheme)
	{
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x000498AF File Offset: 0x00047AAF
	public static void SetSelectedObject(GameObject obj)
	{
		if (InputHandler.GetCurrentUsedInputScheme() == 1)
		{
			EventSystem.current.SetSelectedGameObject(obj);
		}
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x000498C4 File Offset: 0x00047AC4
	private void Deselect()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x000498D1 File Offset: 0x00047AD1
	private void SelectPrevious()
	{
		EventSystem.current.SetSelectedGameObject(UIInputHandler.previouslySelectedControllerElement);
	}

	// Token: 0x04000D07 RID: 3335
	public static InputAction action_confirm;

	// Token: 0x04000D08 RID: 3336
	public static InputAction action_cancel;

	// Token: 0x04000D09 RID: 3337
	public static InputAction action_tabLeft;

	// Token: 0x04000D0A RID: 3338
	public static InputAction action_tabRight;

	// Token: 0x04000D0B RID: 3339
	public static InputAction action_navigateWheel;

	// Token: 0x04000D0C RID: 3340
	public bool confirmWasPressed;

	// Token: 0x04000D0D RID: 3341
	public bool cancelWasPressed;

	// Token: 0x04000D0E RID: 3342
	public bool tabLeftWasPressed;

	// Token: 0x04000D0F RID: 3343
	public bool tabRightWasPressed;

	// Token: 0x04000D11 RID: 3345
	internal static GameObject previouslySelectedControllerElement;
}
