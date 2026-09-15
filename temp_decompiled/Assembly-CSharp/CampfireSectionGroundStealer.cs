using System;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class CampfireSectionGroundStealer : MonoBehaviour
{
	// Token: 0x060004FC RID: 1276 RVA: 0x0001D594 File Offset: 0x0001B794
	private void Awake()
	{
		foreach (object obj in this.groundParent.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.GetComponent<MeshRenderer>().bounds.center.y > base.transform.position.y + this.offset)
			{
				transform.SetParent(base.transform, true);
			}
		}
	}

	// Token: 0x04000561 RID: 1377
	public float offset;

	// Token: 0x04000562 RID: 1378
	public GameObject groundParent;
}
