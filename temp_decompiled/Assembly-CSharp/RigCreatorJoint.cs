using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
[ExecuteInEditMode]
public class RigCreatorJoint : MonoBehaviour
{
	// Token: 0x06000397 RID: 919 RVA: 0x00018232 File Offset: 0x00016432
	private void Awake()
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00018248 File Offset: 0x00016448
	private ConfigurableJoint Joint()
	{
		if (!this.joint)
		{
			this.joint = base.GetComponentInParent<ConfigurableJoint>();
		}
		return this.joint;
	}

	// Token: 0x06000399 RID: 921 RVA: 0x00018269 File Offset: 0x00016469
	private Rigidbody Rig()
	{
		if (!this.rig)
		{
			this.rig = base.GetComponentInParent<Rigidbody>();
		}
		return this.rig;
	}

	// Token: 0x0600039A RID: 922 RVA: 0x0001828A File Offset: 0x0001648A
	private RigCreator RigCreator()
	{
		if (!this.rigCreator)
		{
			this.rigCreator = base.GetComponentInParent<RigCreator>();
		}
		return this.rigCreator;
	}

	// Token: 0x0600039B RID: 923 RVA: 0x000182AB File Offset: 0x000164AB
	private void Update()
	{
		if (this.spring != this.CurrentSpring())
		{
			this.SetSpring(this.spring);
			this.RigCreator().JointChanged(this, this.CurrentSpring());
		}
	}

	// Token: 0x0600039C RID: 924 RVA: 0x000182DC File Offset: 0x000164DC
	private float CurrentSpring()
	{
		return this.Joint().angularXDrive.positionSpring / (this.Rig().mass * this.RigCreator().springMultiplier);
	}

	// Token: 0x0600039D RID: 925 RVA: 0x00018314 File Offset: 0x00016514
	internal void SetSpring(float spring)
	{
		JointDrive angularXDrive = this.Joint().angularXDrive;
		angularXDrive.positionSpring = this.Rig().mass * spring * this.RigCreator().springMultiplier;
		angularXDrive.positionDamper = this.Rig().mass * spring * 0.1f * this.RigCreator().springMultiplier;
		this.Joint().angularXDrive = angularXDrive;
		this.Joint().angularYZDrive = angularXDrive;
	}

	// Token: 0x040003F5 RID: 1013
	public float spring;

	// Token: 0x040003F6 RID: 1014
	internal ConfigurableJoint joint;

	// Token: 0x040003F7 RID: 1015
	internal Rigidbody rig;

	// Token: 0x040003F8 RID: 1016
	internal RigCreator rigCreator;
}
