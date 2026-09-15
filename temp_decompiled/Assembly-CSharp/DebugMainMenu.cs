using System;
using System.Linq;
using TMPro;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000097 RID: 151
public class DebugMainMenu : MonoBehaviour
{
	// Token: 0x060005D1 RID: 1489 RVA: 0x00021240 File Offset: 0x0001F440
	private void Start()
	{
		this.m_matchmakeButton.onClick.AddListener(new UnityAction(this.MatchmakeClicked));
		this.m_debugJoinButton.onClick.AddListener(new UnityAction(this.DebugJoinClicked));
		this.m_debugCreateButton.onClick.AddListener(new UnityAction(this.DebugCreateClicked));
		this.m_debugRejoinButton.onClick.AddListener(new UnityAction(this.DebugRejoinClicked));
		if (this.debugJoinOnAwake)
		{
			this.DebugHaxxClicked();
		}
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x000212CB File Offset: 0x0001F4CB
	private void DebugRejoinClicked()
	{
		Debug.Log("Rejoining...");
		GameHandler.GetService<ConnectionService>();
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x000212E7 File Offset: 0x0001F4E7
	private void DebugCreateClicked()
	{
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<HostState>(false).RoomName = "THEPETHEN";
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x0002130D File Offset: 0x0001F50D
	private void DebugJoinClicked()
	{
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false).RoomName = "THEPETHEN";
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00021334 File Offset: 0x0001F534
	private void DebugHaxxClicked()
	{
		ConnectionService service = GameHandler.GetService<ConnectionService>();
		if (Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "Client") || !DebugMainMenu.first)
		{
			service.StateMachine.SwitchState<JoinSpecificRoomState>(false).RoomName = "THEPETHEN";
		}
		else
		{
			service.StateMachine.SwitchState<HostState>(false).RoomName = "THEPETHEN";
		}
		DebugMainMenu.first = false;
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x000213A0 File Offset: 0x0001F5A0
	private void MatchmakeClicked()
	{
		if (string.IsNullOrEmpty(this.m_usernameField.text))
		{
			Debug.LogError("Failed to get username field...");
			return;
		}
		if (string.IsNullOrEmpty(this.m_roomField.text))
		{
			Debug.LogError("Failed to get room name field...");
			return;
		}
		JoinSpecificRoomState joinSpecificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false);
		joinSpecificRoomState.RoomName = this.m_roomField.text.ToLower();
		joinSpecificRoomState.RegionToJoin = "eu";
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x040005F6 RID: 1526
	[SerializeField]
	private Button m_matchmakeButton;

	// Token: 0x040005F7 RID: 1527
	[SerializeField]
	private Button m_debugJoinButton;

	// Token: 0x040005F8 RID: 1528
	[SerializeField]
	private Button m_debugCreateButton;

	// Token: 0x040005F9 RID: 1529
	[SerializeField]
	private Button m_debugRejoinButton;

	// Token: 0x040005FA RID: 1530
	[SerializeField]
	private TMP_InputField m_usernameField;

	// Token: 0x040005FB RID: 1531
	[SerializeField]
	private TMP_InputField m_roomField;

	// Token: 0x040005FC RID: 1532
	public bool debugJoinOnAwake = true;

	// Token: 0x040005FD RID: 1533
	private static bool first = true;
}
