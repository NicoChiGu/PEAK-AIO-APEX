using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001F1 RID: 497
public class VersionString : MonoBehaviour
{
	// Token: 0x06000F0F RID: 3855 RVA: 0x00049B8E File Offset: 0x00047D8E
	private void Start()
	{
		this.m_text = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x00049B9C File Offset: 0x00047D9C
	private void Update()
	{
		BuildVersion buildVersion;
		buildVersion..ctor(Application.version, "???");
		this.m_text.text = buildVersion.ToString();
		if (string.IsNullOrEmpty(buildVersion.BuildName))
		{
			this.m_text.text = "v" + buildVersion.ToString();
		}
		if (PhotonNetwork.InRoom)
		{
			ConnectionService service = GameHandler.GetService<ConnectionService>();
			if (service != null)
			{
				InRoomState inRoomState = service.StateMachine.CurrentState as InRoomState;
				if (inRoomState != null && !string.IsNullOrEmpty(inRoomState.verifiedLobby))
				{
					TextMeshProUGUI text = this.m_text;
					text.text = string.Concat(new string[]
					{
						text.text,
						" - ",
						PhotonNetwork.CloudRegion,
						" - ",
						inRoomState.verifiedLobby
					});
				}
			}
		}
	}

	// Token: 0x04000D14 RID: 3348
	private TextMeshProUGUI m_text;
}
