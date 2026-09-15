using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200037F RID: 895
	[ExecuteInEditMode]
	public class MobileControlRig : MonoBehaviour
	{
		// Token: 0x060016BE RID: 5822 RVA: 0x00074DA1 File Offset: 0x00072FA1
		private void OnEnable()
		{
			this.CheckEnableControlRig();
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x00074DA9 File Offset: 0x00072FA9
		private void Start()
		{
			if (Object.FindObjectOfType<EventSystem>() == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
			}
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x00074DCF File Offset: 0x00072FCF
		private void CheckEnableControlRig()
		{
			this.EnableControlRig(false);
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x00074DD8 File Offset: 0x00072FD8
		private void EnableControlRig(bool enabled)
		{
			try
			{
				foreach (object obj in base.transform)
				{
					((Transform)obj).gameObject.SetActive(enabled);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
