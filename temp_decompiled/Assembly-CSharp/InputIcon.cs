using System;
using TMPro;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001C7 RID: 455
public class InputIcon : MonoBehaviour
{
	// Token: 0x06000E04 RID: 3588 RVA: 0x00045D7D File Offset: 0x00043F7D
	private void Awake()
	{
		this.text = base.GetComponent<TMP_Text>();
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x00045D8B File Offset: 0x00043F8B
	private void Start()
	{
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerIconSetting>();
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x00045DA2 File Offset: 0x00043FA2
	private void OnEnable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
		this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x00045DD5 File Offset: 0x00043FD5
	private void OnDisable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x00045E00 File Offset: 0x00044000
	private void OnDeviceChange(InputScheme scheme)
	{
		if (this.setting == null)
		{
			if (GameHandler.Instance == null)
			{
				return;
			}
			this.setting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerIconSetting>();
		}
		if (scheme == null || this.setting.Value == ControllerIconSetting.IconMode.KBM)
		{
			this.text.spriteAsset = this.keyboardSprites;
		}
		else if (scheme == 1)
		{
			if (this.setting.Value == ControllerIconSetting.IconMode.Auto)
			{
				GamepadType gamepadType = InputHandler.GetGamepadType();
				if (gamepadType == 1)
				{
					this.text.spriteAsset = this.xboxSprites;
				}
				else if (gamepadType == 2)
				{
					this.text.spriteAsset = this.ps5Sprites;
				}
				else if (gamepadType == 3)
				{
					this.text.spriteAsset = this.ps5Sprites;
				}
				else if (gamepadType == 4)
				{
					this.text.spriteAsset = this.xboxSprites;
				}
				else if (gamepadType == null)
				{
					this.text.spriteAsset = this.xboxSprites;
				}
			}
			else if (this.setting.Value == ControllerIconSetting.IconMode.Style1)
			{
				this.text.spriteAsset = this.xboxSprites;
			}
			else if (this.setting.Value == ControllerIconSetting.IconMode.Style2)
			{
				this.text.spriteAsset = this.ps5Sprites;
			}
		}
		this.SetText(scheme);
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00045F38 File Offset: 0x00044138
	private void SetText(InputScheme scheme)
	{
		if (scheme == 1)
		{
			this.text.enabled = !this.disableIfController;
		}
		else if (scheme == null)
		{
			this.text.enabled = !this.disableIfKeyboard;
		}
		string text;
		if (scheme == 1 && this.action == InputSpriteData.InputAction.Scroll)
		{
			text = SingletonAsset<InputSpriteData>.Instance.GetSpriteTag(InputSpriteData.InputAction.ScrollForward, scheme) + SingletonAsset<InputSpriteData>.Instance.GetSpriteTag(InputSpriteData.InputAction.ScrollBackward, scheme);
		}
		else
		{
			text = SingletonAsset<InputSpriteData>.Instance.GetSpriteTag(this.action, scheme);
		}
		if (!string.IsNullOrEmpty(text))
		{
			this.text.text = text;
		}
		if (scheme == 1 && this.hold != null)
		{
			this.hold.SetActive(this.action == InputSpriteData.InputAction.Throw || this.action == InputSpriteData.InputAction.HoldInteract);
		}
	}

	// Token: 0x04000C24 RID: 3108
	private TMP_Text text;

	// Token: 0x04000C25 RID: 3109
	public GameObject hold;

	// Token: 0x04000C26 RID: 3110
	public InputSpriteData.InputAction action;

	// Token: 0x04000C27 RID: 3111
	public TMP_SpriteAsset keyboardSprites;

	// Token: 0x04000C28 RID: 3112
	public TMP_SpriteAsset xboxSprites;

	// Token: 0x04000C29 RID: 3113
	public TMP_SpriteAsset switchSprites;

	// Token: 0x04000C2A RID: 3114
	public TMP_SpriteAsset ps5Sprites;

	// Token: 0x04000C2B RID: 3115
	public TMP_SpriteAsset ps4Sprites;

	// Token: 0x04000C2C RID: 3116
	public bool disableIfController;

	// Token: 0x04000C2D RID: 3117
	public bool disableIfKeyboard;

	// Token: 0x04000C2E RID: 3118
	private ControllerIconSetting setting;
}
