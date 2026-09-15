using System;
using Peak.Afflictions;
using Peak.Math;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class Mob : MonoBehaviourPunCallbacks
{
	// Token: 0x170000AF RID: 175
	// (get) Token: 0x06000A32 RID: 2610 RVA: 0x0003602A File Offset: 0x0003422A
	// (set) Token: 0x06000A33 RID: 2611 RVA: 0x00036032 File Offset: 0x00034232
	public Character forcedCharacterTarget { get; private set; }

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000A34 RID: 2612 RVA: 0x0003603B File Offset: 0x0003423B
	private bool InTargetingCooldown
	{
		get
		{
			return Time.time < Mathf.Max(this._timeLastCheckedForTarget + this.targetCheckCooldown, this._timeLastSwitchedTarget + this.targetSwitchCooldown);
		}
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000A35 RID: 2613 RVA: 0x00036063 File Offset: 0x00034263
	private bool IsResting
	{
		get
		{
			return Time.time < this._timeLastAttacked + this.postAttackRest;
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000A36 RID: 2614 RVA: 0x00036079 File Offset: 0x00034279
	private bool IsInAttackCooldown
	{
		get
		{
			return Time.time < this._timeLastAttacked + this.attackCooldown;
		}
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0003608F File Offset: 0x0003428F
	public void SetForcedTarget(Character character)
	{
		this.forcedCharacterTarget = character;
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000A38 RID: 2616 RVA: 0x00036098 File Offset: 0x00034298
	// (set) Token: 0x06000A39 RID: 2617 RVA: 0x000360A0 File Offset: 0x000342A0
	internal Mob.MobState mobState
	{
		get
		{
			return this._mobState;
		}
		set
		{
			if (this._mobState != value)
			{
				this._mobState = value;
				if (value == Mob.MobState.Flipping)
				{
					this.lastStartedFlipping = Time.time;
				}
				if (base.photonView.IsMine)
				{
					base.photonView.RPC("RPC_SyncMobState", 1, new object[] { (int)value });
				}
			}
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x000360F9 File Offset: 0x000342F9
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("RPC_SyncMobState", newPlayer, new object[] { (int)this.mobState });
		}
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0003612D File Offset: 0x0003432D
	[PunRPC]
	protected void RPC_SyncMobState(int newMobState)
	{
		this.mobState = (Mob.MobState)newMobState;
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00036136 File Offset: 0x00034336
	private void Awake()
	{
		this.rig = base.GetComponent<Rigidbody>();
		this._mobItem = base.GetComponent<MobItem>();
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x00036150 File Offset: 0x00034350
	private void Start()
	{
		this._timeSpawned = Time.time;
		this._timeLastAttacked = this._timeSpawned + this.attackCooldown + this.postAttackRest;
		this.juicedViewForward = base.transform.forward;
		this.juicedViewUp = base.transform.up;
		this.normal = base.transform.up;
		this.ResetPatrolCounter();
		this.GetNewPatrolPos();
		this.startPos = base.transform.position;
		this.lastPos = base.transform.position;
		this.startUp = base.transform.up;
		MobManager.instance.Register(this);
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x000361FF File Offset: 0x000343FF
	public void OnDestroy()
	{
		if (MobManager.instance != null)
		{
			MobManager.instance.Unregister(this);
		}
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0003621C File Offset: 0x0003441C
	private void GetNewPatrolPos()
	{
		Vector3 onUnitSphere = Random.onUnitSphere;
		Vector3 vector = this.startPos + this.startUp * 1f;
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + onUnitSphere * 15f, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		if (raycastHit.transform)
		{
			this.patrolPos = raycastHit.point;
			return;
		}
		this.untilNextPatrolPos = 0f;
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0003628F File Offset: 0x0003448F
	private void ResetPatrolCounter()
	{
		this.untilNextPatrolPos = Random.Range(0.5f, 5f);
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000A41 RID: 2625 RVA: 0x000362A6 File Offset: 0x000344A6
	private bool dead
	{
		get
		{
			return this.mobState == Mob.MobState.Dead;
		}
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x000362B4 File Offset: 0x000344B4
	private void Update()
	{
		if (this.sleeping)
		{
			return;
		}
		this.UpdateAnimation();
		if (this.dead)
		{
			this.HandleDeath();
			return;
		}
		if (Time.time - this._timeSpawned < 0.2f)
		{
			return;
		}
		this.GetTargetPos();
		this.CalcVars();
		if (base.photonView.IsMine)
		{
			if (this.forceNoMovement)
			{
				this.Attacking();
				this.Targeting();
				return;
			}
			if (this.mobState == Mob.MobState.Walking)
			{
				this.Attacking();
				this.Targeting();
				this.Patrol();
			}
		}
		else if (this.forceNoMovement || this.mobState == Mob.MobState.Walking)
		{
			this.Attacking();
		}
		if (!this.forceNoMovement)
		{
			this.JiggleViewTowardsTransform();
		}
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x00036364 File Offset: 0x00034564
	public void UpdateAnimation()
	{
		this.lerpedWalkSpeed = Mathf.Lerp(this.lerpedWalkSpeed, Vector3.Distance(base.transform.position, this.lastPos) / Time.fixedDeltaTime, 5f * Time.deltaTime);
		this.anim.SetFloat(Mob.WALKSPEED, (this.mobState == Mob.MobState.Walking) ? this.lerpedWalkSpeed : 0f);
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x000363D0 File Offset: 0x000345D0
	public void UpdateSleeping()
	{
		this.visuals.gameObject.SetActive(!this.sleeping);
		if (this.anim.enabled && this.sleeping)
		{
			this.anim.enabled = false;
			return;
		}
		if (!this.anim.enabled && !this.sleeping)
		{
			this.anim.enabled = true;
		}
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0003643C File Offset: 0x0003463C
	private void FixedUpdate()
	{
		if (this.sleeping)
		{
			return;
		}
		if (this.mobState == Mob.MobState.Dead)
		{
			this.rig.constraints = 0;
			return;
		}
		if (Time.time - this._timeSpawned < 0.2f)
		{
			return;
		}
		this.DoGroundRaycast();
		this.lastPos = base.transform.position;
		if (this.forceNoMovement)
		{
			this.rig.constraints = 0;
			return;
		}
		if (!base.photonView.IsMine)
		{
			this.rig.constraints = 0;
			return;
		}
		if (this.mobState == Mob.MobState.RigidbodyControlled)
		{
			this.rig.constraints = 0;
			this.fallTick = 0f;
			this.TestStartFlippingMyself();
			return;
		}
		this.rig.angularVelocity = Vector3.zero;
		this.rig.linearVelocity = Vector3.zero;
		this.rig.constraints = 112;
		if (this.mobState == Mob.MobState.Walking)
		{
			this.Movement();
			this.GroundSnapping();
			this.SetRotationWalking();
			this.TestFalling();
		}
		if (this.mobState == Mob.MobState.Flipping)
		{
			this.SetRotationFlipping();
			if (this.hitGround && Time.time - this.lastStartedFlipping > 1f)
			{
				this.mobState = Mob.MobState.Walking;
			}
		}
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x00036568 File Offset: 0x00034768
	private void LateUpdate()
	{
		if (Time.time - this._timeSpawned < 0.2f)
		{
			return;
		}
		if (this.mobState == Mob.MobState.Walking)
		{
			this.VisualSnapping();
		}
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0003658D File Offset: 0x0003478D
	private void HandleDeath()
	{
		this.anim.SetBool("Cooked", true);
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x000365A0 File Offset: 0x000347A0
	private void TestFalling()
	{
		if (!this.hitGround)
		{
			this.fallTick += Time.fixedDeltaTime;
			if (this.fallTick > 0.5f)
			{
				this.lastFell = Time.time;
				this.mobState = Mob.MobState.RigidbodyControlled;
				return;
			}
		}
		else
		{
			this.fallTick = 0f;
		}
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000A49 RID: 2633 RVA: 0x000365F2 File Offset: 0x000347F2
	[SerializeField]
	private float currentAttackCooldown
	{
		get
		{
			return Time.time - this._timeLastAttacked;
		}
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x00036600 File Offset: 0x00034800
	private void Attacking()
	{
		if (this.targetChar)
		{
			if (!this.attacking)
			{
				if (this.distanceToTarget < this.attackStartDistance && !this.IsInAttackCooldown && base.photonView.IsMine)
				{
					base.photonView.RPC("RPC_StartAttack", 0, Array.Empty<object>());
				}
				this.inRangeForAttackTime = 0f;
			}
			if (this.attacking)
			{
				if (this.inRangeForAttackTime == 0f)
				{
					this.anim.SetTrigger("Attack");
				}
				this.inRangeForAttackTime += Time.deltaTime;
				if (this.inRangeForAttackTime > this.attackTime)
				{
					Debug.Log("angle " + Vector3.Angle((base.transform.forward + Vector3.up).normalized, (this.targetChar.Center - base.transform.position).normalized).ToString());
					if (this.distanceToTarget < this.attackDistance && Vector3.Angle((base.transform.forward + Vector3.up).normalized, (this.targetChar.Center - base.transform.position).normalized) < this.attackAngle)
					{
						this.InflictAttack(this.targetChar);
						this._timeLastAttacked = Time.time;
					}
					else
					{
						this._timeLastAttacked = Time.time - this.whiffRefund * Mathf.Max(this.attackCooldown, this.postAttackRest);
					}
					this.attacking = false;
					return;
				}
			}
		}
		else
		{
			this.inRangeForAttackTime = 0f;
		}
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x000367BB File Offset: 0x000349BB
	[PunRPC]
	protected void RPC_StartAttack()
	{
		this.attacking = true;
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000A4C RID: 2636 RVA: 0x000367C4 File Offset: 0x000349C4
	// (set) Token: 0x06000A4D RID: 2637 RVA: 0x000367CC File Offset: 0x000349CC
	private Character targetChar
	{
		get
		{
			return this._targetChar;
		}
		set
		{
			if (value == this._targetChar)
			{
				return;
			}
			if (value != null)
			{
				this._timeLastSwitchedTarget = Time.time;
			}
			this._targetChar = value;
			if (base.photonView.IsMine)
			{
				int num = ((value == null) ? (-1) : value.photonView.ViewID);
				base.photonView.RPC("RPC_SyncTargetCharacter", 1, new object[] { num });
			}
			this.GetTargetPos();
		}
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0003684C File Offset: 0x00034A4C
	[PunRPC]
	protected void RPC_SyncTargetCharacter(int viewID)
	{
		if (viewID == -1)
		{
			this.targetChar = null;
			return;
		}
		PhotonView photonView = PhotonView.Find(viewID);
		Character character;
		if (photonView && photonView.TryGetComponent<Character>(ref character))
		{
			this.targetChar = character;
		}
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x00036885 File Offset: 0x00034A85
	protected virtual void InflictAttack(Character character)
	{
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x00036888 File Offset: 0x00034A88
	private void Targeting()
	{
		if (this.InTargetingCooldown)
		{
			return;
		}
		if (this.attacking && this.targetChar != null)
		{
			return;
		}
		this._timeLastCheckedForTarget = Time.time;
		Character character = null;
		float num = this.aggroDistance;
		foreach (Character character2 in Character.AllCharacters)
		{
			if (character2.data.fullyConscious)
			{
				float num2 = Vector3.Distance(base.transform.position, character2.Center);
				Affliction affliction;
				if (num2 < num && (!(this.forcedCharacterTarget != null) || !(character2 != this.forcedCharacterTarget)) && !character2.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.PoisonOverTime, out affliction) && !HelperFunctions.LineCheck(this.Center(), character2.Center, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform)
				{
					num = num2;
					character = character2;
				}
			}
		}
		this.targetChar = character;
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0003699C File Offset: 0x00034B9C
	private void JiggleViewTowardsTransform()
	{
		this.viewForwardSpringVelocity = FRILerp.Lerp(this.viewForwardSpringVelocity, (base.transform.forward - this.juicedViewForward) * this.spring, this.drag, true);
		this.viewUpSpringVelocity = FRILerp.Lerp(this.viewUpSpringVelocity, (base.transform.up - this.juicedViewUp) * this.spring, this.drag, true);
		this.juicedViewForward += this.viewForwardSpringVelocity * Time.deltaTime;
		this.juicedViewUp += this.viewUpSpringVelocity * Time.deltaTime;
		if (this.juicedViewForward.NearZero())
		{
			this.juicedViewForward = base.transform.forward;
		}
		if (this.juicedViewUp.NearZero())
		{
			this.juicedViewUp = base.transform.up;
		}
		Quaternion quaternion = Quaternion.LookRotation(this.juicedViewForward.normalized, this.juicedViewUp.normalized);
		this.mesh.rotation = quaternion;
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x00036AC4 File Offset: 0x00034CC4
	private void Patrol()
	{
		this.untilNextPatrolPos -= Time.deltaTime * Mathf.Lerp(30f, 1f, this.overShootFactor);
		if (this.untilNextPatrolPos <= 0f)
		{
			this.ResetPatrolCounter();
			this.GetNewPatrolPos();
		}
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x00036B14 File Offset: 0x00034D14
	private void CalcVars()
	{
		this.distanceToTarget = Vector3.Distance(base.transform.position, this.targetPos);
		if (this.distanceToTarget < this.closestDistance)
		{
			this.closestDistance = this.distanceToTarget;
		}
		float num = Mathf.Clamp01(this.distanceToTarget - this.closestDistance);
		this.overShootFactor = Mathf.InverseLerp(0.02f, 0f, num);
		this.closeToTargetFactor = Mathf.InverseLerp(1.2f, 1.5f, this.distanceToTarget);
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x00036B9C File Offset: 0x00034D9C
	private void SetRotationWalking()
	{
		if (this.IsResting)
		{
			return;
		}
		Vector3 vector = this.targetPos - base.transform.position;
		vector = Vector3.ProjectOnPlane(vector, this.normal);
		Quaternion quaternion = ((vector.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(vector.normalized, this.normal) : base.transform.rotation);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.turnRate * Time.deltaTime * this.overShootFactor);
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x00036C34 File Offset: 0x00034E34
	private void SetRotationFlipping()
	{
		Vector3 up = Vector3.up;
		if (this.groundCheckHitDown.collider)
		{
			up = this.groundCheckHitDown.normal;
		}
		base.transform.up = Vector3.Slerp(base.transform.up, up, 8f * Time.fixedDeltaTime);
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x00036C8C File Offset: 0x00034E8C
	private void TestStartFlippingMyself()
	{
		if (Time.time - this.lastFell > 0.5f && this.rig.linearVelocity.magnitude < this.maxVelocityToStartFlipping)
		{
			this.startFlippingTick += Time.fixedDeltaTime;
			if (this.startFlippingTick > this.minTimeToStartFlipping)
			{
				this.mobState = Mob.MobState.Flipping;
				return;
			}
		}
		else
		{
			this.startFlippingTick = 0f;
		}
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x00036CFC File Offset: 0x00034EFC
	private Vector3 GetTargetPos()
	{
		Vector3 center = this.patrolPos;
		if (this.targetChar)
		{
			center = this.targetChar.Center;
		}
		if (!this.targetPos.Same(center, 0.1f))
		{
			this.targetPos = center;
			this.closestDistance = float.PositiveInfinity;
		}
		return this.targetPos;
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00036D54 File Offset: 0x00034F54
	private bool DoGroundRaycast()
	{
		this.hitGround = false;
		this.groundCheckHitWalking = default(RaycastHit);
		this.groundCheckHitDown = default(RaycastHit);
		this.groundCheckHitWalking = HelperFunctions.LineCheck(this.lastPos, base.transform.position, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		this.groundCheckHitDown = HelperFunctions.LineCheck(base.transform.position, base.transform.position + Vector3.down * 2f, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		if (!this.groundCheckHitWalking.transform)
		{
			this.groundCheckHitWalking = HelperFunctions.LineCheck(this.Center(), this.Under(), HelperFunctions.LayerType.TerrainMap, 0f, 1);
		}
		if (this.groundCheckHitWalking.transform)
		{
			Vector3 vector = this.groundCheckHitWalking.normal;
			if (Vector3.Angle(vector, Vector3.up) < this.maxStandingAngle)
			{
				this.normal = vector;
				this.hitGround = true;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x00036E51 File Offset: 0x00035051
	private void GroundSnapping()
	{
		if (this.groundCheckHitWalking.transform)
		{
			this.rig.MovePosition(this.groundCheckHitWalking.point);
		}
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x00036E7C File Offset: 0x0003507C
	private void VisualSnapping()
	{
		if (this.groundCheckHitWalking.transform)
		{
			this.visuals.transform.position = this.groundCheckHitWalking.point;
			return;
		}
		this.visuals.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x00036ECC File Offset: 0x000350CC
	private Vector3 Under()
	{
		return base.transform.position - this.normal * 1f;
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x00036EEE File Offset: 0x000350EE
	private Vector3 Center()
	{
		return base.transform.position + this.normal * 0.2f;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x00036F10 File Offset: 0x00035110
	private void Movement()
	{
		if (this.IsResting)
		{
			return;
		}
		base.transform.position += base.transform.forward * this.movementSpeed * Time.deltaTime * this.overShootFactor * this.closeToTargetFactor;
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x00036F74 File Offset: 0x00035174
	public bool IsNearCharacter()
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (character != null && Vector3.Distance(this.Center(), character.Center) < this.sleepDistance)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x00036FE8 File Offset: 0x000351E8
	public void TestSleepMode()
	{
		if (this.sleeping)
		{
			if (this.IsNearCharacter())
			{
				this.sleeping = false;
				this.UpdateSleeping();
				return;
			}
		}
		else if (this.mobState == Mob.MobState.Walking && (!this._mobItem || this._mobItem.itemState == ItemState.Ground) && this.rig.linearVelocity.magnitude < 1f && !this.IsNearCharacter())
		{
			this.sleeping = true;
			this.UpdateSleeping();
		}
	}

	// Token: 0x04000995 RID: 2453
	private MobItem _mobItem;

	// Token: 0x04000996 RID: 2454
	public float movementSpeed = 5f;

	// Token: 0x04000997 RID: 2455
	public float turnRate = 300f;

	// Token: 0x04000998 RID: 2456
	public float aggroDistance = 5f;

	// Token: 0x04000999 RID: 2457
	[SerializeField]
	[Range(0f, 10f)]
	private float targetSwitchCooldown = 5f;

	// Token: 0x0400099A RID: 2458
	[SerializeField]
	[Range(0f, 5f)]
	private float targetCheckCooldown = 2f;

	// Token: 0x0400099B RID: 2459
	public float closeToTargetFactor = 1f;

	// Token: 0x0400099C RID: 2460
	public float overShootFactor = 1f;

	// Token: 0x0400099D RID: 2461
	[SerializeField]
	public float distanceToTarget;

	// Token: 0x0400099E RID: 2462
	public float closestDistance;

	// Token: 0x0400099F RID: 2463
	public float attackStartDistance = 2.5f;

	// Token: 0x040009A0 RID: 2464
	public float attackDistance = 1.5f;

	// Token: 0x040009A1 RID: 2465
	[SerializeField]
	private float attackCooldown = 2f;

	// Token: 0x040009A2 RID: 2466
	[Range(0f, 10f)]
	[SerializeField]
	private float postAttackRest;

	// Token: 0x040009A3 RID: 2467
	[Range(0f, 1f)]
	[SerializeField]
	private float whiffRefund = 0.5f;

	// Token: 0x040009A4 RID: 2468
	public float maxVelocityToStartFlipping = 5f;

	// Token: 0x040009A5 RID: 2469
	public float minTimeToStartFlipping = 1f;

	// Token: 0x040009A6 RID: 2470
	public float maxStandingAngle = 70f;

	// Token: 0x040009A7 RID: 2471
	private Vector3 juicedViewForward;

	// Token: 0x040009A8 RID: 2472
	private Vector3 viewForwardSpringVelocity;

	// Token: 0x040009A9 RID: 2473
	private Vector3 juicedViewUp;

	// Token: 0x040009AA RID: 2474
	private Vector3 viewUpSpringVelocity;

	// Token: 0x040009AB RID: 2475
	public float spring = 35f;

	// Token: 0x040009AC RID: 2476
	public float drag = 15f;

	// Token: 0x040009AD RID: 2477
	public float attackTime = 1f;

	// Token: 0x040009AE RID: 2478
	public float attackAngle = 180f;

	// Token: 0x040009AF RID: 2479
	public Transform mesh;

	// Token: 0x040009B0 RID: 2480
	public Animator anim;

	// Token: 0x040009B1 RID: 2481
	public Transform visuals;

	// Token: 0x040009B2 RID: 2482
	public float sleepDistance = 50f;

	// Token: 0x040009B3 RID: 2483
	public bool sleeping;

	// Token: 0x040009B4 RID: 2484
	[SerializeField]
	private Mob.MobState _mobState;

	// Token: 0x040009B6 RID: 2486
	private Rigidbody rig;

	// Token: 0x040009B7 RID: 2487
	[SerializeField]
	private bool attacking;

	// Token: 0x040009B8 RID: 2488
	private float _timeLastSwitchedTarget;

	// Token: 0x040009B9 RID: 2489
	private float _timeLastCheckedForTarget;

	// Token: 0x040009BA RID: 2490
	private float _timeLastAttacked;

	// Token: 0x040009BB RID: 2491
	private float _timeSpawned;

	// Token: 0x040009BC RID: 2492
	private Vector3 targetPos;

	// Token: 0x040009BD RID: 2493
	private float untilNextPatrolPos;

	// Token: 0x040009BE RID: 2494
	private Vector3 patrolPos;

	// Token: 0x040009BF RID: 2495
	private float lastStartedFlipping;

	// Token: 0x040009C0 RID: 2496
	private static readonly int WALKSPEED = Animator.StringToHash("WalkSpeed");

	// Token: 0x040009C1 RID: 2497
	private Vector3 startPos;

	// Token: 0x040009C2 RID: 2498
	private Vector3 startUp;

	// Token: 0x040009C3 RID: 2499
	private float lerpedWalkSpeed;

	// Token: 0x040009C4 RID: 2500
	private float fallTick;

	// Token: 0x040009C5 RID: 2501
	[SerializeField]
	internal bool forceNoMovement;

	// Token: 0x040009C6 RID: 2502
	[SerializeField]
	private float inRangeForAttackTime;

	// Token: 0x040009C7 RID: 2503
	[SerializeField]
	private Character _targetChar;

	// Token: 0x040009C8 RID: 2504
	private float startFlippingTick;

	// Token: 0x040009C9 RID: 2505
	private Vector3 normal;

	// Token: 0x040009CA RID: 2506
	private bool hitGround = true;

	// Token: 0x040009CB RID: 2507
	private float lastFell;

	// Token: 0x040009CC RID: 2508
	private RaycastHit groundCheckHitWalking;

	// Token: 0x040009CD RID: 2509
	private RaycastHit groundCheckHitDown;

	// Token: 0x040009CE RID: 2510
	private Vector3 lastPos;

	// Token: 0x02000469 RID: 1129
	internal enum MobState
	{
		// Token: 0x0400190C RID: 6412
		RigidbodyControlled,
		// Token: 0x0400190D RID: 6413
		Walking,
		// Token: 0x0400190E RID: 6414
		Flipping,
		// Token: 0x0400190F RID: 6415
		Dead
	}
}
