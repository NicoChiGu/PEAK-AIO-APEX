using System;
using System.Collections.Generic;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200027C RID: 636
public class JungleVine : CustomSpawnCondition, IInteractible
{
	// Token: 0x1700011C RID: 284
	// (get) Token: 0x060011B5 RID: 4533 RVA: 0x000594D0 File Offset: 0x000576D0
	public float Length
	{
		get
		{
			return this.totalLength;
		}
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x000594D8 File Offset: 0x000576D8
	private void Awake()
	{
		this.totalLength = 0f;
		this.photonView = base.GetComponent<PhotonView>();
		if (this.colliderRoot == null)
		{
			Debug.Log("No collider root, creating on awake.");
			this.colliderRoot = new GameObject("ColliderRoot").transform;
			this.colliderRoot.parent = base.transform;
			this.colliderRoot.localPosition = Vector3.zero;
			this.colliderRoot.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x0005955A File Offset: 0x0005775A
	private void Start()
	{
		this.SetRendererBounds();
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x00059564 File Offset: 0x00057764
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position + base.transform.forward * this.maxDist, 0.5f);
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		Gizmos.DrawWireCube(componentInChildren.bounds.center, componentInChildren.bounds.size);
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x000595D3 File Offset: 0x000577D3
	public bool IsInteractible(Character interactor)
	{
		return this.colliderType == JungleVine.ColliderType.Capsule;
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x000595E0 File Offset: 0x000577E0
	public void Interact(Character interactor)
	{
		interactor.refs.items.EquipSlot(Optionable<byte>.None);
		int closestChild = this.GetClosestChild(interactor.Center);
		Debug.Log(string.Format("Grabbing Vine with index: {0}", closestChild));
		interactor.GetComponent<PhotonView>().RPC("GrabVineRpc", 0, new object[]
		{
			base.GetComponent<PhotonView>(),
			closestChild
		});
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0005964D File Offset: 0x0005784D
	public void HoverEnter()
	{
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x0005964F File Offset: 0x0005784F
	public void HoverExit()
	{
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x00059651 File Offset: 0x00057851
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x0005965E File Offset: 0x0005785E
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x00059666 File Offset: 0x00057866
	public string GetInteractionText()
	{
		return LocalizedText.GetText("GRAB", true);
	}

	// Token: 0x060011C0 RID: 4544 RVA: 0x00059673 File Offset: 0x00057873
	public string GetName()
	{
		return LocalizedText.GetText(this.displayName, true);
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x00059684 File Offset: 0x00057884
	public void PickTreePlatforms(List<TreePlatform> treePlatformList)
	{
		List<TreePlatform> list = new List<TreePlatform>(treePlatformList);
		int num = 50;
		while (list.Count >= 2 && !this.startTreePlatform && num > 0)
		{
			int num2 = Random.Range(0, list.Count);
			this.startTreePlatform = list[num2];
			num--;
		}
		if (num == 0)
		{
			Debug.LogError("Timed out picking a start tree platform for a vine");
		}
		if (this.startTreePlatform == null)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		List<TreePlatform> list2 = new List<TreePlatform>(treePlatformList);
		Debug.LogError("end list count is " + list2.Count.ToString());
		list2.Remove(this.startTreePlatform);
		for (int i = list2.Count - 1; i >= 0; i--)
		{
			float num3 = Vector3.Distance(list2[i].transform.position, this.startTreePlatform.transform.position);
			if (list2[i].connectedPlatforms.Contains(this.startTreePlatform))
			{
				list2.RemoveAt(i);
			}
			else if (num3 < this.minDist || num3 > this.maxDist)
			{
				list2.RemoveAt(i);
			}
			else if (Mathf.Abs(list2[i].transform.position.y - this.startTreePlatform.transform.position.y) > this.maxHeightDifference)
			{
				list2.RemoveAt(i);
			}
		}
		if (list2.Count == 0)
		{
			if (!this.ConfigVine(base.transform.position))
			{
				Object.DestroyImmediate(base.gameObject);
			}
			return;
		}
		int num4 = Random.Range(0, list2.Count);
		this.endTreePlatform = list2[num4];
		this.startTreePlatform.connectedPlatforms.Add(this.endTreePlatform);
		this.endTreePlatform.connectedPlatforms.Add(this.startTreePlatform);
		if (!this.ConfigVine(base.transform.position))
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x00059888 File Offset: 0x00057A88
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		Vector3 vector = data.normal;
		vector.y *= this.normalYMult;
		vector = vector.normalized;
		RaycastHit raycastHit = HelperFunctions.LineCheck(base.transform.position + vector * 0.02f, base.transform.position + vector * this.maxDist, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		if (!raycastHit.transform)
		{
			return false;
		}
		if (raycastHit.distance < this.minDist)
		{
			return false;
		}
		if (Mathf.Abs(raycastHit.point.y - base.transform.position.y) > this.maxHeightDifference)
		{
			return false;
		}
		bool flag = this.ConfigVine(raycastHit.point);
		BreakableBridge breakableBridge;
		if (flag && base.TryGetComponent<BreakableBridge>(ref breakableBridge))
		{
			breakableBridge.AddCollisionModifiers();
		}
		return flag;
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x00059968 File Offset: 0x00057B68
	public static bool CheckVinePath(Vector3 from, Vector3 to, float hang, out Vector3 mid, float radius = 0f)
	{
		mid = Vector3.Lerp(from, to, 0.5f);
		mid.y += hang;
		for (int i = 0; i < 50; i++)
		{
			float num = (float)i / 49f;
			Vector3 vector = BezierCurve.QuadraticBezier(from, mid, to, num);
			if (i < 49 && HelperFunctions.LineCheck(from, vector, HelperFunctions.LayerType.TerrainMap, radius, 1).transform)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x000599DC File Offset: 0x00057BDC
	[PunRPC]
	public void ForceBuildVine_RPC(Vector3 from, Vector3 to, float hang, Vector3 mid)
	{
		this.ForceBuildVine(from, to, hang, mid);
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x000599EC File Offset: 0x00057BEC
	public void ForceBuildVine(Vector3 from, Vector3 to, float hang, Vector3 mid)
	{
		if (this.colliderRoot == null)
		{
			if (Application.isPlaying)
			{
				Debug.LogWarning("colliderRoot was null, creating new one for " + base.gameObject.name);
			}
			this.colliderRoot = new GameObject("ColliderRoot").transform;
			this.colliderRoot.parent = base.transform;
			this.colliderRoot.localPosition = Vector3.zero;
			this.colliderRoot.localRotation = Quaternion.identity;
		}
		ExtTransform.KillAllChildren(this.colliderRoot, true, false, false);
		float num = Vector3.Distance(from, to) / this.meshLength;
		Renderer componentInChildren = base.GetComponentInChildren<Renderer>();
		componentInChildren.material.SetFloat("_Hang", hang);
		if (this.hangCenter != null)
		{
			this.hangCenter.position = BezierCurve.QuadraticBezier(from, mid, to, 0.5f);
		}
		Vector3 vector = from;
		for (int i = 0; i < 50; i++)
		{
			float num2 = (float)i / 49f;
			Vector3 vector2 = BezierCurve.QuadraticBezier(from, mid, to, num2);
			GameObject gameObject = new GameObject("Collider");
			if (this.connectsPlatforms)
			{
				gameObject.tag = "Wood";
			}
			gameObject.transform.parent = this.colliderRoot;
			if (this.colliderType == JungleVine.ColliderType.Capsule)
			{
				CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.radius = 0.25f;
				capsuleCollider.height = Vector3.Distance(vector, vector2) + 0.5f;
				capsuleCollider.isTrigger = true;
			}
			else
			{
				gameObject.AddComponent<BoxCollider>().size = new Vector3(this.boxShape.x * num, Vector3.Distance(vector, vector2) + 0.5f, this.boxShape.y);
			}
			gameObject.transform.rotation = HelperFunctions.GetRotationWithUp(Vector3.down, vector2 - vector);
			gameObject.transform.position = vector2 - Vector3.down * this.colliderOffset;
			gameObject.gameObject.layer = 21;
			vector = vector2;
		}
		if (this.connectsPlatforms)
		{
			Object.DestroyImmediate(this.colliderRoot.GetChild(0).gameObject);
			Object.DestroyImmediate(this.colliderRoot.GetChild(this.colliderRoot.childCount - 1).gameObject);
		}
		Transform transform = base.transform.Find("Mesh");
		transform.transform.rotation = Quaternion.LookRotation(to - from);
		transform.transform.localScale = Vector3.one * num;
		if (num < 0.5f)
		{
			componentInChildren.material.SetFloat("_LengthScale", num * 2f);
		}
		this.SetRendererBounds();
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x00059C88 File Offset: 0x00057E88
	public bool ConfigVine(Vector3 to)
	{
		ExtCollections.KillAllGameObjects<Collider>(base.GetComponentsInChildren<Collider>(), true);
		float num = Random.Range(-this.maxDown, -this.minDown);
		float num2 = 0f;
		if (this.startTreePlatform && this.endTreePlatform)
		{
			Vector3 vector = this.endTreePlatform.transform.position - this.startTreePlatform.transform.position;
			vector.y = 0f;
			vector = vector.normalized * 8f;
			base.transform.position = this.startTreePlatform.transform.position + vector;
			to = this.endTreePlatform.transform.position - vector;
		}
		else if (this.startTreePlatform)
		{
			Vector3 normalized = Random.onUnitSphere.normalized;
			normalized.y = Random.Range(-0.5f, 0.5f);
			RaycastHit raycastHit = HelperFunctions.LineCheck(this.startTreePlatform.transform.position + normalized * 8f, this.startTreePlatform.transform.position + normalized * this.maxDist, HelperFunctions.LayerType.TerrainMap, 0f, 1);
			if (!raycastHit.transform || (raycastHit.transform.gameObject.GetComponent<MeshRenderer>() && !(raycastHit.transform.gameObject.GetComponent<MeshRenderer>().sharedMaterial != this.bannedMaterial[0])) || raycastHit.transform.gameObject.CompareTag("Redwood") || Vector3.Distance(raycastHit.point, this.startTreePlatform.transform.position) <= this.minDist)
			{
				return false;
			}
			Vector3 point = raycastHit.point;
			Vector3 vector2 = point - this.startTreePlatform.transform.position;
			vector2.y = 0f;
			vector2 = vector2.normalized * 8f;
			base.transform.position = this.startTreePlatform.transform.position + vector2;
			to = point;
		}
		Vector3 position = base.transform.position;
		if (this.connectsPlatforms && Vector3.Angle(to - position, Vector3.up) < 45f)
		{
			return false;
		}
		Vector3 vector3;
		if (!JungleVine.CheckVinePath(position, to, num, out vector3, num2))
		{
			return false;
		}
		this.ForceBuildVine(position, to, num, vector3);
		return true;
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x00059F25 File Offset: 0x00058125
	public void ConnectDebug()
	{
		this.ConfigVine(this.connectTo.transform.position);
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x00059F40 File Offset: 0x00058140
	private void SetRendererBounds()
	{
		Vector3 vector = base.transform.position;
		Vector3 vector2 = base.transform.position;
		if (this.colliderRoot != null)
		{
			this.totalLength = 0f;
			for (int i = 0; i < Mathf.Min(this.colliderRoot.transform.childCount, 50); i++)
			{
				Vector3 position = this.colliderRoot.transform.GetChild(i).transform.position;
				this.totalLength += Vector3.Distance(vector, position);
				vector = position;
				if (position.y < vector2.y)
				{
					vector2 = position;
				}
			}
		}
		Renderer componentInChildren = base.GetComponentInChildren<Renderer>();
		Bounds localBounds = componentInChildren.localBounds;
		localBounds.Encapsulate(componentInChildren.transform.InverseTransformPoint(vector2));
		componentInChildren.localBounds = localBounds;
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x0005A014 File Offset: 0x00058214
	public float LengthFactor()
	{
		if (Mathf.Approximately(0f, this.totalLength))
		{
			if (!this._hasInitializedWrong)
			{
				this._hasInitializedWrong = true;
				Debug.LogError(string.Format("We didn't add up the length for our {0} colliders! ", this.colliderRoot.childCount) + "Returning 0 to avoid NaNs", this);
			}
			return 0f;
		}
		return 1f / this.totalLength;
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x0005A07E File Offset: 0x0005827E
	public float GetPercentFromSegmentIndex(int segmentIndex)
	{
		return (float)segmentIndex / 49f;
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0005A088 File Offset: 0x00058288
	public int GetIndexFromPercentage(float percent)
	{
		return Mathf.RoundToInt(Mathf.Lerp(0f, 49f, percent));
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x0005A09F File Offset: 0x0005829F
	internal Vector3 GetDir(Vector3 facingDirection, Vector3 up)
	{
		if (Vector3.Angle(facingDirection, up) > 90f)
		{
			up *= -1f;
		}
		return up;
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x0005A0C0 File Offset: 0x000582C0
	public float GetVineVel(Vector3 vel, float percent)
	{
		int indexFromPercentage = this.GetIndexFromPercentage(percent);
		Vector3 up = this.colliderRoot.transform.GetChild(indexFromPercentage).up;
		Vector3 dir = this.GetDir(vel, up);
		float num = ((Vector3.Angle(vel, up) > 90f) ? (-1f) : 1f) * Vector3.Project(vel, up).magnitude * Mathf.InverseLerp(0f, -0.5f, dir.y);
		Debug.Log(string.Format("Air speed of {0} converted to vine speed of {1}.", vel.magnitude, num), this);
		return num;
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0005A15C File Offset: 0x0005835C
	public float GetSign(Vector3 dir, float percent)
	{
		Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		float num = 1f;
		if (Vector3.Angle(dir, up) > 90f)
		{
			num = -1f;
		}
		return num;
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x0005A1A4 File Offset: 0x000583A4
	public Vector3 GetUp(float percent)
	{
		Vector3 vector = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		if (Vector3.Angle(Vector3.up, vector) > 90f)
		{
			vector *= -1f;
		}
		return vector;
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x0005A1F0 File Offset: 0x000583F0
	public float UpMult(float percent)
	{
		Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		return (float)((Vector3.Angle(Vector3.up, up) < 90f) ? 1 : (-1));
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x0005A234 File Offset: 0x00058434
	public Vector3 GetPosition(float percent, int clampFromBottom = 0, int clampFromTop = 0)
	{
		int childCount = this.colliderRoot.transform.childCount;
		if (childCount != 50 && !this._hasInitializedWrong)
		{
			this._hasInitializedWrong = true;
			Debug.LogError(string.Format("Expected {0} to have {1} colliders, ", this.colliderRoot.name, 50) + string.Format("but it has only {0}! We'll need to clamp", this.colliderRoot.transform.childCount), this);
		}
		float num = Mathf.Clamp01(percent) * (float)(childCount - 1);
		int num2 = Mathf.FloorToInt(num);
		int num3 = childCount - clampFromTop - 2;
		int num4 = Math.Clamp(num2, clampFromBottom, num3);
		int num5 = num4 + 1;
		float num6 = Mathf.Clamp01(num - (float)num4);
		Vector3 position = this.colliderRoot.transform.GetChild(num4).position;
		Vector3 position2 = this.colliderRoot.transform.GetChild(num5).position;
		return Vector3.Lerp(position, position2, num6);
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0005A310 File Offset: 0x00058510
	private int GetClosestChild(Vector3 center)
	{
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < this.colliderRoot.transform.childCount; i++)
		{
			float num3 = Vector3.Distance(center, this.colliderRoot.transform.GetChild(i).position);
			if (num3 < num)
			{
				num = num3;
				num2 = i;
			}
		}
		if (num2 == -1 && !this._hasInitializedWrong)
		{
			Debug.LogError(string.Format("Uh oh! We failed to find a valid collider child on {0}", this), this);
		}
		return num2;
	}

	// Token: 0x0400103C RID: 4156
	private bool _hasInitializedWrong;

	// Token: 0x0400103D RID: 4157
	public float minDist = 25f;

	// Token: 0x0400103E RID: 4158
	public float maxDist = 50f;

	// Token: 0x0400103F RID: 4159
	public float maxHeightDifference = 100f;

	// Token: 0x04001040 RID: 4160
	public float normalYMult = 1f;

	// Token: 0x04001041 RID: 4161
	public float minDown = 5f;

	// Token: 0x04001042 RID: 4162
	public float maxDown = 30f;

	// Token: 0x04001043 RID: 4163
	public float meshLength = 40f;

	// Token: 0x04001044 RID: 4164
	public int vineType;

	// Token: 0x04001045 RID: 4165
	public float colliderOffset;

	// Token: 0x04001046 RID: 4166
	public JungleVine.ColliderType colliderType;

	// Token: 0x04001047 RID: 4167
	public Transform colliderTransform;

	// Token: 0x04001048 RID: 4168
	public Vector2 boxShape = Vector2.one;

	// Token: 0x04001049 RID: 4169
	public PhotonView photonView;

	// Token: 0x0400104A RID: 4170
	public Transform connectTo;

	// Token: 0x0400104B RID: 4171
	private const int segments = 50;

	// Token: 0x0400104C RID: 4172
	private float totalLength;

	// Token: 0x0400104D RID: 4173
	public Transform hangCenter;

	// Token: 0x0400104E RID: 4174
	public Material[] bannedMaterial;

	// Token: 0x0400104F RID: 4175
	public string displayName;

	// Token: 0x04001050 RID: 4176
	public bool connectsPlatforms;

	// Token: 0x04001051 RID: 4177
	public TreePlatform startTreePlatform;

	// Token: 0x04001052 RID: 4178
	public TreePlatform endTreePlatform;

	// Token: 0x04001053 RID: 4179
	public Transform colliderRoot;

	// Token: 0x020004DC RID: 1244
	public enum ColliderType
	{
		// Token: 0x04001ABF RID: 6847
		Capsule,
		// Token: 0x04001AC0 RID: 6848
		Box
	}
}
