using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class CastToGround : MonoBehaviour
{
	// Token: 0x060004FE RID: 1278 RVA: 0x0001D634 File Offset: 0x0001B834
	private void Start()
	{
		if (this.castOnStart)
		{
			this.castToGround();
		}
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0001D644 File Offset: 0x0001B844
	public void castToGround()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, ref raycastHit))
		{
			base.transform.position = raycastHit.point + this.offset;
			base.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001D6A3 File Offset: 0x0001B8A3
	private void Update()
	{
	}

	// Token: 0x04000563 RID: 1379
	public bool castOnStart = true;

	// Token: 0x04000564 RID: 1380
	public Vector3 offset;
}
