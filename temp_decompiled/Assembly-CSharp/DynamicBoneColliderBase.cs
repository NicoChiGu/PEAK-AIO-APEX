using System;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class DynamicBoneColliderBase : MonoBehaviour
{
	// Token: 0x06000605 RID: 1541 RVA: 0x00022E67 File Offset: 0x00021067
	public virtual bool Collide(ref Vector3 particlePosition, float particleRadius)
	{
		return false;
	}

	// Token: 0x04000628 RID: 1576
	[Tooltip("The axis of the capsule's height.")]
	public DynamicBoneColliderBase.Direction m_Direction = DynamicBoneColliderBase.Direction.Y;

	// Token: 0x04000629 RID: 1577
	[Tooltip("The center of the sphere or capsule, in the object's local space.")]
	public Vector3 m_Center = Vector3.zero;

	// Token: 0x0400062A RID: 1578
	[Tooltip("Constrain bones to outside bound or inside bound.")]
	public DynamicBoneColliderBase.Bound m_Bound;

	// Token: 0x02000429 RID: 1065
	public enum Direction
	{
		// Token: 0x040017EF RID: 6127
		X,
		// Token: 0x040017F0 RID: 6128
		Y,
		// Token: 0x040017F1 RID: 6129
		Z
	}

	// Token: 0x0200042A RID: 1066
	public enum Bound
	{
		// Token: 0x040017F3 RID: 6131
		Outside,
		// Token: 0x040017F4 RID: 6132
		Inside
	}
}
