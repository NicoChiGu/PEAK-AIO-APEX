using System;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class LavaRiverSFXPos : MonoBehaviour
{
	// Token: 0x0600121A RID: 4634 RVA: 0x0005C0D4 File Offset: 0x0005A2D4
	private void Update()
	{
		if (MainCamera.instance)
		{
			base.transform.position = new Vector3(MainCamera.instance.transform.position.x, base.transform.position.y, MainCamera.instance.transform.position.z);
			if (base.transform.position.z < 1050f)
			{
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 1050f);
			}
		}
	}
}
