using System;
using HorizonBasedAmbientOcclusion.Universal;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200015E RID: 350
public class PostHandler : MonoBehaviour
{
	// Token: 0x06000B38 RID: 2872 RVA: 0x0003BEA1 File Offset: 0x0003A0A1
	private void Start()
	{
		this.AOSetting = GameHandler.Instance.SettingsHandler.GetSetting<AOSetting>();
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0003BEB8 File Offset: 0x0003A0B8
	private void LateUpdate()
	{
		HBAO hbao;
		if (this.volume.sharedProfile.TryGet<HBAO>(ref hbao))
		{
			hbao.active = this.AOSetting.Value == 1;
		}
	}

	// Token: 0x04000A79 RID: 2681
	public AOSetting AOSetting;

	// Token: 0x04000A7A RID: 2682
	public Volume volume;
}
