using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200033B RID: 827
public class Spider : MonoBehaviour
{
	// Token: 0x06001545 RID: 5445 RVA: 0x0006C799 File Offset: 0x0006A999
	public void Awake()
	{
		this.line.positionCount = 40;
		this.startingSpiderLocalPos = this.spider.transform.localPosition;
		this.StopDisplayRope();
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x0006C7C4 File Offset: 0x0006A9C4
	public void Start()
	{
		SpiderManager.instance.Register(this);
		float y = this.startPoint.eulerAngles.y;
		this.startPoint.rotation = Quaternion.identity;
		ExtTransform.SetYEuler(this.startPoint, y);
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x0006C809 File Offset: 0x0006AA09
	public void OnDestroy()
	{
		if (SpiderManager.instance != null)
		{
			SpiderManager.instance.Unregister(this);
		}
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x0006C824 File Offset: 0x0006AA24
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		if (this.lastScannedTime + 0.01f > Time.time)
		{
			Gizmos.DrawSphere(base.transform.position - Vector3.up * this.minRaycastDistance, this.castRadius);
			Gizmos.DrawSphere(base.transform.position - Vector3.up * this.castDistance, this.castRadius);
			return;
		}
		Gizmos.DrawWireSphere(base.transform.position - Vector3.up * this.minRaycastDistance, this.castRadius);
		Gizmos.DrawWireSphere(base.transform.position - Vector3.up * this.castDistance, this.castRadius);
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x0006C8FC File Offset: 0x0006AAFC
	public void LateUpdate()
	{
		if (this.spiderState == Spider.SpiderState.Grabbing && this.targetPlayer && !this.targetPlayer.data.dead)
		{
			this.spider.transform.position = this.targetPlayer.Head + Vector3.up;
			Vector3 normalized = (base.transform.position - this.spider.transform.position).normalized;
			Vector3 vector = this.targetPlayer.refs.head.transform.forward;
			vector = Vector3.ProjectOnPlane(vector, normalized).normalized;
			this.spider.transform.rotation = Quaternion.Lerp(this.spider.transform.rotation, Quaternion.LookRotation(vector, normalized), Time.deltaTime * 5f);
		}
		if (this._stunnedTime > 0f)
		{
			this._stunnedTime -= Time.deltaTime;
		}
		this.ropeRender.DisplayRope(this.startPoint.position, this.spiderButt.transform.position, Mathf.Clamp01(this.sinceGrab), this.line);
		this.TestAchievement();
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x0006CA48 File Offset: 0x0006AC48
	private void UpdateAttack(bool isLocal)
	{
		this.sincePoison += Time.fixedDeltaTime;
		if (this.sincePoison >= this.poisonFrequency - 0.25f && !this.attacked)
		{
			this.anim.SetTrigger("Attack");
			this.attacked = true;
		}
		if (this.sincePoison > this.poisonFrequency)
		{
			this.sincePoison = 0f;
			this.attacked = false;
			if (isLocal)
			{
				this.targetPlayer.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, this.poisonDamage, false, true, true);
			}
		}
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x0006CAE0 File Offset: 0x0006ACE0
	private void FixedUpdate()
	{
		if (this.spiderState == Spider.SpiderState.Grabbing)
		{
			this.sinceGrab += Time.fixedDeltaTime;
			this.anim.SetBool("Weaving", this.sinceGrab < this.pullWaitTime);
			if (this.targetPlayer)
			{
				if (this.targetPlayer.data.isCarried)
				{
					this.photonView.RPC("RPCA_LetGo", 0, Array.Empty<object>());
					return;
				}
				this.targetPos = this.targetRig.position;
				if (this.targetPlayer.IsLocal && this.sinceGrab > this.pullWaitTime && !this.targetPlayer.data.dead)
				{
					this.UpdateAttack(true);
				}
				if (!this.targetPlayer.IsLocal)
				{
					this.UpdateAttack(false);
				}
				if (this.targetPlayer.data.dead)
				{
					this.photonView.RPC("RPCA_LetGo", 0, Array.Empty<object>());
					return;
				}
			}
			if (this.targetPlayer && this.targetPlayer.photonView.IsMine)
			{
				if (this.sinceGrab > this.pullWaitTime)
				{
					Vector3 normalized = (this.dragToPoint.position - (this.targetPlayer.Head + Vector3.up)).normalized;
					float num = this.distancePullStrengthCurve.Evaluate(Vector3.Distance(this.dragToPoint.position, this.targetPlayer.Head + Vector3.up));
					this.targetPlayer.refs.movement.ApplyExtraDrag(this.extraDrag, true);
					this.targetPlayer.AddForceToBodyPart(this.targetRig, normalized * (this.dragForce * 0.2f) * num, normalized * this.dragForce * num);
					this.targetPlayer.data.sinceGrounded = Mathf.Clamp(this.targetPlayer.data.sinceGrounded, 0f, 1f);
				}
				else
				{
					Vector3 normalized2 = (this.dragToPoint.position - (this.targetPlayer.Head + Vector3.up)).normalized;
					this.targetPlayer.refs.movement.ApplyExtraDrag(this.extraDrag, true);
					this.targetPlayer.AddForceToBodyPart(this.targetRig, normalized2 * (this.keepUprightForce * 0.2f), normalized2 * this.keepUprightForce);
					this.targetPlayer.data.sinceGrounded = Mathf.Clamp(this.targetPlayer.data.sinceGrounded, 0f, 1f);
				}
				if (this.targetPlayer.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Web) < 0.025f)
				{
					this.photonView.RPC("RPCA_LetGo", 0, Array.Empty<object>());
				}
			}
			this.currentDistance = Vector3.Distance(this.startPoint.position, this.targetPos);
			return;
		}
		this.anim.SetBool("Weaving", false);
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x0006CE0E File Offset: 0x0006B00E
	private void StopDisplayRope()
	{
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x0006CE10 File Offset: 0x0006B010
	public void Scan()
	{
		this.UpdateCulled();
		if (this.spiderState == Spider.SpiderState.Idle && !this.isCulled && this._stunnedTime <= 0f)
		{
			RaycastHit hit = this.GetHit();
			if (hit.transform)
			{
				if (!this.setShadow)
				{
					this.shadow.transform.position = new Vector3(this.shadow.transform.position.x, hit.point.y, this.shadow.transform.position.z);
					this.setShadow = true;
				}
				Character componentInParent = hit.transform.GetComponentInParent<Character>();
				if (componentInParent)
				{
					this.photonView.RPC("RPCA_DropSpider", 0, new object[]
					{
						componentInParent.photonView,
						Vector3.Distance(this.startPoint.position, hit.point)
					});
				}
			}
			this.lastScannedTime = Time.time;
		}
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x0006CF1C File Offset: 0x0006B11C
	private void UpdateCulled()
	{
		Vector2 vector;
		vector..ctor(this.spiderTriggerTransform.position.x, this.spiderTriggerTransform.position.z);
		bool flag = true;
		foreach (Character character in Character.AllCharacters)
		{
			if (!(character == null) && Vector2.Distance(new Vector2(character.Center.x, character.Center.z), vector) < this.cullDistanceXZ)
			{
				flag = false;
			}
		}
		this.SetCulled(flag);
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x0006CFCC File Offset: 0x0006B1CC
	private void SetCulled(bool culled)
	{
		this.isCulled = culled;
		base.gameObject.SetActive(!culled);
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x0006CFE4 File Offset: 0x0006B1E4
	private RaycastHit GetHit()
	{
		RaycastHit raycastHit = default(RaycastHit);
		if (Physics.SphereCast(this.startPoint.position, this.castRadius, -Vector3.up, ref raycastHit, this.castDistance, HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysicalExceptDefault)) && Vector3.Distance(this.startPoint.position, raycastHit.point) < this.minRaycastDistance)
		{
			raycastHit = default(RaycastHit);
		}
		return raycastHit;
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x0006D058 File Offset: 0x0006B258
	[PunRPC]
	public void RPCA_DropSpider(PhotonView characterView, float distance)
	{
		Spider.<>c__DisplayClass61_0 CS$<>8__locals1 = new Spider.<>c__DisplayClass61_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.characterView = characterView;
		CS$<>8__locals1.distance = distance;
		base.StartCoroutine(CS$<>8__locals1.<RPCA_DropSpider>g__DropRoutine|0());
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x0006D08D File Offset: 0x0006B28D
	public void GrabCharacter(PhotonView characterView)
	{
		if (this.spiderState == Spider.SpiderState.Dropped && this._stunnedTime <= 0f)
		{
			this.photonView.RPC("RPCA_GrabCharacter", 0, new object[] { characterView });
		}
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x0006D0C0 File Offset: 0x0006B2C0
	public void Bonk()
	{
		this.photonView.RPC("BonkRPC", 0, Array.Empty<object>());
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x0006D0D8 File Offset: 0x0006B2D8
	[PunRPC]
	private void BonkRPC()
	{
		this.anim.SetTrigger("Bonk");
		if (this.targetPlayer)
		{
			this.targetPlayer.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Web, 1f, true, false);
		}
		this.stunnedParticle.Stop();
		this.stunnedParticle.Play();
		this._stunnedTime = this.bonkStunTime;
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x0006D144 File Offset: 0x0006B344
	[PunRPC]
	public void RPCA_GrabCharacter(PhotonView characterView)
	{
		Character component = characterView.GetComponent<Character>();
		this.targetPlayer = component;
		this.targetRig = component.GetBodypart(BodypartType.Torso).Rig;
		this.sinceGrab = 0f;
		if (this.targetPlayer.photonView.IsMine || this.photonView.IsMine)
		{
			GamefeelHandler.instance.perlin.AddShake(base.transform.position, 5f, 0.2f, 15f, 40f);
		}
		this.spiderState = Spider.SpiderState.Grabbing;
		component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Web, this.webDamage, true, true, true);
		this.targetPlayer.Fall(this.fallTime, 0f);
		if (this.targetPlayer.IsLocal)
		{
			this.caughtLocalPlayer = true;
		}
		this.targetPlayer.BreakCharacterCarrying(false);
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x0006D224 File Offset: 0x0006B424
	[PunRPC]
	private void RPCA_LetGo()
	{
		if (this.targetPlayer != null)
		{
			this.targetPlayer.data.sinceGrounded = 0f;
			this.targetPlayer.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Web, 1f, true, false);
		}
		this.targetRig = null;
		this.targetPlayer = null;
		this.spiderState = Spider.SpiderState.LetGo;
		TweenSettingsExtensions.OnUpdate<TweenerCore<Quaternion, Vector3, QuaternionOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<Quaternion, Vector3, QuaternionOptions>>(ShortcutExtensions.DORotate(this.spider.transform, new Vector3(0f, 0f, 0f), 3f, 0), 27), delegate
		{
		});
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x0006D2D8 File Offset: 0x0006B4D8
	public bool WillAttach(out RaycastHit hit)
	{
		hit = default(RaycastHit);
		return Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, ref hit, this.maxLength, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 0);
	}

	// Token: 0x06001558 RID: 5464 RVA: 0x0006D328 File Offset: 0x0006B528
	private void TestAchievement()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (this.caughtLocalPlayer)
		{
			if (Character.localCharacter.data.dead)
			{
				this.caughtLocalPlayer = false;
				return;
			}
			if (Character.localCharacter.data.isGrounded && Character.localCharacter.refs.afflictions.statusSum < 1f && Vector3.Distance(this.spiderTriggerTransform.position, Character.localCharacter.Center) > this.escapeRadiusForAchievement && Character.localCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Web) < 0.025f)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.WebSecurityBadge);
				this.caughtLocalPlayer = false;
			}
		}
	}

	// Token: 0x040013D4 RID: 5076
	public PhotonView photonView;

	// Token: 0x040013D5 RID: 5077
	public Transform startPoint;

	// Token: 0x040013D6 RID: 5078
	public Transform dragToPoint;

	// Token: 0x040013D7 RID: 5079
	public GameObject spider;

	// Token: 0x040013D8 RID: 5080
	public Transform spiderButt;

	// Token: 0x040013D9 RID: 5081
	public Animator anim;

	// Token: 0x040013DA RID: 5082
	public Transform shadow;

	// Token: 0x040013DB RID: 5083
	public Renderer spiderRenderer;

	// Token: 0x040013DC RID: 5084
	private bool setShadow;

	// Token: 0x040013DD RID: 5085
	public bool isCulled;

	// Token: 0x040013DE RID: 5086
	public float spiderMoveSpeed;

	// Token: 0x040013DF RID: 5087
	public float spiderWaitTime;

	// Token: 0x040013E0 RID: 5088
	public float maxLength = 20f;

	// Token: 0x040013E1 RID: 5089
	public float castRadius;

	// Token: 0x040013E2 RID: 5090
	public float castDistance = 100f;

	// Token: 0x040013E3 RID: 5091
	public float dragForce = 100f;

	// Token: 0x040013E4 RID: 5092
	public float keepUprightForce = 50f;

	// Token: 0x040013E5 RID: 5093
	private bool caughtLocalPlayer;

	// Token: 0x040013E6 RID: 5094
	public float escapeRadiusForAchievement;

	// Token: 0x040013E7 RID: 5095
	private float currentDistance;

	// Token: 0x040013E8 RID: 5096
	public Spider.SpiderState spiderState;

	// Token: 0x040013E9 RID: 5097
	public SFX_Instance[] webMovement;

	// Token: 0x040013EA RID: 5098
	public Transform spiderTriggerTransform;

	// Token: 0x040013EB RID: 5099
	private float sinceGrab;

	// Token: 0x040013EC RID: 5100
	private float sincePoison;

	// Token: 0x040013ED RID: 5101
	private Character targetPlayer;

	// Token: 0x040013EE RID: 5102
	private Vector3 targetPos;

	// Token: 0x040013EF RID: 5103
	private Rigidbody targetRig;

	// Token: 0x040013F0 RID: 5104
	public RopeRender ropeRender;

	// Token: 0x040013F1 RID: 5105
	public LineRenderer line;

	// Token: 0x040013F2 RID: 5106
	public ParticleSystem stunnedParticle;

	// Token: 0x040013F3 RID: 5107
	private Vector3 startingSpiderLocalPos;

	// Token: 0x040013F4 RID: 5108
	public float maxScoutHookTime = 2f;

	// Token: 0x040013F5 RID: 5109
	public float minRaycastDistance = 5f;

	// Token: 0x040013F6 RID: 5110
	public float bonkStunTime = 5f;

	// Token: 0x040013F7 RID: 5111
	public Transform firePoint;

	// Token: 0x040013F8 RID: 5112
	public float poisonDamage = 0.05f;

	// Token: 0x040013F9 RID: 5113
	public float poisonFrequency = 1f;

	// Token: 0x040013FA RID: 5114
	public float webDamage = 0.25f;

	// Token: 0x040013FB RID: 5115
	public float extraDrag = 0.5f;

	// Token: 0x040013FC RID: 5116
	public float pullWaitTime = 1f;

	// Token: 0x040013FD RID: 5117
	public float fallTime = 2f;

	// Token: 0x040013FE RID: 5118
	public float cullDistanceXZ = 30f;

	// Token: 0x040013FF RID: 5119
	private float _stunnedTime;

	// Token: 0x04001400 RID: 5120
	public AnimationCurve distancePullStrengthCurve;

	// Token: 0x04001401 RID: 5121
	public AnimationCurve letGoCurve;

	// Token: 0x04001402 RID: 5122
	private bool attacked;

	// Token: 0x04001403 RID: 5123
	private float lastScannedTime;

	// Token: 0x0200050E RID: 1294
	public enum SpiderState
	{
		// Token: 0x04001B6A RID: 7018
		Idle,
		// Token: 0x04001B6B RID: 7019
		Dropped,
		// Token: 0x04001B6C RID: 7020
		Grabbing,
		// Token: 0x04001B6D RID: 7021
		LetGo
	}
}
