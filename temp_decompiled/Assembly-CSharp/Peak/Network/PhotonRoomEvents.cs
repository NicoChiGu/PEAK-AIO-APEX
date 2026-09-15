using System;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Peak.Network
{
	// Token: 0x020003C8 RID: 968
	public class PhotonRoomEvents : PhotonCallbackTarget, IInRoomCallbacks, INetworkRoomEvents
	{
		// Token: 0x14000016 RID: 22
		// (add) Token: 0x060018F5 RID: 6389 RVA: 0x0007CEAC File Offset: 0x0007B0AC
		// (remove) Token: 0x060018F6 RID: 6390 RVA: 0x0007CEE4 File Offset: 0x0007B0E4
		public event Action PlayerEntered;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x060018F7 RID: 6391 RVA: 0x0007CF1C File Offset: 0x0007B11C
		// (remove) Token: 0x060018F8 RID: 6392 RVA: 0x0007CF54 File Offset: 0x0007B154
		public event Action PlayerLeft;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x060018F9 RID: 6393 RVA: 0x0007CF8C File Offset: 0x0007B18C
		// (remove) Token: 0x060018FA RID: 6394 RVA: 0x0007CFC4 File Offset: 0x0007B1C4
		public event Action RoomOwnerChanged;

		// Token: 0x060018FB RID: 6395 RVA: 0x0007CFF9 File Offset: 0x0007B1F9
		public override void Dispose()
		{
			base.Dispose();
			this.PlayerLeft = null;
			this.RoomOwnerChanged = null;
		}

		// Token: 0x060018FC RID: 6396 RVA: 0x0007D00F File Offset: 0x0007B20F
		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			Action playerEntered = this.PlayerEntered;
			if (playerEntered == null)
			{
				return;
			}
			playerEntered.Invoke();
		}

		// Token: 0x060018FD RID: 6397 RVA: 0x0007D021 File Offset: 0x0007B221
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x060018FE RID: 6398 RVA: 0x0007D023 File Offset: 0x0007B223
		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
		}

		// Token: 0x060018FF RID: 6399 RVA: 0x0007D025 File Offset: 0x0007B225
		public void OnMasterClientSwitched(Player newMasterClient)
		{
			Action roomOwnerChanged = this.RoomOwnerChanged;
			if (roomOwnerChanged == null)
			{
				return;
			}
			roomOwnerChanged.Invoke();
		}

		// Token: 0x06001900 RID: 6400 RVA: 0x0007D037 File Offset: 0x0007B237
		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			Action playerLeft = this.PlayerLeft;
			if (playerLeft == null)
			{
				return;
			}
			playerLeft.Invoke();
		}
	}
}
