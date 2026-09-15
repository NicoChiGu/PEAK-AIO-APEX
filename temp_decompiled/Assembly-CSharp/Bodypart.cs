using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002E RID: 46
[DefaultExecutionOrder(-1)]
public class Bodypart : MonoBehaviour
{
	// Token: 0x17000048 RID: 72
	// (get) Token: 0x0600034B RID: 843 RVA: 0x00016C3F File Offset: 0x00014E3F
	public Rigidbody Rig
	{
		get
		{
			if (this.rig == null)
			{
				this.rig = base.GetComponent<Rigidbody>();
			}
			return this.rig;
		}
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00016C64 File Offset: 0x00014E64
	private void Awake()
	{
		this.startLocal = base.transform.localRotation;
		this.prevPos = base.transform.position;
		this.prevRot = base.transform.rotation;
		this.rig = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00016CB0 File Offset: 0x00014EB0
	public void Start()
	{
		if (this.started)
		{
			return;
		}
		this.started = true;
		this.character = base.GetComponentInParent<Character>();
		this.joint = base.GetComponent<ConfigurableJoint>();
		if (this.joint)
		{
			this.jointParent = this.joint.connectedBody.GetComponent<Bodypart>();
		}
		if (this.rig)
		{
			this.rig.maxAngularVelocity = 50f;
		}
		this.localCenterOfMass = HelperFunctions.GetCenterOfMass(base.transform);
	}

	// Token: 0x0600034E RID: 846 RVA: 0x00016D36 File Offset: 0x00014F36
	internal void RegisterCollider(RigCreatorCollider rigCreatorCollider)
	{
		this.colliders.Add(rigCreatorCollider);
	}

	// Token: 0x0600034F RID: 847 RVA: 0x00016D44 File Offset: 0x00014F44
	internal void InitBodypart(BodypartType setPartType)
	{
		this.partType = setPartType;
	}

	// Token: 0x06000350 RID: 848 RVA: 0x00016D4D File Offset: 0x00014F4D
	private Vector3 WorldCenterOfMass()
	{
		return base.transform.position;
	}

	// Token: 0x06000351 RID: 849 RVA: 0x00016D5C File Offset: 0x00014F5C
	public void SaveAnimationData()
	{
		if (this != this.character.refs.hip)
		{
			this.targetOffsetRelativeToHip = this.WorldCenterOfMass() - this.character.refs.hip.transform.position;
		}
		this.targetRotation = base.transform.localRotation;
		this.targetForward = base.transform.forward;
		this.targetUp = base.transform.up;
	}

	// Token: 0x06000352 RID: 850 RVA: 0x00016DDF File Offset: 0x00014FDF
	public void ResetTransform()
	{
		base.transform.rotation = this.rig.rotation;
		base.transform.position = this.rig.position;
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00016E0D File Offset: 0x0001500D
	internal void Animate(float force, float torque)
	{
		if (this.rig.isKinematic)
		{
			this.SnapToAnim();
			return;
		}
		this.FollowRotation_Joint();
		this.FollowRotation_Rotation(torque);
		this.FollowRotation_Position(force);
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00016E38 File Offset: 0x00015038
	public void SnapToAnim()
	{
		this.Start();
		Vector3 vector = this.WorldTargetPos() - this.WorldCenterOfMass();
		base.transform.position += vector;
		if (this.targetForward != Vector3.zero && this.targetUp != Vector3.zero)
		{
			base.transform.rotation = Quaternion.LookRotation(this.targetForward, this.targetUp);
		}
		if (this.rig.isKinematic)
		{
			return;
		}
		this.rig.linearVelocity *= 0f;
		this.rig.angularVelocity *= 0f;
	}

	// Token: 0x06000355 RID: 853 RVA: 0x00016EF8 File Offset: 0x000150F8
	private void DrawDebug()
	{
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00016EFC File Offset: 0x000150FC
	private void FollowRotation_Joint()
	{
		if (!this.joint)
		{
			return;
		}
		this.joint.SetTargetRotationLocal(this.targetRotation, this.startLocal);
	}

	// Token: 0x06000357 RID: 855 RVA: 0x00016F30 File Offset: 0x00015130
	private void FollowRotation_Rotation(float torque)
	{
		if (this.rig.isKinematic)
		{
			return;
		}
		Vector3 vector = Vector3.Cross(base.transform.forward, this.targetForward).normalized * Vector3.Angle(base.transform.forward, this.targetForward);
		vector += Vector3.Cross(base.transform.up, this.targetUp).normalized * Vector3.Angle(base.transform.up, this.targetUp);
		this.rig.AddTorque(vector * torque, 5);
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00016FD8 File Offset: 0x000151D8
	private void FollowRotation_Position(float force)
	{
		if (!this.character)
		{
			return;
		}
		if (this == this.character.refs.hip)
		{
			return;
		}
		if (this.targetOffsetRelativeToHip == Vector3.zero)
		{
			return;
		}
		Vector3 vector = (this.WorldTargetPos() - this.WorldCenterOfMass()) * force;
		this.AddForce(vector, 5);
		if (this.jointParent)
		{
			Vector3 vector2 = vector * this.rig.mass;
			this.jointParent.AddForce(-vector2 * 0.5f, 0);
			this.character.refs.hip.AddForce(-vector2 * 0.5f, 0);
		}
	}

	// Token: 0x06000359 RID: 857 RVA: 0x000170A0 File Offset: 0x000152A0
	private Vector3 WorldTargetPos()
	{
		return this.character.refs.hip.transform.position + this.targetOffsetRelativeToHip;
	}

	// Token: 0x0600035A RID: 858 RVA: 0x000170C8 File Offset: 0x000152C8
	internal void Drag(float drag, bool ignoreRagdoll = false)
	{
		if (!ignoreRagdoll)
		{
			drag = Mathf.Lerp(1f, drag, this.character.data.currentRagdollControll);
		}
		if (this.rig.isKinematic)
		{
			return;
		}
		this.rig.linearVelocity *= drag;
		this.rig.angularVelocity *= drag;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00017134 File Offset: 0x00015334
	internal void ParasolDrag(float drag, float xzDrag, bool ignoreRagdoll = false)
	{
		if (!ignoreRagdoll)
		{
			drag = Mathf.Lerp(1f, drag, this.character.data.currentRagdollControll);
		}
		if (this.rig.isKinematic)
		{
			return;
		}
		if (this.rig.linearVelocity.y < 0f)
		{
			this.rig.linearVelocity = new Vector3(this.rig.linearVelocity.x * xzDrag, this.rig.linearVelocity.y * drag, this.rig.linearVelocity.z * xzDrag);
		}
	}

	// Token: 0x0600035C RID: 860 RVA: 0x000171CC File Offset: 0x000153CC
	private void OnCollisionEnter(Collision collision)
	{
		if (this.character == null)
		{
			return;
		}
		if (collision.collider.transform.root == base.transform.root)
		{
			return;
		}
		Action<Collision> action = this.collisionEnterAction;
		if (action != null)
		{
			action.Invoke(collision);
		}
		this.character.refs.movement.OnCollision(collision, true, this);
	}

	// Token: 0x0600035D RID: 861 RVA: 0x00017238 File Offset: 0x00015438
	private void OnCollisionStay(Collision collision)
	{
		if (!this.character)
		{
			return;
		}
		if (collision.collider.transform.root == base.transform.root)
		{
			return;
		}
		Action<Collision> action = this.collisionStayAction;
		if (action != null)
		{
			action.Invoke(collision);
		}
		this.character.refs.movement.OnCollision(collision, false, this);
	}

	// Token: 0x0600035E RID: 862 RVA: 0x000172A0 File Offset: 0x000154A0
	internal void Gravity(Vector3 gravity)
	{
		this.AddForce(gravity, 5);
	}

	// Token: 0x0600035F RID: 863 RVA: 0x000172AA File Offset: 0x000154AA
	public void AddForce(Vector3 force, ForceMode forceMode)
	{
		if (this.rig.isKinematic)
		{
			return;
		}
		if (forceMode == 5)
		{
			force *= this.rig.mass;
		}
		this.forcesToAdd += force;
	}

	// Token: 0x06000360 RID: 864 RVA: 0x000172E3 File Offset: 0x000154E3
	internal void ToggleUseGravity(bool useGrav)
	{
		if (this.rig.useGravity != useGrav)
		{
			this.rig.useGravity = useGrav;
		}
	}

	// Token: 0x06000361 RID: 865 RVA: 0x000172FF File Offset: 0x000154FF
	internal void ApplyForces()
	{
		this.rig.AddForce(this.forcesToAdd, 0);
		this.forcesToAdd *= 0f;
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0001732C File Offset: 0x0001552C
	internal void AddMovementForce(float movementForce)
	{
		if (!this.character)
		{
			return;
		}
		Vector3 worldMovementInput_Lerp = this.character.data.worldMovementInput_Lerp;
		this.AddForce(movementForce * worldMovementInput_Lerp, 5);
	}

	// Token: 0x06000363 RID: 867 RVA: 0x00017368 File Offset: 0x00015568
	internal void SetPhysicsMaterial(Bodypart.FrictionType setFrictionType, PhysicsMaterial slipperyMat, PhysicsMaterial normalMat)
	{
		foreach (RigCreatorCollider rigCreatorCollider in this.colliders)
		{
			if (this.frictionType == Bodypart.FrictionType.Grippy)
			{
				rigCreatorCollider.col.sharedMaterial = normalMat;
			}
			else if (this.frictionType == Bodypart.FrictionType.Slippery)
			{
				rigCreatorCollider.col.sharedMaterial = slipperyMat;
			}
			else if (setFrictionType == Bodypart.FrictionType.Grippy)
			{
				rigCreatorCollider.col.sharedMaterial = normalMat;
			}
			else
			{
				rigCreatorCollider.col.sharedMaterial = slipperyMat;
			}
		}
	}

	// Token: 0x0400030A RID: 778
	private Character character;

	// Token: 0x0400030B RID: 779
	public BodypartType partType;

	// Token: 0x0400030C RID: 780
	public Bodypart.FrictionType frictionType;

	// Token: 0x0400030D RID: 781
	private Rigidbody rig;

	// Token: 0x0400030E RID: 782
	internal Bodypart jointParent;

	// Token: 0x0400030F RID: 783
	private Quaternion startLocal = Quaternion.identity;

	// Token: 0x04000310 RID: 784
	private Vector3 localCenterOfMass;

	// Token: 0x04000311 RID: 785
	private ConfigurableJoint joint;

	// Token: 0x04000312 RID: 786
	private Quaternion targetRotation = Quaternion.identity;

	// Token: 0x04000313 RID: 787
	private Quaternion lastTargetRotation = Quaternion.identity;

	// Token: 0x04000314 RID: 788
	private Vector3 targetForward;

	// Token: 0x04000315 RID: 789
	private Vector3 targetUp;

	// Token: 0x04000316 RID: 790
	private Vector3 targetOffsetRelativeToHip;

	// Token: 0x04000317 RID: 791
	internal List<RigCreatorCollider> colliders = new List<RigCreatorCollider>();

	// Token: 0x04000318 RID: 792
	public Vector3 forcesToAdd;

	// Token: 0x04000319 RID: 793
	private bool started;

	// Token: 0x0400031A RID: 794
	private Vector3 prevPos;

	// Token: 0x0400031B RID: 795
	private Quaternion prevRot;

	// Token: 0x0400031C RID: 796
	public Action<Collision> collisionEnterAction;

	// Token: 0x0400031D RID: 797
	public Action<Collision> collisionStayAction;

	// Token: 0x0200040C RID: 1036
	public enum FrictionType
	{
		// Token: 0x04001787 RID: 6023
		Unspecified,
		// Token: 0x04001788 RID: 6024
		Grippy,
		// Token: 0x04001789 RID: 6025
		Slippery
	}
}
