using System;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x02000393 RID: 915
	public struct MicRef
	{
		// Token: 0x060017A8 RID: 6056 RVA: 0x00078677 File Offset: 0x00076877
		public MicRef(MicType micType, DeviceInfo device)
		{
			this.MicType = micType;
			this.Device = device;
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x00078687 File Offset: 0x00076887
		public override string ToString()
		{
			return string.Format("Mic reference: {0}", this.Device.Name);
		}

		// Token: 0x04001613 RID: 5651
		public readonly MicType MicType;

		// Token: 0x04001614 RID: 5652
		public readonly DeviceInfo Device;
	}
}
