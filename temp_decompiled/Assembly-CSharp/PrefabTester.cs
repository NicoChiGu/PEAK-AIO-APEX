using System;
using UnityEngine;

// Token: 0x020002CB RID: 715
public class PrefabTester : MonoBehaviour
{
	// Token: 0x06001354 RID: 4948 RVA: 0x000625F4 File Offset: 0x000607F4
	private void Awake()
	{
		this.instance = base.transform.GetChild(0).gameObject;
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x00062610 File Offset: 0x00060810
	public void Update()
	{
		if (Input.GetKeyDown(116))
		{
			if (this.instance != null)
			{
				Object.Destroy(this.instance);
			}
			this.instance = Object.Instantiate<GameObject>(this.prefab, base.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x04001200 RID: 4608
	public GameObject prefab;

	// Token: 0x04001201 RID: 4609
	public GameObject instance;
}
