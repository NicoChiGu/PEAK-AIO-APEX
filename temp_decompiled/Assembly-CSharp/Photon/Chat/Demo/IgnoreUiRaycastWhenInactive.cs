using System;
using UnityEngine;

namespace Photon.Chat.Demo
{
	// Token: 0x0200039D RID: 925
	public class IgnoreUiRaycastWhenInactive : MonoBehaviour, ICanvasRaycastFilter
	{
		// Token: 0x0600181C RID: 6172 RVA: 0x0007A203 File Offset: 0x00078403
		public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			return base.gameObject.activeInHierarchy;
		}
	}
}
