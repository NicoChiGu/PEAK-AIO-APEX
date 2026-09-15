using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200032F RID: 815
public class ShakyRock : MonoBehaviour
{
	// Token: 0x060014FE RID: 5374 RVA: 0x0006B09A File Offset: 0x0006929A
	private void Awake()
	{
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.rig = base.GetComponent<Rigidbody>();
		this.view = base.GetComponent<PhotonView>();
		this.rig.useGravity = false;
		this.rig.isKinematic = true;
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x0006B0D8 File Offset: 0x000692D8
	private void Start()
	{
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x0006B0DA File Offset: 0x000692DA
	private void Update()
	{
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x0006B0DC File Offset: 0x000692DC
	private void FixedUpdate()
	{
		if (this.isFinished)
		{
			return;
		}
		if (!this.isFalling)
		{
			return;
		}
		if (!this.once)
		{
			this.rig.AddForce(Vector3.back * this.pushOutForce, 2);
			this.once = true;
		}
		if (Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Physics.OverlapSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f), (Collider c) => c != this.meshCollider)).Count > 0)
		{
			return;
		}
		List<Collider> list = Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation), (Collider c) => c != this.meshCollider));
		Debug.Log(string.Format("Count: {0}", list.Count));
		foreach (Collider collider in list)
		{
			Vector3 vector;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, ref vector, ref num))
			{
				Debug.Log("colliding with " + collider.name);
				return;
			}
			Debug.Log("Not colliding with " + collider.name);
		}
		this.scaleOnChange = this.meshCollider.transform.lossyScale;
		this.positionOnChange = this.meshCollider.transform.position;
		this.rotationOnChange = this.meshCollider.transform.rotation;
		this.isFinished = true;
		this.rig.excludeLayers = 0;
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x0006B2FC File Offset: 0x000694FC
	private void OnCollisionEnter(Collision other)
	{
		if (this.isShaking || this.isFalling || this.isFinished)
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

	// Token: 0x06001503 RID: 5379 RVA: 0x0006B360 File Offset: 0x00069560
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

	// Token: 0x06001504 RID: 5380 RVA: 0x0006B3E0 File Offset: 0x000695E0
	private void OnDrawGizmosSelected()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		if (this.isFinished)
		{
			Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.positionOnChange, this.rotationOnChange, this.scaleOnChange);
		}
		this.meshCollider = base.GetComponent<MeshCollider>();
		List<Collider> list = Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Physics.OverlapSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f), (Collider c) => c != this.meshCollider));
		Gizmos.color = ((list.Count > 0) ? Color.red : Color.green);
		Gizmos.DrawWireSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f);
		if (list.Count > 0)
		{
			return;
		}
		List<Collider> list2 = Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation), (Collider c) => c != this.meshCollider));
		Gizmos.color = ((list2.Count > 0) ? Color.red : Color.green);
		Gizmos.DrawWireCube(this.meshCollider.bounds.center, this.meshCollider.bounds.size);
		foreach (Collider collider in list2)
		{
			Vector3 vector;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, ref vector, ref num))
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
				return;
			}
			Debug.Log("Not colliding with " + collider.name);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x0006B69C File Offset: 0x0006989C
	private void Go2()
	{
		GamefeelHandler.instance.AddPerlinShake(this.shakeAmount, 0.2f, 15f);
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x0006B6B8 File Offset: 0x000698B8
	[PunRPC]
	private void ShakeRock()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		base.StartCoroutine(this.<ShakeRock>g__RockShake|28_0());
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x0006B6D8 File Offset: 0x000698D8
	private void Go()
	{
		this.isFalling = true;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
	}

	// Token: 0x0600150D RID: 5389 RVA: 0x0006B789 File Offset: 0x00069989
	[CompilerGenerated]
	private IEnumerator <ShakeRock>g__RockShake|28_0()
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
		this.rig.AddForce(Vector3.back * this.pushOutForce, 2);
		yield break;
	}

	// Token: 0x0400137F RID: 4991
	public float fallTime = 5f;

	// Token: 0x04001380 RID: 4992
	public float amount = 1f;

	// Token: 0x04001381 RID: 4993
	public float shakeScale = 15f;

	// Token: 0x04001382 RID: 4994
	public Transform mesh;

	// Token: 0x04001383 RID: 4995
	public float shakeAmount = 10f;

	// Token: 0x04001384 RID: 4996
	public bool drawGizmos;

	// Token: 0x04001385 RID: 4997
	public float pushOutForce = 10f;

	// Token: 0x04001386 RID: 4998
	private bool isFalling;

	// Token: 0x04001387 RID: 4999
	private bool isFinished;

	// Token: 0x04001388 RID: 5000
	private bool isShaking;

	// Token: 0x04001389 RID: 5001
	private MeshCollider meshCollider;

	// Token: 0x0400138A RID: 5002
	private Transform model;

	// Token: 0x0400138B RID: 5003
	private bool once;

	// Token: 0x0400138C RID: 5004
	private Vector3 positionOnChange;

	// Token: 0x0400138D RID: 5005
	private Rigidbody rig;

	// Token: 0x0400138E RID: 5006
	private Quaternion rotationOnChange;

	// Token: 0x0400138F RID: 5007
	private Vector3 scaleOnChange;

	// Token: 0x04001390 RID: 5008
	private float tickTime;

	// Token: 0x04001391 RID: 5009
	private Vector3 velocity = Vector3.zero;

	// Token: 0x04001392 RID: 5010
	private PhotonView view;
}
