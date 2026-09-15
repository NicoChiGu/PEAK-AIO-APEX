using System;
using Peak.Network;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Peak.UI
{
	// Token: 0x020003B8 RID: 952
	public class KickButton : MonoBehaviour
	{
		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06001874 RID: 6260 RVA: 0x0007C3ED File Offset: 0x0007A5ED
		public Button MyButton
		{
			get
			{
				return this._button;
			}
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x0007C3F5 File Offset: 0x0007A5F5
		public void Init()
		{
			this._button = base.GetComponent<Button>();
			this._button.onClick.AddListener(new UnityAction(this.OpenKickMenu));
			this._slider = base.GetComponentInParent<AudioLevelSlider>();
			this.SetInteractable();
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x0007C431 File Offset: 0x0007A631
		private void OnDestroy()
		{
			if (this._button != null)
			{
				this._button.onClick.RemoveListener(new UnityAction(this.Kick));
			}
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x0007C45D File Offset: 0x0007A65D
		private void Update()
		{
			this.SetInteractable();
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x0007C468 File Offset: 0x0007A668
		private void SetInteractable()
		{
			Player player;
			if (!PlayerHandler.TryGetPlayer(this._slider.player.ActorNumber, out player))
			{
				this._slider.gameObject.SetActive(false);
				return;
			}
			bool isHost = NetCode.Session.IsHost;
			this._button.interactable = isHost;
			this._button.gameObject.SetActive(isHost);
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x0007C4C8 File Offset: 0x0007A6C8
		private void OpenKickMenu()
		{
			GUIManager.instance.pauseMenuMainPage.OpenKickConfirmWindow(this._slider.player.NickName).onClick.AddListener(new UnityAction(this.Kick));
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x0007C4FF File Offset: 0x0007A6FF
		private void Kick()
		{
			PlayerHandler.Kick(this._slider.player.ActorNumber);
		}

		// Token: 0x0400169B RID: 5787
		private Button _button;

		// Token: 0x0400169C RID: 5788
		private AudioLevelSlider _slider;
	}
}
