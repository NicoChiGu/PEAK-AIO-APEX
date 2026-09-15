using System;
using UnityEngine;

// Token: 0x02000129 RID: 297
[RequireComponent(typeof(Item))]
public class RotateTransformOnPickup : MonoBehaviour
{
	// Token: 0x0600096B RID: 2411 RVA: 0x00031FAC File Offset: 0x000301AC
	private void Start()
	{
		if (this.item.itemState == ItemState.Held)
		{
			this.transformToRotate.localEulerAngles += this.rotation;
		}
	}

	// Token: 0x040008CB RID: 2251
	public Vector3 rotation;

	// Token: 0x040008CC RID: 2252
	public Transform transformToRotate;

	// Token: 0x040008CD RID: 2253
	public Item item;
}
