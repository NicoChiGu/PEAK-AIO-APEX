using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200009C RID: 156
[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{
	// Token: 0x060005E5 RID: 1509 RVA: 0x0002164E File Offset: 0x0001F84E
	private void Start()
	{
		if (!this.m_Root)
		{
			this.m_Root = base.transform;
		}
		this.SetupParticles();
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x0002166F File Offset: 0x0001F86F
	private void FixedUpdate()
	{
		if (this.m_UpdateMode == DynamicBone.UpdateMode.AnimatePhysics)
		{
			this.PreUpdate();
		}
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00021680 File Offset: 0x0001F880
	private void Update()
	{
		if (this.m_UpdateMode != DynamicBone.UpdateMode.AnimatePhysics)
		{
			this.PreUpdate();
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00021694 File Offset: 0x0001F894
	private void LateUpdate()
	{
		if (this.m_DistantDisable)
		{
			this.CheckDistance();
		}
		if (this.m_Weight > 0f && (!this.m_DistantDisable || !this.m_DistantDisabled))
		{
			float num = ((this.m_UpdateMode == DynamicBone.UpdateMode.UnscaledTime) ? Time.unscaledDeltaTime : Time.deltaTime);
			this.UpdateDynamicBones(num);
		}
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x000216E9 File Offset: 0x0001F8E9
	private void PreUpdate()
	{
		if (this.m_Weight > 0f && (!this.m_DistantDisable || !this.m_DistantDisabled))
		{
			this.InitTransforms();
		}
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x00021710 File Offset: 0x0001F910
	private void CheckDistance()
	{
		Transform transform = this.m_ReferenceObject;
		if (transform == null && Camera.main != null)
		{
			transform = Camera.main.transform;
		}
		if (transform != null)
		{
			bool flag = (transform.position - base.transform.position).sqrMagnitude > this.m_DistanceToObject * this.m_DistanceToObject;
			if (flag != this.m_DistantDisabled)
			{
				if (!flag)
				{
					this.ResetParticlesPosition();
				}
				this.m_DistantDisabled = flag;
			}
		}
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00021795 File Offset: 0x0001F995
	private void OnEnable()
	{
		this.ResetParticlesPosition();
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0002179D File Offset: 0x0001F99D
	private void OnDisable()
	{
		this.InitTransforms();
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x000217A8 File Offset: 0x0001F9A8
	private void OnValidate()
	{
		this.m_UpdateRate = Mathf.Max(this.m_UpdateRate, 0f);
		this.m_Damping = Mathf.Clamp01(this.m_Damping);
		this.m_Elasticity = Mathf.Clamp01(this.m_Elasticity);
		this.m_Stiffness = Mathf.Clamp01(this.m_Stiffness);
		this.m_Inert = Mathf.Clamp01(this.m_Inert);
		this.m_Friction = Mathf.Clamp01(this.m_Friction);
		this.m_Radius = Mathf.Max(this.m_Radius, 0f);
		if (Application.isEditor && Application.isPlaying)
		{
			this.InitTransforms();
			this.SetupParticles();
		}
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00021850 File Offset: 0x0001FA50
	private void OnDrawGizmosSelected()
	{
		if (!base.enabled || this.m_Root == null)
		{
			return;
		}
		if (Application.isEditor && !Application.isPlaying && base.transform.hasChanged)
		{
			this.InitTransforms();
			this.SetupParticles();
		}
		Gizmos.color = Color.white;
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
				Gizmos.DrawLine(particle.m_Position, particle2.m_Position);
			}
			if (particle.m_Radius > 0f)
			{
				Gizmos.DrawWireSphere(particle.m_Position, particle.m_Radius * this.m_ObjectScale);
			}
		}
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00021919 File Offset: 0x0001FB19
	public void SetWeight(float w)
	{
		if (this.m_Weight != w)
		{
			if (w == 0f)
			{
				this.InitTransforms();
			}
			else if (this.m_Weight == 0f)
			{
				this.ResetParticlesPosition();
			}
			this.m_Weight = w;
		}
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x0002194E File Offset: 0x0001FB4E
	public float GetWeight()
	{
		return this.m_Weight;
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x00021958 File Offset: 0x0001FB58
	private void UpdateDynamicBones(float t)
	{
		if (this.m_Root == null)
		{
			return;
		}
		this.m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		this.m_ObjectMove = base.transform.position - this.m_ObjectPrevPosition;
		this.m_ObjectPrevPosition = base.transform.position;
		int num = 1;
		float num2 = 1f;
		if (this.m_UpdateMode == DynamicBone.UpdateMode.Default)
		{
			if (this.m_UpdateRate > 0f)
			{
				num2 = Time.deltaTime * this.m_UpdateRate;
			}
			else
			{
				num2 = Time.deltaTime;
			}
		}
		else if (this.m_UpdateRate > 0f)
		{
			float num3 = 1f / this.m_UpdateRate;
			this.m_Time += t;
			num = 0;
			while (this.m_Time >= num3)
			{
				this.m_Time -= num3;
				if (++num >= 3)
				{
					this.m_Time = 0f;
					break;
				}
			}
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				this.UpdateParticles1(num2);
				this.UpdateParticles2(num2);
				this.m_ObjectMove = Vector3.zero;
			}
		}
		else
		{
			this.SkipUpdateParticles();
		}
		this.ApplyParticlesToTransforms();
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x00021A80 File Offset: 0x0001FC80
	public void SetupParticles()
	{
		this.m_Particles.Clear();
		if (this.m_Root == null)
		{
			return;
		}
		this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
		this.m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		this.m_ObjectPrevPosition = base.transform.position;
		this.m_ObjectMove = Vector3.zero;
		this.m_BoneTotalLength = 0f;
		this.AppendParticles(this.m_Root, -1, 0f);
		this.UpdateParameters();
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x00021B18 File Offset: 0x0001FD18
	private void AppendParticles(Transform b, int parentIndex, float boneLength)
	{
		DynamicBone.Particle particle = new DynamicBone.Particle();
		particle.m_Transform = b;
		particle.m_ParentIndex = parentIndex;
		if (b != null)
		{
			particle.m_Position = (particle.m_PrevPosition = b.position);
			particle.m_InitLocalPosition = b.localPosition;
			particle.m_InitLocalRotation = b.localRotation;
		}
		else
		{
			Transform transform = this.m_Particles[parentIndex].m_Transform;
			if (this.m_EndLength > 0f)
			{
				Transform parent = transform.parent;
				if (parent != null)
				{
					particle.m_EndOffset = transform.InverseTransformPoint(transform.position * 2f - parent.position) * this.m_EndLength;
				}
				else
				{
					particle.m_EndOffset = new Vector3(this.m_EndLength, 0f, 0f);
				}
			}
			else
			{
				particle.m_EndOffset = transform.InverseTransformPoint(base.transform.TransformDirection(this.m_EndOffset) + transform.position);
			}
			particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
		}
		if (parentIndex >= 0)
		{
			boneLength += (this.m_Particles[parentIndex].m_Transform.position - particle.m_Position).magnitude;
			particle.m_BoneLength = boneLength;
			this.m_BoneTotalLength = Mathf.Max(this.m_BoneTotalLength, boneLength);
		}
		int count = this.m_Particles.Count;
		this.m_Particles.Add(particle);
		if (b != null)
		{
			for (int i = 0; i < b.childCount; i++)
			{
				Transform child = b.GetChild(i);
				bool flag = false;
				if (this.m_Exclusions != null)
				{
					flag = this.m_Exclusions.Contains(child);
				}
				if (!flag)
				{
					this.AppendParticles(child, count, boneLength);
				}
				else if (this.m_EndLength > 0f || this.m_EndOffset != Vector3.zero)
				{
					this.AppendParticles(null, count, boneLength);
				}
			}
			if (b.childCount == 0 && (this.m_EndLength > 0f || this.m_EndOffset != Vector3.zero))
			{
				this.AppendParticles(null, count, boneLength);
			}
		}
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00021D4C File Offset: 0x0001FF4C
	public void UpdateParameters()
	{
		if (this.m_Root == null)
		{
			return;
		}
		this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			particle.m_Damping = this.m_Damping;
			particle.m_Elasticity = this.m_Elasticity;
			particle.m_Stiffness = this.m_Stiffness;
			particle.m_Inert = this.m_Inert;
			particle.m_Friction = this.m_Friction;
			particle.m_Radius = this.m_Radius;
			if (this.m_BoneTotalLength > 0f)
			{
				float num = particle.m_BoneLength / this.m_BoneTotalLength;
				if (this.m_DampingDistrib != null && this.m_DampingDistrib.keys.Length != 0)
				{
					particle.m_Damping *= this.m_DampingDistrib.Evaluate(num);
				}
				if (this.m_ElasticityDistrib != null && this.m_ElasticityDistrib.keys.Length != 0)
				{
					particle.m_Elasticity *= this.m_ElasticityDistrib.Evaluate(num);
				}
				if (this.m_StiffnessDistrib != null && this.m_StiffnessDistrib.keys.Length != 0)
				{
					particle.m_Stiffness *= this.m_StiffnessDistrib.Evaluate(num);
				}
				if (this.m_InertDistrib != null && this.m_InertDistrib.keys.Length != 0)
				{
					particle.m_Inert *= this.m_InertDistrib.Evaluate(num);
				}
				if (this.m_FrictionDistrib != null && this.m_FrictionDistrib.keys.Length != 0)
				{
					particle.m_Friction *= this.m_FrictionDistrib.Evaluate(num);
				}
				if (this.m_RadiusDistrib != null && this.m_RadiusDistrib.keys.Length != 0)
				{
					particle.m_Radius *= this.m_RadiusDistrib.Evaluate(num);
				}
			}
			particle.m_Damping = Mathf.Clamp01(particle.m_Damping);
			particle.m_Elasticity = Mathf.Clamp01(particle.m_Elasticity);
			particle.m_Stiffness = Mathf.Clamp01(particle.m_Stiffness);
			particle.m_Inert = Mathf.Clamp01(particle.m_Inert);
			particle.m_Friction = Mathf.Clamp01(particle.m_Friction);
			particle.m_Radius = Mathf.Max(particle.m_Radius, 0f);
		}
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00021F94 File Offset: 0x00020194
	private void InitTransforms()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_Transform != null)
			{
				particle.m_Transform.localPosition = particle.m_InitLocalPosition;
				particle.m_Transform.localRotation = particle.m_InitLocalRotation;
			}
		}
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x00021FF4 File Offset: 0x000201F4
	private void ResetParticlesPosition()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_Transform != null)
			{
				particle.m_Position = (particle.m_PrevPosition = particle.m_Transform.position);
			}
			else
			{
				Transform transform = this.m_Particles[particle.m_ParentIndex].m_Transform;
				particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
			}
			particle.m_isCollide = false;
		}
		this.m_ObjectPrevPosition = base.transform.position;
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x0002209C File Offset: 0x0002029C
	private void UpdateParticles1(float timeVar)
	{
		Vector3 vector = this.m_Gravity;
		Vector3 normalized = this.m_Gravity.normalized;
		Vector3 vector2 = this.m_Root.TransformDirection(this.m_LocalGravity);
		Vector3 vector3 = normalized * Mathf.Max(Vector3.Dot(vector2, normalized), 0f);
		vector -= vector3;
		vector = (vector + this.m_Force) * (this.m_ObjectScale * timeVar);
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				Vector3 vector4 = particle.m_Position - particle.m_PrevPosition;
				Vector3 vector5 = this.m_ObjectMove * particle.m_Inert;
				particle.m_PrevPosition = particle.m_Position + vector5;
				float num = particle.m_Damping;
				if (particle.m_isCollide)
				{
					num += particle.m_Friction;
					if (num > 1f)
					{
						num = 1f;
					}
					particle.m_isCollide = false;
				}
				particle.m_Position += vector4 * (1f - num) + vector + vector5;
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_Transform.position;
			}
		}
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x0002220C File Offset: 0x0002040C
	private void UpdateParticles2(float timeVar)
	{
		Plane plane = default(Plane);
		for (int i = 1; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
			float num;
			if (particle.m_Transform != null)
			{
				num = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
			}
			else
			{
				num = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
			}
			float num2 = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
			if (num2 > 0f || particle.m_Elasticity > 0f)
			{
				Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
				localToWorldMatrix.SetColumn(3, particle2.m_Position);
				Vector3 vector;
				if (particle.m_Transform != null)
				{
					vector = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
				}
				else
				{
					vector = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
				}
				Vector3 vector2 = vector - particle.m_Position;
				particle.m_Position += vector2 * (particle.m_Elasticity * timeVar);
				if (num2 > 0f)
				{
					vector2 = vector - particle.m_Position;
					float magnitude = vector2.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle.m_Position += vector2 * ((magnitude - num3) / magnitude);
					}
				}
			}
			if (this.m_Colliders != null)
			{
				float num4 = particle.m_Radius * this.m_ObjectScale;
				for (int j = 0; j < this.m_Colliders.Count; j++)
				{
					DynamicBoneColliderBase dynamicBoneColliderBase = this.m_Colliders[j];
					if (dynamicBoneColliderBase != null && dynamicBoneColliderBase.enabled)
					{
						particle.m_isCollide |= dynamicBoneColliderBase.Collide(ref particle.m_Position, num4);
					}
				}
			}
			if (this.m_FreezeAxis != DynamicBone.FreezeAxis.None)
			{
				switch (this.m_FreezeAxis)
				{
				case DynamicBone.FreezeAxis.X:
					plane.SetNormalAndPosition(particle2.m_Transform.right, particle2.m_Position);
					break;
				case DynamicBone.FreezeAxis.Y:
					plane.SetNormalAndPosition(particle2.m_Transform.up, particle2.m_Position);
					break;
				case DynamicBone.FreezeAxis.Z:
					plane.SetNormalAndPosition(particle2.m_Transform.forward, particle2.m_Position);
					break;
				}
				particle.m_Position -= plane.normal * plane.GetDistanceToPoint(particle.m_Position);
			}
			Vector3 vector3 = particle2.m_Position - particle.m_Position;
			float magnitude2 = vector3.magnitude;
			if (magnitude2 > 0f)
			{
				particle.m_Position += vector3 * ((magnitude2 - num) / magnitude2);
			}
		}
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00022518 File Offset: 0x00020718
	private void SkipUpdateParticles()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				particle.m_PrevPosition += this.m_ObjectMove;
				particle.m_Position += this.m_ObjectMove;
				DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
				float num;
				if (particle.m_Transform != null)
				{
					num = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
				}
				else
				{
					num = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
				}
				float num2 = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
				if (num2 > 0f)
				{
					Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
					localToWorldMatrix.SetColumn(3, particle2.m_Position);
					Vector3 vector;
					if (particle.m_Transform != null)
					{
						vector = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
					}
					else
					{
						vector = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
					}
					Vector3 vector2 = vector - particle.m_Position;
					float magnitude = vector2.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle.m_Position += vector2 * ((magnitude - num3) / magnitude);
					}
				}
				Vector3 vector3 = particle2.m_Position - particle.m_Position;
				float magnitude2 = vector3.magnitude;
				if (magnitude2 > 0f)
				{
					particle.m_Position += vector3 * ((magnitude2 - num) / magnitude2);
				}
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_Transform.position;
			}
		}
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x0002271D File Offset: 0x0002091D
	private static Vector3 MirrorVector(Vector3 v, Vector3 axis)
	{
		return v - axis * (Vector3.Dot(v, axis) * 2f);
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00022738 File Offset: 0x00020938
	private void ApplyParticlesToTransforms()
	{
		for (int i = 1; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
			if (particle2.m_Transform.childCount <= 1)
			{
				Vector3 vector;
				if (particle.m_Transform != null)
				{
					vector = particle.m_Transform.localPosition;
				}
				else
				{
					vector = particle.m_EndOffset;
				}
				Vector3 vector2 = particle.m_Position - particle2.m_Position;
				Quaternion quaternion = Quaternion.FromToRotation(particle2.m_Transform.TransformDirection(vector), vector2);
				particle2.m_Transform.rotation = quaternion * particle2.m_Transform.rotation;
			}
			if (particle.m_Transform != null)
			{
				particle.m_Transform.position = particle.m_Position;
			}
		}
	}

	// Token: 0x04000604 RID: 1540
	[Tooltip("The root of the transform hierarchy to apply physics.")]
	public Transform m_Root;

	// Token: 0x04000605 RID: 1541
	[Tooltip("Internal physics simulation rate.")]
	public float m_UpdateRate = 60f;

	// Token: 0x04000606 RID: 1542
	public DynamicBone.UpdateMode m_UpdateMode = DynamicBone.UpdateMode.Default;

	// Token: 0x04000607 RID: 1543
	[Tooltip("How much the bones slowed down.")]
	[Range(0f, 1f)]
	public float m_Damping = 0.1f;

	// Token: 0x04000608 RID: 1544
	public AnimationCurve m_DampingDistrib;

	// Token: 0x04000609 RID: 1545
	[Tooltip("How much the force applied to return each bone to original orientation.")]
	[Range(0f, 1f)]
	public float m_Elasticity = 0.1f;

	// Token: 0x0400060A RID: 1546
	public AnimationCurve m_ElasticityDistrib;

	// Token: 0x0400060B RID: 1547
	[Tooltip("How much bone's original orientation are preserved.")]
	[Range(0f, 1f)]
	public float m_Stiffness = 0.1f;

	// Token: 0x0400060C RID: 1548
	public AnimationCurve m_StiffnessDistrib;

	// Token: 0x0400060D RID: 1549
	[Tooltip("How much character's position change is ignored in physics simulation.")]
	[Range(0f, 1f)]
	public float m_Inert;

	// Token: 0x0400060E RID: 1550
	public AnimationCurve m_InertDistrib;

	// Token: 0x0400060F RID: 1551
	[Tooltip("How much the bones slowed down when collide.")]
	public float m_Friction;

	// Token: 0x04000610 RID: 1552
	public AnimationCurve m_FrictionDistrib;

	// Token: 0x04000611 RID: 1553
	[Tooltip("Each bone can be a sphere to collide with colliders. Radius describe sphere's size.")]
	public float m_Radius;

	// Token: 0x04000612 RID: 1554
	public AnimationCurve m_RadiusDistrib;

	// Token: 0x04000613 RID: 1555
	[Tooltip("If End Length is not zero, an extra bone is generated at the end of transform hierarchy.")]
	public float m_EndLength;

	// Token: 0x04000614 RID: 1556
	[Tooltip("If End Offset is not zero, an extra bone is generated at the end of transform hierarchy.")]
	public Vector3 m_EndOffset = Vector3.zero;

	// Token: 0x04000615 RID: 1557
	[Tooltip("The force apply to bones. Partial force apply to character's initial pose is cancelled out.")]
	public Vector3 m_Gravity = Vector3.zero;

	// Token: 0x04000616 RID: 1558
	[Tooltip("The force apply to bones.")]
	public Vector3 m_Force = Vector3.zero;

	// Token: 0x04000617 RID: 1559
	[Tooltip("Collider objects interact with the bones.")]
	public List<DynamicBoneColliderBase> m_Colliders;

	// Token: 0x04000618 RID: 1560
	[Tooltip("Bones exclude from physics simulation.")]
	public List<Transform> m_Exclusions;

	// Token: 0x04000619 RID: 1561
	[Tooltip("Constrain bones to move on specified plane.")]
	public DynamicBone.FreezeAxis m_FreezeAxis;

	// Token: 0x0400061A RID: 1562
	[Tooltip("Disable physics simulation automatically if character is far from camera or player.")]
	public bool m_DistantDisable;

	// Token: 0x0400061B RID: 1563
	public Transform m_ReferenceObject;

	// Token: 0x0400061C RID: 1564
	public float m_DistanceToObject = 20f;

	// Token: 0x0400061D RID: 1565
	private Vector3 m_LocalGravity = Vector3.zero;

	// Token: 0x0400061E RID: 1566
	private Vector3 m_ObjectMove = Vector3.zero;

	// Token: 0x0400061F RID: 1567
	private Vector3 m_ObjectPrevPosition = Vector3.zero;

	// Token: 0x04000620 RID: 1568
	private float m_BoneTotalLength;

	// Token: 0x04000621 RID: 1569
	private float m_ObjectScale = 1f;

	// Token: 0x04000622 RID: 1570
	private float m_Time;

	// Token: 0x04000623 RID: 1571
	private float m_Weight = 1f;

	// Token: 0x04000624 RID: 1572
	private bool m_DistantDisabled;

	// Token: 0x04000625 RID: 1573
	private List<DynamicBone.Particle> m_Particles = new List<DynamicBone.Particle>();

	// Token: 0x02000426 RID: 1062
	public enum UpdateMode
	{
		// Token: 0x040017D6 RID: 6102
		Normal,
		// Token: 0x040017D7 RID: 6103
		AnimatePhysics,
		// Token: 0x040017D8 RID: 6104
		UnscaledTime,
		// Token: 0x040017D9 RID: 6105
		Default
	}

	// Token: 0x02000427 RID: 1063
	public enum FreezeAxis
	{
		// Token: 0x040017DB RID: 6107
		None,
		// Token: 0x040017DC RID: 6108
		X,
		// Token: 0x040017DD RID: 6109
		Y,
		// Token: 0x040017DE RID: 6110
		Z
	}

	// Token: 0x02000428 RID: 1064
	private class Particle
	{
		// Token: 0x040017DF RID: 6111
		public Transform m_Transform;

		// Token: 0x040017E0 RID: 6112
		public int m_ParentIndex = -1;

		// Token: 0x040017E1 RID: 6113
		public float m_Damping;

		// Token: 0x040017E2 RID: 6114
		public float m_Elasticity;

		// Token: 0x040017E3 RID: 6115
		public float m_Stiffness;

		// Token: 0x040017E4 RID: 6116
		public float m_Inert;

		// Token: 0x040017E5 RID: 6117
		public float m_Friction;

		// Token: 0x040017E6 RID: 6118
		public float m_Radius;

		// Token: 0x040017E7 RID: 6119
		public float m_BoneLength;

		// Token: 0x040017E8 RID: 6120
		public bool m_isCollide;

		// Token: 0x040017E9 RID: 6121
		public Vector3 m_Position = Vector3.zero;

		// Token: 0x040017EA RID: 6122
		public Vector3 m_PrevPosition = Vector3.zero;

		// Token: 0x040017EB RID: 6123
		public Vector3 m_EndOffset = Vector3.zero;

		// Token: 0x040017EC RID: 6124
		public Vector3 m_InitLocalPosition = Vector3.zero;

		// Token: 0x040017ED RID: 6125
		public Quaternion m_InitLocalRotation = Quaternion.identity;
	}
}
