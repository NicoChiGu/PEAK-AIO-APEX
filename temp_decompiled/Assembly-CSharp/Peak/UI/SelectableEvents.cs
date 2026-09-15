using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Peak.UI
{
	// Token: 0x020003B9 RID: 953
	public class SelectableEvents : MonoBehaviour, ISubmitHandler, IEventSystemHandler
	{
		// Token: 0x1400000A RID: 10
		// (add) Token: 0x0600187C RID: 6268 RVA: 0x0007C520 File Offset: 0x0007A720
		// (remove) Token: 0x0600187D RID: 6269 RVA: 0x0007C558 File Offset: 0x0007A758
		public event Action<BaseEventData> Submitted;

		// Token: 0x0600187E RID: 6270 RVA: 0x0007C58D File Offset: 0x0007A78D
		public void OnSubmit(BaseEventData eventData)
		{
			Action<BaseEventData> submitted = this.Submitted;
			if (submitted == null)
			{
				return;
			}
			submitted.Invoke(eventData);
		}
	}
}
