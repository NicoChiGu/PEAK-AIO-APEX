using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x02000399 RID: 921
	public class ChannelSelector : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x060017F1 RID: 6129 RVA: 0x00079439 File Offset: 0x00077639
		public void SetChannel(string channel)
		{
			this.Channel = channel;
			base.GetComponentInChildren<Text>().text = this.Channel;
		}

		// Token: 0x060017F2 RID: 6130 RVA: 0x00079453 File Offset: 0x00077653
		public void OnPointerClick(PointerEventData eventData)
		{
			Object.FindFirstObjectByType<ChatGui>().ShowChannel(this.Channel);
		}

		// Token: 0x0400163A RID: 5690
		public string Channel;
	}
}
