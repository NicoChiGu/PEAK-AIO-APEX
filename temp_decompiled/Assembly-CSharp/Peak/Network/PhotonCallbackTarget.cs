using System;
using Photon.Pun;
using UnityEngine;

namespace Peak.Network
{
	// Token: 0x020003C7 RID: 967
	public abstract class PhotonCallbackTarget : IDisposable
	{
		// Token: 0x060018F3 RID: 6387 RVA: 0x0007CE71 File Offset: 0x0007B071
		protected PhotonCallbackTarget()
		{
			Application.quitting += new Action(this.Dispose);
			PhotonNetwork.AddCallbackTarget(this);
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x0007CE91 File Offset: 0x0007B091
		public virtual void Dispose()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
			Application.quitting -= new Action(this.Dispose);
		}
	}
}
