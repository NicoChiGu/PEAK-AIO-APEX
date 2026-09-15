using System;
using UnityEngine;

// Token: 0x020001C1 RID: 449
[CreateAssetMenu(fileName = "EmoteWheelData", menuName = "Scriptable Objects/EmoteWheelData")]
public class EmoteWheelData : ScriptableObject
{
	// Token: 0x06000DDA RID: 3546 RVA: 0x0004523A File Offset: 0x0004343A
	public void AddNameToCSV()
	{
		LocalizedText.AppendCSVLine(this.emoteName.ToUpperInvariant() + "," + this.emoteName.ToLowerInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
	}

	// Token: 0x04000BE7 RID: 3047
	public string emoteName;

	// Token: 0x04000BE8 RID: 3048
	public Sprite emoteSprite;

	// Token: 0x04000BE9 RID: 3049
	public string anim;

	// Token: 0x04000BEA RID: 3050
	public bool requireGrounded;
}
