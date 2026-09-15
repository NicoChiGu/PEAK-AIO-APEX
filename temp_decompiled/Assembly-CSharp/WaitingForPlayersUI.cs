using System;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001F3 RID: 499
public class WaitingForPlayersUI : MonoBehaviour
{
	// Token: 0x06000F18 RID: 3864 RVA: 0x00049DB4 File Offset: 0x00047FB4
	private void Update()
	{
		for (int i = 0; i < this.scoutImages.Length; i++)
		{
			this.scoutImages[i].gameObject.SetActive(false);
		}
		int num = 0;
		foreach (Player player in PlayerHandler.GetAllPlayers())
		{
			bool hasClosedEndScreen = player.hasClosedEndScreen;
			PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(player.photonView.Owner);
			Color color = Singleton<Customization>.Instance.skins[playerData.customizationData.currentSkin].color;
			if (num < this.scoutImages.Length)
			{
				this.scoutImages[num].gameObject.SetActive(true);
				this.scoutImages[num].color = (hasClosedEndScreen ? color : this.notReadyColor);
			}
			num++;
		}
	}

	// Token: 0x04000D17 RID: 3351
	public Image[] scoutImages;

	// Token: 0x04000D18 RID: 3352
	public Color notReadyColor;
}
