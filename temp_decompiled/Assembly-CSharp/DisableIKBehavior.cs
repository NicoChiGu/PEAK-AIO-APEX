using System;
using UnityEngine;

// Token: 0x0200024B RID: 587
public class DisableIKBehavior : StateMachineBehaviour
{
	// Token: 0x06001109 RID: 4361 RVA: 0x00055EB4 File Offset: 0x000540B4
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponentInParent<Character>().data.overrideIKForSeconds = 0.1f;
	}
}
