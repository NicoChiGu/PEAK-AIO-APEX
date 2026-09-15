using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000BA RID: 186
public class GameOverHandler : Singleton<GameOverHandler>
{
	// Token: 0x060006CF RID: 1743 RVA: 0x00026D98 File Offset: 0x00024F98
	protected override void Awake()
	{
		base.Awake();
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x00026DAC File Offset: 0x00024FAC
	public void LocalPlayerHasClosedEndScreen()
	{
		this.view.RPC("PlayerHasClosedEndScreen", 0, Array.Empty<object>());
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00026DC4 File Offset: 0x00024FC4
	[PunRPC]
	public void PlayerHasClosedEndScreen(PhotonMessageInfo messageInfo)
	{
		int actorNumber = messageInfo.Sender.ActorNumber;
		Player player;
		if (!PlayerHandler.TryGetPlayer(actorNumber, out player))
		{
			Debug.LogError(string.Format("Player not found: {0}", actorNumber));
			return;
		}
		player.hasClosedEndScreen = true;
		Debug.Log(string.Format("{0} Player has closed end screen", player));
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00026E14 File Offset: 0x00025014
	public void LoadAirport()
	{
		this.view.RPC("LoadAirportMaster", 2, Array.Empty<object>());
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00026E2C File Offset: 0x0002502C
	[PunRPC]
	public void LoadAirportMaster()
	{
		this.view.RPC("BeginAirportLoadRPC", 0, Array.Empty<object>());
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00026E44 File Offset: 0x00025044
	[PunRPC]
	public void BeginAirportLoadRPC()
	{
		Debug.Log("Load Island RPC..");
		SceneSwitchingStatus sceneSwitchingStatus;
		if (GameHandler.TryGetStatus<SceneSwitchingStatus>(out sceneSwitchingStatus))
		{
			Debug.Log("Already loading... ");
			return;
		}
		GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 0f) });
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00026EA4 File Offset: 0x000250A4
	public void ForceEveryPlayerDoneWithEndScreen()
	{
		this.view.RPC("ForceEveryPlayerDoneWithEndScreenRPC", 0, Array.Empty<object>());
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00026EBC File Offset: 0x000250BC
	[PunRPC]
	public void ForceEveryPlayerDoneWithEndScreenRPC()
	{
		Debug.Log("Force every player closed end screen");
		foreach (Player player in PlayerHandler.GetAllPlayers())
		{
			player.hasClosedEndScreen = true;
		}
	}

	// Token: 0x040006DD RID: 1757
	private PhotonView view;
}
