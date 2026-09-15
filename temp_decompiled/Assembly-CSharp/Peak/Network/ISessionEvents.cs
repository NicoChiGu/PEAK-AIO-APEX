using System;

namespace Peak.Network
{
	// Token: 0x020003C0 RID: 960
	public interface ISessionEvents
	{
		// Token: 0x1400000E RID: 14
		// (add) Token: 0x060018A7 RID: 6311
		// (remove) Token: 0x060018A8 RID: 6312
		event Action ConnectedAndReady;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x060018A9 RID: 6313
		// (remove) Token: 0x060018AA RID: 6314
		event Action<INetworkEventData> NetworkEventReceived;
	}
}
