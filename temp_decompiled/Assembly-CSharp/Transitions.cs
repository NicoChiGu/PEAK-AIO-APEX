using System;
using UnityEngine;

// Token: 0x02000357 RID: 855
public class Transitions : MonoBehaviour
{
	// Token: 0x060015ED RID: 5613 RVA: 0x000713E8 File Offset: 0x0006F5E8
	private void Awake()
	{
		Transitions.instance = this;
		this.transitions = base.GetComponentsInChildren<Transition>(true);
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x00071400 File Offset: 0x0006F600
	public void PlayTransition(TransitionType transitionType, Action action, float transitionInSpeed = 1f, float transitionOutSpeed = 1f)
	{
		Transitions.<>c__DisplayClass3_0 CS$<>8__locals1 = new Transitions.<>c__DisplayClass3_0();
		CS$<>8__locals1.transitionInSpeed = transitionInSpeed;
		CS$<>8__locals1.action = action;
		CS$<>8__locals1.transitionOutSpeed = transitionOutSpeed;
		CS$<>8__locals1.transition = this.GetTransition(transitionType);
		base.StartCoroutine(CS$<>8__locals1.<PlayTransition>g__IPlayTransition|0());
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x00071444 File Offset: 0x0006F644
	private Transition GetTransition(TransitionType transitionType)
	{
		for (int i = 0; i < this.transitions.Length; i++)
		{
			if (this.transitions[i].transitionType == transitionType)
			{
				return this.transitions[i];
			}
		}
		return null;
	}

	// Token: 0x040014C6 RID: 5318
	private Transition[] transitions;

	// Token: 0x040014C7 RID: 5319
	public static Transitions instance;
}
