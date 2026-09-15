using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000356 RID: 854
public abstract class Transition : MonoBehaviour
{
	// Token: 0x060015EA RID: 5610
	public abstract IEnumerator TransitionIn(float speed = 1f);

	// Token: 0x060015EB RID: 5611
	public abstract IEnumerator TransitionOut(float speed = 1f);

	// Token: 0x040014C5 RID: 5317
	public TransitionType transitionType;
}
