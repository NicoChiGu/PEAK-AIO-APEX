using System;
using Photon.Pun;

// Token: 0x02000151 RID: 337
public abstract class OnNetworkStart : MonoBehaviourPunCallbacks
{
	// Token: 0x06000AD1 RID: 2769 RVA: 0x00039E8E File Offset: 0x0003808E
	private void Start()
	{
		this.TryRunningNetworkStart();
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00039E96 File Offset: 0x00038096
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.TryRunningNetworkStart();
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x00039EA4 File Offset: 0x000380A4
	private void TryRunningNetworkStart()
	{
		if (this.hasRunNetworkStart)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			this.NetworkStart();
			this.hasRunNetworkStart = true;
		}
	}

	// Token: 0x06000AD4 RID: 2772
	public abstract void NetworkStart();

	// Token: 0x04000A14 RID: 2580
	private bool hasRunNetworkStart;
}
