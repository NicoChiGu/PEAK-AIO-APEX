using System;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class ChessStabilizer : MonoBehaviour
{
	// Token: 0x06001090 RID: 4240 RVA: 0x00053402 File Offset: 0x00051602
	private void Start()
	{
		this.startingPos = base.transform.position;
		this.startingRot = base.transform.rotation;
		if (this.startKinematic)
		{
			this.item.rig.isKinematic = true;
		}
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x00053440 File Offset: 0x00051640
	private void FixedUpdate()
	{
		if (this.item.itemState == ItemState.Ground && !this.item.rig.isKinematic)
		{
			Vector3 up = base.transform.up;
			Vector3 normalized = Vector3.Cross(up, Vector3.up).normalized;
			float num = Vector3.Angle(up, Vector3.up);
			Vector3 vector = normalized * num * this.torqueStrength;
			Vector3 vector2 = -this.item.rig.angularVelocity * this.dampingStrength;
			this.item.rig.AddTorque(vector + vector2, 5);
			this.groundTimer += Time.fixedDeltaTime;
			if (this.groundTimer > 2f && this.item.rig.linearVelocity.sqrMagnitude < 0.5f && this.item.rig.angularVelocity.sqrMagnitude < 0.5f && Vector3.Angle(base.transform.up, Vector3.up) < 2f)
			{
				this.item.rig.isKinematic = true;
			}
		}
	}

	// Token: 0x04000EBF RID: 3775
	public bool startKinematic;

	// Token: 0x04000EC0 RID: 3776
	public Item item;

	// Token: 0x04000EC1 RID: 3777
	private Vector3 startingPos;

	// Token: 0x04000EC2 RID: 3778
	private Quaternion startingRot;

	// Token: 0x04000EC3 RID: 3779
	private float groundTimer;

	// Token: 0x04000EC4 RID: 3780
	public float torqueStrength = 10f;

	// Token: 0x04000EC5 RID: 3781
	public float dampingStrength = 5f;
}
