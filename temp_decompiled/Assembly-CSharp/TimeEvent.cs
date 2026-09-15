using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000350 RID: 848
public class TimeEvent : MonoBehaviour
{
	// Token: 0x060015C5 RID: 5573 RVA: 0x00070480 File Offset: 0x0006E680
	private void Update()
	{
		this.counter += Time.deltaTime;
		if (this.counter > this.rate)
		{
			if (!this.repeating)
			{
				base.enabled = false;
			}
			this.timeEvent.Invoke();
			this.counter = 0f;
		}
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x000704D2 File Offset: 0x0006E6D2
	private void OnEnable()
	{
		this.counter = 0f;
	}

	// Token: 0x0400149E RID: 5278
	private float counter;

	// Token: 0x0400149F RID: 5279
	public float rate = 2f;

	// Token: 0x040014A0 RID: 5280
	public bool repeating;

	// Token: 0x040014A1 RID: 5281
	public UnityEvent timeEvent;
}
