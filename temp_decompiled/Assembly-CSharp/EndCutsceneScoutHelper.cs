using System;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class EndCutsceneScoutHelper : MonoBehaviour
{
	// Token: 0x0600111C RID: 4380 RVA: 0x000561F0 File Offset: 0x000543F0
	private void OnEnable()
	{
		base.GetComponent<Animator>().SetBool("Alone", this.alone);
	}

	// Token: 0x04000F8A RID: 3978
	public bool alone;
}
