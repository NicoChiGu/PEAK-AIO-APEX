using System;

namespace Peak.Network
{
	// Token: 0x020003BF RID: 959
	public interface INetworkRoomEvents
	{
		// Token: 0x1400000B RID: 11
		// (add) Token: 0x060018A1 RID: 6305
		// (remove) Token: 0x060018A2 RID: 6306
		event Action PlayerEntered;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x060018A3 RID: 6307
		// (remove) Token: 0x060018A4 RID: 6308
		event Action PlayerLeft;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x060018A5 RID: 6309
		// (remove) Token: 0x060018A6 RID: 6310
		event Action RoomOwnerChanged;
	}
}
