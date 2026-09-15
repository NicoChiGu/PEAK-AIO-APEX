using System;
using UnityEngine;

// Token: 0x02000250 RID: 592
public class EnabledRandom : MonoBehaviour
{
	// Token: 0x06001118 RID: 4376 RVA: 0x0005619F File Offset: 0x0005439F
	private void Start()
	{
		this.odds = Random.Range(0, 4);
		if (this.odds < 2)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000F87 RID: 3975
	public int odds = 1;
}
