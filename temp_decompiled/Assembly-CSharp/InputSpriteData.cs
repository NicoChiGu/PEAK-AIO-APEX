using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020000C9 RID: 201
[CreateAssetMenu(fileName = "InputSpriteData", menuName = "Scouts/Input Sprite Data")]
public class InputSpriteData : SingletonAsset<InputSpriteData>
{
	// Token: 0x060007C1 RID: 1985 RVA: 0x0002B174 File Offset: 0x00029374
	public string GetSpriteTag(InputSpriteData.InputAction action, InputScheme scheme)
	{
		if (scheme == null)
		{
			if (InputSpriteData.ActionToHardcodedSpriteKeyboard.ContainsKey(action))
			{
				return InputSpriteData.ActionToHardcodedSpriteKeyboard[action];
			}
			if (InputSpriteData.ActionToBackendNameKeyboard.ContainsKey(action))
			{
				bool flag;
				string bindingPath = InputSpriteData.GetBindingPath(InputSpriteData.ActionToBackendNameKeyboard[action], scheme, out flag);
				return this.GetSpriteTagFromInputPathKeyboard(bindingPath);
			}
			Debug.Log(string.Format("Failed to find backend name for {0}", action));
		}
		else if (scheme == 1)
		{
			if (action == InputSpriteData.InputAction.Aim)
			{
				return "<sprite=17 tint=1>";
			}
			if (action == InputSpriteData.InputAction.Move)
			{
				return "<sprite=16 tint=1>";
			}
			if (InputSpriteData.ActionToBackendNameGamepad.ContainsKey(action))
			{
				bool flag2;
				string bindingPath2 = InputSpriteData.GetBindingPath(InputSpriteData.ActionToBackendNameGamepad[action], scheme, out flag2);
				return this.GetSpriteTagFromInputPathGamepad(bindingPath2);
			}
			Debug.Log(string.Format("Failed to find backend name for {0}", action));
		}
		return "";
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0002B23C File Offset: 0x0002943C
	public static string GetBindingPath(string actionName, InputScheme scheme, out bool hasOverride)
	{
		hasOverride = false;
		global::UnityEngine.InputSystem.InputAction inputAction = InputSystem.actions.FindAction(actionName, false);
		if (inputAction != null)
		{
			foreach (InputBinding inputBinding in inputAction.bindings)
			{
				if (scheme == null && (inputBinding.effectivePath.Contains("<Keyboard>") || inputBinding.effectivePath.Contains("<Mouse>")))
				{
					hasOverride = !string.IsNullOrEmpty(inputBinding.overridePath);
					return inputBinding.effectivePath;
				}
				if ((scheme == 1 || scheme == 2) && inputBinding.effectivePath.Contains("<Gamepad>"))
				{
					hasOverride = !string.IsNullOrEmpty(inputBinding.overridePath);
					return inputBinding.effectivePath;
				}
			}
		}
		return "";
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0002B32C File Offset: 0x0002952C
	public static string GetPathEnd(string inputPath)
	{
		if (string.IsNullOrEmpty(inputPath))
		{
			return "";
		}
		string[] array = inputPath.Split("/", 0);
		return array[array.Length - 1];
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0002B350 File Offset: 0x00029550
	public string GetSpriteTagFromInputPathGamepad(string inputPath)
	{
		if (string.IsNullOrEmpty(inputPath))
		{
			return "";
		}
		string[] array = inputPath.Split("/", 0);
		string text = array[array.Length - 1];
		string text2;
		if (this.inputPathToSpriteTagGamepad.TryGetValue(text, ref text2))
		{
			return text2;
		}
		return null;
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0002B394 File Offset: 0x00029594
	public string GetSpriteTagFromInputPathKeyboard(string inputPath)
	{
		if (string.IsNullOrEmpty(inputPath))
		{
			return "<sprite=124 tint=1>";
		}
		string[] array = inputPath.Split("/", 0);
		string text = array[array.Length - 1];
		string text2;
		if (array[0] == "<Mouse>" && this.inputPathToSpriteTagMouse.TryGetValue(text, ref text2))
		{
			return text2;
		}
		string text3;
		if (this.inputPathToSpriteTagKeyboard.TryGetValue(text, ref text3))
		{
			return text3;
		}
		return "<sprite=124 tint=1>";
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x0002B3FC File Offset: 0x000295FC
	public InputSpriteData()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("leftShoulder", "<sprite=4 tint=1>");
		dictionary.Add("rightShoulder", "<sprite=5 tint=1>");
		dictionary.Add("leftTrigger", "<sprite=6 tint=1>");
		dictionary.Add("rightTrigger", "<sprite=7 tint=1>");
		dictionary.Add("buttonNorth", "<sprite=3 tint=1>");
		dictionary.Add("buttonSouth", "<sprite=0 tint=1>");
		dictionary.Add("buttonWest", "<sprite=2 tint=1>");
		dictionary.Add("buttonEast", "<sprite=1 tint=1>");
		dictionary.Add("up", "<sprite=12 tint=1>");
		dictionary.Add("down", "<sprite=13 tint=1>");
		dictionary.Add("left", "<sprite=14 tint=1>");
		dictionary.Add("right", "<sprite=15 tint=1>");
		dictionary.Add("start", "<sprite=8 tint=1>");
		dictionary.Add("select", "<sprite=9 tint=1>");
		dictionary.Add("leftStickPress", "<sprite=10 tint=1>");
		dictionary.Add("rightStickPress", "<sprite=11 tint=1>");
		this.inputPathToSpriteTagGamepad = dictionary;
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2.Add("0", "<sprite=0 tint=1>");
		dictionary2.Add("1", "<sprite=1 tint=1>");
		dictionary2.Add("2", "<sprite=2 tint=1>");
		dictionary2.Add("3", "<sprite=3 tint=1>");
		dictionary2.Add("4", "<sprite=4 tint=1>");
		dictionary2.Add("5", "<sprite=5 tint=1>");
		dictionary2.Add("6", "<sprite=6 tint=1>");
		dictionary2.Add("7", "<sprite=7 tint=1>");
		dictionary2.Add("8", "<sprite=8 tint=1>");
		dictionary2.Add("9", "<sprite=9 tint=1>");
		dictionary2.Add("a", "<sprite=10 tint=1>");
		dictionary2.Add("b", "<sprite=11 tint=1>");
		dictionary2.Add("c", "<sprite=12 tint=1>");
		dictionary2.Add("d", "<sprite=13 tint=1>");
		dictionary2.Add("e", "<sprite=14 tint=1>");
		dictionary2.Add("f", "<sprite=15 tint=1>");
		dictionary2.Add("g", "<sprite=16 tint=1>");
		dictionary2.Add("h", "<sprite=17 tint=1>");
		dictionary2.Add("i", "<sprite=18 tint=1>");
		dictionary2.Add("j", "<sprite=19 tint=1>");
		dictionary2.Add("k", "<sprite=20 tint=1>");
		dictionary2.Add("l", "<sprite=21 tint=1>");
		dictionary2.Add("m", "<sprite=22 tint=1>");
		dictionary2.Add("n", "<sprite=23 tint=1>");
		dictionary2.Add("o", "<sprite=24 tint=1>");
		dictionary2.Add("p", "<sprite=25 tint=1>");
		dictionary2.Add("q", "<sprite=26 tint=1>");
		dictionary2.Add("r", "<sprite=27 tint=1>");
		dictionary2.Add("s", "<sprite=28 tint=1>");
		dictionary2.Add("t", "<sprite=29 tint=1>");
		dictionary2.Add("u", "<sprite=30 tint=1>");
		dictionary2.Add("v", "<sprite=31 tint=1>");
		dictionary2.Add("w", "<sprite=32 tint=1>");
		dictionary2.Add("x", "<sprite=33 tint=1>");
		dictionary2.Add("y", "<sprite=34 tint=1>");
		dictionary2.Add("z", "<sprite=35 tint=1>");
		dictionary2.Add("f1", "<sprite=36 tint=1>");
		dictionary2.Add("f2", "<sprite=37 tint=1>");
		dictionary2.Add("f3", "<sprite=38 tint=1>");
		dictionary2.Add("f4", "<sprite=39 tint=1>");
		dictionary2.Add("f5", "<sprite=40 tint=1>");
		dictionary2.Add("f6", "<sprite=41 tint=1>");
		dictionary2.Add("f7", "<sprite=42 tint=1>");
		dictionary2.Add("f8", "<sprite=43 tint=1>");
		dictionary2.Add("f9", "<sprite=44 tint=1>");
		dictionary2.Add("f10", "<sprite=45 tint=1>");
		dictionary2.Add("f11", "<sprite=46 tint=1>");
		dictionary2.Add("f12", "<sprite=47 tint=1>");
		dictionary2.Add("minus", "<sprite=78 tint=1>");
		dictionary2.Add("equals", "<sprite=80 tint=1>");
		dictionary2.Add("leftBracket", "<sprite=82 tint=1>");
		dictionary2.Add("rightBracket", "<sprite=83 tint=1>");
		dictionary2.Add("backquote", "<sprite=81 tint=1>");
		dictionary2.Add("tab", "<sprite=53 tint=1>");
		dictionary2.Add("leftShift", "<sprite=51 tint=1>");
		dictionary2.Add("rightShift", "<sprite=51 tint=1>");
		dictionary2.Add("shift", "<sprite=51 tint=1>");
		dictionary2.Add("leftCtrl", "<sprite=49 tint=1>");
		dictionary2.Add("rightCtrl", "<sprite=49 tint=1>");
		dictionary2.Add("ctrl", "<sprite=49 tint=1>");
		dictionary2.Add("leftAlt", "<sprite=50 tint=1>");
		dictionary2.Add("rightAlt", "<sprite=50 tint=1>");
		dictionary2.Add("alt", "<sprite=50 tint=1>");
		dictionary2.Add("space", "<sprite=69 tint=1>");
		dictionary2.Add("semicolon", "<sprite=85 tint=1>");
		dictionary2.Add("quote", "<sprite=100 tint=1>");
		dictionary2.Add("comma", "<sprite=87 tint=1>");
		dictionary2.Add("period", "<sprite=88 tint=1>");
		dictionary2.Add("slash", "<sprite=76 tint=1>");
		dictionary2.Add("backslash", "<sprite=84 tint=1>");
		dictionary2.Add("insert", "<sprite=70 tint=1>");
		dictionary2.Add("delete", "<sprite=71 tint=1>");
		dictionary2.Add("home", "<sprite=72 tint=1>");
		dictionary2.Add("end", "<sprite=73 tint=1>");
		dictionary2.Add("pageUp", "<sprite=74 tint=1>");
		dictionary2.Add("pageDown", "<sprite=75 tint=1>");
		dictionary2.Add("upArrow", "<sprite=56 tint=1>");
		dictionary2.Add("downArrow", "<sprite=58 tint=1>");
		dictionary2.Add("leftArrow", "<sprite=59 tint=1>");
		dictionary2.Add("rightArrow", "<sprite=57 tint=1>");
		dictionary2.Add("numpad0", "<sprite=127 tint=1>");
		dictionary2.Add("numpad1", "<sprite=128 tint=1>");
		dictionary2.Add("numpad2", "<sprite=129 tint=1>");
		dictionary2.Add("numpad3", "<sprite=130 tint=1>");
		dictionary2.Add("numpad4", "<sprite=131 tint=1>");
		dictionary2.Add("numpad5", "<sprite=132 tint=1>");
		dictionary2.Add("numpad6", "<sprite=133 tint=1>");
		dictionary2.Add("numpad7", "<sprite=134 tint=1>");
		dictionary2.Add("numpad8", "<sprite=135 tint=1>");
		dictionary2.Add("numpad9", "<sprite=136 tint=1>");
		dictionary2.Add("numpadPlus", "<sprite=119 tint=1>");
		dictionary2.Add("numpadMinus", "<sprite=118 tint=1>");
		dictionary2.Add("numpadDivide", "<sprite=120 tint=1>");
		dictionary2.Add("numpadMultiply", "<sprite=121 tint=1>");
		dictionary2.Add("numpadEnter", "<sprite=122 tint=1>");
		dictionary2.Add("numpadPeriod", "<sprite=123 tint=1>");
		dictionary2.Add("capsLock", "<sprite=52 tint=1>");
		dictionary2.Add("backspace", "<sprite=67 tint=1>");
		dictionary2.Add("enter", "<sprite=68 tint=1>");
		dictionary2.Add("esc", "<sprite=54 tint=1>");
		this.inputPathToSpriteTagKeyboard = dictionary2;
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
		dictionary3.Add("down", "<sprite=112 tint=1>");
		dictionary3.Add("up", "<sprite=112 tint=1>");
		dictionary3.Add("scroll", "<sprite=112 tint=1>");
		dictionary3.Add("leftButton", "<sprite=109 tint=1>");
		dictionary3.Add("rightButton", "<sprite=110 tint=1>");
		dictionary3.Add("middleButton", "<sprite=111 tint=1>");
		this.inputPathToSpriteTagMouse = dictionary3;
		base..ctor();
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x0002BBD0 File Offset: 0x00029DD0
	// Note: this type is marked as 'beforefieldinit'.
	static InputSpriteData()
	{
		Dictionary<InputSpriteData.InputAction, string> dictionary = new Dictionary<InputSpriteData.InputAction, string>();
		dictionary.Add(InputSpriteData.InputAction.Aim, "<sprite=108 tint=1>");
		dictionary.Add(InputSpriteData.InputAction.Move, "<sprite=115 tint=1>");
		dictionary.Add(InputSpriteData.InputAction.Scroll, "<sprite=112 tint=1>");
		InputSpriteData.ActionToHardcodedSpriteKeyboard = dictionary;
		Dictionary<InputSpriteData.InputAction, string> dictionary2 = new Dictionary<InputSpriteData.InputAction, string>();
		dictionary2.Add(InputSpriteData.InputAction.Interact, "Interact");
		dictionary2.Add(InputSpriteData.InputAction.HoldInteract, "Interact");
		dictionary2.Add(InputSpriteData.InputAction.UsePrimary, "UsePrimary");
		dictionary2.Add(InputSpriteData.InputAction.UseSecondary, "UseSecondary");
		dictionary2.Add(InputSpriteData.InputAction.Scroll, "Scroll");
		dictionary2.Add(InputSpriteData.InputAction.Throw, "Drop");
		dictionary2.Add(InputSpriteData.InputAction.Drop, "Drop");
		dictionary2.Add(InputSpriteData.InputAction.Slot1, "Hotbar1");
		dictionary2.Add(InputSpriteData.InputAction.Slot2, "Hotbar2");
		dictionary2.Add(InputSpriteData.InputAction.Slot3, "Hotbar3");
		dictionary2.Add(InputSpriteData.InputAction.Slot4, "Hotbar4");
		dictionary2.Add(InputSpriteData.InputAction.SpectateLeft, "Hotbar1");
		dictionary2.Add(InputSpriteData.InputAction.SpectateRight, "Hotbar2");
		dictionary2.Add(InputSpriteData.InputAction.Move, "Move");
		dictionary2.Add(InputSpriteData.InputAction.Aim, "Aim");
		dictionary2.Add(InputSpriteData.InputAction.Sprint, "Sprint");
		dictionary2.Add(InputSpriteData.InputAction.Jump, "Jump");
		dictionary2.Add(InputSpriteData.InputAction.Crouch, "Crouch");
		dictionary2.Add(InputSpriteData.InputAction.Ping, "Ping");
		dictionary2.Add(InputSpriteData.InputAction.SlotLeft, "SelectSlotBackward");
		dictionary2.Add(InputSpriteData.InputAction.SlotRight, "SelectSlotForward");
		dictionary2.Add(InputSpriteData.InputAction.DeselectSlot, "UnselectSlot");
		dictionary2.Add(InputSpriteData.InputAction.Emote, "Emote");
		dictionary2.Add(InputSpriteData.InputAction.PushToTalk, "PushToTalk");
		dictionary2.Add(InputSpriteData.InputAction.TabLeft, "TabLeft");
		dictionary2.Add(InputSpriteData.InputAction.TabRight, "TabRight");
		dictionary2.Add(InputSpriteData.InputAction.MoveForward, "MoveForward");
		dictionary2.Add(InputSpriteData.InputAction.MoveBackward, "MoveBackward");
		dictionary2.Add(InputSpriteData.InputAction.MoveLeft, "MoveLeft");
		dictionary2.Add(InputSpriteData.InputAction.MoveRight, "MoveRight");
		dictionary2.Add(InputSpriteData.InputAction.ScrollForward, "ScrollForward");
		dictionary2.Add(InputSpriteData.InputAction.ScrollBackward, "ScrollBackward");
		dictionary2.Add(InputSpriteData.InputAction.Pause, "Pause");
		InputSpriteData.ActionToBackendNameKeyboard = dictionary2;
		Dictionary<InputSpriteData.InputAction, string> dictionary3 = new Dictionary<InputSpriteData.InputAction, string>();
		dictionary3.Add(InputSpriteData.InputAction.Interact, "Interact");
		dictionary3.Add(InputSpriteData.InputAction.HoldInteract, "Interact");
		dictionary3.Add(InputSpriteData.InputAction.UsePrimary, "UsePrimary");
		dictionary3.Add(InputSpriteData.InputAction.UseSecondary, "UseSecondary");
		dictionary3.Add(InputSpriteData.InputAction.Scroll, "Scroll");
		dictionary3.Add(InputSpriteData.InputAction.Throw, "Drop");
		dictionary3.Add(InputSpriteData.InputAction.Drop, "Drop");
		dictionary3.Add(InputSpriteData.InputAction.Slot1, "Hotbar1");
		dictionary3.Add(InputSpriteData.InputAction.Slot2, "Hotbar2");
		dictionary3.Add(InputSpriteData.InputAction.Slot3, "Hotbar3");
		dictionary3.Add(InputSpriteData.InputAction.Slot4, "Hotbar4");
		dictionary3.Add(InputSpriteData.InputAction.SpectateLeft, "SelectSlotBackward");
		dictionary3.Add(InputSpriteData.InputAction.SpectateRight, "SelectSlotForward");
		dictionary3.Add(InputSpriteData.InputAction.Move, "Move");
		dictionary3.Add(InputSpriteData.InputAction.Aim, "Aim");
		dictionary3.Add(InputSpriteData.InputAction.Sprint, "SprintToggle");
		dictionary3.Add(InputSpriteData.InputAction.Jump, "Jump");
		dictionary3.Add(InputSpriteData.InputAction.Crouch, "CrouchToggle");
		dictionary3.Add(InputSpriteData.InputAction.Ping, "Ping");
		dictionary3.Add(InputSpriteData.InputAction.SlotLeft, "SelectSlotBackward");
		dictionary3.Add(InputSpriteData.InputAction.SlotRight, "SelectSlotForward");
		dictionary3.Add(InputSpriteData.InputAction.DeselectSlot, "UnselectSlot");
		dictionary3.Add(InputSpriteData.InputAction.Emote, "Emote");
		dictionary3.Add(InputSpriteData.InputAction.PushToTalk, "PushToTalk");
		dictionary3.Add(InputSpriteData.InputAction.TabLeft, "TabLeft");
		dictionary3.Add(InputSpriteData.InputAction.TabRight, "TabRight");
		dictionary3.Add(InputSpriteData.InputAction.MoveForward, "MoveForward");
		dictionary3.Add(InputSpriteData.InputAction.MoveBackward, "MoveBackward");
		dictionary3.Add(InputSpriteData.InputAction.MoveLeft, "MoveLeft");
		dictionary3.Add(InputSpriteData.InputAction.MoveRight, "MoveRight");
		dictionary3.Add(InputSpriteData.InputAction.ScrollForward, "ScrollForward");
		dictionary3.Add(InputSpriteData.InputAction.ScrollBackward, "ScrollBackward");
		dictionary3.Add(InputSpriteData.InputAction.Pause, "Pause");
		InputSpriteData.ActionToBackendNameGamepad = dictionary3;
	}

	// Token: 0x04000796 RID: 1942
	public TMP_SpriteAsset keyboardSprites;

	// Token: 0x04000797 RID: 1943
	public TMP_SpriteAsset xboxSprites;

	// Token: 0x04000798 RID: 1944
	public TMP_SpriteAsset switchSprites;

	// Token: 0x04000799 RID: 1945
	public TMP_SpriteAsset ps5Sprites;

	// Token: 0x0400079A RID: 1946
	public TMP_SpriteAsset ps4Sprites;

	// Token: 0x0400079B RID: 1947
	public static Dictionary<InputSpriteData.InputAction, string> ActionToHardcodedSpriteKeyboard;

	// Token: 0x0400079C RID: 1948
	public static Dictionary<InputSpriteData.InputAction, string> ActionToBackendNameKeyboard;

	// Token: 0x0400079D RID: 1949
	public static Dictionary<InputSpriteData.InputAction, string> ActionToBackendNameGamepad;

	// Token: 0x0400079E RID: 1950
	private Dictionary<string, string> inputPathToSpriteTagGamepad;

	// Token: 0x0400079F RID: 1951
	private Dictionary<string, string> inputPathToSpriteTagKeyboard;

	// Token: 0x040007A0 RID: 1952
	private Dictionary<string, string> inputPathToSpriteTagMouse;

	// Token: 0x02000447 RID: 1095
	public enum InputAction
	{
		// Token: 0x0400184F RID: 6223
		Interact,
		// Token: 0x04001850 RID: 6224
		HoldInteract,
		// Token: 0x04001851 RID: 6225
		UsePrimary,
		// Token: 0x04001852 RID: 6226
		UseSecondary,
		// Token: 0x04001853 RID: 6227
		Scroll,
		// Token: 0x04001854 RID: 6228
		Throw,
		// Token: 0x04001855 RID: 6229
		Drop,
		// Token: 0x04001856 RID: 6230
		Slot1,
		// Token: 0x04001857 RID: 6231
		Slot2,
		// Token: 0x04001858 RID: 6232
		Slot3,
		// Token: 0x04001859 RID: 6233
		Slot4,
		// Token: 0x0400185A RID: 6234
		SpectateLeft,
		// Token: 0x0400185B RID: 6235
		SpectateRight,
		// Token: 0x0400185C RID: 6236
		Move,
		// Token: 0x0400185D RID: 6237
		Aim,
		// Token: 0x0400185E RID: 6238
		Sprint,
		// Token: 0x0400185F RID: 6239
		Jump,
		// Token: 0x04001860 RID: 6240
		Crouch,
		// Token: 0x04001861 RID: 6241
		Ping,
		// Token: 0x04001862 RID: 6242
		SlotLeft,
		// Token: 0x04001863 RID: 6243
		SlotRight,
		// Token: 0x04001864 RID: 6244
		DeselectSlot,
		// Token: 0x04001865 RID: 6245
		Emote,
		// Token: 0x04001866 RID: 6246
		PushToTalk,
		// Token: 0x04001867 RID: 6247
		TabLeft,
		// Token: 0x04001868 RID: 6248
		TabRight,
		// Token: 0x04001869 RID: 6249
		MoveForward,
		// Token: 0x0400186A RID: 6250
		MoveBackward,
		// Token: 0x0400186B RID: 6251
		MoveLeft,
		// Token: 0x0400186C RID: 6252
		MoveRight,
		// Token: 0x0400186D RID: 6253
		ScrollForward,
		// Token: 0x0400186E RID: 6254
		ScrollBackward,
		// Token: 0x0400186F RID: 6255
		Pause
	}
}
