using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000295 RID: 661
public class LocalPlayerEvent : MonoBehaviour
{
	// Token: 0x0600123D RID: 4669 RVA: 0x0005C575 File Offset: 0x0005A775
	public void Start()
	{
		if (base.GetComponentInParent<Character>().IsLocal)
		{
			this.isLocalEvent.Invoke();
		}
	}

	// Token: 0x040010BE RID: 4286
	public UnityEvent isLocalEvent;
}
