using System;
using UnityEngine;

// Token: 0x0200009F RID: 159
[AddComponentMenu("Dynamic Bone/Dynamic Bone Plane Collider")]
public class DynamicBonePlaneCollider : DynamicBoneColliderBase
{
	// Token: 0x06000607 RID: 1543 RVA: 0x00022E84 File Offset: 0x00021084
	private void OnValidate()
	{
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x00022E88 File Offset: 0x00021088
	public override bool Collide(ref Vector3 particlePosition, float particleRadius)
	{
		Vector3 vector = Vector3.up;
		switch (this.m_Direction)
		{
		case DynamicBoneColliderBase.Direction.X:
			vector = base.transform.right;
			break;
		case DynamicBoneColliderBase.Direction.Y:
			vector = base.transform.up;
			break;
		case DynamicBoneColliderBase.Direction.Z:
			vector = base.transform.forward;
			break;
		}
		Vector3 vector2 = base.transform.TransformPoint(this.m_Center);
		Plane plane;
		plane..ctor(vector, vector2);
		float distanceToPoint = plane.GetDistanceToPoint(particlePosition);
		if (this.m_Bound == DynamicBoneColliderBase.Bound.Outside)
		{
			if (distanceToPoint < 0f)
			{
				particlePosition -= vector * distanceToPoint;
				return true;
			}
		}
		else if (distanceToPoint > 0f)
		{
			particlePosition -= vector * distanceToPoint;
			return true;
		}
		return false;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00022F58 File Offset: 0x00021158
	private void OnDrawGizmosSelected()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.m_Bound == DynamicBoneColliderBase.Bound.Outside)
		{
			Gizmos.color = Color.yellow;
		}
		else
		{
			Gizmos.color = Color.magenta;
		}
		Vector3 vector = Vector3.up;
		switch (this.m_Direction)
		{
		case DynamicBoneColliderBase.Direction.X:
			vector = base.transform.right;
			break;
		case DynamicBoneColliderBase.Direction.Y:
			vector = base.transform.up;
			break;
		case DynamicBoneColliderBase.Direction.Z:
			vector = base.transform.forward;
			break;
		}
		Vector3 vector2 = base.transform.TransformPoint(this.m_Center);
		Gizmos.DrawLine(vector2, vector2 + vector);
	}
}
