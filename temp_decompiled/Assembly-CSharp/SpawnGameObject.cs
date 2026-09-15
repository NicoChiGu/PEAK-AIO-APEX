using System;
using UnityEngine;

// Token: 0x02000338 RID: 824
public class SpawnGameObject : MonoBehaviour
{
	// Token: 0x0600153D RID: 5437 RVA: 0x0006C43D File Offset: 0x0006A63D
	public void Go()
	{
		Object.Instantiate<GameObject>(this.toSpawn, base.transform.position, base.transform.rotation);
	}

	// Token: 0x040013BF RID: 5055
	public GameObject toSpawn;
}
