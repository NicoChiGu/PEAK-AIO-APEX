using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x0200039A RID: 922
	[ExecuteInEditMode]
	public class ChatAppIdCheckerUI : MonoBehaviour
	{
		// Token: 0x060017F4 RID: 6132 RVA: 0x00079470 File Offset: 0x00077670
		public void Update()
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
			{
				text = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the PhotonServerSettings file.";
			}
			this.Description.text = text;
		}

		// Token: 0x0400163B RID: 5691
		public Text Description;

		// Token: 0x0400163C RID: 5692
		public bool WizardOpenedOnce;
	}
}
