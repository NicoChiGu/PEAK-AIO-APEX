using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.ControllerSupport;

// Token: 0x02000012 RID: 18
public class CharacterInput : MonoBehaviour
{
	// Token: 0x06000187 RID: 391 RVA: 0x0000AF54 File Offset: 0x00009154
	public void Init()
	{
		Rebinding.LoadRebindingsFromFile(null);
		CharacterInput.action_pause = InputSystem.actions.FindAction("Pause", false);
		CharacterInput.action_move = InputSystem.actions.FindAction("Move", false);
		CharacterInput.action_moveForward = InputSystem.actions.FindAction("MoveForward", false);
		CharacterInput.action_moveBackward = InputSystem.actions.FindAction("MoveBackward", false);
		CharacterInput.action_moveLeft = InputSystem.actions.FindAction("MoveLeft", false);
		CharacterInput.action_moveRight = InputSystem.actions.FindAction("MoveRight", false);
		CharacterInput.action_look = InputSystem.actions.FindAction("Look", false);
		CharacterInput.action_jump = InputSystem.actions.FindAction("Jump", false);
		CharacterInput.action_sprint = InputSystem.actions.FindAction("Sprint", false);
		CharacterInput.action_sprintToggle = InputSystem.actions.FindAction("SprintToggle", false);
		CharacterInput.action_interact = InputSystem.actions.FindAction("Interact", false);
		CharacterInput.action_drop = InputSystem.actions.FindAction("Drop", false);
		CharacterInput.action_crouch = InputSystem.actions.FindAction("Crouch", false);
		CharacterInput.action_crouchToggle = InputSystem.actions.FindAction("CrouchToggle", false);
		CharacterInput.action_usePrimary = InputSystem.actions.FindAction("UsePrimary", false);
		CharacterInput.action_useSecondary = InputSystem.actions.FindAction("UseSecondary", false);
		CharacterInput.action_scroll = InputSystem.actions.FindAction("Scroll", false);
		CharacterInput.push_to_talk = InputSystem.actions.FindAction("PushToTalk", false);
		CharacterInput.action_emote = InputSystem.actions.FindAction("Emote", false);
		CharacterInput.action_ping = InputSystem.actions.FindAction("Ping", false);
		for (int i = 0; i < CharacterInput.hotbarActions.Length; i++)
		{
			CharacterInput.hotbarActions[i] = InputSystem.actions.FindAction(string.Format("Hotbar{0}", i + 1), false);
		}
		CharacterInput.action_selectSlotForward = InputSystem.actions.FindAction("SelectSlotForward", false);
		CharacterInput.action_selectSlotBackward = InputSystem.actions.FindAction("SelectSlotBackward", false);
		CharacterInput.action_unselectSlot = InputSystem.actions.FindAction("UnselectSlot", false);
		CharacterInput.action_selectBackpack = InputSystem.actions.FindAction("SelectBackpack", false);
		CharacterInput.action_scrollBackward = InputSystem.actions.FindAction("ScrollBackward", false);
		CharacterInput.action_scrollForward = InputSystem.actions.FindAction("ScrollForward", false);
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0000B1BF File Offset: 0x000093BF
	public bool SelectSlotWasPressed(int key)
	{
		return this.HotbarKeyWasPressed(key);
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0000B1C8 File Offset: 0x000093C8
	public bool SelectSlotIsPressed(int key)
	{
		return this.HotbarKeyIsPressed(key);
	}

	// Token: 0x0600018A RID: 394 RVA: 0x0000B1D1 File Offset: 0x000093D1
	public bool HotbarKeyWasPressed(int key)
	{
		return !this.itemSwitchBlocked && key >= 0 && key < CharacterInput.hotbarActions.Length && CharacterInput.hotbarActions[key].WasPressedThisFrame();
	}

	// Token: 0x0600018B RID: 395 RVA: 0x0000B1F9 File Offset: 0x000093F9
	public bool HotbarKeyIsPressed(int key)
	{
		return !this.itemSwitchBlocked && key >= 0 && key < CharacterInput.hotbarActions.Length && CharacterInput.hotbarActions[key].IsPressed();
	}

	// Token: 0x0600018C RID: 396 RVA: 0x0000B224 File Offset: 0x00009424
	internal void Sample(bool playerMovementActive)
	{
		this.ResetInput();
		this.pauseWasPressed = CharacterInput.action_pause.WasPressedThisFrame();
		this.interactWasPressed = CharacterInput.action_interact.WasPressedThisFrame();
		this.interactIsPressed = CharacterInput.action_interact.IsPressed();
		this.interactWasReleased = CharacterInput.action_interact.WasReleasedThisFrame();
		this.emoteIsPressed = CharacterInput.action_emote.IsPressed();
		if (playerMovementActive)
		{
			this.movementInput = Vector2.ClampMagnitude(this.GetMovementInput(), 1f);
			this.sprintWasPressed = CharacterInput.action_sprint.WasPressedThisFrame();
			this.sprintIsPressed = CharacterInput.action_sprint.IsPressed();
			this.sprintToggleIsPressed = CharacterInput.action_sprintToggle.IsPressed();
			this.sprintToggleWasPressed = CharacterInput.action_sprintToggle.WasPressedThisFrame();
			this.jumpWasPressed = CharacterInput.action_jump.WasPressedThisFrame();
			this.jumpIsPressed = CharacterInput.action_jump.IsPressed();
			this.dropWasPressed = CharacterInput.action_drop.WasPressedThisFrame();
			this.dropIsPressed = CharacterInput.action_drop.IsPressed();
			this.dropWasReleased = CharacterInput.action_drop.WasReleasedThisFrame();
			this.lookInput = CharacterInput.action_look.ReadValue<Vector2>();
			this.scrollBackwardWasPressed = CharacterInput.action_scrollBackward.WasPressedThisFrame();
			this.scrollForwardWasPressed = CharacterInput.action_scrollForward.WasPressedThisFrame();
			this.scrollBackwardIsPressed = CharacterInput.action_scrollBackward.IsPressed();
			this.scrollForwardIsPressed = CharacterInput.action_scrollForward.IsPressed();
			this.scrollInput = CharacterInput.action_scroll.ReadValue<float>();
			this.usePrimaryWasPressed = CharacterInput.action_usePrimary.WasPressedThisFrame();
			this.usePrimaryIsPressed = CharacterInput.action_usePrimary.IsPressed();
			this.usePrimaryWasReleased = CharacterInput.action_usePrimary.WasReleasedThisFrame();
			this.useSecondaryWasPressed = CharacterInput.action_useSecondary.WasPressedThisFrame();
			this.useSecondaryIsPressed = CharacterInput.action_useSecondary.IsPressed();
			this.useSecondaryWasReleased = CharacterInput.action_useSecondary.WasReleasedThisFrame();
			this.crouchWasPressed = CharacterInput.action_crouch.WasPressedThisFrame();
			this.crouchIsPressed = CharacterInput.action_crouch.IsPressed();
			this.crouchToggleWasPressed = CharacterInput.action_crouchToggle.WasPressedThisFrame();
			if (InputHandler.GetCurrentUsedInputScheme() == null)
			{
				this.spectateLeftWasPressed = this.HotbarKeyWasPressed(0);
				this.spectateRightWasPressed = this.HotbarKeyWasPressed(1);
			}
			else
			{
				this.spectateLeftWasPressed = CharacterInput.action_selectSlotBackward.WasPressedThisFrame();
				this.spectateRightWasPressed = CharacterInput.action_selectSlotForward.WasPressedThisFrame();
			}
			this.selectBackpackWasPressed = CharacterInput.action_selectBackpack.WasPerformedThisFrame();
			this.pingWasPressed = CharacterInput.action_ping.WasPressedThisFrame();
			this.pushToTalkPressed = CharacterInput.push_to_talk.IsPressed();
			this.unselectSlotWasPressed = CharacterInput.action_unselectSlot.WasPressedThisFrame() && !this.itemSwitchBlocked;
		}
		this.selectSlotForwardWasPressed = CharacterInput.action_selectSlotForward.WasPressedThisFrame() && !this.itemSwitchBlocked;
		this.selectSlotBackwardWasPressed = CharacterInput.action_selectSlotBackward.WasPressedThisFrame() && !this.itemSwitchBlocked;
		this.unselectSlotWasPressed = CharacterInput.action_unselectSlot.WasPressedThisFrame() && !this.itemSwitchBlocked;
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0000B510 File Offset: 0x00009710
	private Vector2 GetMovementInput()
	{
		if (InputHandler.GetCurrentUsedInputScheme() == 1)
		{
			return CharacterInput.action_move.ReadValue<Vector2>();
		}
		Vector2 vector = Vector2.zero;
		if (CharacterInput.action_moveForward.IsPressed())
		{
			vector += Vector2.up;
		}
		if (CharacterInput.action_moveBackward.IsPressed())
		{
			vector -= Vector2.up;
		}
		if (CharacterInput.action_moveRight.IsPressed())
		{
			vector += Vector2.right;
		}
		if (CharacterInput.action_moveLeft.IsPressed())
		{
			vector -= Vector2.right;
		}
		return vector;
	}

	// Token: 0x0600018E RID: 398 RVA: 0x0000B597 File Offset: 0x00009797
	internal void SampleAlways()
	{
	}

	// Token: 0x0600018F RID: 399 RVA: 0x0000B59C File Offset: 0x0000979C
	internal void ResetInput()
	{
		this.lookInput = Vector2.zero;
		this.movementInput = Vector2.zero;
		this.sprintIsPressed = false;
		this.jumpWasPressed = false;
		this.jumpIsPressed = false;
		this.useSecondaryIsPressed = false;
		this.useSecondaryWasPressed = false;
		this.useSecondaryWasReleased = false;
		this.usePrimaryWasPressed = false;
		this.usePrimaryIsPressed = false;
		this.usePrimaryWasReleased = false;
		this.interactWasPressed = false;
		this.interactIsPressed = false;
		this.interactWasReleased = false;
		this.dropWasPressed = false;
		this.dropIsPressed = false;
		this.dropWasReleased = false;
		this.sprintWasPressed = false;
		this.sprintToggleWasPressed = false;
		this.crouchWasPressed = false;
		this.crouchToggleWasPressed = false;
		this.crouchIsPressed = false;
		this.emoteIsPressed = false;
	}

	// Token: 0x04000152 RID: 338
	public InputActionAsset actions;

	// Token: 0x04000153 RID: 339
	public static InputAction action_move;

	// Token: 0x04000154 RID: 340
	public static InputAction action_moveForward;

	// Token: 0x04000155 RID: 341
	public static InputAction action_moveBackward;

	// Token: 0x04000156 RID: 342
	public static InputAction action_moveLeft;

	// Token: 0x04000157 RID: 343
	public static InputAction action_moveRight;

	// Token: 0x04000158 RID: 344
	public static InputAction action_look;

	// Token: 0x04000159 RID: 345
	public static InputAction action_jump;

	// Token: 0x0400015A RID: 346
	public static InputAction action_sprint;

	// Token: 0x0400015B RID: 347
	public static InputAction action_sprintToggle;

	// Token: 0x0400015C RID: 348
	public static InputAction action_interact;

	// Token: 0x0400015D RID: 349
	public static InputAction action_drop;

	// Token: 0x0400015E RID: 350
	public static InputAction action_crouch;

	// Token: 0x0400015F RID: 351
	public static InputAction action_crouchToggle;

	// Token: 0x04000160 RID: 352
	public static InputAction action_usePrimary;

	// Token: 0x04000161 RID: 353
	public static InputAction action_useSecondary;

	// Token: 0x04000162 RID: 354
	public static InputAction action_scroll;

	// Token: 0x04000163 RID: 355
	public static InputAction action_emote;

	// Token: 0x04000164 RID: 356
	public static InputAction action_ping;

	// Token: 0x04000165 RID: 357
	public static InputAction action_pause;

	// Token: 0x04000166 RID: 358
	public static InputAction action_scrollBackward;

	// Token: 0x04000167 RID: 359
	public static InputAction action_scrollForward;

	// Token: 0x04000168 RID: 360
	public static InputAction action_selectSlotForward;

	// Token: 0x04000169 RID: 361
	public static InputAction action_selectSlotBackward;

	// Token: 0x0400016A RID: 362
	public static InputAction action_unselectSlot;

	// Token: 0x0400016B RID: 363
	public static InputAction action_selectBackpack;

	// Token: 0x0400016C RID: 364
	public static InputAction[] hotbarActions = new InputAction[9];

	// Token: 0x0400016D RID: 365
	public static InputAction push_to_talk;

	// Token: 0x0400016E RID: 366
	public Vector2 movementInput;

	// Token: 0x0400016F RID: 367
	public Vector2 lookInput;

	// Token: 0x04000170 RID: 368
	public float scrollInput;

	// Token: 0x04000171 RID: 369
	public bool crouchIsPressed;

	// Token: 0x04000172 RID: 370
	public bool crouchWasPressed;

	// Token: 0x04000173 RID: 371
	public bool crouchToggleWasPressed;

	// Token: 0x04000174 RID: 372
	public bool sprintIsPressed;

	// Token: 0x04000175 RID: 373
	public bool sprintToggleIsPressed;

	// Token: 0x04000176 RID: 374
	public bool sprintWasPressed;

	// Token: 0x04000177 RID: 375
	public bool sprintToggleWasPressed;

	// Token: 0x04000178 RID: 376
	public bool pauseWasPressed;

	// Token: 0x04000179 RID: 377
	public bool jumpWasPressed;

	// Token: 0x0400017A RID: 378
	public bool jumpIsPressed;

	// Token: 0x0400017B RID: 379
	public bool interactWasPressed;

	// Token: 0x0400017C RID: 380
	public bool interactIsPressed;

	// Token: 0x0400017D RID: 381
	public bool interactWasReleased;

	// Token: 0x0400017E RID: 382
	public bool dropWasPressed;

	// Token: 0x0400017F RID: 383
	public bool dropIsPressed;

	// Token: 0x04000180 RID: 384
	public bool dropWasReleased;

	// Token: 0x04000181 RID: 385
	public bool usePrimaryWasPressed;

	// Token: 0x04000182 RID: 386
	public bool usePrimaryIsPressed;

	// Token: 0x04000183 RID: 387
	public bool usePrimaryWasReleased;

	// Token: 0x04000184 RID: 388
	public bool useSecondaryWasPressed;

	// Token: 0x04000185 RID: 389
	public bool useSecondaryIsPressed;

	// Token: 0x04000186 RID: 390
	public bool useSecondaryWasReleased;

	// Token: 0x04000187 RID: 391
	public bool pingWasPressed;

	// Token: 0x04000188 RID: 392
	public bool selectSlotForwardWasPressed;

	// Token: 0x04000189 RID: 393
	public bool selectSlotBackwardWasPressed;

	// Token: 0x0400018A RID: 394
	public bool unselectSlotWasPressed;

	// Token: 0x0400018B RID: 395
	public bool selectBackpackWasPressed;

	// Token: 0x0400018C RID: 396
	public bool scrollBackwardWasPressed;

	// Token: 0x0400018D RID: 397
	public bool scrollForwardWasPressed;

	// Token: 0x0400018E RID: 398
	public bool scrollBackwardIsPressed;

	// Token: 0x0400018F RID: 399
	public bool scrollForwardIsPressed;

	// Token: 0x04000190 RID: 400
	public bool emoteIsPressed;

	// Token: 0x04000191 RID: 401
	public bool spectateLeftWasPressed;

	// Token: 0x04000192 RID: 402
	public bool spectateRightWasPressed;

	// Token: 0x04000193 RID: 403
	public bool pushToTalkPressed;

	// Token: 0x04000194 RID: 404
	public bool itemSwitchBlocked;
}
