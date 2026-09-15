using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200027D RID: 637
public class JustEvent : MonoBehaviour
{
	// Token: 0x060011D4 RID: 4564 RVA: 0x0005A3EF File Offset: 0x000585EF
	private void CallEvent1()
	{
		this.event1.Invoke();
	}

	// Token: 0x04001054 RID: 4180
	public UnityEvent event1;
}
