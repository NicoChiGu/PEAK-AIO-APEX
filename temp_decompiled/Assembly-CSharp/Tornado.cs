using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000354 RID: 852
public class Tornado : MonoBehaviour
{
	// Token: 0x060015D2 RID: 5586 RVA: 0x00070768 File Offset: 0x0006E968
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		base.transform.localScale = Vector3.zero;
		this.lifeTime = Random.Range(this.tornadoLifetimeMin, this.tornadoLifetimeMax);
		this.tornadoPos = base.transform.position;
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x000707BC File Offset: 0x0006E9BC
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.transform.position, this.range);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.captureDistance);
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x0007080C File Offset: 0x0006EA0C
	private void Update()
	{
		if (this.view.IsMine)
		{
			this.syncCounter += Time.deltaTime;
			if (this.syncCounter > 0.5f)
			{
				this.syncCounter = 0f;
				this.view.RPC("RPCA_SyncTornado", 0, new object[] { this.vel });
			}
		}
		if (!this.dying)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 0.25f);
			this.lifeTime -= Time.deltaTime;
			if (this.lifeTime < 0f && this.view.IsMine)
			{
				this.view.RPC("RPCA_TornadoDie", 0, Array.Empty<object>());
			}
		}
		else
		{
			base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, Vector3.zero, Time.deltaTime * 0.2f);
			this.tornadoSFX.SetBool("Die", true);
			if (base.transform.localScale.x < 0.01f && this.view.IsMine)
			{
				PhotonNetwork.Destroy(base.gameObject);
			}
		}
		if (this.view.IsMine)
		{
			this.TargetSelection();
		}
		this.Movement();
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x00070976 File Offset: 0x0006EB76
	[PunRPC]
	private void RPCA_SyncTornado(Vector3 syncVel)
	{
		this.vel = syncVel;
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x0007097F File Offset: 0x0006EB7F
	[PunRPC]
	private void RPCA_TornadoDie()
	{
		this.dying = true;
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x00070988 File Offset: 0x0006EB88
	private void TargetSelection()
	{
		this.selectNewTargetInSeconds -= Time.deltaTime;
		if (!this.target || this.selectNewTargetInSeconds < 0f || (this.target && HelperFunctions.FlatDistance(base.transform.position, this.target.position) < 10f))
		{
			this.selectNewTargetInSeconds = Random.Range(5f, 30f);
			this.PickTarget();
		}
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x00070A0B File Offset: 0x0006EC0B
	private void PickTarget()
	{
		this.view.RPC("RPCA_SelectTargetPos", 0, new object[] { Random.Range(0, this.targetParent.childCount) });
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x00070A3D File Offset: 0x0006EC3D
	[PunRPC]
	public void RPCA_SelectTargetPos(int targetID)
	{
		this.target = this.targetParent.GetChild(targetID);
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x00070A54 File Offset: 0x0006EC54
	private void Movement()
	{
		if (this.target == null)
		{
			return;
		}
		this.vel = FRILerp.Lerp(this.vel, (this.target.position - this.tornadoPos).Flat().normalized * 15f, 0.15f, true);
		this.tornadoPos += this.vel * Time.deltaTime;
		RaycastHit groundPosRaycast = HelperFunctions.GetGroundPosRaycast(this.tornadoPos + Vector3.up * 200f, HelperFunctions.LayerType.Terrain, 0f);
		if (groundPosRaycast.transform && Vector3.Distance(this.tornadoPos, groundPosRaycast.point) < 100f)
		{
			base.transform.position = groundPosRaycast.point;
			return;
		}
		base.transform.position = this.tornadoPos;
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x00070B48 File Offset: 0x0006ED48
	private void FixedUpdate()
	{
		if (base.transform.localScale.x < 0.1f)
		{
			if (this.caughtCharacters.Count > 0)
			{
				this.caughtCharacters.Clear();
			}
			if (this.ignoredCharacters.Count > 0)
			{
				this.ignoredCharacters.Clear();
			}
			return;
		}
		this.AttractCharacters();
		this.CapturedCharacter();
		this.Feedback();
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x00070BB4 File Offset: 0x0006EDB4
	private void CapturedCharacter()
	{
		if (this.caughtCharacters.Count == 0)
		{
			return;
		}
		foreach (Character character in new List<Character>(this.caughtCharacters))
		{
			if (!this.ignoredCharacters.Contains(character) && !(character == null))
			{
				float num = 15f;
				Vector3 vector = (character.Center - base.transform.position).Flat().normalized * num;
				Vector3 vector2 = base.transform.position + vector;
				float y = HelperFunctions.GetGroundPos(vector2, HelperFunctions.LayerType.Terrain, 0f).y;
				if (y > vector2.y)
				{
					vector2.y = y;
				}
				Vector3 vector3 = (vector2 - character.Center).Flat();
				Vector3 normalized = Vector3.Cross(Vector3.up, vector).normalized;
				character.AddForce(normalized * this.force, 1f, 1f);
				character.AddForce(vector3 * this.force * 0.2f, 1f, 1f);
				character.AddForce(Vector3.up * (19f + Mathf.Abs(this.Height(character) * 1f)), 1f, 1f);
				character.ClampSinceGrounded(0.5f);
				if (character.IsLocal)
				{
					character.GetBodypartRig(BodypartType.Torso).AddTorque(Vector3.up * 200f, 5);
					character.GetBodypartRig(BodypartType.Hip).AddTorque(Vector3.up * 200f, 5);
					character.GetBodypartRig(BodypartType.Torso).AddTorque(vector.normalized * 100f, 5);
					character.GetBodypartRig(BodypartType.Hip).AddTorque(vector.normalized * 100f, 5);
				}
				else
				{
					character.GetBodypartRig(BodypartType.Torso).AddTorque(Vector3.up * 500f, 5);
					character.GetBodypartRig(BodypartType.Hip).AddTorque(Vector3.up * 500f, 5);
					character.GetBodypartRig(BodypartType.Torso).AddTorque(vector.normalized * 500f, 5);
					character.GetBodypartRig(BodypartType.Hip).AddTorque(vector.normalized * 500f, 5);
				}
				character.refs.movement.ApplyExtraDrag(0.95f, true);
				character.RPCA_Fall(0.5f);
				if (character.IsLocal && this.LetTargetGo(character, vector2))
				{
					this.view.RPC("RPCA_ThrowPlayer", 0, new object[] { character.refs.view.ViewID });
				}
			}
		}
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x00070EB0 File Offset: 0x0006F0B0
	private bool LetTargetGo(Character target, Vector3 orbitSpot)
	{
		return (this.Height(target) > 50f && target.data.avarageVelocity.x > 0f != target.Center.x > 0f) || Vector3.Distance(orbitSpot, target.Center) > 30f || target.IsStuck();
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x00070F18 File Offset: 0x0006F118
	private float Height(Character target)
	{
		return target.Center.y - base.transform.position.y;
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x00070F38 File Offset: 0x0006F138
	[PunRPC]
	private void RPCA_ThrowPlayer(int targetView)
	{
		Tornado.<>c__DisplayClass32_0 CS$<>8__locals1 = new Tornado.<>c__DisplayClass32_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.target = PhotonView.Find(targetView).GetComponent<Character>();
		if (this.caughtCharacters.Contains(CS$<>8__locals1.target))
		{
			this.caughtCharacters.Remove(CS$<>8__locals1.target);
		}
		base.StartCoroutine(CS$<>8__locals1.<RPCA_ThrowPlayer>g__IIgnoreChar|0());
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x00070F98 File Offset: 0x0006F198
	private void Feedback()
	{
		this.counter += Time.deltaTime;
		if (this.counter > 0.2f)
		{
			GamefeelHandler.instance.AddPerlinShakeProximity(base.transform.position, 3f, 0.4f, 15f, this.range * 1f);
			this.counter = 0f;
		}
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x00071000 File Offset: 0x0006F200
	private void AttractCharacters()
	{
		float num = base.transform.localScale.x * this.range;
		float num2 = base.transform.localScale.x * this.captureDistance;
		foreach (Character character in Character.AllCharacters)
		{
			if (!this.ignoredCharacters.Contains(character) && !this.caughtCharacters.Contains(character))
			{
				float num3 = HelperFunctions.FlatDistance(base.transform.position, character.Center);
				if (num3 <= num && this.Height(character) >= -10f && this.Height(character) <= 50f)
				{
					float num4 = Mathf.Clamp01(1f - num3 / num);
					float num5 = this.inStrC.Evaluate(num4);
					float num6 = this.upStrC.Evaluate(num4);
					Vector3 normalized = (base.transform.position - character.Center).Flat().normalized;
					float num7 = 1f;
					if (character.data.isCrouching)
					{
						num7 = 0.25f;
					}
					character.AddForce(normalized * this.force * num5 * 1.2f * num7 + Vector3.up * this.force * num6 * num7, 0.8f, 1f);
					if (num3 < num2 && character.IsLocal && !character.IsStuck())
					{
						this.view.RPC("RPCA_CaptureCharacter", 0, new object[] { character.refs.view.ViewID });
					}
				}
			}
		}
	}

	// Token: 0x060015E2 RID: 5602 RVA: 0x00071200 File Offset: 0x0006F400
	[PunRPC]
	private void RPCA_CaptureCharacter(int targetViewID)
	{
		Character component = PhotonView.Find(targetViewID).GetComponent<Character>();
		this.caughtCharacters.Add(component);
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x00071225 File Offset: 0x0006F425
	[PunRPC]
	internal void RPCA_InitTornado(int targetViewID)
	{
		this.view = base.GetComponent<PhotonView>();
		this.targetParent = PhotonView.Find(targetViewID).transform.Find("TornadoPoints");
	}

	// Token: 0x040014AA RID: 5290
	public Animator tornadoSFX;

	// Token: 0x040014AB RID: 5291
	public float force;

	// Token: 0x040014AC RID: 5292
	public float range = 25f;

	// Token: 0x040014AD RID: 5293
	public float captureDistance = 10f;

	// Token: 0x040014AE RID: 5294
	public AnimationCurve inStrC;

	// Token: 0x040014AF RID: 5295
	public AnimationCurve upStrC;

	// Token: 0x040014B0 RID: 5296
	private PhotonView view;

	// Token: 0x040014B1 RID: 5297
	private float lifeTime;

	// Token: 0x040014B2 RID: 5298
	private bool dying;

	// Token: 0x040014B3 RID: 5299
	private Vector3 tornadoPos;

	// Token: 0x040014B4 RID: 5300
	public float tornadoLifetimeMin = 30f;

	// Token: 0x040014B5 RID: 5301
	public float tornadoLifetimeMax = 120f;

	// Token: 0x040014B6 RID: 5302
	private Transform targetParent;

	// Token: 0x040014B7 RID: 5303
	public Transform target;

	// Token: 0x040014B8 RID: 5304
	private float syncCounter;

	// Token: 0x040014B9 RID: 5305
	private float selectNewTargetInSeconds;

	// Token: 0x040014BA RID: 5306
	private Vector3 vel;

	// Token: 0x040014BB RID: 5307
	private List<Character> ignoredCharacters = new List<Character>();

	// Token: 0x040014BC RID: 5308
	private List<Character> caughtCharacters = new List<Character>();

	// Token: 0x040014BD RID: 5309
	private float counter;
}
