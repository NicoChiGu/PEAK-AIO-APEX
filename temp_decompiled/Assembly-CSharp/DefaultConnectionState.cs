using System;
using Peak.Network;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class DefaultConnectionState : ConnectionState
{
	// Token: 0x17000066 RID: 102
	// (get) Token: 0x06000565 RID: 1381 RVA: 0x0001F66B File Offset: 0x0001D86B
	private bool InLobby
	{
		get
		{
			return NetCode.Matchmaking.InLobby;
		}
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0001F677 File Offset: 0x0001D877
	public override void Enter()
	{
		base.Enter();
		if (Time.frameCount > 3 && this.InLobby)
		{
			GameHandler.GetService<SteamLobbyHandler>().LeaveLobby();
		}
	}
}
