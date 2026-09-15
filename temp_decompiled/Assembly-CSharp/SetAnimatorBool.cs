using System;
using UnityEngine;

// Token: 0x02000323 RID: 803
public class SetAnimatorBool : MonoBehaviour
{
	// Token: 0x060014B8 RID: 5304 RVA: 0x00069984 File Offset: 0x00067B84
	private void Update()
	{
		base.GetComponent<Animator>().SetBool(this.param, this.on);
	}

	// Token: 0x04001333 RID: 4915
	public string param = "Enabled";

	// Token: 0x04001334 RID: 4916
	public bool on;
}
