using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x02000014 RID: 20
public class CharacterMovement : MonoBehaviour
{
	// Token: 0x060001CB RID: 459 RVA: 0x0000DBA4 File Offset: 0x0000BDA4
	private void Start()
	{
		this.character = base.GetComponent<Character>();
		this.mouseSensSetting = GameHandler.Instance.SettingsHandler.GetSetting<MouseSensitivitySetting>();
		this.controllerSensSetting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerSensitivitySetting>();
		this.invertXSetting = GameHandler.Instance.SettingsHandler.GetSetting<InvertXSetting>();
		this.invertYSetting = GameHandler.Instance.SettingsHandler.GetSetting<InvertYSetting>();
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000DC11 File Offset: 0x0000BE11
	internal bool CanMoveCamera()
	{
		return !this.character.data.usingWheel;
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000DC28 File Offset: 0x0000BE28
	private void Update()
	{
		if (this.character.data.lastStoodOnPlayer)
		{
			this.CheckForPalJump(this.character.data.lastStoodOnPlayer);
		}
		if (this.character.IsLocal)
		{
			if (Singleton<MainCameraMovement>.Instance && Singleton<MainCameraMovement>.Instance.isGodCam)
			{
				this.character.input.ResetInput();
			}
			else
			{
				this.character.input.Sample(this.character.CanDoInput());
			}
		}
		if (this.CanMoveCamera())
		{
			this.CameraLook();
		}
		if (this.character.input.jumpWasPressed)
		{
			this.TryToJump();
		}
		this.SetMovementState();
		this.character.CalculateWorldMovementDir();
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000DCEB File Offset: 0x0000BEEB
	private void SetCrouch(bool setCrouch)
	{
		if (setCrouch != this.character.data.isCrouching)
		{
			this.character.refs.view.RPC("RPCA_SetCrouch", 0, new object[] { setCrouch });
		}
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000DD2A File Offset: 0x0000BF2A
	[PunRPC]
	public void RPCA_SetCrouch(bool setCrouch)
	{
		this.character.data.isCrouching = setCrouch;
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x0000DD40 File Offset: 0x0000BF40
	private void SetMovementState()
	{
		if (!this.character.refs.view.IsMine)
		{
			return;
		}
		if (this.character.input.crouchToggleWasPressed)
		{
			this.crouchToggleEnabled = !this.crouchToggleEnabled;
		}
		if ((this.crouchToggleEnabled || this.character.input.crouchIsPressed) && this.character.data.isGrounded)
		{
			this.SetCrouch(true);
		}
		else
		{
			this.SetCrouch(false);
		}
		if (this.character.data.sinceGrounded > 0.2f || this.character.data.isSprinting || this.character.data.isClimbing || this.character.data.isRopeClimbing)
		{
			this.SetCrouch(false);
		}
		if (!this.character.data.isGrounded || this.character.data.isSprinting)
		{
			this.crouchToggleEnabled = false;
		}
		if (!this.character.data.isGrounded)
		{
			this.character.data.isSprinting = this.character.input.movementInput.y > 0.01f && (this.character.input.sprintIsPressed || this.sprintToggleEnabled) && this.character.CheckSprint();
			if (!this.character.data.isSprinting)
			{
				this.sprintToggleEnabled = false;
			}
			return;
		}
		if (this.character.input.sprintToggleWasPressed)
		{
			this.sprintToggleEnabled = !this.sprintToggleEnabled;
		}
		this.character.data.isSprinting = this.character.input.movementInput.y > 0.01f && (this.character.input.sprintIsPressed || this.sprintToggleEnabled) && this.character.CheckSprint() && !this.character.OutOfRegularStamina();
		if (this.character.data.isSprinting)
		{
			this.character.UseStamina(this.sprintStaminaUsage * Time.deltaTime, true);
			return;
		}
		this.sprintToggleEnabled = false;
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0000DF7C File Offset: 0x0000C17C
	private void CameraLook()
	{
		float num = ((InputHandler.GetCurrentUsedInputScheme() == null) ? this.mouseSensSetting.Value : this.controllerSensSetting.Value);
		CharacterData data = this.character.data;
		data.lookValues.x = data.lookValues.x + this.character.input.lookInput.x * num * (float)((this.invertXSetting.Value == null) ? 1 : (-1));
		CharacterData data2 = this.character.data;
		data2.lookValues.y = data2.lookValues.y + this.character.input.lookInput.y * num * (float)((this.invertYSetting.Value == null) ? 1 : (-1));
		this.character.data.lookValues.y = Mathf.Clamp(this.character.data.lookValues.y, -85f, 85f);
		this.character.RecalculateLookDirections();
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000E074 File Offset: 0x0000C274
	private void FixedUpdate()
	{
		this.UpdateVariables();
		this.RaycastGroundCheck();
		this.EvaluateGroundChecks();
		if (this.character.data.isGrounded && this.character.CheckStand())
		{
			this.Stand();
		}
		Vector3 gravityForce = this.GetGravityForce();
		float num = this.GetMovementForce();
		if (this.character.data.currentItem)
		{
			this.character.refs.items.AddGravity(gravityForce);
			this.character.refs.items.AddMovementForce(num);
			this.character.refs.items.AddDrag(this.drag, 1f);
		}
		for (int i = 0; i < this.character.refs.ragdoll.partList.Count; i++)
		{
			this.character.refs.ragdoll.partList[i].Animate(this.animationForce * this.character.data.currentRagdollControll, this.animationTorque * this.character.data.currentRagdollControll);
			if (!this.character.data.isGrounded)
			{
				this.character.refs.ragdoll.partList[i].Gravity(gravityForce * this.character.data.currentRagdollControll * this.balloonFloatMultiplier);
			}
			this.character.refs.ragdoll.partList[i].ToggleUseGravity(this.character.data.currentRagdollControll < 0.9f);
			this.character.refs.ragdoll.partList[i].AddMovementForce(num * this.character.data.currentRagdollControll);
			this.character.refs.ragdoll.partList[i].Drag(this.drag, false);
			this.character.refs.ragdoll.partList[i].ApplyForces();
		}
		this.ResetFixedVars();
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000E2AB File Offset: 0x0000C4AB
	public void SetWaterMovementModifier(float value)
	{
		this.waterMovementModifier = Mathf.Min(value, this.waterMovementModifier);
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0000E2BF File Offset: 0x0000C4BF
	private void ResetFixedVars()
	{
		this.waterMovementModifier = 1f;
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0000E2CC File Offset: 0x0000C4CC
	public void ApplyExtraDrag(float extraDrag, bool ignoreRagdoll = false)
	{
		if (this.character.data.currentItem)
		{
			this.character.refs.items.AddDrag(Mathf.Lerp(1f, extraDrag, this.character.data.currentRagdollControll), 1f);
		}
		for (int i = 0; i < this.character.refs.ragdoll.partList.Count; i++)
		{
			this.character.refs.ragdoll.partList[i].Drag(extraDrag, ignoreRagdoll);
		}
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000E36C File Offset: 0x0000C56C
	public void ApplyParasolDrag(float extraDrag, float extraXZDrag, bool ignoreRagdoll = false)
	{
		if (this.character.data.currentItem)
		{
			this.character.refs.items.AddParasolDrag(Mathf.Lerp(1f, extraDrag, this.character.data.currentRagdollControll), Mathf.Lerp(1f, extraXZDrag, this.character.data.currentRagdollControll), 1f);
		}
		for (int i = 0; i < this.character.refs.ragdoll.partList.Count; i++)
		{
			this.character.refs.ragdoll.partList[i].ParasolDrag(extraDrag, extraXZDrag, ignoreRagdoll);
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x060001D7 RID: 471 RVA: 0x0000E428 File Offset: 0x0000C628
	private float CurrentMovementForce
	{
		get
		{
			if (!Application.isPlaying || !(this.character != null) || !(this.character.data != null))
			{
				return 0f;
			}
			return this.GetMovementForce();
		}
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x0000E460 File Offset: 0x0000C660
	private float GetMovementForce()
	{
		if (!this.character.CheckMovement())
		{
			return 0f;
		}
		float num = Mathf.Max(this.movementForce * this.movementModifier, 0f);
		if (this.character.data.isSprinting)
		{
			num *= this.sprintMultiplier;
		}
		if (this.character.data.isCrouching)
		{
			num *= 0.5f;
		}
		return num * this.waterMovementModifier;
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x0000E4D8 File Offset: 0x0000C6D8
	private void TryToJump()
	{
		bool flag = this.character.refs.afflictions.isWebbed && !this.character.OutOfStamina();
		if (this.character.data.jumpsRemaining <= 0 && !flag)
		{
			return;
		}
		if (!this.character.CheckJump())
		{
			return;
		}
		if (this.character.data.sinceGrounded > 0.2f && !flag)
		{
			return;
		}
		if (this.character.data.sinceJump < 0.3f)
		{
			return;
		}
		if (this.character.data.chargingJump)
		{
			return;
		}
		this.character.refs.view.RPC("JumpRpc", 0, new object[] { false });
	}

	// Token: 0x060001DA RID: 474 RVA: 0x0000E5A4 File Offset: 0x0000C7A4
	[PunRPC]
	public void JumpRpc(bool isPalJump)
	{
		CharacterMovement.<>c__DisplayClass45_0 CS$<>8__locals1 = new CharacterMovement.<>c__DisplayClass45_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.staminaCostMult = 1f;
		CS$<>8__locals1.jumpMult = 1f;
		CS$<>8__locals1.jumpDir = Vector3.up;
		if (isPalJump)
		{
			CS$<>8__locals1.staminaCostMult = 0f;
			CS$<>8__locals1.jumpMult = 2f;
			this.character.data.sincePalJump = 0f;
			CS$<>8__locals1.jumpDir += this.character.data.lookDirection_Flat * 0.25f;
			for (int i = 0; i < this.boostPlayer.Length; i++)
			{
				this.boostPlayer[i].Play(this.character.Center);
			}
		}
		this.character.data.jumpsRemaining--;
		this.character.data.isCrouching = false;
		this.character.data.chargingJump = true;
		this.character.OnStartJump();
		base.StartCoroutine(CS$<>8__locals1.<JumpRpc>g__IDoJump|0());
	}

	// Token: 0x060001DB RID: 475 RVA: 0x0000E6B8 File Offset: 0x0000C8B8
	private void UpdateVariables()
	{
		if (this.character.refs.ragdoll == null || this.character.refs.ragdoll.partList == null)
		{
			return;
		}
		this.character.data.avarageLastFrameVelocity = this.character.data.avarageVelocity;
		this.character.data.avarageVelocity = Vector3.zero;
		for (int i = 0; i < this.character.refs.ragdoll.partList.Count; i++)
		{
			this.character.data.avarageVelocity += this.character.refs.ragdoll.partList[i].Rig.linearVelocity / (float)this.character.refs.ragdoll.partList.Count;
		}
	}

	// Token: 0x060001DC RID: 476 RVA: 0x0000E7B4 File Offset: 0x0000C9B4
	private Vector3 GetGravityForce()
	{
		float num = 0f;
		if (!this.character.data.isGrounded && this.character.CheckGravity())
		{
			float sinceGrounded = this.character.data.sinceGrounded;
			float num2 = this.jumpGravityCurve.Evaluate(sinceGrounded * this.gravityCurveSpeed);
			if (this.character.data.isJumping)
			{
				num = Mathf.Lerp(this.jumpGravity, this.maxGravity, num2);
			}
			else
			{
				num = Mathf.Lerp(0f, this.maxGravity, num2);
			}
		}
		return num * Vector3.up;
	}

	// Token: 0x060001DD RID: 477 RVA: 0x0000E850 File Offset: 0x0000CA50
	private void Stand()
	{
		float targetHeadHeight = this.character.data.targetHeadHeight;
		float num = Mathf.InverseLerp(targetHeadHeight, targetHeadHeight - this.standSmooth, this.character.data.currentHeadHeight);
		float num2 = Mathf.InverseLerp(targetHeadHeight, targetHeadHeight + this.standSmooth, this.character.data.currentHeadHeight);
		Vector3 vector;
		vector..ctor(0f, (num - num2) * this.standForce * this.character.data.currentRagdollControll, 0f);
		this.character.GetBodypart(BodypartType.Head).AddForce(vector, 5);
		this.character.GetBodypart(BodypartType.Torso).AddForce(vector, 5);
		this.character.GetBodypart(BodypartType.Hip).AddForce(vector, 5);
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000E910 File Offset: 0x0000CB10
	protected virtual void EvaluateGroundChecks()
	{
		CharacterMovement.PlayerGroundSample playerGroundSample = null;
		for (int i = 0; i < this.groundSamples.Count; i++)
		{
			if (playerGroundSample == null)
			{
				playerGroundSample = this.groundSamples[i];
			}
			else if (this.groundSamples[i].point.y > playerGroundSample.point.y)
			{
				playerGroundSample = this.groundSamples[i];
			}
		}
		if (playerGroundSample == null)
		{
			playerGroundSample = this.IsLodged();
		}
		if (playerGroundSample != null && this.CanStand())
		{
			if (!this.character.data.isGrounded)
			{
				this.Land(playerGroundSample);
			}
			this.character.data.hasClimbedSinceGrounded = false;
			this.character.data.jumpsRemaining = 1;
			this.character.data.isJumping = false;
			this.character.data.isGrounded = true;
			this.character.data.groundNormal = playerGroundSample.normal;
			this.character.data.groundPos = playerGroundSample.point;
			this.character.data.currentHeadHeight = this.character.GetBodypart(BodypartType.Head).Rig.transform.position.y - playerGroundSample.point.y;
		}
		else
		{
			this.character.data.isGrounded = false;
		}
		this.groundSamples.Clear();
		this.groundSamples_All.Clear();
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000EA80 File Offset: 0x0000CC80
	private CharacterMovement.PlayerGroundSample IsLodged()
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		CharacterMovement.PlayerGroundSample playerGroundSample = null;
		for (int i = 0; i < this.groundSamples_All.Count; i++)
		{
			Vector3 normal = this.groundSamples_All[i].normal;
			if (normal.y > 0f)
			{
				zero2..ctor(Mathf.Min(zero2.x, normal.x), Mathf.Min(zero2.y, normal.y), Mathf.Min(zero2.z, normal.z));
				zero..ctor(Mathf.Max(zero.x, normal.x), Mathf.Max(zero.y, normal.y), Mathf.Max(zero.z, normal.z));
			}
			if (playerGroundSample == null)
			{
				playerGroundSample = this.groundSamples_All[i];
			}
			else if (this.groundSamples_All[i].point.y > playerGroundSample.point.y)
			{
				playerGroundSample = this.groundSamples_All[i];
			}
		}
		Vector3 vector = (zero + zero2) / 2f;
		if (vector.magnitude < 0.1f)
		{
			playerGroundSample = null;
		}
		if (playerGroundSample != null && !this.AcceptableAngle(Vector3.Angle(vector, Vector3.up)))
		{
			playerGroundSample = null;
		}
		return playerGroundSample;
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000EBD8 File Offset: 0x0000CDD8
	private bool CanStand()
	{
		return this.character.data.sinceJump > 0.3f && this.character.data.currentClimbHandle == null && !this.character.data.isClimbing;
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000EC2C File Offset: 0x0000CE2C
	private void Land(CharacterMovement.PlayerGroundSample bestSample)
	{
		if (this.character.data.sinceGrounded > 0.5f)
		{
			this.CheckFallDamage();
			if (this.character.IsLocal)
			{
				GUIManager.instance.ReticleLand();
			}
			this.character.OnLand(this.character.data.sinceGrounded);
		}
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000EC88 File Offset: 0x0000CE88
	private void CheckFallDamage()
	{
		if (this.FallTime() > this.fallDamageTime)
		{
			float num = Mathf.Max(this.FallFactor(3f, 1.5f), 0.05f);
			float num2 = num;
			num = Mathf.Min(num, this.MaxVelDmg());
			float num3 = num / num2;
			if (num >= 0.025f)
			{
				if (num > 0.3f && this.character.IsLocal)
				{
					this.character.Fall(num * 5f, 0f);
				}
				num *= Ascents.fallDamageMultiplier;
				if (this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num, false, true, true))
				{
					Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken, num);
				}
			}
		}
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000ED38 File Offset: 0x0000CF38
	private float MaxVelDmg()
	{
		float num = Mathf.Abs(Mathf.Min(this.character.data.avarageLastFrameVelocity.y, 0f));
		return Mathf.Pow(Mathf.InverseLerp(10f, 20f, num), 1.5f);
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0000ED84 File Offset: 0x0000CF84
	private float FallTime()
	{
		float num = Mathf.Min(this.character.data.sinceJump, this.character.data.sinceGrounded);
		if (this.character.data.sinceGrounded - this.character.data.sinceJump > -0.05f)
		{
			num -= 0.5f;
		}
		if (this.character.data.lastBouncedTime + 2.25f > Time.time)
		{
			num -= this.character.data.lastBouncedTime + 2.5f - Time.time;
		}
		return num;
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0000EE24 File Offset: 0x0000D024
	private float FallFactor(float maxTime = 3f, float pow = 1.5f)
	{
		float num = this.FallTime();
		return Mathf.Pow(Mathf.InverseLerp(this.fallDamageTime, maxTime, num), 1.5f);
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000EE4F File Offset: 0x0000D04F
	public void AddGroundSample(CharacterMovement.PlayerGroundSample sample)
	{
		this.groundSamples.Add(sample);
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0000EE5D File Offset: 0x0000D05D
	public void AddGroundSample_All(CharacterMovement.PlayerGroundSample sample)
	{
		this.groundSamples_All.Add(sample);
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000EE6C File Offset: 0x0000D06C
	private bool AcceptableAngle(float angle)
	{
		float num = this.maxAngle;
		return angle < num;
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0000EE84 File Offset: 0x0000D084
	private void RaycastGroundCheck()
	{
		Vector3 position = this.character.GetBodypartRig(BodypartType.Hip).position;
		Vector3 vector = position + Vector3.down * (this.character.data.targetHipHeight + 0.3f);
		RaycastHit raycastHit = HelperFunctions.LineCheck(position, vector, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		if (raycastHit.transform)
		{
			CollisionModifier component = raycastHit.collider.GetComponent<CollisionModifier>();
			if (component)
			{
				if (!component.standable)
				{
					return;
				}
				if (!component.CanStand(this.character))
				{
					return;
				}
			}
			float num = Vector3.Angle(Vector3.up, raycastHit.normal);
			if (!this.AcceptableAngle(num))
			{
				if (!this.character.data.isClimbing && !this.character.data.isRopeClimbing && ((double)this.character.data.sinceFallSlide < 0.2 || this.character.data.sinceGrounded < 2f))
				{
					this.character.data.sinceFallSlide = 0f;
					this.shakeCooldown += Time.deltaTime;
					this.ApplyExtraDrag(0.9f, false);
					this.LowerFall(num);
					if (this.shakeCooldown > 0.1f && this.FallTime() > this.fallDamageTime)
					{
						if (this.character.IsLocal)
						{
							GamefeelHandler.instance.AddPerlinShake(5f * this.FallFactor(3f, 1f), 0.2f, 10f);
						}
						this.shakeCooldown = 0f;
					}
				}
				return;
			}
			if (this.StandableRig(raycastHit.rigidbody) && this.DoGroundChecks() && this.character.data.groundedFor > 0.1f)
			{
				this.AddGroundSample(new CharacterMovement.PlayerGroundSample(raycastHit.point, raycastHit.normal));
			}
		}
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000F074 File Offset: 0x0000D274
	private void LowerFall(float upAngle)
	{
		float num = Mathf.InverseLerp(60f, 40f, upAngle);
		if (this.character.data.sinceGrounded > 1f)
		{
			this.character.data.sinceGrounded = Mathf.MoveTowards(this.character.data.sinceGrounded, 1f, num * Time.deltaTime * 2f);
		}
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0000F0E0 File Offset: 0x0000D2E0
	internal void OnCollision(Collision collision, bool collisionEnter, Bodypart bodypart)
	{
		CollisionModifier component = collision.collider.GetComponent<CollisionModifier>();
		if (component)
		{
			component.Collide(this.character, collision.contacts[0], collision, bodypart);
			if (!component.standable)
			{
				return;
			}
			if (!component.CanStand(this.character))
			{
				return;
			}
		}
		bool flag = false;
		if (this.StandOnPlayer(collision))
		{
			flag = true;
		}
		else if (!this.StandableRig(collision.rigidbody))
		{
			return;
		}
		float num = Vector3.Angle(Vector3.up, collision.contacts[0].normal);
		if (this.DoGroundChecks())
		{
			if (this.AcceptableAngle(num) || flag)
			{
				this.AddGroundSample(new CharacterMovement.PlayerGroundSample(collision.contacts[0].point, collision.contacts[0].normal));
			}
			this.AddGroundSample_All(new CharacterMovement.PlayerGroundSample(collision.contacts[0].point, collision.contacts[0].normal));
		}
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000F1DC File Offset: 0x0000D3DC
	private bool StandOnPlayer(Collision collision)
	{
		if (this.character.data.sincePalJump < 0.5f)
		{
			return false;
		}
		if (!collision.rigidbody)
		{
			return false;
		}
		Character componentInParent = collision.rigidbody.GetComponentInParent<Character>();
		if (componentInParent == this.character)
		{
			return false;
		}
		if (!componentInParent)
		{
			return false;
		}
		if (this.character.data.isCrouching)
		{
			return false;
		}
		if (!componentInParent.data.isCrouching)
		{
			return false;
		}
		this.character.data.sinceStandOnPlayer = 0f;
		this.character.data.lastStoodOnPlayer = componentInParent;
		return true;
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0000F284 File Offset: 0x0000D484
	private void CheckForPalJump(Character c)
	{
		if (this.character.data.sinceStandOnPlayer < 0.3f && c.data.sinceJump < 0.3f)
		{
			this.character.data.lastStoodOnPlayer = null;
			if (this.character.refs.view.IsMine)
			{
				this.character.refs.view.RPC("JumpRpc", 0, new object[] { true });
			}
		}
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000F30C File Offset: 0x0000D50C
	private bool StandableRig(Rigidbody rig)
	{
		return rig == null || rig.mass > 500f || rig.isKinematic;
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000F333 File Offset: 0x0000D533
	private bool DoGroundChecks()
	{
		return !this.character.data.isClimbing;
	}

	// Token: 0x040001B2 RID: 434
	private Character character;

	// Token: 0x040001B3 RID: 435
	public float movementForce = 10f;

	// Token: 0x040001B4 RID: 436
	public float movementModifier = 1f;

	// Token: 0x040001B5 RID: 437
	private float waterMovementModifier = 1f;

	// Token: 0x040001B6 RID: 438
	public float sprintMultiplier = 1f;

	// Token: 0x040001B7 RID: 439
	public float sprintStaminaUsage = 0.025f;

	// Token: 0x040001B8 RID: 440
	public float drag = 0.85f;

	// Token: 0x040001B9 RID: 441
	public float movementTurnSpeed = 2f;

	// Token: 0x040001BA RID: 442
	public float animationForce = 100f;

	// Token: 0x040001BB RID: 443
	public float animationTorque = 10f;

	// Token: 0x040001BC RID: 444
	public float standForce;

	// Token: 0x040001BD RID: 445
	public float standSmooth = 0.2f;

	// Token: 0x040001BE RID: 446
	public float jumpImpulse;

	// Token: 0x040001BF RID: 447
	public float jumpGravity = 10f;

	// Token: 0x040001C0 RID: 448
	public float jumpStaminaUsage;

	// Token: 0x040001C1 RID: 449
	public float jumpStaminaUsageSprinting;

	// Token: 0x040001C2 RID: 450
	public float maxGravity = -20f;

	// Token: 0x040001C3 RID: 451
	public AnimationCurve jumpGravityCurve;

	// Token: 0x040001C4 RID: 452
	public float gravityCurveSpeed = 1f;

	// Token: 0x040001C5 RID: 453
	public float airMovementTurnSpeed = 2f;

	// Token: 0x040001C6 RID: 454
	public SFX_Instance[] boostPlayer;

	// Token: 0x040001C7 RID: 455
	private MouseSensitivitySetting mouseSensSetting;

	// Token: 0x040001C8 RID: 456
	private ControllerSensitivitySetting controllerSensSetting;

	// Token: 0x040001C9 RID: 457
	private InvertXSetting invertXSetting;

	// Token: 0x040001CA RID: 458
	private InvertYSetting invertYSetting;

	// Token: 0x040001CB RID: 459
	private bool sprintToggleEnabled;

	// Token: 0x040001CC RID: 460
	private bool crouchToggleEnabled;

	// Token: 0x040001CD RID: 461
	public float balloonFloatMultiplier;

	// Token: 0x040001CE RID: 462
	public float balloonJumpMultiplier;

	// Token: 0x040001CF RID: 463
	private float fallDamageTime = 1.5f;

	// Token: 0x040001D0 RID: 464
	private float shakeCooldown;

	// Token: 0x040001D1 RID: 465
	private float maxAngle = 50f;

	// Token: 0x040001D2 RID: 466
	private List<CharacterMovement.PlayerGroundSample> groundSamples = new List<CharacterMovement.PlayerGroundSample>();

	// Token: 0x040001D3 RID: 467
	private List<CharacterMovement.PlayerGroundSample> groundSamples_All = new List<CharacterMovement.PlayerGroundSample>();

	// Token: 0x02000401 RID: 1025
	[Serializable]
	public class PlayerGroundSample
	{
		// Token: 0x06001A0A RID: 6666 RVA: 0x0007F252 File Offset: 0x0007D452
		public PlayerGroundSample(Vector3 point, Vector3 normal)
		{
			this.point = point;
			this.normal = normal;
		}

		// Token: 0x04001746 RID: 5958
		public Vector3 point;

		// Token: 0x04001747 RID: 5959
		public Vector3 normal;
	}
}
