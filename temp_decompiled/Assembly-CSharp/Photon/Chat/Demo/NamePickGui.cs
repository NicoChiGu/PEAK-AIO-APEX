using System;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x0200039E RID: 926
	[RequireComponent(typeof(ChatGui))]
	public class NamePickGui : MonoBehaviour
	{
		// Token: 0x0600181E RID: 6174 RVA: 0x0007A218 File Offset: 0x00078418
		public void Start()
		{
			this.chatNewComponent = Object.FindFirstObjectByType<ChatGui>();
			string @string = PlayerPrefs.GetString("NamePickUserName");
			if (!string.IsNullOrEmpty(@string))
			{
				this.idInput.text = @string;
			}
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x0007A24F File Offset: 0x0007844F
		public void EndEditOnEnter()
		{
			if (Input.GetKey(13) || Input.GetKey(271))
			{
				this.StartChat();
			}
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x0007A26C File Offset: 0x0007846C
		public void StartChat()
		{
			ChatGui chatGui = Object.FindFirstObjectByType<ChatGui>();
			chatGui.UserName = this.idInput.text.Trim();
			chatGui.Connect();
			base.enabled = false;
			PlayerPrefs.SetString("NamePickUserName", chatGui.UserName);
		}

		// Token: 0x04001658 RID: 5720
		private const string UserNamePlayerPref = "NamePickUserName";

		// Token: 0x04001659 RID: 5721
		public ChatGui chatNewComponent;

		// Token: 0x0400165A RID: 5722
		public InputField idInput;
	}
}
