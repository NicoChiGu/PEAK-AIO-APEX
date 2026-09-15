using System;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000385 RID: 901
	[RequireComponent(typeof(Toggle))]
	[DisallowMultipleComponent]
	public class BetterToggle : MonoBehaviour
	{
		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600170A RID: 5898 RVA: 0x00075704 File Offset: 0x00073904
		// (remove) Token: 0x0600170B RID: 5899 RVA: 0x00075738 File Offset: 0x00073938
		public static event BetterToggle.OnToggle ToggleValueChanged;

		// Token: 0x0600170C RID: 5900 RVA: 0x0007576B File Offset: 0x0007396B
		private void Start()
		{
			this.toggle = base.GetComponent<Toggle>();
			this.toggle.onValueChanged.AddListener(delegate
			{
				this.OnToggleValueChanged();
			});
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x00075795 File Offset: 0x00073995
		public void OnToggleValueChanged()
		{
			if (BetterToggle.ToggleValueChanged != null)
			{
				BetterToggle.ToggleValueChanged(this.toggle);
			}
		}

		// Token: 0x04001591 RID: 5521
		private Toggle toggle;

		// Token: 0x0200052A RID: 1322
		// (Invoke) Token: 0x06001DCE RID: 7630
		public delegate void OnToggle(Toggle toggle);
	}
}
