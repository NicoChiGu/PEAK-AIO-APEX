using System;
using Photon.Pun;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x02000226 RID: 550
public class CharacterClimbing : MonoBehaviour
{
	// Token: 0x0600102B RID: 4139 RVA: 0x000504B8 File Offset: 0x0004E6B8
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.character = base.GetComponent<Character>();
		Character character = this.character;
		character.dragTowardsAction = (Action<Vector3, float>)Delegate.Combine(character.dragTowardsAction, new Action<Vector3, float>(this.GetDragged));
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x00050504 File Offset: 0x0004E704
	private void FixedUpdate()
	{
		if (this.character.data.currentClimbHandle)
		{
			this.HandleClimbHandle();
		}
		if (this.character.data.isClimbing)
		{
			this.Climbing();
		}
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x0005053C File Offset: 0x0004E73C
	private void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.ClimbHandleUpdate();
		this.sinceLastClimbStarted += Time.deltaTime;
		if (!this.character.data.isClimbing)
		{
			this.sprintHasBeenPressedSinceClimb = false;
			this.climbToggledOn = false;
			if (this.character.data.currentClimbHandle == null)
			{
				this.TryToStartWallClimb(false, default(Vector3), false, 1.25f);
			}
			return;
		}
		if (this.character.input.sprintWasPressed || this.character.input.sprintToggleWasPressed)
		{
			this.sprintHasBeenPressedSinceClimb = true;
		}
		if (this.sprintHasBeenPressedSinceClimb && (this.character.input.sprintIsPressed || this.character.input.sprintToggleIsPressed) && this.character.data.sinceClimbJump > 1f && this.character.data.outOfStaminaFor < 0.5f && this.character.input.movementInput.magnitude > 0.1f && this.character.input.movementInput.normalized.y > -0.9f)
		{
			this.character.refs.view.RPC("RPCA_ClimbJump", 0, Array.Empty<object>());
		}
		this.sinceShake += Time.deltaTime;
		if (this.character.OutOfStamina() && this.sinceShake > 0.1f && this.character.refs.view.IsMine)
		{
			GamefeelHandler.instance.AddPerlinShake(3f * Mathf.Clamp01(this.character.data.outOfStaminaFor * 1f), 0.2f, 10f);
			this.sinceShake = 0f;
		}
		float num = this.maxStaminaUsage * Mathf.Clamp(this.character.input.movementInput.magnitude, 0f, 1f);
		float num2 = this.minStaminaUsage * this.climbingStamMinimumMultiplier;
		num = Mathf.Clamp(num, num2, this.maxStaminaUsage);
		if (!this.character.data.staticClimbCost)
		{
			num *= this.GetAngleUsage();
		}
		num *= this.character.data.staminaMod;
		this.character.UseStamina(num * Time.deltaTime, true);
		this.TestAchievement();
		if (this.character.input.jumpWasPressed || (this.character.input.usePrimaryWasReleased && !this.climbToggledOn) || this.character.data.currentRagdollControll < 0.25f)
		{
			this.view.RPC("StopClimbingRpc", 0, new object[] { this.GetFallSpeed() });
		}
		this.climbingStamMinimumMultiplier = 1f;
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x0005082C File Offset: 0x0004EA2C
	private float GetAngleUsage()
	{
		float num = Vector3.Angle(Vector3.up, this.character.data.climbNormal);
		float num2 = Mathf.InverseLerp(40f, 60f, num);
		return Mathf.Lerp(0.2f, 1f, num2);
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x00050878 File Offset: 0x0004EA78
	private void ClimbHandleUpdate()
	{
		if (this.character.data.currentClimbHandle && this.view.IsMine)
		{
			if (this.character.data.fullyPassedOut || this.character.data.dead)
			{
				this.CancelHandle(false);
				return;
			}
			if (this.character.input.jumpWasPressed)
			{
				if (GameHandler.Instance.SettingsHandler.GetSetting<JumpToClimbSetting>().Value == 1)
				{
					this.CancelHandle(true);
					return;
				}
				this.CancelHandle(false);
				return;
			}
			else
			{
				if (this.character.data.isRopeClimbing)
				{
					this.CancelHandle(false);
					return;
				}
				if (this.character.data.isVineClimbing)
				{
					this.CancelHandle(false);
					return;
				}
			}
		}
		else
		{
			this.handleOffset = Vector2.zero;
		}
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x00050954 File Offset: 0x0004EB54
	public void CancelHandle(bool grabWall = true)
	{
		if (grabWall && this.character.IsLocal)
		{
			this.TryToStartWallClimb(true, this.character.data.currentClimbHandle.transform.forward, false, 1.25f);
		}
		this.character.data.currentClimbHandle.view.RPC("RPCA_UnHang", 0, new object[] { this.view });
		this.handleOffset = Vector2.zero;
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x000509D4 File Offset: 0x0004EBD4
	private void HandleClimbHandle()
	{
		this.handleOffset = Vector2.Lerp(this.handleOffset, this.character.input.movementInput, Time.fixedDeltaTime);
		if (this.handleOffset.magnitude > 0.3f && this.view.IsMine)
		{
			this.CancelHandle(true);
			return;
		}
		this.character.data.sinceGrounded = 0f;
		Vector3 vector = (this.character.GetBodypartRig(BodypartType.Hand_R).position + this.character.GetBodypartRig(BodypartType.Hand_L).position) * 0.5f;
		Vector3 vector2 = this.character.data.currentClimbHandle.transform.TransformPoint(new Vector3(0f, -0.7f, -0.3f));
		this.character.MoveBodypartTowardsPoint(BodypartType.Hand_L, vector2, 100f, 1f);
		this.character.MoveBodypartTowardsPoint(BodypartType.Hand_R, vector2, 100f, 1f);
		Vector3 vector3 = this.character.TorsoPos() - vector;
		Vector3 vector4 = vector2 + vector3 - this.character.TorsoPos();
		vector4 += this.character.data.currentClimbHandle.transform.up * this.handleOffset.y;
		vector4 += this.character.data.currentClimbHandle.transform.right * this.handleOffset.x;
		this.character.AddForce(vector4 * 50f, 1f, 1f);
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x00050B80 File Offset: 0x0004ED80
	public void StopClimbing()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		Debug.Log("StopClimbing");
		this.view.RPC("StopClimbingRpc", 0, new object[] { this.GetFallSpeed() });
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x00050BC0 File Offset: 0x0004EDC0
	[PunRPC]
	public void RPCA_ClimbJump()
	{
		this.character.data.sinceClimbJump = 0f;
		this.character.UseStamina(0.2f, true);
		this.playerSlide += this.character.input.movementInput.normalized * 8f;
		if (this.view.IsMine && !this.character.isBot)
		{
			GamefeelHandler.instance.AddPerlinShake(10f, 0.5f, 10f);
			GUIManager.instance.ClimbJump();
		}
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x00050C64 File Offset: 0x0004EE64
	private void GetDragged(Vector3 targetPos, float force)
	{
		this.character.data.climbPos += Vector3.ClampMagnitude(targetPos - this.character.Center, 1f) * (force * Time.fixedDeltaTime * 0.1f);
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x00050CBC File Offset: 0x0004EEBC
	private void Climbing()
	{
		if (!this.character.OutOfStamina())
		{
			this.character.data.sinceGrounded = 0f;
		}
		if (this.character.data.sinceClimbJump > 0.5f)
		{
			this.playerSlide += Vector2.down * 200f * Mathf.Clamp01(Mathf.Pow(this.character.data.outOfStaminaFor * 0.15f, 2f)) * Time.fixedDeltaTime;
		}
		if (!this.SampleWall(this.GetRequestedPostition()).transform)
		{
			if (this.view.IsMine)
			{
				this.view.RPC("StopClimbingRpc", 0, new object[] { this.GetFallSpeed() });
			}
			return;
		}
		this.character.refs.movement.ApplyExtraDrag(this.climbDrag, false);
		this.character.AddForce(this.GetClimbDirection(), 1f, 1f);
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x00050DDC File Offset: 0x0004EFDC
	private float GetFallSpeed()
	{
		float num = Mathf.InverseLerp(-5f, -60f, this.playerSlide.y) * 5f;
		float num2 = 0f;
		return Mathf.Max(num, num2);
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x00050E18 File Offset: 0x0004F018
	private Vector3 GetRequestedPostition()
	{
		Vector3 normalized = Vector3.ProjectOnPlane(Vector3.up, this.character.data.climbNormal).normalized;
		Vector3 normalized2 = Vector3.Cross(normalized, this.character.data.climbNormal).normalized;
		Vector3 vector = Vector3.zero;
		ClimbModifierSurface climbMod = this.character.data.climbMod;
		float num = 1f;
		if (climbMod)
		{
			num = climbMod.speedMultiplier;
		}
		if (climbMod && climbMod.onlySlideDown)
		{
			vector += normalized * -3f;
		}
		else if (this.character.data.sinceClimbJump > 0.5f && !this.character.OutOfStamina())
		{
			if (this.character.input.movementInput.y < 0f)
			{
				vector += normalized * -3f;
			}
			else
			{
				vector += normalized * (this.character.input.movementInput.y * this.character.data.staminaMod * num);
			}
		}
		vector += this.playerSlide.y * normalized * num;
		vector += this.playerSlide.x * -normalized2 * num;
		vector += normalized * -0.5f * Mathf.Clamp01(this.character.data.slippy);
		this.playerSlide *= 0.97f;
		this.playerSlide = Vector2.MoveTowards(this.playerSlide, Vector2.zero, Time.fixedDeltaTime * 15f);
		vector += -normalized2 * (this.character.input.movementInput.x * this.character.data.staminaMod * num);
		if (this.character.data.currentClimbHandle)
		{
			Vector3 vector2 = Vector3.ClampMagnitude(this.HandlePos() - this.character.data.climbPos, 1f) * 5f;
			float num2 = 1f;
			if (this.character.data.sinceClimbHandle > 0.5f)
			{
				num2 = Mathf.Lerp(1f, 0.15f, this.character.input.movementInput.magnitude);
			}
			vector = Vector3.Lerp(vector, vector2, num2);
		}
		return this.character.data.climbPos + vector * (this.climbSpeed * Time.fixedDeltaTime * this.climbSpeedMod);
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x000510F1 File Offset: 0x0004F2F1
	private Vector3 HandlePos()
	{
		return this.character.data.currentClimbHandle.transform.position + Vector3.down * 1.5f;
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x00051121 File Offset: 0x0004F321
	private Vector3 GetClimbDirection()
	{
		return (this.VisualClimberPos() - this.character.TorsoPos()) * this.climbForce;
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x00051144 File Offset: 0x0004F344
	private Vector3 VisualClimberPos()
	{
		return this.GetVisualClimberPos(this.character.data.climbPos, this.character.data.climbNormal);
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x0005116C File Offset: 0x0004F36C
	private Vector3 GetVisualClimberPos(Vector3 samplePos, Vector3 sampleNormal)
	{
		return samplePos + sampleNormal * 0.4f;
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x00051180 File Offset: 0x0004F380
	private RaycastHit SampleWall(Vector3 samplePos)
	{
		this.character.data.staticClimbCost = false;
		Vector3 vector = this.RaycastPos();
		Vector3 vector2 = samplePos + this.character.data.climbNormal * 0.5f;
		Vector3 vector3 = samplePos + this.character.data.climbNormal * -1f;
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector2, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.1f, 1);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.2f, 1);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.3f, 1);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.4f, 1);
		}
		if (raycastHit.transform == null)
		{
			return default(RaycastHit);
		}
		if (raycastHit.transform)
		{
			this.character.data.climbMod = raycastHit.collider.GetComponent<ClimbModifierSurface>();
			float num = Vector3.Angle(raycastHit.normal, Vector3.up);
			if (this.character.data.climbMod)
			{
				num = this.character.data.climbMod.OverrideClimbAngle(this.character, num);
				this.character.data.staticClimbCost = this.character.data.climbMod.staticClimbCost;
			}
			float num2 = num - 90f;
			if (num2 > 0f)
			{
				if (Mathf.Abs(num2) > (float)(this.character.OutOfStamina() ? 60 : 80))
				{
					return default(RaycastHit);
				}
			}
			else if (this.character.data.sinceClimbJump > 0.3f)
			{
				if (this.character.input.movementInput.magnitude < 0.1f)
				{
					if (Mathf.Abs(num2) > 60f)
					{
						this.CheckFallDamage(raycastHit);
						return default(RaycastHit);
					}
				}
				else if (Mathf.Abs(num2) > 40f)
				{
					this.CheckFallDamage(raycastHit);
					return default(RaycastHit);
				}
			}
			if (this.character.data.climbMod != null)
			{
				this.character.data.climbMod.OnClimb(this.character);
			}
			this.character.data.climbPos = raycastHit.point;
			this.character.data.climbNormal = raycastHit.normal;
			this.character.data.climbHit = raycastHit;
		}
		return raycastHit;
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x00051464 File Offset: 0x0004F664
	private void CheckFallDamage(RaycastHit hit)
	{
		if (this.playerSlide.y > 0f)
		{
			return;
		}
		float num = (Mathf.Abs(this.playerSlide.y) - 15f) * 0.035f;
		if (num < 0.15f)
		{
			return;
		}
		num -= 0.05f;
		this.character.data.sinceGrounded = 0f;
		this.playerSlide = Vector2.zero;
		if (num > 0.3f && this.character.IsLocal)
		{
			this.character.Fall(num * 5f, 0f);
		}
		Debug.Log("Damage: " + num.ToString());
		num *= Ascents.fallDamageMultiplier;
		if (this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num, false, true, true))
		{
			Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken, num);
		}
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x00051544 File Offset: 0x0004F744
	private bool AcceptableGrabAngle(Vector3 normal, Collider collider)
	{
		float num = Vector3.Angle(normal, Vector3.up);
		ClimbModifierSurface component = collider.GetComponent<ClimbModifierSurface>();
		if (component)
		{
			num = component.OverrideClimbAngle(this.character, num);
		}
		float num2 = num - 90f;
		if (num2 > 0f)
		{
			if (Mathf.Abs(num2) > 80f)
			{
				return false;
			}
		}
		else if (Mathf.Abs(num2) > 40f)
		{
			return false;
		}
		return true;
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x000515AC File Offset: 0x0004F7AC
	private void TryToStartWallClimb(bool forceAttempt = false, Vector3 overide = default(Vector3), bool botGrab = false, float raycastDistance = 1.25f)
	{
		string text = "Trying to start wall climb.";
		if (!this.CanClimb())
		{
			return;
		}
		if (this.character.isBot && !botGrab)
		{
			return;
		}
		if (!this.view.IsMine)
		{
			return;
		}
		Vector3 vector = MainCamera.instance.transform.position;
		Vector3 vector2 = this.character.data.lookDirection;
		if (botGrab)
		{
			vector = this.character.Center;
			vector2 = this.character.data.lookDirection_Flat.normalized;
		}
		if (forceAttempt)
		{
			vector2 = overide;
		}
		Vector3 vector3 = vector + vector2 * raycastDistance;
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		if (!raycastHit.transform)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.05f, 1);
		}
		if (!raycastHit.transform)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.1f, 1);
		}
		if (raycastHit.transform && this.AcceptableGrabAngle(raycastHit.normal, raycastHit.collider) && (this.sinceLastClimbStarted > 1f || !this.character.OutOfStamina()))
		{
			this.character.data.sinceCanClimb = 0f;
			if (this.character.data.sincePressClimb < 0.1f || this.validJumpToClimb || forceAttempt || botGrab)
			{
				this.character.refs.items.EquipSlot(Optionable<byte>.None);
				if (InputHandler.GetCurrentUsedInputScheme() == 1)
				{
					this.climbToggledOn = true;
				}
				this.sinceLastClimbStarted = 0f;
				this.view.RPC("StartClimbRpc", 0, new object[] { raycastHit.point, raycastHit.normal });
				text += "\nClimb started.";
			}
		}
	}

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06001040 RID: 4160 RVA: 0x00051784 File Offset: 0x0004F984
	private bool validJumpToClimb
	{
		get
		{
			return this.character.input.jumpWasPressed && this.character.data.sinceGrounded > 0.1f && GameHandler.Instance.SettingsHandler.GetSetting<JumpToClimbSetting>().Value == 1;
		}
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x000517D4 File Offset: 0x0004F9D4
	public bool CanClimb()
	{
		return this.character.data.sinceClimb >= 0.2f && !this.character.data.isRopeClimbing && !this.character.data.isVineClimbing;
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x00051823 File Offset: 0x0004FA23
	private Vector3 RaycastPos()
	{
		return this.character.data.climbPos + this.character.data.climbNormal * 0.4f;
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x00051854 File Offset: 0x0004FA54
	[PunRPC]
	private void StartClimbRpc(Vector3 climbPos, Vector3 climbNormal)
	{
		float num = 0f;
		if (this.character.data.hasClimbedSinceGrounded)
		{
			Vector3 vector = this.GetVisualClimberPos(climbPos, climbNormal) - (this.character.Center + Vector3.up * 0.5f);
			vector = Vector3.ProjectOnPlane(vector * 1.5f, climbNormal);
			float num2 = vector.magnitude;
			if (Vector3.Dot(vector, Vector3.up) < 0f)
			{
				num2 = 0f;
			}
			num2 = Mathf.Max(num2, 0.1f);
			this.character.UseStamina(0.15f * num2, true);
			if (this.character.OutOfStamina())
			{
				num += -num2 * this.outOfStamAttachSlide;
			}
		}
		if (this.character.data.avarageVelocity.y < 0f)
		{
			num += this.character.data.avarageVelocity.y * 1.5f;
		}
		this.character.OutOfStamina();
		this.playerSlide = new Vector2(this.playerSlide.x, num);
		this.character.data.climbPos = climbPos;
		this.character.data.climbNormal = climbNormal;
		this.character.data.hasClimbedSinceGrounded = true;
		this.character.data.isClimbing = true;
		this.character.data.isGrounded = false;
		this.character.data.sinceStartClimb = 0f;
		this.character.OnStartClimb();
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x000519E8 File Offset: 0x0004FBE8
	public void StopAnyClimbing()
	{
		if (this.character.data.isVineClimbing)
		{
			this.character.refs.vineClimbing.Stop();
			return;
		}
		if (this.character.data.isRopeClimbing)
		{
			this.character.refs.ropeHandling.Stop();
			return;
		}
		if (this.character.data.isClimbing)
		{
			this.view.RPC("StopClimbingRpc", 0, new object[] { 0f });
		}
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x00051A7C File Offset: 0x0004FC7C
	[PunRPC]
	public void StopClimbingRpc(float setFall)
	{
		this.character.data.isClimbing = false;
		this.character.data.isJumping = false;
		this.character.data.sinceGrounded = setFall;
		if (this.character.OutOfStamina())
		{
			this.character.data.sinceGrounded = Mathf.Clamp(this.character.data.sinceGrounded, 0.5f, 1000f);
		}
		this.playerSlide = Vector2.zero;
		this.climbToggledOn = false;
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x00051B0A File Offset: 0x0004FD0A
	internal void StartHang(ClimbHandle climbHandle)
	{
		this.character.data.currentClimbHandle = climbHandle;
		this.character.data.sinceClimbHandle = 0f;
		this.character.data.sinceGrounded = 0f;
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00051B48 File Offset: 0x0004FD48
	internal void TryClimb(float raycastDistance = 1.25f)
	{
		this.TryToStartWallClimb(false, default(Vector3), true, raycastDistance);
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x00051B68 File Offset: 0x0004FD68
	internal void TestAchievement()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		if (this.character.data.isClimbing && (this.character.Center.y - this.character.data.lastGroundedHeight) * CharacterStats.unitsToMeters >= 50f)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.EnduranceBadge);
		}
	}

	// Token: 0x04000E8D RID: 3725
	private Character character;

	// Token: 0x04000E8E RID: 3726
	public float outOfStamAttachSlide = 1f;

	// Token: 0x04000E8F RID: 3727
	public float climbForce;

	// Token: 0x04000E90 RID: 3728
	public float climbSpeed;

	// Token: 0x04000E91 RID: 3729
	public float climbSpeedMod = 1f;

	// Token: 0x04000E92 RID: 3730
	public float climbDrag = 0.85f;

	// Token: 0x04000E93 RID: 3731
	public float maxStaminaUsage = 0.2f;

	// Token: 0x04000E94 RID: 3732
	public float minStaminaUsage = 0.02f;

	// Token: 0x04000E95 RID: 3733
	[HideInInspector]
	public float climbingStamMinimumMultiplier = 1f;

	// Token: 0x04000E96 RID: 3734
	private PhotonView view;

	// Token: 0x04000E97 RID: 3735
	public Vector2 playerSlide;

	// Token: 0x04000E98 RID: 3736
	private float sinceShake;

	// Token: 0x04000E99 RID: 3737
	private Vector2 handleOffset;

	// Token: 0x04000E9A RID: 3738
	private bool sprintHasBeenPressedSinceClimb;

	// Token: 0x04000E9B RID: 3739
	private bool climbToggledOn;

	// Token: 0x04000E9C RID: 3740
	private float sinceLastClimbStarted;
}
