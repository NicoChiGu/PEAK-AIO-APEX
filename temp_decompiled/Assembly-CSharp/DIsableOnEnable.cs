using System;
using UnityEngine;

// Token: 0x0200024C RID: 588
public class DIsableOnEnable : MonoBehaviour
{
	// Token: 0x0600110B RID: 4363 RVA: 0x00055ED3 File Offset: 0x000540D3
	private void OnEnable()
	{
		this.objectToDisable.SetActive(false);
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x00055EE1 File Offset: 0x000540E1
	private void OnDisable()
	{
		this.objectToDisable.SetActive(true);
	}

	// Token: 0x04000F7A RID: 3962
	public GameObject objectToDisable;
}
