using System;
using TMPro;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class NextLevelUI : MonoBehaviour
{
	// Token: 0x06000E3D RID: 3645 RVA: 0x00046CF2 File Offset: 0x00044EF2
	private void Start()
	{
		this.nextLevelService = GameHandler.GetService<NextLevelService>();
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x00046D00 File Offset: 0x00044F00
	private void Update()
	{
		if (this.nextLevelService.Data.IsSome)
		{
			this.timer.text = this.ParseSeconds(this.nextLevelService.Data.Value.SecondsLeft);
			return;
		}
		this.timer.text = this.ParseSeconds(this.nextLevelService.SecondsLeftFallback);
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00046D68 File Offset: 0x00044F68
	public string ParseSeconds(int seconds)
	{
		if (seconds < 0)
		{
			return "-- -- --";
		}
		int num = Mathf.FloorToInt((float)seconds / 3600f);
		int num2 = Mathf.FloorToInt((float)(seconds - num * 3600) / 60f);
		float num3 = (float)(seconds - (num * 3600 + num2 * 60));
		return string.Format("{0}h {1}m {2}s", num, num2, num3);
	}

	// Token: 0x04000C5C RID: 3164
	public TextMeshProUGUI timer;

	// Token: 0x04000C5D RID: 3165
	private NextLevelService nextLevelService;
}
