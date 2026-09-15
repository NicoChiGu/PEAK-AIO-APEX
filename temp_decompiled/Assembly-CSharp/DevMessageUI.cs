using System;
using TMPro;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class DevMessageUI : MonoBehaviour
{
	// Token: 0x06000DC0 RID: 3520 RVA: 0x00044DFF File Offset: 0x00042FFF
	private void Start()
	{
		this.service = GameHandler.GetService<NextLevelService>();
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x00044E0C File Offset: 0x0004300C
	private void Update()
	{
		bool flag = this.service.Data.IsSome && !string.IsNullOrEmpty(this.service.Data.Value.DevMessage);
		if (flag)
		{
			this.message = (this.useDebugMessage ? this.debugMessage : this.service.Data.Value.DevMessage);
			if (this.message.StartsWith("#"))
			{
				this.message = this.message.Remove(0, 1);
				this.parent.SetActive(false);
				this.shillParent.SetActive(true);
			}
			else
			{
				this.parent.SetActive(true);
				this.shillParent.SetActive(false);
			}
		}
		else
		{
			this.parent.SetActive(false);
			this.shillParent.SetActive(false);
		}
		if (flag)
		{
			TextMeshProUGUI[] array = this.texts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].text = this.message;
			}
		}
	}

	// Token: 0x04000BD4 RID: 3028
	public GameObject parent;

	// Token: 0x04000BD5 RID: 3029
	public GameObject shillParent;

	// Token: 0x04000BD6 RID: 3030
	public TextMeshProUGUI[] texts;

	// Token: 0x04000BD7 RID: 3031
	private NextLevelService service;

	// Token: 0x04000BD8 RID: 3032
	public bool useDebugMessage;

	// Token: 0x04000BD9 RID: 3033
	public string debugMessage;

	// Token: 0x04000BDA RID: 3034
	private string message;
}
