using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000016 RID: 22
[DefaultExecutionOrder(-99)]
public class CharacterRagdoll : MonoBehaviour
{
	// Token: 0x060001F4 RID: 500 RVA: 0x0000F468 File Offset: 0x0000D668
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
		Bodypart[] componentsInChildren = base.GetComponentsInChildren<Bodypart>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.RegisterBodypart(componentsInChildren[i]);
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000F4A0 File Offset: 0x0000D6A0
	private void Start()
	{
		this.rotationBefore = this.character.refs.rigCreator.transform.rotation;
		if (this.character.refs.ikRigBuilder)
		{
			this.character.refs.ikRigBuilder.Build(this.character.refs.animator.playableGraph);
		}
		if (this.forceDefaultPoseOnStart)
		{
			this.character.refs.animator.playableGraph.Evaluate(0f);
		}
		this.character.refs.animator.playableGraph.Stop();
		this.character.refs.animator.playableGraph.SetTimeUpdateMode(3);
		this.RegisterBodypartColliders();
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000F57C File Offset: 0x0000D77C
	private void SetPhysicsMats()
	{
		foreach (Bodypart bodypart in this.partList)
		{
			bodypart.SetPhysicsMaterial(this.GetFrictionType(), this.slipperyMat, this.normalMat);
		}
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000F5E0 File Offset: 0x0000D7E0
	public void ToggleCollision(bool enableCollision)
	{
		for (int i = 0; i < this.colliderList.Count; i++)
		{
			this.colliderList[i].enabled = enableCollision;
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000F618 File Offset: 0x0000D818
	public void ToggleKinematic(bool enableKinematic)
	{
		this.character.data.isKinecmatic = enableKinematic;
		for (int i = 0; i < this.rigidbodies.Count; i++)
		{
			this.rigidbodies[i].isKinematic = enableKinematic;
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x0000F65E File Offset: 0x0000D85E
	private Bodypart.FrictionType GetFrictionType()
	{
		if (this.character.data.currentRagdollControll < 0.9f)
		{
			return Bodypart.FrictionType.Grippy;
		}
		return Bodypart.FrictionType.Slippery;
	}

	// Token: 0x060001FA RID: 506 RVA: 0x0000F67A File Offset: 0x0000D87A
	private void OnDestroy()
	{
		if (this.m_PlayableGraph.IsValid())
		{
			this.m_PlayableGraph.Destroy();
		}
		this.DeregisterBodypartColliders();
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000F69C File Offset: 0x0000D89C
	private void RegisterBodypart(Bodypart bodypart)
	{
		this.partList.Add(bodypart);
		this.partDict.Add(bodypart.partType, bodypart);
		this.rigidbodies.Add(bodypart.Rig);
		bodypart.Rig.mass *= this.massMultiplier;
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000F6F0 File Offset: 0x0000D8F0
	public static bool TryGetCharacterFromCollider(Collider c, out Character character)
	{
		return CharacterRagdoll.COLLIDERS_TO_CHARACTERS.TryGetValue(c, ref character);
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000F700 File Offset: 0x0000D900
	private void RegisterBodypartColliders()
	{
		foreach (Bodypart bodypart in this.partList)
		{
			foreach (RigCreatorCollider rigCreatorCollider in bodypart.colliders)
			{
				CapsuleCollider capsuleCollider = rigCreatorCollider.Col();
				if (capsuleCollider)
				{
					CharacterRagdoll.COLLIDERS_TO_CHARACTERS.TryAdd(capsuleCollider, this.character);
				}
			}
		}
	}

	// Token: 0x060001FE RID: 510 RVA: 0x0000F7A4 File Offset: 0x0000D9A4
	private void DeregisterBodypartColliders()
	{
		List<Collider> list = new List<Collider>();
		foreach (KeyValuePair<Collider, Character> keyValuePair in CharacterRagdoll.COLLIDERS_TO_CHARACTERS)
		{
			if (keyValuePair.Value == this.character)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (Collider collider in list)
		{
			CharacterRagdoll.COLLIDERS_TO_CHARACTERS.Remove(collider);
		}
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000F85C File Offset: 0x0000DA5C
	public void FixedUpdate()
	{
		this.SetPhysicsMats();
		if (this.firstFrame)
		{
			this.firstFrame = false;
			return;
		}
		if (this.character.data.currentItem)
		{
			this.character.refs.animations.PrepIK();
		}
		this.RotateCharacter();
		this.character.refs.ikRigBuilder.SyncLayers();
		this.character.refs.ikRigBuilder.Evaluate(Time.fixedDeltaTime);
		this.character.refs.animator.playableGraph.Evaluate(Time.fixedDeltaTime);
		this.character.refs.animations.ConfigureIK();
		for (int i = 0; i < this.partList.Count; i++)
		{
			this.partList[i].SaveAnimationData();
			this.DrawLines(this.partList[i].jointParent, this.partList[i]);
		}
		this.SaveAdditionalTransformPositions();
		this.ResetRotation();
		for (int j = 0; j < this.partList.Count; j++)
		{
			this.partList[j].ResetTransform();
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000F994 File Offset: 0x0000DB94
	public void SnapToAnimation()
	{
		for (int i = 0; i < this.character.refs.ragdoll.partList.Count; i++)
		{
			this.character.refs.ragdoll.partList[i].SnapToAnim();
		}
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000F9E8 File Offset: 0x0000DBE8
	private void DrawLines(Bodypart parent, Bodypart part)
	{
		if (parent)
		{
			Debug.DrawLine(this.character.GetAnimationRelativePosition(part.transform.position), this.character.GetAnimationRelativePosition(parent.transform.position), Color.white);
			Debug.DrawLine(this.character.GetAnimationRelativePosition(part.transform.position), part.Rig.position, Color.red);
		}
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000FA5E File Offset: 0x0000DC5E
	private void RotateCharacter()
	{
		this.rotationBefore = this.character.refs.rigCreator.transform.rotation;
		this.character.SetRotation();
	}

	// Token: 0x06000203 RID: 515 RVA: 0x0000FA8B File Offset: 0x0000DC8B
	private void ResetRotation()
	{
		this.character.refs.rigCreator.transform.rotation = this.rotationBefore;
	}

	// Token: 0x06000204 RID: 516 RVA: 0x0000FAB0 File Offset: 0x0000DCB0
	private void SaveAdditionalTransformPositions()
	{
		Bodypart bodypart = this.character.GetBodypart(BodypartType.Head);
		Vector3 vector = bodypart.transform.position - this.character.refs.rigCreator.transform.position;
		this.character.data.targetHeadHeight = vector.y;
		this.character.refs.animationHeadTransform.position = bodypart.transform.position;
		this.character.refs.animationHeadTransform.rotation = bodypart.transform.rotation;
		this.character.refs.animationLookTransform.position = bodypart.transform.position;
		this.character.refs.animationLookTransform.rotation = Quaternion.Euler(-this.character.data.lookValues.y * 0.5f, this.character.data.lookValues.x, 0f);
		Bodypart bodypart2 = this.character.GetBodypart(BodypartType.Hip);
		Vector3 vector2 = bodypart2.transform.position - this.character.refs.rigCreator.transform.position;
		this.character.data.targetHipHeight = vector2.y;
		this.character.refs.animationHipTransform.position = bodypart2.transform.position;
		this.character.refs.animationHipTransform.rotation = bodypart2.transform.rotation;
		if (this.character.data.currentItem)
		{
			this.character.refs.animationItemTransform.position = this.character.refs.animationLookTransform.TransformPoint(this.character.data.currentItem.defaultPos);
			Vector3 vector3 = this.character.data.lookDirection * this.character.data.currentItem.defaultForward.z;
			vector3 += this.character.data.lookDirection_Right * this.character.data.currentItem.defaultForward.x;
			vector3 += this.character.data.lookDirection_Up * this.character.data.currentItem.defaultForward.y;
			this.character.refs.animationItemTransform.rotation = Quaternion.LookRotation(vector3);
		}
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000FD60 File Offset: 0x0000DF60
	public void HaltBodyVelocity()
	{
		foreach (Rigidbody rigidbody in this.rigidbodies)
		{
			rigidbody.linearVelocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000FDC0 File Offset: 0x0000DFC0
	public void MoveAllRigsInDirection(Vector3 delta)
	{
		foreach (Rigidbody rigidbody in this.rigidbodies)
		{
			rigidbody.MovePosition(rigidbody.position + delta);
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000FE1C File Offset: 0x0000E01C
	internal void SetInterpolation(bool interpolateEnabled)
	{
		foreach (Bodypart bodypart in this.partList)
		{
			bodypart.Rig.interpolation = (interpolateEnabled ? 1 : 0);
		}
	}

	// Token: 0x040001D5 RID: 469
	public float massMultiplier = 1f;

	// Token: 0x040001D6 RID: 470
	public List<Bodypart> partList = new List<Bodypart>();

	// Token: 0x040001D7 RID: 471
	public Dictionary<BodypartType, Bodypart> partDict = new Dictionary<BodypartType, Bodypart>();

	// Token: 0x040001D8 RID: 472
	private List<Rigidbody> rigidbodies = new List<Rigidbody>();

	// Token: 0x040001D9 RID: 473
	private Character character;

	// Token: 0x040001DA RID: 474
	public bool forceDefaultPoseOnStart = true;

	// Token: 0x040001DB RID: 475
	public PhysicsMaterial slipperyMat;

	// Token: 0x040001DC RID: 476
	public PhysicsMaterial normalMat;

	// Token: 0x040001DD RID: 477
	internal List<Collider> colliderList = new List<Collider>();

	// Token: 0x040001DE RID: 478
	private PlayableGraph m_PlayableGraph;

	// Token: 0x040001DF RID: 479
	private static Dictionary<Collider, Character> COLLIDERS_TO_CHARACTERS = new Dictionary<Collider, Character>();

	// Token: 0x040001E0 RID: 480
	private bool firstFrame = true;

	// Token: 0x040001E1 RID: 481
	private Quaternion rotationBefore;
}
