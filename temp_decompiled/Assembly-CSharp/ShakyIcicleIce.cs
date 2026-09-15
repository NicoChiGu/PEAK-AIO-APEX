using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200032D RID: 813
public class ShakyIcicleIce : MonoBehaviour
{
	// Token: 0x060014D7 RID: 5335 RVA: 0x00069D5C File Offset: 0x00067F5C
	private void Awake()
	{
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.rig = base.GetComponent<Rigidbody>();
		this.view = base.GetComponent<PhotonView>();
		this.fractureRoot.gameObject.SetActive(false);
		this.rig.useGravity = false;
		this.rig.isKinematic = true;
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x00069DB6 File Offset: 0x00067FB6
	private void Start()
	{
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x00069DB8 File Offset: 0x00067FB8
	private void Update()
	{
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x00069DBC File Offset: 0x00067FBC
	private void SetIgnoreColliders()
	{
		this.ignoreColliders = new HashSet<Collider>();
		HashSet<Collider> hashSet = Enumerable.ToHashSet<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation), (Collider c) => c != this.meshCollider));
		Vector3 vector = base.transform.TransformVector(this.innerCheck);
		Vector3 vector2 = base.transform.position + -base.transform.up * vector.y;
		Debug.Log(string.Format("Count: {0}", hashSet.Count));
		foreach (Collider collider in hashSet)
		{
			Vector3 vector3;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, ref vector3, ref num))
			{
				this.ignoreColliders.Add(collider);
			}
			else if (Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(vector2, vector, base.transform.rotation), (Collider c) => c != this.meshCollider)).Count > 0)
			{
				this.ignoreColliders.Add(collider);
			}
		}
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x00069F54 File Offset: 0x00068154
	private bool CheckInTheClear()
	{
		HashSet<Collider> hashSet = Enumerable.ToHashSet<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation), (Collider c) => c != this.meshCollider));
		if (hashSet.Count == 0 || !Enumerable.Any<Collider>(hashSet, (Collider c) => this.ignoreColliders.Contains(c)))
		{
			this.scaleOnChange = this.meshCollider.transform.lossyScale;
			this.positionOnChange = this.meshCollider.transform.position;
			this.rotationOnChange = this.meshCollider.transform.rotation;
			return true;
		}
		return false;
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x0006A010 File Offset: 0x00068210
	private void FixedUpdate()
	{
		if (!this.isFalling)
		{
			return;
		}
		if (this.isFractured)
		{
			return;
		}
		if (!this.once)
		{
			this.once = true;
		}
		if (!this.isInTheClear)
		{
			this.isInTheClear = this.CheckInTheClear();
			if (this.isInTheClear)
			{
				this.ignoreColliders.Clear();
				this.rig.excludeLayers = 0;
			}
		}
		Vector3 vector;
		Vector3 vector2;
		List<Collider> list;
		if (this.CheckBoundingBox(out vector, out vector2, out list))
		{
			this.isFractured = true;
			this.mesh.gameObject.SetActive(false);
			Object.Destroy(this.meshCollider);
			Object.Destroy(base.GetComponent<MeshRenderer>());
			this.fractureRoot.gameObject.SetActive(true);
			Object.Destroy(this.rig);
		}
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x0006A0D0 File Offset: 0x000682D0
	private void OnCollisionEnter(Collision other)
	{
		if (this.isShaking || this.isFalling)
		{
			return;
		}
		Character componentInParent = other.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		Debug.Log("Before Shake rock");
		this.view.RPC("ShakeRock", 0, Array.Empty<object>());
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x0006A12C File Offset: 0x0006832C
	private void OnCollisionStay(Collision other)
	{
		if (!this.isShaking)
		{
			return;
		}
		Character componentInParent = other.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		this.tickTime += Time.deltaTime;
		if ((double)this.tickTime > 0.1)
		{
			this.tickTime = 0f;
			GamefeelHandler.instance.AddPerlinShake(this.shakeAmount, 0.2f, 15f);
		}
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x0006A1AC File Offset: 0x000683AC
	private bool CheckInnerBox(out Vector3 halfExtends, out Vector3 innerCheckPosition)
	{
		halfExtends = base.transform.TransformVector(this.innerCheck);
		innerCheckPosition = base.transform.position + -base.transform.up * halfExtends.y;
		return Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(innerCheckPosition, halfExtends, base.transform.rotation), (Collider c) => c != this.meshCollider)), (Collider c) => !this.ignoreColliders.Contains(c))).Count > 0;
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x0006A254 File Offset: 0x00068454
	public bool CheckBoundingBox(out Vector3 halfExtends, out Vector3 position, out List<Collider> colliders)
	{
		halfExtends = this.meshCollider.bounds.extents;
		position = this.meshCollider.bounds.center;
		colliders = Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(position, halfExtends, base.transform.rotation), (Collider c) => c != this.meshCollider));
		colliders = Enumerable.ToList<Collider>(Enumerable.Where<Collider>(colliders, (Collider c) => !this.ignoreColliders.Contains(c)));
		return colliders.Count > 0;
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x0006A2EC File Offset: 0x000684EC
	public bool ConvexMeshCollision(List<Collider> colliders)
	{
		foreach (Collider collider in colliders)
		{
			Vector3 vector;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, ref vector, ref num))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x0006A384 File Offset: 0x00068584
	private void OnDrawGizmosSelected()
	{
		if (!this.drawGizmos || this.isFractured)
		{
			return;
		}
		this.meshCollider = base.GetComponent<MeshCollider>();
		if (this.isInTheClear)
		{
			Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.positionOnChange, this.rotationOnChange, this.scaleOnChange);
		}
		foreach (Collider collider in this.ignoreColliders)
		{
			Debug.DrawLine(base.transform.position, collider.bounds.center);
		}
		this.CheckInTheClear();
		Vector3 vector;
		Vector3 vector2;
		Gizmos.color = (this.CheckInnerBox(out vector, out vector2) ? Color.red : Color.green);
		Gizmos.DrawCube(vector2, vector * 2f);
		Vector3 vector3;
		Vector3 vector4;
		List<Collider> list;
		Gizmos.color = (this.CheckBoundingBox(out vector3, out vector4, out list) ? Color.red : Color.green);
		Gizmos.DrawWireCube(vector4, vector3 * 2f);
		Gizmos.color = (this.ConvexMeshCollision(list) ? Color.red : Color.green);
		Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x0006A4FC File Offset: 0x000686FC
	[PunRPC]
	private void ShakeRock()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		base.StartCoroutine(this.<ShakeRock>g__RockShake|36_0());
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x0006A51C File Offset: 0x0006871C
	private void Go()
	{
		this.isFalling = true;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x0006A616 File Offset: 0x00068816
	[CompilerGenerated]
	private IEnumerator <ShakeRock>g__RockShake|36_0()
	{
		Debug.Log("Start shaking");
		float duration = 0f;
		Debug.Log(string.Format("duration: {0}, fallTime: {1}", duration, this.fallTime));
		while (duration < this.fallTime)
		{
			Debug.Log(string.Format("duration: {0}, fallTime: {1}", duration, this.fallTime));
			Vector3 vector = Vector2.zero;
			vector.x += Perlin.Noise(Time.time * this.shakeScale, 0f, 0f) - 0.5f;
			vector.y += Perlin.Noise(0f, Time.time * this.shakeScale, 0f) - 0.5f;
			vector.z += Perlin.Noise(0f, 0f, Time.time * this.shakeScale) - 0.5f;
			vector *= this.amount * Time.deltaTime;
			duration += Time.deltaTime;
			Debug.Log(string.Format("offset: {0}", vector));
			this.mesh.localPosition = vector;
			yield return null;
		}
		Debug.Log("Done shaking");
		this.isShaking = false;
		this.mesh.localPosition = ExtDataTypes.ToVec(0);
		this.isFalling = true;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
		yield break;
	}

	// Token: 0x04001348 RID: 4936
	public float fallTime = 5f;

	// Token: 0x04001349 RID: 4937
	public float amount = 1f;

	// Token: 0x0400134A RID: 4938
	public float shakeScale = 15f;

	// Token: 0x0400134B RID: 4939
	public Transform mesh;

	// Token: 0x0400134C RID: 4940
	public float shakeAmount = 10f;

	// Token: 0x0400134D RID: 4941
	public bool drawGizmos;

	// Token: 0x0400134E RID: 4942
	public float pushOutForce = 10f;

	// Token: 0x0400134F RID: 4943
	private bool isFalling;

	// Token: 0x04001350 RID: 4944
	private bool isInTheClear;

	// Token: 0x04001351 RID: 4945
	private bool isShaking;

	// Token: 0x04001352 RID: 4946
	private MeshCollider meshCollider;

	// Token: 0x04001353 RID: 4947
	private Transform model;

	// Token: 0x04001354 RID: 4948
	private bool once;

	// Token: 0x04001355 RID: 4949
	private Vector3 positionOnChange;

	// Token: 0x04001356 RID: 4950
	private Rigidbody rig;

	// Token: 0x04001357 RID: 4951
	private Quaternion rotationOnChange;

	// Token: 0x04001358 RID: 4952
	private Vector3 scaleOnChange;

	// Token: 0x04001359 RID: 4953
	private float tickTime;

	// Token: 0x0400135A RID: 4954
	private Vector3 velocity = Vector3.zero;

	// Token: 0x0400135B RID: 4955
	private PhotonView view;

	// Token: 0x0400135C RID: 4956
	public Vector3 innerCheck;

	// Token: 0x0400135D RID: 4957
	private HashSet<Collider> ignoreColliders = new HashSet<Collider>();

	// Token: 0x0400135E RID: 4958
	public Transform fractureRoot;

	// Token: 0x0400135F RID: 4959
	private bool isFractured;
}
