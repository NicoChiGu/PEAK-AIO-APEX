using System;
using Photon.Realtime;

namespace Peak.Network
{
	// Token: 0x020003C2 RID: 962
	public interface ISessionAPI : ISessionEvents
	{
		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060018AD RID: 6317 RVA: 0x0007C891 File Offset: 0x0007AA91
		[Obsolete("Call IsHost instead")]
		bool IsMasterClient
		{
			get
			{
				return this.IsHost;
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060018AE RID: 6318
		bool IsHost { get; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060018AF RID: 6319
		int HostId { get; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060018B0 RID: 6320
		bool IsConnectedAndReady { get; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060018B1 RID: 6321
		// (set) Token: 0x060018B2 RID: 6322
		string NickName { get; set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060018B3 RID: 6323
		int SeatNumber { get; }

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060018B4 RID: 6324
		// (set) Token: 0x060018B5 RID: 6325
		AuthenticationValues AuthValues { get; set; }

		// Token: 0x060018B6 RID: 6326
		void InitializeNetcodeBackend();

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060018B7 RID: 6327
		bool InRoom { get; }

		// Token: 0x060018B8 RID: 6328
		void Kick(string id);
	}
}
