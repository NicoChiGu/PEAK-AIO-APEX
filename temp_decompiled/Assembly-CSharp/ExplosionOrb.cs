using System;
using UnityEngine;

// Token: 0x02000256 RID: 598
internal readonly struct ExplosionOrb
{
	// Token: 0x0600112D RID: 4397 RVA: 0x000567A7 File Offset: 0x000549A7
	public ExplosionOrb(Vector3 pos, Vector3 dir, float delay = 0f, float size = 1f, float speed = 1f)
	{
		this.position = pos;
		this.direction = dir;
		this.delay = delay;
		this.size = size;
		this.speed = speed;
	}

	// Token: 0x04000FA2 RID: 4002
	public readonly Vector3 position;

	// Token: 0x04000FA3 RID: 4003
	public readonly Vector3 direction;

	// Token: 0x04000FA4 RID: 4004
	public readonly float delay;

	// Token: 0x04000FA5 RID: 4005
	public readonly float size;

	// Token: 0x04000FA6 RID: 4006
	public readonly float speed;
}
