using System;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class PlayerTrackParticles : MonoBehaviour
{
	// Token: 0x06000B34 RID: 2868 RVA: 0x0003BCCA File Offset: 0x00039ECA
	private void Start()
	{
		if (this.bounds.center != base.transform.position)
		{
			this.bounds.center = base.transform.position;
		}
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0003BD00 File Offset: 0x00039F00
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.inBounds = this.bounds.Contains(Character.observedCharacter.Center);
		if (!this.inBounds)
		{
			return;
		}
		if (Vector3.Distance(this.lastPlayerPos, Character.observedCharacter.Center) > this.repositionDelta)
		{
			Vector3 vector = this.fx.transform.position - this.positionOffset;
			if (this.x)
			{
				vector..ctor(Character.observedCharacter.Center.x, vector.y, vector.z);
			}
			if (this.y)
			{
				vector..ctor(vector.x, Character.observedCharacter.Center.y, vector.z);
			}
			if (this.z)
			{
				vector..ctor(vector.x, vector.y, Character.observedCharacter.Center.z);
			}
			this.fx.transform.position = vector + this.positionOffset;
			this.lastPlayerPos = Character.observedCharacter.Center;
		}
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0003BE28 File Offset: 0x0003A028
	private void OnDrawGizmosSelected()
	{
		if (this.bounds.center != base.transform.position)
		{
			this.bounds.center = base.transform.position;
		}
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x04000A70 RID: 2672
	public Bounds bounds;

	// Token: 0x04000A71 RID: 2673
	public GameObject fx;

	// Token: 0x04000A72 RID: 2674
	[Header("Track Axis")]
	public bool x;

	// Token: 0x04000A73 RID: 2675
	public bool y;

	// Token: 0x04000A74 RID: 2676
	public bool z;

	// Token: 0x04000A75 RID: 2677
	public float repositionDelta = 50f;

	// Token: 0x04000A76 RID: 2678
	private Vector3 lastPlayerPos = Vector3.positiveInfinity;

	// Token: 0x04000A77 RID: 2679
	public bool inBounds;

	// Token: 0x04000A78 RID: 2680
	public Vector3 positionOffset;
}
