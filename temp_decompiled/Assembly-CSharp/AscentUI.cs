using System;
using TMPro;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200005C RID: 92
public class AscentUI : MonoBehaviour
{
	// Token: 0x06000481 RID: 1153 RVA: 0x0001B4A0 File Offset: 0x000196A0
	private void Update()
	{
		int currentAscent = Ascents._currentAscent;
		this.text.text = SingletonAsset<AscentData>.Instance.ascents[currentAscent + 1].localizedTitle;
		if (currentAscent == 0)
		{
			this.text.text = "";
		}
	}

	// Token: 0x04000505 RID: 1285
	public TextMeshProUGUI text;
}
