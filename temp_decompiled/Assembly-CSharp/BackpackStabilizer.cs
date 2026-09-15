using System;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class BackpackStabilizer : MonoBehaviour
{
	// Token: 0x06000F78 RID: 3960 RVA: 0x0004CD36 File Offset: 0x0004AF36
	private void Start()
	{
		this.startingPos = base.transform.position;
		this.startingRot = base.transform.rotation;
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x0004CD5C File Offset: 0x0004AF5C
	private void FixedUpdate()
	{
		if (this.backpack.photonView.IsMine && this.backpack.itemState == ItemState.Ground && this.backpack.lastThrownAmount < 0.15f && this.timeSinceSpawned < 0.5f)
		{
			this.timeSinceSpawned += Time.fixedDeltaTime;
			if (this.timeSinceSpawned >= 0.5f)
			{
				Debug.Log("Distance moved: " + Vector3.Distance(base.transform.position, this.startingPos).ToString());
			}
			if (Vector3.Distance(base.transform.position, this.startingPos) > 5f)
			{
				this.ResetPosition();
			}
		}
		if (this.backpack.itemState == ItemState.Ground)
		{
			Vector3 up = base.transform.up;
			Vector3 normalized = Vector3.Cross(up, Vector3.up).normalized;
			float num = Vector3.Angle(up, Vector3.up);
			Vector3 vector = normalized * num * this.torqueStrength;
			this.backpack.rig.AddTorque(vector, 5);
		}
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x0004CE7C File Offset: 0x0004B07C
	private void ResetPosition()
	{
		this.backpack.photonView.RPC("SetKinematicAndResetSyncData", 0, new object[] { true, this.startingPos, this.startingRot });
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x0004CECA File Offset: 0x0004B0CA
	private void Teleport()
	{
		base.transform.position += Vector3.up * 6f;
	}

	// Token: 0x04000DCE RID: 3534
	public Backpack backpack;

	// Token: 0x04000DCF RID: 3535
	private Vector3 startingPos;

	// Token: 0x04000DD0 RID: 3536
	private Quaternion startingRot;

	// Token: 0x04000DD1 RID: 3537
	private float timeSinceSpawned;

	// Token: 0x04000DD2 RID: 3538
	private const float failsafeTime = 0.5f;

	// Token: 0x04000DD3 RID: 3539
	private const float failsafeDistance = 5f;

	// Token: 0x04000DD4 RID: 3540
	public float torqueStrength = 10f;
}
