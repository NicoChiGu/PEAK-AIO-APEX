using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C4 RID: 452
public class EndScreenScoutWindow : MonoBehaviour
{
	// Token: 0x06000DF8 RID: 3576 RVA: 0x000459BC File Offset: 0x00043BBC
	public void Init(Character character)
	{
		if (character != null)
		{
			if (character.IsLocal)
			{
				this.scoutName.fontStyle = 4;
			}
			this.scoutName.text = character.characterName;
			Color playerColor = character.refs.customization.PlayerColor;
			playerColor.a = this.panelAlpha;
			this.panel.color = playerColor;
			this.altitude.text = "0m";
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x00045A3E File Offset: 0x00043C3E
	public void UpdateAltitude(int m)
	{
		this.altitude.text = m.ToString() + "m";
	}

	// Token: 0x04000C1C RID: 3100
	public TMP_Text scoutName;

	// Token: 0x04000C1D RID: 3101
	public TMP_Text altitude;

	// Token: 0x04000C1E RID: 3102
	public float panelAlpha = 0.25f;

	// Token: 0x04000C1F RID: 3103
	public Image panel;
}
