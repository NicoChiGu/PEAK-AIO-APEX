using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	// Token: 0x0200038E RID: 910
	public class SidebarToggle : MonoBehaviour
	{
		// Token: 0x06001757 RID: 5975 RVA: 0x00076FE6 File Offset: 0x000751E6
		private void Awake()
		{
			this.sidebarButton.onClick.RemoveAllListeners();
			this.sidebarButton.onClick.AddListener(new UnityAction(this.ToggleSidebar));
			this.ToggleSidebar(this.sidebarOpen);
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x00077020 File Offset: 0x00075220
		[ContextMenu("ToggleSidebar")]
		private void ToggleSidebar()
		{
			this.sidebarOpen = !this.sidebarOpen;
			this.ToggleSidebar(this.sidebarOpen);
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x0007703D File Offset: 0x0007523D
		private void ToggleSidebar(bool open)
		{
			if (!open)
			{
				this.panelsHolder.SetPosX(0f);
				return;
			}
			this.panelsHolder.SetPosX(this.sidebarWidth);
		}

		// Token: 0x040015D1 RID: 5585
		[SerializeField]
		private Button sidebarButton;

		// Token: 0x040015D2 RID: 5586
		[SerializeField]
		private RectTransform panelsHolder;

		// Token: 0x040015D3 RID: 5587
		private float sidebarWidth = 300f;

		// Token: 0x040015D4 RID: 5588
		private bool sidebarOpen = true;
	}
}
