using System;
using UnityEngine;

// Token: 0x02000100 RID: 256
[RequireComponent(typeof(Item))]
public class Antigrav : MonoBehaviour
{
	// Token: 0x06000874 RID: 2164 RVA: 0x0002E460 File Offset: 0x0002C660
	private void Start()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0002E46E File Offset: 0x0002C66E
	private void FixedUpdate()
	{
		if (this.item.itemState == ItemState.Ground)
		{
			this.item.rig.AddForce(-Physics.gravity * this.intensity, 5);
		}
	}

	// Token: 0x0400081D RID: 2077
	private Item item;

	// Token: 0x0400081E RID: 2078
	public float intensity = 1f;
}
