using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x0200039F RID: 927
	public class EventSystemSpawner : MonoBehaviour
	{
		// Token: 0x06001822 RID: 6178 RVA: 0x0007A2BA File Offset: 0x000784BA
		private void OnEnable()
		{
			if (Object.FindFirstObjectByType<EventSystem>() == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
			}
		}
	}
}
