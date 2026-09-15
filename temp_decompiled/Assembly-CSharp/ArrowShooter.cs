using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000059 RID: 89
public class ArrowShooter : MonoBehaviourPunCallbacks
{
	// Token: 0x06000466 RID: 1126 RVA: 0x0001ADD1 File Offset: 0x00018FD1
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0001ADDF File Offset: 0x00018FDF
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.view.IsMine)
		{
			this.view.RPC("WarningArrows_RPC", 3, new object[] { Random.Range(1, 5) });
		}
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x0001AE1A File Offset: 0x0001901A
	private void Start()
	{
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0001AE1C File Offset: 0x0001901C
	private void Update()
	{
		if (this.empty)
		{
			return;
		}
		if (!this.reloading)
		{
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.castRadius, this.shooter.forward, ref raycastHit, this.range))
			{
				bool flag = false;
				if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
				{
					this.targetCharacter = raycastHit.collider.gameObject.GetComponentInParent<Character>();
					this.target = raycastHit.collider.transform;
					this.hitTarget = raycastHit.point;
					flag = true;
				}
				if (!flag)
				{
					this.target = raycastHit.collider.transform;
					this.hitTarget = raycastHit.point;
				}
			}
			if (this.target != null)
			{
				this.moveAcumulator += Vector3.Distance(this.target.position, this.targetLastPosition);
				if (this.moveAcumulator > 0f)
				{
					this.moveAcumulator -= this.movementCooldown * Time.deltaTime;
				}
				this.targetLastPosition = this.target.position;
			}
			else
			{
				this.moveAcumulator = 0f;
			}
			if (this.moveAcumulator > this.movementThreshold)
			{
				this.view.RPC("FireArrow_RPC", 3, Array.Empty<object>());
			}
		}
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0001AF7C File Offset: 0x0001917C
	public void testFire()
	{
		if (this.view.IsMine)
		{
			this.view.RPC("FireArrow_RPC", 3, new object[] { base.transform.position + base.transform.forward });
		}
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x0001AFD0 File Offset: 0x000191D0
	[PunRPC]
	public void FireArrow_RPC()
	{
		this.firedParticles.Play();
		Vector3 vector = this.hitTarget - base.transform.position;
		Vector3 vector2 = base.transform.position + vector * 0.5f;
		ParticleSystem particleSystem = Object.Instantiate<ParticleSystem>(this.trailParticles, vector2, Quaternion.identity);
		particleSystem.shape.radius = Vector3.Distance(this.hitTarget, base.transform.position) / 2f;
		particleSystem.transform.rotation = Quaternion.LookRotation(vector, base.transform.up);
		if (this.targetCharacter != null)
		{
			this.targetCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, (float)this.damagePips * 0.025f, false, true, true);
		}
		Arrow arrow = Object.Instantiate<Arrow>(this.arrowPrefab, this.hitTarget, Quaternion.identity);
		arrow.transform.rotation = quaternion.LookRotation(vector, Vector3.up);
		arrow.transform.parent = this.target;
		arrow.stuckArrow(true);
		Rigidbody rigidbody;
		if (this.target.gameObject.TryGetComponent<Rigidbody>(ref rigidbody))
		{
			rigidbody.AddForce(vector.normalized * this.force, 1);
		}
		this.arrows.Add(arrow);
		this.checkMaxArrows();
		base.StartCoroutine(this.<FireArrow_RPC>g__Reload|28_0());
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0001B148 File Offset: 0x00019348
	[PunRPC]
	public void WarningArrows_RPC(int count)
	{
		for (int i = 0; i < count; i++)
		{
			Vector3 vector = base.transform.up * Random.Range(-1f, 1f) + base.transform.right * Random.Range(-1f, 1f);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + vector, base.transform.forward, ref raycastHit, this.range))
			{
				MonoBehaviour.print(raycastHit.collider.gameObject.name);
				Arrow arrow = Object.Instantiate<Arrow>(this.arrowPrefab, raycastHit.point, Quaternion.identity);
				arrow.stuckArrow(true);
				arrow.transform.rotation = quaternion.LookRotation(raycastHit.point - base.transform.position, Vector3.up);
				arrow.transform.Rotate(new Vector3((float)Random.Range(-10, 10), (float)Random.Range(-10, 10), (float)Random.Range(-10, 10)));
				arrow.transform.parent = raycastHit.transform;
			}
		}
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0001B287 File Offset: 0x00019487
	public void checkMaxArrows()
	{
		if (this.arrows.Count >= this.maxArrows)
		{
			this.emptyParticles.Play();
			this.empty = true;
		}
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0001B2B0 File Offset: 0x000194B0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position + this.shooter.forward * this.range, this.castRadius);
		Gizmos.DrawLine(base.transform.position, base.transform.position + this.shooter.forward * this.range);
		Gizmos.DrawRay(base.transform.position, this.hitTarget - base.transform.position);
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0001B36F File Offset: 0x0001956F
	[CompilerGenerated]
	private IEnumerator <FireArrow_RPC>g__Reload|28_0()
	{
		this.target = null;
		this.moveAcumulator = 0f;
		this.targetCharacter = null;
		this.reloading = true;
		yield return new WaitForSeconds(this.reloadTime);
		this.reloading = false;
		yield break;
	}

	// Token: 0x040004EC RID: 1260
	[FormerlySerializedAs("damage")]
	public int damagePips;

	// Token: 0x040004ED RID: 1261
	public float force;

	// Token: 0x040004EE RID: 1262
	public float range;

	// Token: 0x040004EF RID: 1263
	public float castRadius;

	// Token: 0x040004F0 RID: 1264
	public float movementThreshold;

	// Token: 0x040004F1 RID: 1265
	public float movementCooldown;

	// Token: 0x040004F2 RID: 1266
	public Arrow arrowPrefab;

	// Token: 0x040004F3 RID: 1267
	public List<Arrow> arrows = new List<Arrow>();

	// Token: 0x040004F4 RID: 1268
	public int maxArrows = 100;

	// Token: 0x040004F5 RID: 1269
	private PhotonView view;

	// Token: 0x040004F6 RID: 1270
	public float reloadTime;

	// Token: 0x040004F7 RID: 1271
	private bool reloading;

	// Token: 0x040004F8 RID: 1272
	public Transform shooter;

	// Token: 0x040004F9 RID: 1273
	public Transform target;

	// Token: 0x040004FA RID: 1274
	private Vector3 hitTarget;

	// Token: 0x040004FB RID: 1275
	private Vector3 targetLastPosition;

	// Token: 0x040004FC RID: 1276
	private float moveAcumulator;

	// Token: 0x040004FD RID: 1277
	public Character targetCharacter;

	// Token: 0x040004FE RID: 1278
	public ParticleSystem trailParticles;

	// Token: 0x040004FF RID: 1279
	public ParticleSystem firedParticles;

	// Token: 0x04000500 RID: 1280
	public ParticleSystem emptyParticles;

	// Token: 0x04000501 RID: 1281
	public bool empty;

	// Token: 0x04000502 RID: 1282
	private bool initialized;
}
