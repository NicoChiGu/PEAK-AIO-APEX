using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200031F RID: 799
public class Scoutmaster : MonoBehaviour
{
	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06001481 RID: 5249 RVA: 0x0006825D File Offset: 0x0006645D
	private bool targetForced
	{
		get
		{
			return Time.time < this.targetForcedUntil;
		}
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x0006826C File Offset: 0x0006646C
	public static bool GetPrimaryScoutmaster(out Scoutmaster scoutmaster)
	{
		if (Scoutmaster.AllScoutmasters.Count == 0)
		{
			scoutmaster = null;
			return false;
		}
		scoutmaster = Scoutmaster.AllScoutmasters[0];
		return true;
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06001483 RID: 5251 RVA: 0x0006828D File Offset: 0x0006648D
	// (set) Token: 0x06001484 RID: 5252 RVA: 0x00068295 File Offset: 0x00066495
	public Character currentTarget
	{
		get
		{
			return this._currentTarget;
		}
		set
		{
			if (this.targetForced)
			{
				return;
			}
			this._currentTarget = value;
		}
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x000682A7 File Offset: 0x000664A7
	private void OnEnable()
	{
		Scoutmaster.AllScoutmasters.Add(this);
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x000682B4 File Offset: 0x000664B4
	internal void SetCurrentTarget(Character setCurrentTarget, float forceForTime = 0f)
	{
		if (setCurrentTarget != this.currentTarget)
		{
			this.view.RPC("RPCA_SetCurrentTarget", 0, new object[]
			{
				(setCurrentTarget == null) ? (-1) : setCurrentTarget.photonView.ViewID,
				forceForTime
			});
		}
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x0006830E File Offset: 0x0006650E
	[PunRPC]
	private void RPCA_SetCurrentTarget(int targetViewID, float forceForTime)
	{
		if (targetViewID == -1)
		{
			this.currentTarget = null;
		}
		else
		{
			this.currentTarget = PhotonNetwork.GetPhotonView(targetViewID).GetComponent<Character>();
		}
		if (forceForTime > 0f)
		{
			this.targetForcedUntil = Time.time + forceForTime;
		}
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x00068343 File Offset: 0x00066543
	private void OnDestroy()
	{
		this.mat.SetFloat(this.STRENGTHID, 0f);
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x0006835B File Offset: 0x0006655B
	private void OnDisable()
	{
		this.mat.SetFloat(this.STRENGTHID, 0f);
		Scoutmaster.AllScoutmasters.Remove(this);
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x00068380 File Offset: 0x00066580
	private void Start()
	{
		this.animVars = base.GetComponentInChildren<ScoutmasterAnimVars>();
		this.character = base.GetComponent<Character>();
		this.view = base.GetComponent<PhotonView>();
		this.character.data.isScoutmaster = true;
		this.mat.SetFloat(this.STRENGTHID, 0f);
		this.mat.SetFloat(this.GRAINMULTID, (float)(GUIManager.instance.photosensitivity ? 0 : 1));
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x000683FC File Offset: 0x000665FC
	private void CalcVars()
	{
		this.sinceLookForTarget += Time.deltaTime;
		bool flag = this.currentTarget && this.CanSeeTarget(this.currentTarget);
		if (this.currentTarget)
		{
			if (!flag)
			{
				this.sinceSeenTarget += Time.deltaTime;
			}
			else
			{
				this.sinceSeenTarget = 0f;
			}
		}
		else
		{
			this.sinceSeenTarget = 0f;
		}
		if (this.currentTarget)
		{
			this.distanceToTarget = Vector3.Distance(this.character.Center, this.currentTarget.Center);
		}
		this.sinceAnyoneCanSeeMe += Time.deltaTime;
		if (this.AnyoneCanSeeMe())
		{
			this.sinceAnyoneCanSeeMe = 0f;
		}
		if (!this.currentTarget)
		{
			this.targetHasSeenMeCounter = 0f;
			return;
		}
		bool flag2 = Vector3.Distance(this.character.Center, this.currentTarget.Center) < 10f;
		bool flag3 = HelperFunctions.LineCheck(this.character.Center, this.currentTarget.Head, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform == null;
		if (Vector3.Angle(this.currentTarget.data.lookDirection, this.character.Center - this.currentTarget.Head) > 70f)
		{
			flag3 = false;
		}
		if (flag2 && flag3)
		{
			this.targetHasSeenMeCounter += Time.deltaTime * 1f;
			return;
		}
		if (flag3)
		{
			this.targetHasSeenMeCounter += Time.deltaTime * 0.3f;
			return;
		}
		if (flag2 && flag)
		{
			this.targetHasSeenMeCounter += Time.deltaTime * 0.15f;
			return;
		}
		this.targetHasSeenMeCounter = Mathf.MoveTowards(this.targetHasSeenMeCounter, 0f, Time.deltaTime * 0.1f);
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x000685EC File Offset: 0x000667EC
	private bool CanSeeTarget(Character currentTarget)
	{
		return HelperFunctions.LineCheck(this.character.Head, currentTarget.Center + Random.insideUnitSphere * 0.5f, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform == null;
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x00068638 File Offset: 0x00066838
	private void DoVisuals()
	{
		float num = 0f;
		if (this.currentTarget)
		{
			this.currentTarget.data.myersDistance = Vector3.Distance(this.character.Center, this.currentTarget.Center);
		}
		if (this.currentTarget && this.currentTarget.IsLocal)
		{
			num = Mathf.InverseLerp(50f, 5f, this.distanceToTarget);
			this.mat.SetFloat(this.GRAINMULTID, (float)(GUIManager.instance.photosensitivity ? 0 : 1));
		}
		this.mat.SetFloat(this.STRENGTHID, Mathf.Lerp(this.mat.GetFloat(this.STRENGTHID), num, Time.deltaTime * 0.5f));
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x00068708 File Offset: 0x00066908
	private void FixedUpdate()
	{
		if (this.animVars.reaching && this.character.data.grabbedPlayer == null && this.currentTarget)
		{
			Rigidbody bodypartRig = this.character.GetBodypartRig(BodypartType.Hand_R);
			Vector3 normalized = (this.currentTarget.Center - bodypartRig.transform.position).normalized;
			bodypartRig.AddForce(normalized * this.reachForce, 5);
		}
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x0006878C File Offset: 0x0006698C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.achievementDistance);
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x000687B0 File Offset: 0x000669B0
	private void Update()
	{
		this.UpdateAchievement();
		this.DoVisuals();
		if (!this.view.IsMine)
		{
			return;
		}
		this.tpCounter += Time.deltaTime;
		this.ResetInput();
		this.CalcVars();
		if (this.chillForSeconds > 0f)
		{
			this.chillForSeconds -= Time.deltaTime;
			return;
		}
		if (this.currentTarget == null)
		{
			this.EvasiveBehaviour();
			this.LookForTarget();
			return;
		}
		if (this.distanceToTarget > 80f)
		{
			this.TeleportCloseToTarget();
		}
		else
		{
			this.Chase();
		}
		this.VerifyTarget();
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x00068851 File Offset: 0x00066A51
	private void UpdateAchievement()
	{
		this.achievementTestTick += Time.deltaTime;
		if (this.achievementTestTick > 1f)
		{
			this.achievementTestTick = 0f;
			this.TestAchievement();
		}
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x00068883 File Offset: 0x00066A83
	private void TestAchievement()
	{
		if (Vector3.Distance(this.character.Center, Character.localCharacter.Center) <= this.achievementDistance)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.MentorshipBadge);
		}
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x000688B4 File Offset: 0x00066AB4
	private void VerifyTarget()
	{
		if (this.ViableTargets() < 2)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		Character closestOther = this.GetClosestOther(this.currentTarget);
		Character highestCharacter = this.GetHighestCharacter(null);
		Character highestCharacter2 = this.GetHighestCharacter(highestCharacter);
		if (highestCharacter.Center.y > this.maxAggroHeight)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		if (this.currentTarget != highestCharacter)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		if (highestCharacter.Center.y < highestCharacter2.Center.y + this.attackHeightDelta - 20f)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		if (Vector3.Distance(closestOther.Center, this.currentTarget.Center) < 15f)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x0006898C File Offset: 0x00066B8C
	private Character GetClosestOther(Character currentTarget)
	{
		List<Character> allCharacters = Character.AllCharacters;
		float num = float.MaxValue;
		Character character = null;
		foreach (Character character2 in allCharacters)
		{
			if (!character2.isBot && !(character2 == currentTarget))
			{
				float num2 = Vector3.Distance(character2.Center, currentTarget.Center);
				if (num2 < num)
				{
					num = num2;
					character = character2;
				}
			}
		}
		return character;
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x00068A10 File Offset: 0x00066C10
	private void EvasiveBehaviour()
	{
		if (!this.discovered)
		{
			this.discovered = this.GetPlayerWhoSeesMe();
		}
		if (this.discovered)
		{
			this.Flee();
			if (this.sinceAnyoneCanSeeMe > 0.5f)
			{
				this.TeleportFarAway();
			}
		}
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x00068A5C File Offset: 0x00066C5C
	public void TeleportFarAway()
	{
		if (this.tpCounter < 5f)
		{
			return;
		}
		this.tpCounter = 0f;
		this.view.RPC("WarpPlayerRPC", 0, new object[]
		{
			new Vector3(0f, 0f, 5000f),
			false
		});
		this.view.RPC("StopClimbingRpc", 0, new object[] { 0f });
		this.discovered = null;
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x00068AEC File Offset: 0x00066CEC
	private Character GetPlayerWhoSeesMe()
	{
		Vector3 vector = this.character.Center + Vector3.up * Random.Range(-0.5f, 0.5f);
		foreach (Character character in Character.AllCharacters)
		{
			if (!character.isBot && Vector3.Angle(vector - character.Head, character.data.lookDirection) <= 80f && HelperFunctions.LineCheck(character.Head, vector, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform == null)
			{
				return character;
			}
		}
		return null;
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x00068BB8 File Offset: 0x00066DB8
	private void Flee()
	{
		Vector3 normalized = (this.character.Center - this.discovered.Center).normalized;
		Vector3 vector = this.character.Center + normalized * 10f;
		if (this.character.data.isClimbing)
		{
			this.ClimbTowards(vector, 1f);
			return;
		}
		this.WalkTowards(vector, 1f);
		this.character.input.sprintIsPressed = true;
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x00068C44 File Offset: 0x00066E44
	private bool AnyoneCanSeeMe()
	{
		Vector3 vector = this.character.Head + Vector3.up * 0.3f + Random.insideUnitSphere * 0.5f;
		Vector3 vector2 = this.character.HipPos() - Vector3.up * 0.3f + Random.insideUnitSphere * 0.5f;
		return this.AnyoneCanSeePos(vector) || this.AnyoneCanSeePos(vector2);
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x00068CD0 File Offset: 0x00066ED0
	private bool AnyoneCanSeePos(Vector3 pos)
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (!character.isBot && Vector3.Angle(pos - character.Head, character.data.lookDirection) <= 80f)
			{
				if (HelperFunctions.LineCheck(character.Head, pos, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform == null)
				{
					Debug.DrawLine(character.Head, pos, Color.blue);
					return true;
				}
				Debug.DrawLine(character.Head, pos, Color.red);
			}
		}
		return false;
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x00068D98 File Offset: 0x00066F98
	private void TeleportCloseToTarget()
	{
		this.Teleport(this.currentTarget, 50f, 70f, 15f);
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x00068DB8 File Offset: 0x00066FB8
	private void Teleport(Character target, float minDistanceToTarget = 35f, float maxDistanceToTarget = 45f, float maxHeightDifference = 15f)
	{
		if (this.tpCounter < 5f)
		{
			return;
		}
		this.tpCounter = 0f;
		Debug.Log("Trying to Teleport");
		if (target == null)
		{
			target = this.GetHighestCharacter(null);
		}
		Vector3 center = this.character.Center;
		int i = 50;
		while (i > 0)
		{
			i--;
			Vector3 onUnitSphere = Random.onUnitSphere;
			Vector3 vector = target.Center + Vector3.up * 500f + onUnitSphere * 95f;
			Vector3 vector2 = Vector3.down;
			if (i < 25)
			{
				vector = target.Center + Vector3.up;
				vector2 = Random.onUnitSphere;
			}
			RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + vector2 * 1000f, HelperFunctions.LayerType.TerrainMap, 0f, 1);
			if (raycastHit.transform)
			{
				float num = Vector3.Distance(raycastHit.point, target.Center);
				float num2 = Mathf.Abs(raycastHit.point.y - target.Center.y);
				if (num < maxDistanceToTarget && num2 < maxHeightDifference && num > minDistanceToTarget && !this.AnyoneCanSeePos(raycastHit.point + Vector3.up))
				{
					Debug.Log("Teleporting");
					this.view.RPC("WarpPlayerRPC", 0, new object[]
					{
						raycastHit.point + Vector3.up,
						false
					});
					this.view.RPC("StopClimbingRpc", 0, new object[] { 0f });
					this.discovered = null;
					return;
				}
			}
		}
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x00068F74 File Offset: 0x00067174
	private void Chase()
	{
		if (this.sinceSeenTarget > 30f && !this.AnyoneCanSeeMe())
		{
			this.sinceSeenTarget = 0f;
			this.TeleportCloseToTarget();
			if (Random.value < 0.1f)
			{
				this.currentTarget = null;
			}
			return;
		}
		if (this.character.data.isClimbing)
		{
			this.ClimbTowards(this.currentTarget.Head, 1f);
			if (this.currentTarget.Center.y < this.character.Center.y && !HelperFunctions.LineCheck(this.character.Center, this.currentTarget.Center, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform)
			{
				this.character.refs.climbing.StopClimbing();
				return;
			}
		}
		else
		{
			if (this.character.data.grabbedPlayer)
			{
				this.HoldPlayer();
				return;
			}
			this.LookAt(this.currentTarget.Head);
			float num = Vector3.Distance(this.character.Center, this.currentTarget.Center);
			if (num > 5f || this.targetHasSeenMeCounter > 1f)
			{
				this.WalkTowards(this.currentTarget.Head, 1f);
			}
			if (this.targetHasSeenMeCounter > 1f)
			{
				this.character.input.sprintIsPressed = num < 15f;
				if (Vector3.Distance(this.character.Center, this.currentTarget.Center) < 3f && this.character.data.sinceClimb > 1f && this.character.data.isGrounded)
				{
					this.character.input.useSecondaryIsPressed = true;
				}
			}
		}
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x0006914C File Offset: 0x0006734C
	private void StandStill()
	{
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x0006914E File Offset: 0x0006734E
	private void ResetInput()
	{
		this.character.input.ResetInput();
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x00069160 File Offset: 0x00067360
	private void HoldPlayer()
	{
		this.currentTarget.data.sinceGrounded = 0f;
		this.character.input.useSecondaryIsPressed = true;
		Vector3 lookDirection = this.character.data.lookDirection;
		lookDirection.y = 0.6f;
		lookDirection.Normalize();
		this.character.data.lookValues = HelperFunctions.DirectionToLook(lookDirection);
		if (!this.isThrowing)
		{
			this.view.RPC("RPCA_Throw", 0, Array.Empty<object>());
		}
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x000691F0 File Offset: 0x000673F0
	[PunRPC]
	public void RPCA_Throw()
	{
		base.StartCoroutine(this.IThrow());
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x000691FF File Offset: 0x000673FF
	private IEnumerator IThrow()
	{
		this.isThrowing = true;
		if (this.view.IsMine)
		{
			this.RotateToMostEvilThrowDirection();
		}
		if (this.currentTarget.IsLocal)
		{
			GamefeelHandler.instance.AddPerlinShake(15f, 0.5f, 15f);
		}
		GamefeelHandler.instance.AddPerlinShake(3f, 3f, 15f);
		float c = 0f;
		while (c < 3.2f)
		{
			this.currentTarget.data.lookValues = HelperFunctions.DirectionToLook(this.character.Head - this.currentTarget.Head);
			c += Time.deltaTime;
			yield return null;
		}
		Vector3 vector = -this.character.data.lookDirection;
		vector.y = 0f;
		vector.Normalize();
		vector.y = 0.3f;
		this.character.refs.grabbing.Throw(vector * 1500f, 3f);
		this.currentTarget.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f, true, true, true);
		this.isThrowing = false;
		this.chillForSeconds = 2f;
		yield break;
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x00069210 File Offset: 0x00067410
	private void RotateToMostEvilThrowDirection()
	{
		Vector3[] circularDirections = HelperFunctions.GetCircularDirections(10);
		float num = 10f;
		float num2 = 1000f;
		Vector3 center = this.character.Center;
		Vector3 vector = this.character.data.lookDirection_Flat;
		float num3 = 0f;
		foreach (Vector3 vector2 in circularDirections)
		{
			Vector3 vector3 = center + vector2 * num;
			if (!HelperFunctions.LineCheck(center, vector3, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform)
			{
				RaycastHit raycastHit = HelperFunctions.LineCheck(vector3, center + vector3 + Vector3.down * num2, HelperFunctions.LayerType.TerrainMap, 0f, 1);
				if (raycastHit.transform && raycastHit.distance > num3)
				{
					vector = vector2;
					num3 = raycastHit.distance;
				}
			}
		}
		this.character.data.lookValues = HelperFunctions.DirectionToLook(-vector);
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x00069318 File Offset: 0x00067518
	private void ClimbTowards(Vector3 targetPos, float mult)
	{
		this.LookAt(targetPos);
		float num = Mathf.Clamp(this.character.GetBodypart(BodypartType.Torso).transform.InverseTransformPoint(targetPos).x * 0.25f, -1f, 1f);
		this.character.input.movementInput = new Vector2(num, mult);
		this.character.data.currentStamina = 1f;
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x0006938C File Offset: 0x0006758C
	private void WalkTowards(Vector3 targetPos, float mult)
	{
		this.LookAt(targetPos);
		float num = HelperFunctions.FlatDistance(this.character.Center, targetPos);
		if (Vector3.Distance(this.character.Center, targetPos) < 5f)
		{
			if (num < 2.5f)
			{
				mult *= 0f;
			}
			else if (num < 1.5f)
			{
				mult *= -1f;
			}
		}
		this.character.input.movementInput = new Vector2(0f, mult);
		this.character.refs.climbing.TryClimb(1.25f);
		if (HelperFunctions.LineCheck(this.character.Center, this.character.Center + Vector3.down * 3f, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform == null)
		{
			this.character.input.jumpWasPressed = true;
		}
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x0006947A File Offset: 0x0006767A
	private void LookAt(Vector3 lookAtPos)
	{
		this.character.data.lookValues = HelperFunctions.DirectionToLook(lookAtPos - this.character.Head);
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x000694A8 File Offset: 0x000676A8
	private int ViableTargets()
	{
		List<Character> allCharacters = Character.AllCharacters;
		int num = 0;
		foreach (Character character in allCharacters)
		{
			if (!character.isBot && !character.data.dead && !character.data.fullyPassedOut)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x0006951C File Offset: 0x0006771C
	private void LookForTarget()
	{
		if (this.ViableTargets() < 2)
		{
			return;
		}
		if (this.sinceLookForTarget < 30f)
		{
			return;
		}
		this.sinceLookForTarget = 0f;
		if (Random.value > 0.1f)
		{
			return;
		}
		Character highestCharacter = this.GetHighestCharacter(null);
		Character highestCharacter2 = this.GetHighestCharacter(highestCharacter);
		if (highestCharacter.Center.y > highestCharacter2.Center.y + this.attackHeightDelta && highestCharacter.Center.y < this.maxAggroHeight)
		{
			this.SetCurrentTarget(highestCharacter, 0f);
		}
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x000695A8 File Offset: 0x000677A8
	private Character GetHighestCharacter(Character ignoredCharacter)
	{
		List<Character> allCharacters = Character.AllCharacters;
		Character character = null;
		foreach (Character character2 in allCharacters)
		{
			if (!character2.isBot && !character2.data.dead && !character2.data.fullyPassedOut && !(character2 == ignoredCharacter) && (character == null || character2.Center.y > character.Center.y))
			{
				character = character2;
			}
		}
		return character;
	}

	// Token: 0x04001312 RID: 4882
	public float reachForce;

	// Token: 0x04001313 RID: 4883
	private float targetForcedUntil;

	// Token: 0x04001314 RID: 4884
	private Character _currentTarget;

	// Token: 0x04001315 RID: 4885
	internal static List<Scoutmaster> AllScoutmasters = new List<Scoutmaster>();

	// Token: 0x04001316 RID: 4886
	public Character discovered;

	// Token: 0x04001317 RID: 4887
	private ScoutmasterAnimVars animVars;

	// Token: 0x04001318 RID: 4888
	public float achievementDistance;

	// Token: 0x04001319 RID: 4889
	private int STRENGTHID = Shader.PropertyToID("_Strength");

	// Token: 0x0400131A RID: 4890
	private int GRAINMULTID = Shader.PropertyToID("_GrainMult");

	// Token: 0x0400131B RID: 4891
	private Character character;

	// Token: 0x0400131C RID: 4892
	private PhotonView view;

	// Token: 0x0400131D RID: 4893
	public Material mat;

	// Token: 0x0400131E RID: 4894
	private float sinceLookForTarget;

	// Token: 0x0400131F RID: 4895
	private float distanceToTarget;

	// Token: 0x04001320 RID: 4896
	private float sinceAnyoneCanSeeMe = 10f;

	// Token: 0x04001321 RID: 4897
	private float achievementTestTick;

	// Token: 0x04001322 RID: 4898
	private float attackHeightDelta = 100f;

	// Token: 0x04001323 RID: 4899
	private float tpCounter;

	// Token: 0x04001324 RID: 4900
	public float targetHasSeenMeCounter;

	// Token: 0x04001325 RID: 4901
	private float sinceSeenTarget;

	// Token: 0x04001326 RID: 4902
	private bool isThrowing;

	// Token: 0x04001327 RID: 4903
	private float chillForSeconds;

	// Token: 0x04001328 RID: 4904
	private float maxAggroHeight = 825f;
}
