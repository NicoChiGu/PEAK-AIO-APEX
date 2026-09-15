using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D6 RID: 470
public class PassportButton : MonoBehaviour
{
	// Token: 0x06000E6F RID: 3695 RVA: 0x000472A0 File Offset: 0x000454A0
	public void SetButton(CustomizationOption option, int index)
	{
		if (option != null)
		{
			base.gameObject.SetActive(true);
			if (option.IsLocked && !this.manager.testUnlockAll)
			{
				this.lockedIcon.gameObject.SetActive(true);
				this.icon.gameObject.SetActive(false);
			}
			else
			{
				this.lockedIcon.gameObject.SetActive(false);
				this.icon.gameObject.SetActive(true);
				this.icon.texture = option.texture;
				if (option.type == Customization.Type.Skin)
				{
					this.icon.color = option.color;
				}
				else
				{
					this.icon.color = Color.white;
				}
				if (option.type == Customization.Type.Eyes)
				{
					this.icon.material = this.eyeMaterial;
				}
				else
				{
					this.icon.material = null;
				}
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		this.currentOption = option;
		this.currentIndex = index;
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x000473A4 File Offset: 0x000455A4
	public void Click()
	{
		if (!this.currentOption.IsLocked || this.manager.testUnlockAll)
		{
			this.manager.SetOption(this.currentOption, this.currentIndex);
		}
	}

	// Token: 0x04000C69 RID: 3177
	public Button button;

	// Token: 0x04000C6A RID: 3178
	public PassportManager manager;

	// Token: 0x04000C6B RID: 3179
	public RawImage icon;

	// Token: 0x04000C6C RID: 3180
	public RawImage lockedIcon;

	// Token: 0x04000C6D RID: 3181
	public Image border;

	// Token: 0x04000C6E RID: 3182
	private CustomizationOption currentOption;

	// Token: 0x04000C6F RID: 3183
	private int currentIndex;

	// Token: 0x04000C70 RID: 3184
	public Material eyeMaterial;
}
