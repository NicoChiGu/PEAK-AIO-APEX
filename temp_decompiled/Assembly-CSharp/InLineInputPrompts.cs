using System;
using TMPro;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001C6 RID: 454
[RequireComponent(typeof(TMP_Text))]
public class InLineInputPrompts : MonoBehaviour
{
	// Token: 0x06000DFD RID: 3581 RVA: 0x00045AB0 File Offset: 0x00043CB0
	private void Awake()
	{
		this.text = base.GetComponent<TMP_Text>();
		this.loc = base.GetComponent<LocalizedText>();
		this.originalText = this.text.text;
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerIconSetting>();
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x00045AF0 File Offset: 0x00043CF0
	private void OnEnable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
		this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x00045B23 File Offset: 0x00043D23
	private void OnDisable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x00045B4B File Offset: 0x00043D4B
	private void OnDeviceChange(InputScheme scheme)
	{
		this.UpdateText(scheme);
		this.UpdateSprites(scheme);
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x00045B5C File Offset: 0x00043D5C
	private void UpdateText(InputScheme scheme)
	{
		string text = this.originalText;
		if (this.loc)
		{
			text = this.loc.GetText();
			this.loc.enabled = false;
		}
		if (text.Contains("[") && text.Contains("]"))
		{
			foreach (object obj in Enum.GetValues(typeof(InputSpriteData.InputAction)))
			{
				if (text.ToUpperInvariant().Contains(obj.ToString().ToUpperInvariant()))
				{
					string spriteTag = SingletonAsset<InputSpriteData>.Instance.GetSpriteTag((InputSpriteData.InputAction)obj, scheme);
					if (!string.IsNullOrEmpty(spriteTag))
					{
						string text2 = string.Format("[{0}]", obj).ToUpperInvariant();
						text = text.Replace(text2, spriteTag);
					}
				}
			}
		}
		this.text.text = text;
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x00045C5C File Offset: 0x00043E5C
	private void UpdateSprites(InputScheme scheme)
	{
		if (scheme == null || this.setting.Value == ControllerIconSetting.IconMode.KBM)
		{
			this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.keyboardSprites;
			return;
		}
		if (scheme == 1)
		{
			if (this.setting.Value == ControllerIconSetting.IconMode.Auto)
			{
				GamepadType gamepadType = InputHandler.GetGamepadType();
				if (gamepadType == 1)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
					return;
				}
				if (gamepadType == 2)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps5Sprites;
					return;
				}
				if (gamepadType == 3)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps5Sprites;
					return;
				}
				if (gamepadType == 4)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
					return;
				}
				if (gamepadType == null)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
					return;
				}
			}
			else
			{
				if (this.setting.Value == ControllerIconSetting.IconMode.Style1)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
					return;
				}
				if (this.setting.Value == ControllerIconSetting.IconMode.Style2)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps5Sprites;
				}
			}
		}
	}

	// Token: 0x04000C20 RID: 3104
	private TMP_Text text;

	// Token: 0x04000C21 RID: 3105
	private LocalizedText loc;

	// Token: 0x04000C22 RID: 3106
	private string originalText;

	// Token: 0x04000C23 RID: 3107
	private ControllerIconSetting setting;
}
