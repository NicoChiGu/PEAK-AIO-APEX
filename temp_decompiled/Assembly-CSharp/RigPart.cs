using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000032 RID: 50
[Serializable]
public class RigPart
{
	// Token: 0x04000321 RID: 801
	[HideInInspector]
	public bool justCreated;

	// Token: 0x04000322 RID: 802
	public BodypartType partType;

	// Token: 0x04000323 RID: 803
	public float mass = 10f;

	// Token: 0x04000324 RID: 804
	public float spring = 10f;

	// Token: 0x04000325 RID: 805
	public Transform transform;

	// Token: 0x04000326 RID: 806
	public List<RigCreatorColliderData> colliders = new List<RigCreatorColliderData>();

	// Token: 0x04000327 RID: 807
	public RigCreatorRigidbody rigHandler;

	// Token: 0x04000328 RID: 808
	public Rigidbody rig;

	// Token: 0x04000329 RID: 809
	public ConfigurableJoint joint;

	// Token: 0x0400032A RID: 810
	public RigCreatorJoint jointHandler;
}
