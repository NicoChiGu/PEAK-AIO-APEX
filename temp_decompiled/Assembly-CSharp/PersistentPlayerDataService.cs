using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.PhotonUtility;

// Token: 0x0200008B RID: 139
public class PersistentPlayerDataService : GameService, IDisposable
{
	// Token: 0x06000590 RID: 1424 RVA: 0x0001FC84 File Offset: 0x0001DE84
	public PersistentPlayerDataService()
	{
		this.syncPersistentPlayerDataHandle = CustomCommands<CustomCommandType>.RegisterListener<SyncPersistentPlayerDataPackage>(new Action<SyncPersistentPlayerDataPackage>(this.OnSyncReceived));
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0001FCB9 File Offset: 0x0001DEB9
	public void Dispose()
	{
		CustomCommands<CustomCommandType>.UnregisterListener(this.syncPersistentPlayerDataHandle);
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x0001FCC8 File Offset: 0x0001DEC8
	private void OnSyncReceived(SyncPersistentPlayerDataPackage package)
	{
		Debug.Log("On Sync Received!");
		this.PersistentPlayerDatas[package.ActorNumber] = package.Data;
		if (this.OnChangeActions.ContainsKey(package.ActorNumber))
		{
			this.OnChangeActions[package.ActorNumber].Invoke(package.Data);
		}
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0001FD25 File Offset: 0x0001DF25
	public PersistentPlayerData GetPlayerData(Player player)
	{
		return this.GetPlayerData(player.ActorNumber);
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x0001FD34 File Offset: 0x0001DF34
	public PersistentPlayerData GetPlayerData(int actorNumber)
	{
		if (!this.PersistentPlayerDatas.ContainsKey(actorNumber))
		{
			this.PersistentPlayerDatas[actorNumber] = new PersistentPlayerData();
			Debug.Log(string.Format("Initializing player data for player: {0}", actorNumber));
		}
		return this.PersistentPlayerDatas[actorNumber];
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0001FD84 File Offset: 0x0001DF84
	public void SetPlayerData(Player player, PersistentPlayerData playerData)
	{
		this.PersistentPlayerDatas[player.ActorNumber] = playerData;
		Debug.Log("Setting Player Data for: " + player.NickName);
		if (this.OnChangeActions.ContainsKey(player.ActorNumber))
		{
			Action<PersistentPlayerData> action = this.OnChangeActions[player.ActorNumber];
			if (action != null)
			{
				action.Invoke(playerData);
			}
		}
		CustomCommands<CustomCommandType>.SendPackage(new SyncPersistentPlayerDataPackage
		{
			Data = playerData,
			ActorNumber = player.ActorNumber
		}, 0);
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x0001FE08 File Offset: 0x0001E008
	public void SubscribeToPlayerDataChange(Player player, Action<PersistentPlayerData> onChange)
	{
		if (!this.OnChangeActions.ContainsKey(player.ActorNumber))
		{
			this.OnChangeActions[player.ActorNumber] = onChange;
			return;
		}
		Dictionary<int, Action<PersistentPlayerData>> onChangeActions = this.OnChangeActions;
		int actorNumber = player.ActorNumber;
		onChangeActions[actorNumber] = (Action<PersistentPlayerData>)Delegate.Combine(onChangeActions[actorNumber], onChange);
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x0001FE64 File Offset: 0x0001E064
	public void UnsubscribeToPlayerDataChange(Player player, Action<PersistentPlayerData> onChange)
	{
		if (this.OnChangeActions.ContainsKey(player.ActorNumber))
		{
			Dictionary<int, Action<PersistentPlayerData>> onChangeActions = this.OnChangeActions;
			int actorNumber = player.ActorNumber;
			onChangeActions[actorNumber] = (Action<PersistentPlayerData>)Delegate.Remove(onChangeActions[actorNumber], onChange);
		}
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x0001FEAC File Offset: 0x0001E0AC
	public void SyncToPlayer(Player newPlayer)
	{
		foreach (KeyValuePair<int, PersistentPlayerData> keyValuePair in this.PersistentPlayerDatas)
		{
			int num;
			PersistentPlayerData persistentPlayerData;
			keyValuePair.Deconstruct(ref num, ref persistentPlayerData);
			int num2 = num;
			PersistentPlayerData persistentPlayerData2 = persistentPlayerData;
			Player player;
			if (PhotonNetwork.TryGetPlayer(num2, ref player) && !player.IsInactive)
			{
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					TargetActors = new int[] { newPlayer.ActorNumber }
				};
				CustomCommands<CustomCommandType>.SendPackage(new SyncPersistentPlayerDataPackage
				{
					Data = persistentPlayerData2,
					ActorNumber = num2
				}, raiseEventOptions);
			}
		}
	}

	// Token: 0x040005B4 RID: 1460
	private Dictionary<int, PersistentPlayerData> PersistentPlayerDatas = new Dictionary<int, PersistentPlayerData>();

	// Token: 0x040005B5 RID: 1461
	private Dictionary<int, Action<PersistentPlayerData>> OnChangeActions = new Dictionary<int, Action<PersistentPlayerData>>();

	// Token: 0x040005B6 RID: 1462
	private ListenerHandle syncPersistentPlayerDataHandle;
}
