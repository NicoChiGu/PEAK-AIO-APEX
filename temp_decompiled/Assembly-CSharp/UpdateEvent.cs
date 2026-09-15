using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000364 RID: 868
public class UpdateEvent : MonoBehaviour
{
	// Token: 0x0600161F RID: 5663 RVA: 0x000723E8 File Offset: 0x000705E8
	private void Update()
	{
		this.updateEvent.Invoke();
	}

	// Token: 0x04001507 RID: 5383
	public UnityEvent updateEvent;
}
