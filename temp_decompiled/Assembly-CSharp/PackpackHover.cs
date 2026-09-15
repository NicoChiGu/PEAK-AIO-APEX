using System;
using UnityEngine;

// Token: 0x020002B2 RID: 690
public class PackpackHover : MonoBehaviour
{
	// Token: 0x060012E1 RID: 4833 RVA: 0x0005FFB8 File Offset: 0x0005E1B8
	private void Start()
	{
		this.forward = base.transform.forward;
		this.up = base.transform.up;
		this.item = base.GetComponent<Item>();
		this.rig = base.GetComponent<Rigidbody>();
		this.hit = HelperFunctions.LineCheck(base.transform.position, base.transform.position + Vector3.down * 2f, HelperFunctions.LayerType.TerrainMap, 0f, 1);
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x0006003C File Offset: 0x0005E23C
	private void FixedUpdate()
	{
		if (this.rig == null)
		{
			return;
		}
		if (!this.hit.transform)
		{
			return;
		}
		if (this.item.itemState != ItemState.Ground)
		{
			return;
		}
		if (!this.item.photonView.IsMine)
		{
			return;
		}
		Vector3 vector = this.hit.point + this.hit.normal * 1f;
		this.rig.AddForce((vector - base.transform.position) * 60f, 5);
		Vector3 vector2 = Vector3.Cross(base.transform.forward, this.forward).normalized * Vector3.Angle(base.transform.forward, this.forward);
		vector2 += Vector3.Cross(base.transform.up, this.up).normalized * Vector3.Angle(base.transform.up, this.up);
		this.rig.AddTorque(vector2 * 100f, 5);
		this.rig.linearVelocity *= 0.8f;
		this.rig.angularVelocity *= 0.8f;
	}

	// Token: 0x04001186 RID: 4486
	private Rigidbody rig;

	// Token: 0x04001187 RID: 4487
	private RaycastHit hit;

	// Token: 0x04001188 RID: 4488
	private Item item;

	// Token: 0x04001189 RID: 4489
	private Vector3 forward;

	// Token: 0x0400118A RID: 4490
	private Vector3 up;
}
