using System;
using UnityEngine;

// Token: 0x0200030A RID: 778
public class RandomAct : MonoBehaviour
{
	// Token: 0x0600142D RID: 5165 RVA: 0x0006648F File Offset: 0x0006468F
	private void Start()
	{
		base.GetComponent<Animator>().SetInteger("Act", this.act);
	}

	// Token: 0x040012B0 RID: 4784
	public int act;
}
