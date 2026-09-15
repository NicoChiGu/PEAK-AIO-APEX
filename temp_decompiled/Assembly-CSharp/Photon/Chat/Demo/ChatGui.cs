using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x0200039B RID: 923
	public class ChatGui : MonoBehaviour, IChatClientListener
	{
		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060017F6 RID: 6134 RVA: 0x000794B3 File Offset: 0x000776B3
		// (set) Token: 0x060017F7 RID: 6135 RVA: 0x000794BB File Offset: 0x000776BB
		public string UserName { get; set; }

		// Token: 0x060017F8 RID: 6136 RVA: 0x000794C4 File Offset: 0x000776C4
		public void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			this.UserIdText.text = "";
			this.StateText.text = "";
			this.StateText.gameObject.SetActive(true);
			this.UserIdText.gameObject.SetActive(true);
			this.Title.SetActive(true);
			this.ChatPanel.gameObject.SetActive(false);
			this.ConnectingLabel.SetActive(false);
			if (string.IsNullOrEmpty(this.UserName))
			{
				this.UserName = "user" + (Environment.TickCount % 99).ToString();
			}
			this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
			bool flag = !string.IsNullOrEmpty(this.chatAppSettings.AppIdChat);
			this.missingAppIdErrorPanel.SetActive(!flag);
			this.UserIdFormPanel.gameObject.SetActive(flag);
			if (!flag)
			{
				Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
			}
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x000795CC File Offset: 0x000777CC
		public void Connect()
		{
			this.UserIdFormPanel.gameObject.SetActive(false);
			this.chatClient = new ChatClient(this, 0);
			this.chatClient.UseBackgroundWorkerForSending = true;
			this.chatClient.AuthValues = new AuthenticationValues(this.UserName);
			this.chatClient.ConnectUsingSettings(this.chatAppSettings);
			this.ChannelToggleToInstantiate.gameObject.SetActive(false);
			Debug.Log("Connecting as: " + this.UserName);
			this.ConnectingLabel.SetActive(true);
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x0007965D File Offset: 0x0007785D
		public void OnDestroy()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Disconnect(13);
			}
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x00079674 File Offset: 0x00077874
		public void OnApplicationQuit()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Disconnect(13);
			}
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x0007968C File Offset: 0x0007788C
		public void Update()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Service();
			}
			if (this.StateText == null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.StateText.gameObject.SetActive(this.ShowState);
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x000796DC File Offset: 0x000778DC
		public void OnEnterSend()
		{
			if (Input.GetKey(13) || Input.GetKey(271))
			{
				this.SendChatMessage(this.InputFieldChat.text);
				this.InputFieldChat.text = "";
			}
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x00079714 File Offset: 0x00077914
		public void OnClickSend()
		{
			if (this.InputFieldChat != null)
			{
				this.SendChatMessage(this.InputFieldChat.text);
				this.InputFieldChat.text = "";
			}
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x00079748 File Offset: 0x00077948
		private void SendChatMessage(string inputLine)
		{
			if (string.IsNullOrEmpty(inputLine))
			{
				return;
			}
			if ("test".Equals(inputLine))
			{
				if (this.TestLength != this.testBytes.Length)
				{
					this.testBytes = new byte[this.TestLength];
				}
				this.chatClient.SendPrivateMessage(this.chatClient.AuthValues.UserId, this.testBytes, true);
			}
			bool flag = this.chatClient.PrivateChannels.ContainsKey(this.selectedChannelName);
			string text = string.Empty;
			if (flag)
			{
				text = this.selectedChannelName.Split(new char[] { ':' })[1];
			}
			if (inputLine.get_Chars(0).Equals('\\'))
			{
				string[] array = inputLine.Split(new char[] { ' ' }, 2);
				if (array[0].Equals("\\help"))
				{
					this.PostHelpToCurrentChannel();
				}
				if (array[0].Equals("\\state"))
				{
					int num = 0;
					List<string> list = new List<string>();
					list.Add("i am state " + num.ToString());
					string[] array2 = array[1].Split(new char[] { ' ', ',' });
					if (array2.Length != 0)
					{
						num = int.Parse(array2[0]);
					}
					if (array2.Length > 1)
					{
						list.Add(array2[1]);
					}
					this.chatClient.SetOnlineStatus(num, list.ToArray());
					return;
				}
				if ((array[0].Equals("\\subscribe") || array[0].Equals("\\s")) && !string.IsNullOrEmpty(array[1]))
				{
					this.chatClient.Subscribe(array[1].Split(new char[] { ' ', ',' }));
					return;
				}
				if ((array[0].Equals("\\unsubscribe") || array[0].Equals("\\u")) && !string.IsNullOrEmpty(array[1]))
				{
					this.chatClient.Unsubscribe(array[1].Split(new char[] { ' ', ',' }));
					return;
				}
				if (array[0].Equals("\\clear"))
				{
					if (flag)
					{
						this.chatClient.PrivateChannels.Remove(this.selectedChannelName);
						return;
					}
					ChatChannel chatChannel;
					if (this.chatClient.TryGetChannel(this.selectedChannelName, flag, ref chatChannel))
					{
						chatChannel.ClearMessages();
						return;
					}
				}
				else if (array[0].Equals("\\msg") && !string.IsNullOrEmpty(array[1]))
				{
					string[] array3 = array[1].Split(new char[] { ' ', ',' }, 2);
					if (array3.Length < 2)
					{
						return;
					}
					string text2 = array3[0];
					string text3 = array3[1];
					this.chatClient.SendPrivateMessage(text2, text3, false);
					return;
				}
				else
				{
					if ((!array[0].Equals("\\join") && !array[0].Equals("\\j")) || string.IsNullOrEmpty(array[1]))
					{
						Debug.Log("The command '" + array[0] + "' is invalid.");
						return;
					}
					string[] array4 = array[1].Split(new char[] { ' ', ',' }, 2);
					if (this.channelToggles.ContainsKey(array4[0]))
					{
						this.ShowChannel(array4[0]);
						return;
					}
					this.chatClient.Subscribe(new string[] { array4[0] });
					return;
				}
			}
			else
			{
				if (flag)
				{
					this.chatClient.SendPrivateMessage(text, inputLine, false);
					return;
				}
				this.chatClient.PublishMessage(this.selectedChannelName, inputLine, false);
			}
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x00079AA0 File Offset: 0x00077CA0
		public void PostHelpToCurrentChannel()
		{
			Text currentChannelText = this.CurrentChannelText;
			currentChannelText.text += ChatGui.HelpText;
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x00079ABD File Offset: 0x00077CBD
		public void DebugReturn(DebugLevel level, string message)
		{
			if (level == 1)
			{
				Debug.LogError(message);
				return;
			}
			if (level == 2)
			{
				Debug.LogWarning(message);
				return;
			}
			Debug.Log(message);
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x00079ADC File Offset: 0x00077CDC
		public void OnConnected()
		{
			if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length != 0)
			{
				this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
			}
			this.ConnectingLabel.SetActive(false);
			this.UserIdText.text = "Connected as " + this.UserName;
			this.ChatPanel.gameObject.SetActive(true);
			if (this.FriendsList != null && this.FriendsList.Length != 0)
			{
				this.chatClient.AddFriends(this.FriendsList);
				foreach (string text in this.FriendsList)
				{
					if (this.FriendListUiItemtoInstantiate != null && text != this.UserName)
					{
						this.InstantiateFriendButton(text);
					}
				}
			}
			if (this.FriendListUiItemtoInstantiate != null)
			{
				this.FriendListUiItemtoInstantiate.SetActive(false);
			}
			this.chatClient.SetOnlineStatus(2);
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x00079BD0 File Offset: 0x00077DD0
		public void OnDisconnected()
		{
			Debug.Log("OnDisconnected()");
			this.ConnectingLabel.SetActive(false);
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x00079BE8 File Offset: 0x00077DE8
		public void OnChatStateChange(ChatState state)
		{
			this.StateText.text = state.ToString();
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x00079C04 File Offset: 0x00077E04
		public void OnSubscribed(string[] channels, bool[] results)
		{
			foreach (string text in channels)
			{
				this.chatClient.PublishMessage(text, "says 'hi'.", false);
				if (this.ChannelToggleToInstantiate != null)
				{
					this.InstantiateChannelButton(text);
				}
			}
			Debug.Log("OnSubscribed: " + string.Join(", ", channels));
			this.ShowChannel(channels[0]);
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x00079C70 File Offset: 0x00077E70
		public void OnSubscribed(string channel, string[] users, Dictionary<object, object> properties)
		{
			Debug.LogFormat("OnSubscribed: {0}, users.Count: {1} Channel-props: {2}.", new object[]
			{
				channel,
				users.Length,
				Extensions.ToStringFull(properties)
			});
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x00079C9C File Offset: 0x00077E9C
		private void InstantiateChannelButton(string channelName)
		{
			if (this.channelToggles.ContainsKey(channelName))
			{
				Debug.Log("Skipping creation for an existing channel toggle.");
				return;
			}
			Toggle toggle = Object.Instantiate<Toggle>(this.ChannelToggleToInstantiate);
			toggle.gameObject.SetActive(true);
			toggle.GetComponentInChildren<ChannelSelector>().SetChannel(channelName);
			toggle.transform.SetParent(this.ChannelToggleToInstantiate.transform.parent, false);
			this.channelToggles.Add(channelName, toggle);
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x00079D10 File Offset: 0x00077F10
		private void InstantiateFriendButton(string friendId)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.FriendListUiItemtoInstantiate);
			gameObject.gameObject.SetActive(true);
			FriendItem component = gameObject.GetComponent<FriendItem>();
			component.FriendId = friendId;
			gameObject.transform.SetParent(this.FriendListUiItemtoInstantiate.transform.parent, false);
			this.friendListItemLUT[friendId] = component;
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x00079D6C File Offset: 0x00077F6C
		public void OnUnsubscribed(string[] channels)
		{
			foreach (string text in channels)
			{
				if (this.channelToggles.ContainsKey(text))
				{
					Object.Destroy(this.channelToggles[text].gameObject);
					this.channelToggles.Remove(text);
					Debug.Log("Unsubscribed from channel '" + text + "'.");
					if (text == this.selectedChannelName && this.channelToggles.Count > 0)
					{
						IEnumerator<KeyValuePair<string, Toggle>> enumerator = this.channelToggles.GetEnumerator();
						enumerator.MoveNext();
						KeyValuePair<string, Toggle> keyValuePair = enumerator.Current;
						this.ShowChannel(keyValuePair.Key);
						keyValuePair = enumerator.Current;
						keyValuePair.Value.isOn = true;
					}
				}
				else
				{
					Debug.Log("Can't unsubscribe from channel '" + text + "' because you are currently not subscribed to it.");
				}
			}
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x00079E51 File Offset: 0x00078051
		public void OnGetMessages(string channelName, string[] senders, object[] messages)
		{
			if (channelName.Equals(this.selectedChannelName))
			{
				this.ShowChannel(this.selectedChannelName);
			}
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x00079E70 File Offset: 0x00078070
		public void OnPrivateMessage(string sender, object message, string channelName)
		{
			this.InstantiateChannelButton(channelName);
			byte[] array = message as byte[];
			if (array != null)
			{
				Debug.Log("Message with byte[].Length: " + array.Length.ToString());
			}
			if (this.selectedChannelName.Equals(channelName))
			{
				this.ShowChannel(channelName);
			}
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x00079EC0 File Offset: 0x000780C0
		public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
		{
			Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
			if (this.friendListItemLUT.ContainsKey(user))
			{
				FriendItem friendItem = this.friendListItemLUT[user];
				if (friendItem != null)
				{
					friendItem.OnFriendStatusUpdate(status, gotMessage, message);
				}
			}
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x00079F1D File Offset: 0x0007811D
		public void OnUserSubscribed(string channel, string user)
		{
			Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", new object[] { channel, user });
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x00079F37 File Offset: 0x00078137
		public void OnUserUnsubscribed(string channel, string user)
		{
			Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", new object[] { channel, user });
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x00079F51 File Offset: 0x00078151
		public void OnChannelPropertiesChanged(string channel, string userId, Dictionary<object, object> properties)
		{
			Debug.LogFormat("OnChannelPropertiesChanged: {0} by {1}. Props: {2}.", new object[]
			{
				channel,
				userId,
				Extensions.ToStringFull(properties)
			});
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x00079F74 File Offset: 0x00078174
		public void OnUserPropertiesChanged(string channel, string targetUserId, string senderUserId, Dictionary<object, object> properties)
		{
			Debug.LogFormat("OnUserPropertiesChanged: (channel:{0} user:{1}) by {2}. Props: {3}.", new object[]
			{
				channel,
				targetUserId,
				senderUserId,
				Extensions.ToStringFull(properties)
			});
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x00079F9C File Offset: 0x0007819C
		public void OnErrorInfo(string channel, string error, object data)
		{
			Debug.LogFormat("OnErrorInfo for channel {0}. Error: {1} Data: {2}", new object[] { channel, error, data });
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x00079FBC File Offset: 0x000781BC
		public void AddMessageToSelectedChannel(string msg)
		{
			ChatChannel chatChannel = null;
			if (!this.chatClient.TryGetChannel(this.selectedChannelName, ref chatChannel))
			{
				Debug.Log("AddMessageToSelectedChannel failed to find channel: " + this.selectedChannelName);
				return;
			}
			if (chatChannel != null)
			{
				chatChannel.Add("Bot", msg, 0);
			}
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x0007A008 File Offset: 0x00078208
		public void ShowChannel(string channelName)
		{
			if (string.IsNullOrEmpty(channelName))
			{
				return;
			}
			ChatChannel chatChannel = null;
			if (!this.chatClient.TryGetChannel(channelName, ref chatChannel))
			{
				Debug.Log("ShowChannel failed to find channel: " + channelName);
				return;
			}
			this.selectedChannelName = channelName;
			this.CurrentChannelText.text = chatChannel.ToStringMessages();
			Debug.Log("ShowChannel: " + this.selectedChannelName);
			foreach (KeyValuePair<string, Toggle> keyValuePair in this.channelToggles)
			{
				keyValuePair.Value.isOn = keyValuePair.Key == channelName;
			}
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x0007A0CC File Offset: 0x000782CC
		public void OpenDashboard()
		{
			Application.OpenURL("https://dashboard.photonengine.com");
		}

		// Token: 0x0400163D RID: 5693
		public string[] ChannelsToJoinOnConnect;

		// Token: 0x0400163E RID: 5694
		public string[] FriendsList;

		// Token: 0x0400163F RID: 5695
		public int HistoryLengthToFetch;

		// Token: 0x04001641 RID: 5697
		private string selectedChannelName;

		// Token: 0x04001642 RID: 5698
		public ChatClient chatClient;

		// Token: 0x04001643 RID: 5699
		protected internal ChatAppSettings chatAppSettings;

		// Token: 0x04001644 RID: 5700
		public GameObject missingAppIdErrorPanel;

		// Token: 0x04001645 RID: 5701
		public GameObject ConnectingLabel;

		// Token: 0x04001646 RID: 5702
		public RectTransform ChatPanel;

		// Token: 0x04001647 RID: 5703
		public GameObject UserIdFormPanel;

		// Token: 0x04001648 RID: 5704
		public InputField InputFieldChat;

		// Token: 0x04001649 RID: 5705
		public Text CurrentChannelText;

		// Token: 0x0400164A RID: 5706
		public Toggle ChannelToggleToInstantiate;

		// Token: 0x0400164B RID: 5707
		public GameObject FriendListUiItemtoInstantiate;

		// Token: 0x0400164C RID: 5708
		private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();

		// Token: 0x0400164D RID: 5709
		private readonly Dictionary<string, FriendItem> friendListItemLUT = new Dictionary<string, FriendItem>();

		// Token: 0x0400164E RID: 5710
		public bool ShowState = true;

		// Token: 0x0400164F RID: 5711
		public GameObject Title;

		// Token: 0x04001650 RID: 5712
		public Text StateText;

		// Token: 0x04001651 RID: 5713
		public Text UserIdText;

		// Token: 0x04001652 RID: 5714
		private static string HelpText = "\n    -- HELP --\nTo subscribe to channel(s) (channelnames are case sensitive) :  \n\t<color=#E07B00>\\subscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\s</color> <color=green><list of channelnames></color>\n\nTo leave channel(s):\n\t<color=#E07B00>\\unsubscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\u</color> <color=green><list of channelnames></color>\n\nTo switch the active channel\n\t<color=#E07B00>\\join</color> <color=green><channelname></color>\n\tor\n\t<color=#E07B00>\\j</color> <color=green><channelname></color>\n\nTo send a private message: (username are case sensitive)\n\t\\<color=#E07B00>msg</color> <color=green><username></color> <color=green><message></color>\n\nTo change status:\n\t\\<color=#E07B00>state</color> <color=green><stateIndex></color> <color=green><message></color>\n<color=green>0</color> = Offline <color=green>1</color> = Invisible <color=green>2</color> = Online <color=green>3</color> = Away \n<color=green>4</color> = Do not disturb <color=green>5</color> = Looking For Group <color=green>6</color> = Playing\n\nTo clear the current chat tab (private chats get closed):\n\t<color=#E07B00>\\clear</color>";

		// Token: 0x04001653 RID: 5715
		public int TestLength = 2048;

		// Token: 0x04001654 RID: 5716
		private byte[] testBytes = new byte[2048];
	}
}
