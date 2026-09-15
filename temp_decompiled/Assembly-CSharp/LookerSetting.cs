using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000196 RID: 406
public class LookerSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000CC6 RID: 3270 RVA: 0x00041A1B File Offset: 0x0003FC1B
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x00041A1D File Offset: 0x0003FC1D
	protected override OffOnMode GetDefaultValue()
	{
		return 1;
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00041A20 File Offset: 0x0003FC20
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00041A23 File Offset: 0x0003FC23
	public string GetDisplayName()
	{
		return "ENABLETHELOOKER";
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x00041A2A File Offset: 0x0003FC2A
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x00041A31 File Offset: 0x0003FC31
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}
