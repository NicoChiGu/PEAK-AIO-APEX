using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using Sirenix.Utilities;
using UnityEngine;

// Token: 0x0200032E RID: 814
public class ShakyIcicleIce2 : MonoBehaviour
{
	// Token: 0x17000147 RID: 327
	// (get) Token: 0x060014EF RID: 5359 RVA: 0x0006A625 File Offset: 0x00068825
	private bool IsLocalPlayerClimbing
	{
		get
		{
			return Character.localCharacter.data.isClimbing && Character.localCharacter.data.climbHit.collider == this.meshCollider;
		}
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x060014F0 RID: 5360 RVA: 0x0006A659 File Offset: 0x00068859
	private float DistanceToLocalPlayer
	{
		get
		{
			return Vector3.Distance(Character.localCharacter.Center, base.transform.position);
		}
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x0006A678 File Offset: 0x00068878
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.photonView = base.GetComponent<PhotonView>();
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.startPeicesCount = this.fracturedRoot.transform.childCount;
		this.fracturedRoot.gameObject.SetActive(false);
		this.source.volume = 0f;
		this.source.Stop();
		if (Random.Range(0f, 1f) > this.fallChance)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x0006A70C File Offset: 0x0006890C
	private void Start()
	{
		this.fracturedRoot.gameObject.SetActive(true);
		this.stuckies = this.GetStuckPieces();
		this.fracturedRoot.gameObject.SetActive(false);
		this.fullMesh.gameObject.SetActive(true);
		if (this.fallOnStart)
		{
			this.Fall_Rpc();
		}
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x0006A768 File Offset: 0x00068968
	private void Update()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (!this.isShaking && !this.isFalling)
		{
			if (Enumerable.ToList<Character>(Enumerable.Where<Character>(Enumerable.ToList<Character>(Enumerable.Where<Character>(PlayerHandler.GetAllPlayerCharacters(), (Character p) => p.data.isClimbing)), (Character p) => p.data.climbHit.collider == this.meshCollider)).Count > 0)
			{
				this.photonView.RPC("ShakeRock_Rpc", 0, Array.Empty<object>());
			}
		}
		this.timeUntilShake -= Time.deltaTime;
		if (this.isShaking && this.IsLocalPlayerClimbing && this.timeUntilShake <= 0f)
		{
			GamefeelHandler.instance.AddPerlinShake(this.climbingScreenShake, 0.2f, 15f);
			Debug.Log("Clime shake");
			this.timeUntilShake = this.screenShakeTickTime;
		}
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x0006A854 File Offset: 0x00068A54
	private void FixedUpdate()
	{
		if (this.rig == null)
		{
			return;
		}
		this.lastLinearVelocity = this.rig.linearVelocity;
		this.lastAngularVelocity = this.rig.angularVelocity;
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x0006A887 File Offset: 0x00068A87
	public void OnDestroy()
	{
		Object.DestroyImmediate(this.stuckiesRoot);
		Object.DestroyImmediate(this.shardsRoot);
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x0006A8A0 File Offset: 0x00068AA0
	private void OnCollisionEnter(Collision other)
	{
		if (!this.isFalling)
		{
			return;
		}
		if ((float)this.fracturedRoot.transform.childCount < (float)this.startPeicesCount * this.maxFracturePercent)
		{
			return;
		}
		bool flag = false;
		HashSet<Collider> hashSet = new HashSet<Collider>();
		foreach (ContactPoint contactPoint in other.contacts)
		{
			Collider[] array = Physics.OverlapSphere(contactPoint.point, this.contactExplosionRadius);
			LinqExtensions.AddRange<Collider>(hashSet, array);
		}
		foreach (Collider collider in hashSet)
		{
			if (collider.transform.parent != this.fracturedRoot)
			{
				if (this.shards.Contains(collider.gameObject))
				{
					this.rig.linearVelocity = this.lastLinearVelocity * this.collisionDamp;
					this.rig.angularVelocity = this.lastAngularVelocity;
				}
			}
			else
			{
				flag = true;
				if (this.shardsRoot == null)
				{
					this.shardsRoot = new GameObject("ShardsRoot");
					this.shardsRoot.transform.position = collider.transform.position;
				}
				collider.gameObject.AddComponent<Rigidbody>().mass = this.fracturedMass;
				collider.transform.parent = this.shardsRoot.transform;
				this.shards.Add(collider.gameObject);
			}
		}
		if (flag)
		{
			this.rig.linearVelocity = this.lastLinearVelocity * this.collisionDamp;
			this.rig.angularVelocity = this.lastAngularVelocity;
		}
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x0006AA68 File Offset: 0x00068C68
	private void OnDrawGizmosSelected()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.rig = base.GetComponent<Rigidbody>();
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.startShakeDistance);
		if (this.isFalling)
		{
			return;
		}
		foreach (Collider collider in this.GetStuckPieces())
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireMesh(collider.GetComponent<MeshCollider>().sharedMesh, collider.transform.position, collider.transform.rotation);
		}
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x0006AB30 File Offset: 0x00068D30
	[PunRPC]
	private void ShakeRock_Rpc()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		this.source.Play();
		this.source.volume = 0.7f;
		if (this.DistanceToLocalPlayer < this.startShakeDistance)
		{
			Debug.Log(string.Format("start shake {0}", this.startShakeAmount));
			GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount, 0.2f, 15f);
		}
		base.StartCoroutine(this.<ShakeRock_Rpc>g__RockShake|42_0());
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x0006ABB8 File Offset: 0x00068DB8
	[PunRPC]
	private void Fall_Rpc()
	{
		if (Character.localCharacter.data.isClimbing && Character.localCharacter.data.climbHit.collider == this.meshCollider)
		{
			Character.localCharacter.refs.climbing.StopClimbing();
		}
		this.popSound.Play(base.transform.position);
		if (this.DistanceToLocalPlayer < this.startShakeDistance)
		{
			Debug.Log(string.Format("fall shake {0}", this.startShakeAmount));
			GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount, 0.2f, 15f);
		}
		this.fracturedRoot.gameObject.SetActive(true);
		this.fullMesh.gameObject.SetActive(false);
		this.rig = base.gameObject.AddComponent<Rigidbody>();
		this.rig.mass = 1000f;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
		this.meshCollider.enabled = false;
		Object.DestroyImmediate(this.meshCollider);
		foreach (Collider collider in this.stuckies)
		{
			if (this.stuckiesRoot == null)
			{
				this.stuckiesRoot = new GameObject("StuckiesRoot");
				this.stuckiesRoot.transform.position = collider.transform.position;
			}
			collider.transform.parent = this.stuckiesRoot.transform;
			collider.enabled = true;
		}
		this.startPeicesCount = this.fracturedRoot.transform.childCount;
		Debug.Log("Falling");
		this.isFalling = true;
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x0006AD94 File Offset: 0x00068F94
	private List<Collider> GetStuckPieces()
	{
		List<MeshCollider> piecsColliders = Enumerable.ToList<MeshCollider>(this.fracturedRoot.GetComponentsInChildren<MeshCollider>());
		List<Collider> list = Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Enumerable.ToList<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation), (Collider c) => c != this.meshCollider)), (Collider c) => ExtGameObject.IsInLayer(c.gameObject, HelperFunctions.LayerType.TerrainMap.ToLayerMask()))), (Collider c) => !Enumerable.Contains<Collider>(piecsColliders, c)));
		HashSet<Collider> hashSet = new HashSet<Collider>();
		foreach (Collider collider in list)
		{
			foreach (MeshCollider meshCollider in piecsColliders)
			{
				Vector3 vector;
				float num;
				if (Physics.ComputePenetration(meshCollider, meshCollider.transform.position, meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, ref vector, ref num))
				{
					hashSet.Add(meshCollider);
				}
			}
		}
		HashSet<Collider> hashSet2 = new HashSet<Collider>();
		foreach (MeshCollider meshCollider2 in piecsColliders)
		{
			using (HashSet<Collider>.Enumerator enumerator3 = hashSet.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (enumerator3.Current.transform.position.y < meshCollider2.transform.position.y)
					{
						hashSet2.Add(meshCollider2);
					}
				}
			}
		}
		LinqExtensions.AddRange<Collider>(hashSet, hashSet2);
		return Enumerable.ToList<Collider>(hashSet);
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x0006B08B File Offset: 0x0006928B
	[CompilerGenerated]
	private IEnumerator <ShakeRock_Rpc>g__RockShake|42_0()
	{
		Debug.Log("Start shaking");
		float duration = 0f;
		while (duration < this.fallTime)
		{
			Vector3 vector = Vector2.zero;
			vector.x += Perlin.Noise(Time.time * this.shakeScale, 0f, 0f) - 0.5f;
			vector.y += Perlin.Noise(0f, Time.time * this.shakeScale, 0f) - 0.5f;
			vector.z += Perlin.Noise(0f, 0f, Time.time * this.shakeScale) - 0.5f;
			vector *= this.amount * Time.deltaTime;
			duration += Time.deltaTime;
			this.fullMesh.localPosition = vector;
			yield return null;
		}
		Debug.Log("Done shaking");
		this.isShaking = false;
		this.fullMesh.localPosition = ExtDataTypes.ToVec(0);
		this.source.volume = 0f;
		this.source.Stop();
		if (this.photonView.IsMine)
		{
			this.photonView.RPC("Fall_Rpc", 0, Array.Empty<object>());
		}
		yield break;
	}

	// Token: 0x04001360 RID: 4960
	public float fallChance = 0.5f;

	// Token: 0x04001361 RID: 4961
	public float contactExplosionRadius = 0.2f;

	// Token: 0x04001362 RID: 4962
	public float maxFracturePercent = 0.5f;

	// Token: 0x04001363 RID: 4963
	public float fracturedMass = 1f;

	// Token: 0x04001364 RID: 4964
	public float collisionDamp;

	// Token: 0x04001365 RID: 4965
	public float shakeScale = 30f;

	// Token: 0x04001366 RID: 4966
	public float fallTime = 5f;

	// Token: 0x04001367 RID: 4967
	public float amount = 1f;

	// Token: 0x04001368 RID: 4968
	public float startShakeDistance = 10f;

	// Token: 0x04001369 RID: 4969
	public float startShakeAmount = 400f;

	// Token: 0x0400136A RID: 4970
	public float climbingScreenShake = 240f;

	// Token: 0x0400136B RID: 4971
	public float screenShakeTickTime = 0.2f;

	// Token: 0x0400136C RID: 4972
	public bool isFalling;

	// Token: 0x0400136D RID: 4973
	public bool isShaking;

	// Token: 0x0400136E RID: 4974
	public bool fallOnStart;

	// Token: 0x0400136F RID: 4975
	public Transform fullMesh;

	// Token: 0x04001370 RID: 4976
	public Transform fracturedRoot;

	// Token: 0x04001371 RID: 4977
	public SFX_Instance popSound;

	// Token: 0x04001372 RID: 4978
	private readonly List<GameObject> shards = new List<GameObject>();

	// Token: 0x04001373 RID: 4979
	private Vector3 lastAngularVelocity;

	// Token: 0x04001374 RID: 4980
	private Vector3 lastLinearVelocity;

	// Token: 0x04001375 RID: 4981
	private MeshCollider meshCollider;

	// Token: 0x04001376 RID: 4982
	private PhotonView photonView;

	// Token: 0x04001377 RID: 4983
	private Rigidbody rig;

	// Token: 0x04001378 RID: 4984
	private GameObject shardsRoot;

	// Token: 0x04001379 RID: 4985
	private AudioSource source;

	// Token: 0x0400137A RID: 4986
	private int startPeicesCount;

	// Token: 0x0400137B RID: 4987
	private List<Collider> stuckies = new List<Collider>();

	// Token: 0x0400137C RID: 4988
	private GameObject stuckiesRoot;

	// Token: 0x0400137D RID: 4989
	private float timeUntilShake;

	// Token: 0x0400137E RID: 4990
	public bool drawGizmos;
}
