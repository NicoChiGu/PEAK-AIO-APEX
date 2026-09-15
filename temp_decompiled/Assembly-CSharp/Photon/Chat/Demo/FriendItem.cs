using System;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x0200039C RID: 924
	public class FriendItem : MonoBehaviour
	{
		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06001818 RID: 6168 RVA: 0x0007A132 File Offset: 0x00078332
		// (set) Token: 0x06001817 RID: 6167 RVA: 0x0007A124 File Offset: 0x00078324
		[HideInInspector]
		public string FriendId
		{
			get
			{
				return this.NameLabel.text;
			}
			set
			{
				this.NameLabel.text = value;
			}
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x0007A13F File Offset: 0x0007833F
		public void Awake()
		{
			this.Health.text = string.Empty;
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x0007A154 File Offset: 0x00078354
		public void OnFriendStatusUpdate(int status, bool gotMessage, object message)
		{
			string text;
			switch (status)
			{
			case 1:
				text = "Invisible";
				break;
			case 2:
				text = "Online";
				break;
			case 3:
				text = "Away";
				break;
			case 4:
				text = "Do not disturb";
				break;
			case 5:
				text = "Looking For Game/Group";
				break;
			case 6:
				text = "Playing";
				break;
			default:
				text = "Offline";
				break;
			}
			this.StatusLabel.text = text;
			if (gotMessage)
			{
				string text2 = string.Empty;
				if (message != null)
				{
					string[] array = message as string[];
					if (array != null && array.Length >= 2)
					{
						text2 = array[1] + "%";
					}
				}
				this.Health.text = text2;
			}
		}

		// Token: 0x04001655 RID: 5717
		public Text NameLabel;

		// Token: 0x04001656 RID: 5718
		public Text StatusLabel;

		// Token: 0x04001657 RID: 5719
		public Text Health;
	}
}
