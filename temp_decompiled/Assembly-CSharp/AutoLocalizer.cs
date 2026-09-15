using System;
using TMPro;
using UnityEngine;

// Token: 0x02000206 RID: 518
public class AutoLocalizer : MonoBehaviour
{
	// Token: 0x06000F71 RID: 3953 RVA: 0x0004CB84 File Offset: 0x0004AD84
	public void AutoLoc()
	{
		string text = base.GetComponent<TMP_Text>().text;
		if (this.toUpper)
		{
			text = text.ToUpper();
		}
		if (text.Contains(',') || text.Contains('.'))
		{
			text = "\"" + text + "\"";
		}
		LocalizedText.AppendCSVLine(this.index + "," + text + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		LocalizedText localizedText = base.gameObject.AddComponent<LocalizedText>();
		localizedText.index = this.index;
		localizedText.DebugReload();
		Object.DestroyImmediate(this);
	}

	// Token: 0x04000DC5 RID: 3525
	public string index;

	// Token: 0x04000DC6 RID: 3526
	public bool toUpper;
}
