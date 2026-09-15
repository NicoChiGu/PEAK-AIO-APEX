using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class RigCreator : MonoBehaviour
{
	// Token: 0x0600036A RID: 874 RVA: 0x0001756F File Offset: 0x0001576F
	public void StartClear()
	{
		this.aboutToClear = true;
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00017578 File Offset: 0x00015778
	public void ClearNo()
	{
		this.ClearStates();
	}

	// Token: 0x0600036C RID: 876 RVA: 0x00017580 File Offset: 0x00015780
	public void ClearYes()
	{
		this.ClearStates();
		this.ClearDataAndRig();
	}

	// Token: 0x0600036D RID: 877 RVA: 0x0001758E File Offset: 0x0001578E
	public void AutoGenerate()
	{
		this.FindParts();
		this.GenerateData();
	}

	// Token: 0x0600036E RID: 878 RVA: 0x0001759C File Offset: 0x0001579C
	private void ClearStates()
	{
		this.aboutToClear = false;
	}

	// Token: 0x0600036F RID: 879 RVA: 0x000175A8 File Offset: 0x000157A8
	private void GenerateData()
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].justCreated)
			{
				this.InitPart(this.parts[i]);
			}
			else
			{
				this.ApplyPartData(this.parts[i]);
			}
			this.parts[i].justCreated = false;
		}
	}

	// Token: 0x06000370 RID: 880 RVA: 0x00017616 File Offset: 0x00015816
	private void InitPart(RigPart part)
	{
		this.AutoGenerateCollidersForPart(part);
		this.AddRigidbodyToPart(part);
		this.AddJointToPart(part);
		this.AddBodyPartScript(part);
	}

	// Token: 0x06000371 RID: 881 RVA: 0x00017634 File Offset: 0x00015834
	private void ApplyPartData(RigPart rigPart)
	{
		this.SyncCollidersFromData(rigPart);
		this.SyncRigidbodyFromData(rigPart);
		this.SyncJointFromData(rigPart);
		this.SyncBodypartScript(rigPart);
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00017652 File Offset: 0x00015852
	private void SyncBodypartScript(RigPart rigPart)
	{
		if (!rigPart.transform.GetComponent<Bodypart>())
		{
			this.AddBodyPartScript(rigPart);
		}
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0001766D File Offset: 0x0001586D
	private void SyncJointFromData(RigPart rigPart)
	{
		if (rigPart.joint == null)
		{
			this.AddJointToPart(rigPart);
		}
	}

	// Token: 0x06000374 RID: 884 RVA: 0x00017684 File Offset: 0x00015884
	private void SyncRigidbodyFromData(RigPart rigPart)
	{
		if (rigPart.rig == null)
		{
			this.AddRigidbodyToPart(rigPart);
		}
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0001769C File Offset: 0x0001589C
	private void AddRigidbodyToPart(RigPart rigPart)
	{
		Rigidbody rigidbody = rigPart.transform.gameObject.AddComponent<Rigidbody>();
		rigidbody.mass = rigPart.mass;
		rigidbody.interpolation = 1;
		RigCreatorRigidbody rigCreatorRigidbody = rigPart.transform.gameObject.AddComponent<RigCreatorRigidbody>();
		rigCreatorRigidbody.mass = rigPart.mass;
		rigPart.rig = rigidbody;
		rigPart.rigHandler = rigCreatorRigidbody;
	}

	// Token: 0x06000376 RID: 886 RVA: 0x000176F8 File Offset: 0x000158F8
	private void SyncCollidersFromData(RigPart rigPart)
	{
		for (int i = 0; i < rigPart.colliders.Count; i++)
		{
			if (rigPart.colliders[i].colliderObject == null)
			{
				rigPart.colliders[i] = this.CreateColliderObject(rigPart.colliders[i].colliderPosition, rigPart.colliders[i].colliderRotation, rigPart.colliders[i].colliderScale, rigPart.transform, rigPart.colliders[i].height, rigPart.colliders[i].radius, false);
			}
		}
	}

	// Token: 0x06000377 RID: 887 RVA: 0x000177A8 File Offset: 0x000159A8
	private RigCreatorColliderData CreateColliderObject(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, float height, float radius, bool isWorldSpace = true)
	{
		GameObject gameObject = new GameObject("RigCollider");
		if (isWorldSpace)
		{
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
		gameObject.transform.SetParent(parent);
		if (!isWorldSpace)
		{
			gameObject.transform.localPosition = position;
			gameObject.transform.localRotation = rotation;
			gameObject.transform.localScale = scale;
		}
		CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.direction = 2;
		capsuleCollider.radius = radius;
		capsuleCollider.height = height;
		RigCreatorColliderData rigCreatorColliderData = new RigCreatorColliderData();
		rigCreatorColliderData.colliderPosition = capsuleCollider.transform.position;
		rigCreatorColliderData.colliderRotation = capsuleCollider.transform.rotation;
		rigCreatorColliderData.colliderScale = capsuleCollider.transform.localScale;
		rigCreatorColliderData.radius = capsuleCollider.radius;
		rigCreatorColliderData.height = capsuleCollider.height;
		RigCreatorCollider rigCreatorCollider = gameObject.AddComponent<RigCreatorCollider>();
		rigCreatorColliderData.colliderObject = rigCreatorCollider;
		return rigCreatorColliderData;
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0001788D File Offset: 0x00015A8D
	private void AddBodyPartScript(RigPart rigPart)
	{
		rigPart.transform.gameObject.AddComponent<Bodypart>().InitBodypart(rigPart.partType);
	}

	// Token: 0x06000379 RID: 889 RVA: 0x000178AC File Offset: 0x00015AAC
	private void AddJointToPart(RigPart rigPart)
	{
		Rigidbody componentInParent = rigPart.transform.parent.GetComponentInParent<Rigidbody>();
		if (!componentInParent)
		{
			return;
		}
		ConfigurableJoint configurableJoint = this.SpawnJoint(rigPart.rig, componentInParent, rigPart.spring);
		rigPart.joint = configurableJoint;
		rigPart.jointHandler = rigPart.transform.gameObject.AddComponent<RigCreatorJoint>();
		rigPart.jointHandler.spring = rigPart.spring;
		rigPart.jointHandler.SetSpring(rigPart.spring);
	}

	// Token: 0x0600037A RID: 890 RVA: 0x00017928 File Offset: 0x00015B28
	internal ConfigurableJoint SpawnJoint(Rigidbody ownRig, Rigidbody otherRig, float spring)
	{
		ConfigurableJoint configurableJoint = ownRig.gameObject.AddComponent<ConfigurableJoint>();
		SoftJointLimit softJointLimit = configurableJoint.lowAngularXLimit;
		softJointLimit.limit = -177f;
		configurableJoint.lowAngularXLimit = softJointLimit;
		softJointLimit = configurableJoint.highAngularXLimit;
		softJointLimit.limit = 177f;
		configurableJoint.highAngularXLimit = softJointLimit;
		softJointLimit = configurableJoint.angularYLimit;
		softJointLimit.limit = 177f;
		configurableJoint.angularYLimit = softJointLimit;
		softJointLimit = configurableJoint.angularZLimit;
		softJointLimit.limit = 177f;
		configurableJoint.angularZLimit = softJointLimit;
		configurableJoint.angularXMotion = 1;
		configurableJoint.angularYMotion = 1;
		configurableJoint.angularZMotion = 1;
		configurableJoint.xMotion = 0;
		configurableJoint.yMotion = 0;
		configurableJoint.zMotion = 0;
		configurableJoint.projectionMode = 1;
		configurableJoint.connectedBody = otherRig;
		return configurableJoint;
	}

	// Token: 0x0600037B RID: 891 RVA: 0x000179E0 File Offset: 0x00015BE0
	private void AutoGenerateCollidersForPart(RigPart rigPart)
	{
		Transform transform = null;
		float num = 0f;
		for (int i = rigPart.transform.childCount - 1; i >= 0; i--)
		{
			float num2 = Vector3.Distance(rigPart.transform.GetChild(i).position, rigPart.transform.position);
			if (num2 > num)
			{
				num = num2;
				transform = rigPart.transform.GetChild(i);
			}
		}
		Vector3 vector = Vector3.Lerp(rigPart.transform.position, transform.position, 0.5f);
		Quaternion quaternion = Quaternion.LookRotation(transform.position - rigPart.transform.position);
		float num3 = Vector3.Distance(transform.position, rigPart.transform.position);
		rigPart.colliders.Add(this.CreateColliderObject(vector, quaternion, Vector3.one, rigPart.transform, num3, 0.1f, true));
	}

	// Token: 0x0600037C RID: 892 RVA: 0x00017AC4 File Offset: 0x00015CC4
	private void FindParts()
	{
		for (int i = 0; i < 179; i++)
		{
			if (this.Contains((BodypartType)i))
			{
				BodypartType bodypartType = (BodypartType)i;
				Transform transform = HelperFunctions.FindChildRecursive(bodypartType.ToString(), base.transform);
				if (transform)
				{
					this.GetPartFromPartType((BodypartType)i).transform = transform;
				}
			}
			else
			{
				BodypartType bodypartType = (BodypartType)i;
				Transform transform2 = HelperFunctions.FindChildRecursive(bodypartType.ToString(), base.transform);
				if (transform2)
				{
					RigPart rigPart = new RigPart();
					rigPart.transform = transform2;
					rigPart.partType = (BodypartType)i;
					rigPart.justCreated = true;
					this.parts.Add(rigPart);
				}
			}
		}
	}

	// Token: 0x0600037D RID: 893 RVA: 0x00017B74 File Offset: 0x00015D74
	private RigPart GetPartFromPartType(BodypartType partType)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].partType == partType)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00017BBC File Offset: 0x00015DBC
	private bool Contains(BodypartType targetType)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].partType == targetType)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600037F RID: 895 RVA: 0x00017BF8 File Offset: 0x00015DF8
	private void ClearDataAndRig()
	{
		for (int i = this.parts.Count - 1; i >= 0; i--)
		{
			for (int j = this.parts[i].colliders.Count - 1; j >= 0; j--)
			{
				Object.DestroyImmediate(this.parts[i].colliders[j].colliderObject.gameObject);
			}
			this.parts[i].colliders.Clear();
			Bodypart component = this.parts[i].transform.GetComponent<Bodypart>();
			if (component)
			{
				Object.DestroyImmediate(component);
			}
			Object.DestroyImmediate(this.parts[i].joint);
			if (this.parts[i].jointHandler)
			{
				Object.DestroyImmediate(this.parts[i].jointHandler);
			}
			Object.DestroyImmediate(this.parts[i].rig);
			Object.DestroyImmediate(this.parts[i].rigHandler);
		}
		this.parts.Clear();
	}

	// Token: 0x06000380 RID: 896 RVA: 0x00017D24 File Offset: 0x00015F24
	private RigPart GetPartFromJointObject(RigCreatorJoint jointObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].jointHandler == jointObject)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00017D70 File Offset: 0x00015F70
	private RigPart GetPartFromRigObject(RigCreatorRigidbody rigObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].rigHandler == rigObject)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00017DBC File Offset: 0x00015FBC
	private RigPart GetPartFromColliderObject(RigCreatorCollider colliderObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			for (int j = this.parts[i].colliders.Count - 1; j >= 0; j--)
			{
				if (this.parts[i].colliders[j].colliderObject == colliderObject)
				{
					return this.parts[i];
				}
			}
		}
		return null;
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00017E34 File Offset: 0x00016034
	private RigCreatorColliderData GetColliderDataFromColliderObject(RigCreatorCollider colliderObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			for (int j = this.parts[i].colliders.Count - 1; j >= 0; j--)
			{
				if (this.parts[i].colliders[j].colliderObject == colliderObject)
				{
					return this.parts[i].colliders[j];
				}
			}
		}
		return null;
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00017EB8 File Offset: 0x000160B8
	internal void RemoveCollider(RigCreatorCollider rigCreatorCollider)
	{
		RigCreatorColliderData colliderDataFromColliderObject = this.GetColliderDataFromColliderObject(rigCreatorCollider);
		if (colliderDataFromColliderObject != null)
		{
			RigPart partFromColliderObject = this.GetPartFromColliderObject(rigCreatorCollider);
			if (partFromColliderObject != null)
			{
				partFromColliderObject.colliders.Remove(colliderDataFromColliderObject);
			}
		}
	}

	// Token: 0x06000385 RID: 901 RVA: 0x00017EE8 File Offset: 0x000160E8
	internal void ColliderChanged(RigCreatorCollider rigCreatorCollider, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, float height, float radius)
	{
		RigCreatorColliderData rigCreatorColliderData = this.GetColliderDataFromColliderObject(rigCreatorCollider);
		if (rigCreatorColliderData == null)
		{
			RigPart rigPart = this.GetPartFromColliderObject(rigCreatorCollider);
			if (rigPart == null)
			{
				rigPart = this.FindPartFromName(rigCreatorCollider.transform.parent.name);
			}
			if (rigPart == null)
			{
				return;
			}
			rigCreatorColliderData = new RigCreatorColliderData();
			rigCreatorColliderData.colliderObject = rigCreatorCollider;
			rigPart.colliders.Add(rigCreatorColliderData);
		}
		rigCreatorColliderData.colliderPosition = localPosition;
		rigCreatorColliderData.colliderRotation = localRotation;
		rigCreatorColliderData.colliderScale = localScale;
		rigCreatorColliderData.height = height;
		rigCreatorColliderData.radius = radius;
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00017F65 File Offset: 0x00016165
	internal void RigidbodyChanged(RigCreatorRigidbody rigObject, float mass)
	{
		this.GetPartFromRigObject(rigObject).mass = mass;
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00017F74 File Offset: 0x00016174
	internal void JointChanged(RigCreatorJoint jointObject, float spring)
	{
		this.GetPartFromJointObject(jointObject).spring = spring;
	}

	// Token: 0x06000388 RID: 904 RVA: 0x00017F84 File Offset: 0x00016184
	private RigPart FindPartFromName(string targetName)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].partType.ToString() == targetName)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x0400031E RID: 798
	[HideInInspector]
	public bool aboutToClear;

	// Token: 0x0400031F RID: 799
	public float springMultiplier = 1f;

	// Token: 0x04000320 RID: 800
	public List<RigPart> parts;
}
