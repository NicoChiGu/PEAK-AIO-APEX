using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200002B RID: 43
public class MushroomZombie : MonoBehaviourPunCallbacks
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060002F9 RID: 761 RVA: 0x00014BBC File Offset: 0x00012DBC
	private bool targetForced
	{
		get
		{
			return Time.time < this.targetForcedUntil;
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060002FA RID: 762 RVA: 0x00014BCB File Offset: 0x00012DCB
	// (set) Token: 0x060002FB RID: 763 RVA: 0x00014BD4 File Offset: 0x00012DD4
	public MushroomZombie.State currentState
	{
		get
		{
			return this._currentState;
		}
		set
		{
			if (this._currentState != value)
			{
				Debug.Log("Zombie state set to " + value.ToString());
				this._currentState = value;
				if (value == MushroomZombie.State.Dead)
				{
					this.timeDiedAt = Time.time;
				}
				if (base.photonView.IsMine)
				{
					this.PushState();
				}
			}
		}
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060002FC RID: 764 RVA: 0x00014C2F File Offset: 0x00012E2F
	// (set) Token: 0x060002FD RID: 765 RVA: 0x00014C37 File Offset: 0x00012E37
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

	// Token: 0x060002FE RID: 766 RVA: 0x00014C4C File Offset: 0x00012E4C
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

	// Token: 0x060002FF RID: 767 RVA: 0x00014CA6 File Offset: 0x00012EA6
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

	// Token: 0x06000300 RID: 768 RVA: 0x00014CDC File Offset: 0x00012EDC
	private void OnDestroy()
	{
		GlobalEvents.OnCharacterFell = (Action<Character, float>)Delegate.Remove(GlobalEvents.OnCharacterFell, new Action<Character, float>(this.TestCharacterFell));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Remove(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		ZombieManager.Instance.DeRegisterZombie(this);
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00014D34 File Offset: 0x00012F34
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.view = base.GetComponent<PhotonView>();
		this.timeAwoke = Time.time;
		GlobalEvents.OnCharacterFell = (Action<Character, float>)Delegate.Combine(GlobalEvents.OnCharacterFell, new Action<Character, float>(this.TestCharacterFell));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Combine(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		if (this.isNPCZombie)
		{
			ZombieManager.Instance.RegisterZombie(this);
		}
		GameUtils.instance.StartCoroutine(this.AwakeRoutine());
		this.InitMushroomVisuals();
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00014DCE File Offset: 0x00012FCE
	private void Start()
	{
		if (this.isNPCZombie)
		{
			this.StartSleeping();
			base.StartCoroutine(this.RevealZombie());
		}
		this.character.isZombie = true;
		base.StartCoroutine(this.ZombieGrunts());
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00014E04 File Offset: 0x00013004
	private IEnumerator ZombieGrunts()
	{
		for (;;)
		{
			if (!base.photonView.IsMine)
			{
				yield return null;
			}
			else
			{
				if (this.currentState != MushroomZombie.State.Dead && this.currentState != MushroomZombie.State.Sleeping)
				{
					float num = Random.Range(this.zombieGruntWaitTimeMinMax.x, this.zombieGruntWaitTimeMinMax.y);
					yield return new WaitForSeconds(num);
					base.photonView.RPC("RPC_PlaySFX", 0, new object[] { 0 });
				}
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06000304 RID: 772 RVA: 0x00014E14 File Offset: 0x00013014
	[PunRPC]
	private void RPC_PlaySFX(int index)
	{
		switch (index)
		{
		case 0:
			this.gruntSFX.PlayFromSource(this.character.Center, this.gruntSource);
			return;
		case 1:
			this.knockoutSFX.Play(this.character.Center);
			return;
		case 2:
		{
			SFX_Instance[] array = this.biteSFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play(this.character.Center);
			}
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00014E90 File Offset: 0x00013090
	private void InitMushroomVisuals()
	{
		this.mushroomGrowTimes.Clear();
		foreach (GameObject gameObject in this.mushroomVisuals)
		{
			this.mushroomGrowTimes.Add(Random.Range(this.minMaxMushroomGrowTime.x, this.minMaxMushroomGrowTime.y));
			gameObject.SetActive(true);
			gameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		}
	}

	// Token: 0x06000306 RID: 774 RVA: 0x00014F34 File Offset: 0x00013134
	[PunRPC]
	private void RPC_SetOutfit(bool hasSkirt)
	{
		this.wearingSkirt = hasSkirt;
		this.skirt.SetActive(hasSkirt);
		this.shorts.SetActive(!hasSkirt);
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00014F58 File Offset: 0x00013158
	private IEnumerator AwakeRoutine()
	{
		yield return null;
		if (this.character.refs.customization.overridePhotonPlayer != null)
		{
			base.gameObject.name = string.Format("Zombie [{0} : {1}]", this.character.refs.customization.overridePhotonPlayer.NickName, this.character.refs.customization.overridePhotonPlayer.ActorNumber);
		}
		else
		{
			base.gameObject.name = "Zombie (NPC)";
			bool flag = Util.Coinflip();
			base.photonView.RPC("RPC_SetOutfit", 0, new object[] { flag });
		}
		this.SetZombieEyes();
		yield break;
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00014F68 File Offset: 0x00013168
	private void SetZombieEyes()
	{
		CustomizationRefs refs = this.character.refs.customization.refs;
		for (int i = 0; i < refs.EyeRenderers.Length; i++)
		{
			refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.zombieEyeTexture);
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x00014FBC File Offset: 0x000131BC
	[PunRPC]
	public void RPC_Arise(int sourceCharacterPhotonID)
	{
		PhotonView photonView = PhotonNetwork.GetPhotonView(sourceCharacterPhotonID);
		if (photonView == null)
		{
			return;
		}
		this.isNPCZombie = false;
		this.spawnedFromCharacter = photonView.GetComponent<Character>();
		foreach (Bodypart bodypart in base.transform.GetComponentsInChildren<Bodypart>())
		{
			Bodypart bodypart2 = this.spawnedFromCharacter.GetBodypart(bodypart.partType);
			if (!(bodypart2 == null))
			{
				bodypart.Rig.MovePosition(bodypart2.Rig.position);
				bodypart.Rig.MoveRotation(bodypart2.Rig.rotation);
			}
		}
		this.character.data.currentRagdollControll = 0f;
		Debug.Log("Spawned Zombie from " + photonView.Owner.NickName);
		base.gameObject.name = string.Format("Zombie [{0} : {1}]", photonView.Owner.NickName, photonView.Owner.ActorNumber);
		this.character.refs.customization.overridePhotonPlayer = this.spawnedFromCharacter.photonView.Owner;
		this.spawnedFromCharacter.FinishZombifying();
		this.currentState = MushroomZombie.State.WakingUp;
	}

	// Token: 0x0600030A RID: 778 RVA: 0x000150EC File Offset: 0x000132EC
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
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00015194 File Offset: 0x00013394
	private bool CanSeeTarget(Character currentTarget)
	{
		return !(currentTarget == null) && HelperFunctions.LineCheck(this.character.Head, currentTarget.Center + Random.insideUnitSphere * 0.5f, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform == null;
	}

	// Token: 0x0600030C RID: 780 RVA: 0x000151EB File Offset: 0x000133EB
	private void FixedUpdate()
	{
	}

	// Token: 0x0600030D RID: 781 RVA: 0x000151F0 File Offset: 0x000133F0
	private void Update()
	{
		if (this.mushroomsGrowing)
		{
			this.UpdateMushroomGrowth();
		}
		if (this.view.IsMine)
		{
			if (this.spawnedFromCharacter && !this.spawnedFromCharacter.data.dead)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
			this.ResetInput();
			this.CalcVars();
			this.ValidateTarget();
			switch (this.currentState)
			{
			case MushroomZombie.State.Sleeping:
				this.DoSleeping();
				break;
			case MushroomZombie.State.WakingUp:
				this.DoWakingUp();
				break;
			case MushroomZombie.State.Idle:
				this.DoIdle();
				break;
			case MushroomZombie.State.Chasing:
				this.DoChasing();
				break;
			case MushroomZombie.State.Lunging:
				this.DoLunging();
				break;
			case MushroomZombie.State.LungeRecovery:
				this.DoLungeRecovery();
				break;
			case MushroomZombie.State.Dead:
				this.character.data.passedOut = true;
				this.character.data.fallSeconds = 10f;
				break;
			}
		}
		else if (this.currentState == MushroomZombie.State.Dead || this.currentState == MushroomZombie.State.Sleeping)
		{
			this.character.data.passedOut = true;
		}
		else if (this.currentState == MushroomZombie.State.WakingUp)
		{
			this.DoWakingUp();
		}
		this.UpdateMouth();
		this.biteColliderObject.SetActive(this.currentState == MushroomZombie.State.Lunging);
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00015330 File Offset: 0x00013530
	private void UpdateMushroomGrowth()
	{
		bool flag = false;
		for (int i = 0; i < this.mushroomVisuals.Count; i++)
		{
			GameObject gameObject = this.mushroomVisuals[i];
			if (gameObject.transform.localScale.x < 1f)
			{
				float num = this.mushroomGrowTimes[i];
				gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale, Vector3.one, 1f / num * Time.deltaTime);
				flag = true;
			}
		}
		if (!flag)
		{
			this.mushroomsGrowing = false;
		}
	}

	// Token: 0x0600030F RID: 783 RVA: 0x000153BF File Offset: 0x000135BF
	public void StartSleeping()
	{
		this.currentState = MushroomZombie.State.Sleeping;
	}

	// Token: 0x06000310 RID: 784 RVA: 0x000153C8 File Offset: 0x000135C8
	public void HideAllRenderers()
	{
		this.character.refs.customization.HideAllRenderers();
		this.visible = false;
	}

	// Token: 0x06000311 RID: 785 RVA: 0x000153E6 File Offset: 0x000135E6
	public IEnumerator RevealZombie()
	{
		this.character.refs.customization.HideAllRenderers();
		yield return new WaitForSeconds(1f);
		if (this != null)
		{
			this.FadeInRenderers();
		}
		this.visible = true;
		yield break;
	}

	// Token: 0x06000312 RID: 786 RVA: 0x000153F8 File Offset: 0x000135F8
	private void DoSleeping()
	{
		this.character.data.fallSeconds = 10f;
		if (!this.visible)
		{
			return;
		}
		foreach (Character character in Character.AllCharacters)
		{
			if (this.TargetIsValid(character))
			{
				float num = Vector3.Distance(this.character.Center, character.Center);
				float num2 = Vector3.Angle(character.refs.head.transform.forward, this.character.Center - character.refs.head.transform.position);
				if (num < this.distanceBeforeWakeup && num2 <= this.lookAngleBeforeWakeup && this.HasLineOfSight(character))
				{
					this.WakeUpFromSleep();
					break;
				}
			}
		}
	}

	// Token: 0x06000313 RID: 787 RVA: 0x000154E8 File Offset: 0x000136E8
	private void WakeUpFromSleep()
	{
		this.character.data.fallSeconds = 0f;
		this.character.data.currentRagdollControll = 0f;
		this.character.refs.animations.ResetForceSpeed();
		this.character.refs.ragdoll.ToggleKinematic(false);
		this.currentState = MushroomZombie.State.WakingUp;
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00015551 File Offset: 0x00013751
	private bool HasLineOfSight(Character otherCharacter)
	{
		return !Physics.Linecast(otherCharacter.Center, this.character.Center + Vector3.up, HelperFunctions.terrainMapMask);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00015580 File Offset: 0x00013780
	private void FadeInRenderers()
	{
		this.character.refs.customization.ShowAllRenderers();
		for (int i = 0; i < this.character.refs.customization.refs.AllRenderers.Length; i++)
		{
			for (int j = 0; j < this.character.refs.customization.refs.AllRenderers[i].materials.Length; j++)
			{
				this.character.refs.customization.refs.AllRenderers[i].materials[j].SetFloat("_Opacity", 0f);
				ShortcutExtensions.DOFloat(this.character.refs.customization.refs.AllRenderers[i].materials[j], 1f, "_Opacity", 1.5f);
			}
		}
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0001566A File Offset: 0x0001386A
	private void ValidateTarget()
	{
		if (this.currentTarget && !this.TargetIsValid(this.currentTarget))
		{
			this.currentTarget = null;
		}
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0001568E File Offset: 0x0001388E
	private void StartIdle()
	{
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("RPC_PlaySFX", 0, new object[] { 0 });
		}
		this.currentState = MushroomZombie.State.Idle;
	}

	// Token: 0x06000318 RID: 792 RVA: 0x000156C4 File Offset: 0x000138C4
	private void DoIdle()
	{
		this.idledFor += Time.deltaTime;
		if (this.idledFor > 2f)
		{
			this.TryLookForTarget();
			if (this.currentTarget)
			{
				this.LookAt(this.currentTarget.Head);
				if ((this.distanceToTarget >= this.distanceBeforeChase || this.timeSpentAwake > 2f) && this.HasLineOfSight(this.currentTarget))
				{
					this.StartChasing();
				}
			}
		}
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00015744 File Offset: 0x00013944
	private void StartLunging()
	{
		if (this.currentTarget == null)
		{
			return;
		}
		this.lungeTargetForward = this.character.Center + (this.currentTarget.Head - this.character.Center) * 100f;
		this.character.input.jumpWasPressed = true;
		this.currentState = MushroomZombie.State.Lunging;
		this.timeSpentLunging = 0f;
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x0600031A RID: 794 RVA: 0x000157BE File Offset: 0x000139BE
	private float timeSpentAwake
	{
		get
		{
			return Time.time - this.timeAwoke;
		}
	}

	// Token: 0x0600031B RID: 795 RVA: 0x000157CC File Offset: 0x000139CC
	private void DoLungeRecovery()
	{
		if (this.character.data.fallSeconds > 0f || this.character.data.passedOut || this.character.data.fullyPassedOut)
		{
			return;
		}
		this.timeSpentRecoveringFromLunge += Time.deltaTime;
		if (this.timeSpentRecoveringFromLunge >= this.lungeRecoveryTime)
		{
			this.timeSpentRecoveringFromLunge = 0f;
			this.StartChasing();
		}
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00015848 File Offset: 0x00013A48
	private void DoLunging()
	{
		this.WalkTowards(this.lungeTargetForward, 1.2f, false, false, true);
		this.timeSpentLunging += Time.deltaTime;
		if (this.timeSpentLunging >= this.lungeTime)
		{
			this.timeSpentLunging = 0f;
			this.currentState = MushroomZombie.State.LungeRecovery;
			this.character.Fall(3f, 0f);
			this.PushState();
		}
	}

	// Token: 0x0600031D RID: 797 RVA: 0x000158B8 File Offset: 0x00013AB8
	private void DoWakingUp()
	{
		this.character.data.passedOut = true;
		this.character.data.currentRagdollControll = 0f;
		if (base.photonView.IsMine)
		{
			this.timeSpentWakingUp += Time.deltaTime;
			if (this.timeSpentWakingUp >= this.initialWakeUpTime)
			{
				this.timeSpentWakingUp = 0f;
				this.character.data.passedOut = false;
				this.StartIdle();
			}
		}
	}

	// Token: 0x0600031E RID: 798 RVA: 0x0001593A File Offset: 0x00013B3A
	private void StartChasing()
	{
		this.timeSpentChasing = 0f;
		this.currentState = MushroomZombie.State.Chasing;
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0001594E File Offset: 0x00013B4E
	private void DoChasing()
	{
		this.timeSpentChasing += Time.deltaTime;
		this.TryLookForTarget();
		if (this.currentTarget != null)
		{
			this.Chase();
			this.TryLunge();
			this.VerifyTarget();
		}
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x06000320 RID: 800 RVA: 0x00015988 File Offset: 0x00013B88
	private bool readyToSprint
	{
		get
		{
			return this.timeSpentChasing > this.chaseTimeBeforeSprint;
		}
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00015998 File Offset: 0x00013B98
	private void VerifyTarget()
	{
		if (this.currentTarget == null)
		{
			return;
		}
		if (!this.TargetIsValid(this.currentTarget))
		{
			this.SetCurrentTarget(null, 0f);
		}
	}

	// Token: 0x06000322 RID: 802 RVA: 0x000159C4 File Offset: 0x00013BC4
	private void Chase()
	{
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
			this.WalkTowards(this.currentTarget.Head, 1f, true, true, false);
		}
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00015A7C File Offset: 0x00013C7C
	private void TryLunge()
	{
		if (this.currentState == MushroomZombie.State.Chasing && this.readyToSprint && this.character.data.isGrounded && this.character.input.sprintIsPressed && this.distanceToTarget <= this.zombieLungeDistance && this.CanSeeTarget(this.currentTarget))
		{
			this.StartLunging();
		}
	}

	// Token: 0x06000324 RID: 804 RVA: 0x00015AE0 File Offset: 0x00013CE0
	private void ResetInput()
	{
		this.character.input.ResetInput();
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00015AF4 File Offset: 0x00013CF4
	private void ClimbTowards(Vector3 targetPos, float mult)
	{
		this.LookAt(targetPos);
		float num = Mathf.Clamp(this.character.GetBodypart(BodypartType.Torso).transform.InverseTransformPoint(targetPos).x * 0.25f, -1f, 1f);
		this.character.input.movementInput = new Vector2(num, mult);
		this.character.data.currentStamina = 1f;
	}

	// Token: 0x06000326 RID: 806 RVA: 0x00015B66 File Offset: 0x00013D66
	private void SetSprint(bool sprinting)
	{
		this.character.input.sprintIsPressed = sprinting;
		if (sprinting != this.character.data.isSprinting)
		{
			this.character.data.isSprinting = sprinting;
			this.PushState();
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00015BA4 File Offset: 0x00013DA4
	private void WalkTowards(Vector3 targetPos, float mult, bool tryClimb = true, bool tryJump = true, bool forceSprint = false)
	{
		float num = HelperFunctions.FlatDistance(this.character.Center, targetPos);
		if (Vector3.Distance(this.character.Center, targetPos) < 5f && num < 1.5f)
		{
			mult *= 0f;
		}
		this.character.input.movementInput = new Vector2(0f, mult);
		this.SetSprint(forceSprint || (this.readyToSprint && this.distanceToTarget < this.zombieSprintDistance && this.CanSeeTarget(this.currentTarget)));
		if (this.character.input.sprintIsPressed)
		{
			this.LookAt(targetPos);
		}
		else
		{
			this.LookAt(targetPos);
		}
		if (tryClimb)
		{
			this.character.refs.climbing.TryClimb(1.25f);
		}
		if (tryJump && HelperFunctions.LineCheck(this.character.Center, this.character.Center + Vector3.down * 3f, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform == null)
		{
			this.SetSprint(false);
			this.character.input.jumpWasPressed = true;
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x00015CDA File Offset: 0x00013EDA
	private void LookAt(Vector3 lookAtPos)
	{
		this.character.data.lookValues = HelperFunctions.DirectionToLook(lookAtPos - this.character.Head);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00015D08 File Offset: 0x00013F08
	private void TryLookForTarget()
	{
		if (this.sinceLookForTarget < 10f)
		{
			return;
		}
		Character closestCharacter = this.GetClosestCharacter(null);
		this.SetCurrentTarget(closestCharacter, 0f);
		this.sinceLookForTarget = 0f;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x00015D42 File Offset: 0x00013F42
	private bool TargetIsValid(Character target)
	{
		return !target.isBot && !target.data.dead && !target.data.fullyPassedOut;
	}

	// Token: 0x0600032B RID: 811 RVA: 0x00015D6C File Offset: 0x00013F6C
	private Character GetClosestCharacter(Character ignoredCharacter)
	{
		List<Character> allCharacters = Character.AllCharacters;
		Character character = null;
		float num = float.MaxValue;
		foreach (Character character2 in allCharacters)
		{
			if (!(character2 == ignoredCharacter) && this.TargetIsValid(character2))
			{
				float num2 = Vector3.Distance(character2.Center, this.character.Center);
				if (character == null || num2 < num)
				{
					character = character2;
					num = num2;
				}
			}
		}
		return character;
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00015E00 File Offset: 0x00014000
	private void PushState()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		base.photonView.RPC("RPC_SyncState", 1, new object[]
		{
			(int)this.currentState,
			this.character.data.isSprinting,
			this.character.data.fallSeconds,
			this.character.data.passedOut
		});
	}

	// Token: 0x0600032D RID: 813 RVA: 0x00015E88 File Offset: 0x00014088
	[PunRPC]
	private void RPC_SyncState(int state, bool isSprinting, float fallSeconds, bool passedOut)
	{
		this.currentState = (MushroomZombie.State)state;
		this.character.data.isSprinting = isSprinting;
		this.character.data.fallSeconds = fallSeconds;
		this.character.data.passedOut = passedOut;
	}

	// Token: 0x0600032E RID: 814 RVA: 0x00015EC8 File Offset: 0x000140C8
	private void TestCharacterFell(Character c, float time)
	{
		if (this.view.IsMine && c == this.character && this.currentState != MushroomZombie.State.Dead)
		{
			if (this.currentState != MushroomZombie.State.Lunging && this.currentState != MushroomZombie.State.LungeRecovery)
			{
				base.photonView.RPC("RPC_PlaySFX", 0, new object[] { 1 });
			}
			if (this.timeSpentAwake > this.lifetime)
			{
				this.Die();
				return;
			}
			if (this.currentState != MushroomZombie.State.LungeRecovery)
			{
				this.timeSpentLunging = 0f;
				this.currentState = MushroomZombie.State.LungeRecovery;
				this.character.data.fallSeconds = 3f;
				this.PushState();
			}
		}
	}

	// Token: 0x0600032F RID: 815 RVA: 0x00015F7C File Offset: 0x0001417C
	private void Die()
	{
		this.currentState = MushroomZombie.State.Dead;
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00015F88 File Offset: 0x00014188
	private void TestCharacterPassedOut(Character c)
	{
		if (this.view.IsMine && c == this.character && this.currentState != MushroomZombie.State.Dead)
		{
			if (this.currentState != MushroomZombie.State.Lunging)
			{
				base.photonView.RPC("RPC_PlaySFX", 0, new object[] { 1 });
			}
			if (this.timeSpentAwake > this.lifetime)
			{
				this.Die();
				return;
			}
			this.timeSpentLunging = 0f;
			this.currentState = MushroomZombie.State.LungeRecovery;
		}
	}

	// Token: 0x06000331 RID: 817 RVA: 0x00016009 File Offset: 0x00014209
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (base.photonView.IsMine)
		{
			this.PushState();
			base.photonView.RPC("RPC_SetOutfit", newPlayer, new object[] { this.wearingSkirt });
		}
	}

	// Token: 0x06000332 RID: 818 RVA: 0x00016044 File Offset: 0x00014244
	private void UpdateMouth()
	{
		if (this.currentState == MushroomZombie.State.Lunging)
		{
			this.animatedMouth.mouthRenderer.material.SetInt("_UseTalkSprites", 1);
			Material material = this.animatedMouth.mouthRenderer.material;
			string text = "_TalkSprite";
			Texture2D[] mouthTextures = this.animatedMouth.mouthTextures;
			material.SetTexture(text, mouthTextures[mouthTextures.Length - 1]);
			return;
		}
		this.animatedMouth.mouthRenderer.material.SetInt("_UseTalkSprites", 0);
	}

	// Token: 0x06000333 RID: 819 RVA: 0x000160BC File Offset: 0x000142BC
	public void OnBitCharacter(Character c)
	{
		this.character.Fall(8f, 0f);
		if (c.IsLocal)
		{
			if (this.currentState != MushroomZombie.State.Lunging)
			{
				base.photonView.RPC("RPC_PlaySFX", 0, new object[] { 2 });
			}
			Debug.Log("Bit by zombie");
			Singleton<AchievementManager>.Instance.SetRunBasedInt(RUNBASEDVALUETYPE.BitByZombie, 1);
		}
	}

	// Token: 0x06000334 RID: 820 RVA: 0x00016128 File Offset: 0x00014328
	public void DestroyZombie()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (this.currentState == MushroomZombie.State.Dead)
		{
			base.photonView.RPC("RPC_SpawnSkelly", 0, Array.Empty<object>());
			if (this.spawner)
			{
				Object.Destroy(this.spawner.gameObject);
			}
			PhotonNetwork.Destroy(base.gameObject);
			return;
		}
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x06000335 RID: 821 RVA: 0x00016196 File Offset: 0x00014396
	[PunRPC]
	private void RPC_SpawnSkelly()
	{
		((GameObject)Object.Instantiate(Resources.Load("Skeleton"))).GetComponent<Skelleton>().SpawnSkelly(this.character);
	}

	// Token: 0x06000336 RID: 822 RVA: 0x000161BC File Offset: 0x000143BC
	public bool ReadyToDisable()
	{
		if (this.currentState == MushroomZombie.State.Dead && this.timeDiedAt + 10f < Time.time)
		{
			return true;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (Character character in Character.AllCharacters)
		{
			float num = Vector3.Distance(character.Center, this.character.Center);
			if (num <= 100f)
			{
				flag2 = true;
			}
			if (num <= this.distanceToEnable + 5f)
			{
				flag = true;
			}
		}
		return !flag2 || ((this.currentState == MushroomZombie.State.Dead || this.currentState == MushroomZombie.State.Sleeping) && !flag);
	}

	// Token: 0x040002C6 RID: 710
	public float reachForce;

	// Token: 0x040002C7 RID: 711
	public float targetForcedUntil;

	// Token: 0x040002C8 RID: 712
	public AnimatedMouth animatedMouth;

	// Token: 0x040002C9 RID: 713
	public Character _currentTarget;

	// Token: 0x040002CA RID: 714
	public float zombieSprintDistance = 20f;

	// Token: 0x040002CB RID: 715
	public float lookAngleBeforeWakeup = 30f;

	// Token: 0x040002CC RID: 716
	public float distanceBeforeWakeup = 30f;

	// Token: 0x040002CD RID: 717
	public float initialWakeUpTime = 5f;

	// Token: 0x040002CE RID: 718
	public float distanceBeforeChase = 30f;

	// Token: 0x040002CF RID: 719
	public float chaseTimeBeforeSprint = 3f;

	// Token: 0x040002D0 RID: 720
	public float zombieLungeDistance = 10f;

	// Token: 0x040002D1 RID: 721
	public float lungeTime = 1.5f;

	// Token: 0x040002D2 RID: 722
	public float lungeRecoveryTime = 5f;

	// Token: 0x040002D3 RID: 723
	public float biteStunTime = 3f;

	// Token: 0x040002D4 RID: 724
	public float biteInitialInjury;

	// Token: 0x040002D5 RID: 725
	public float biteInitialSpores;

	// Token: 0x040002D6 RID: 726
	public float biteDelayBeforeSpores;

	// Token: 0x040002D7 RID: 727
	public float biteSporesPerSecond;

	// Token: 0x040002D8 RID: 728
	public float totalBiteSporesTime;

	// Token: 0x040002D9 RID: 729
	public GameObject skirt;

	// Token: 0x040002DA RID: 730
	public GameObject shorts;

	// Token: 0x040002DB RID: 731
	public Texture zombieEyeTexture;

	// Token: 0x040002DC RID: 732
	public bool isNPCZombie = true;

	// Token: 0x040002DD RID: 733
	public float distanceToEnable;

	// Token: 0x040002DE RID: 734
	public List<GameObject> mushroomVisuals;

	// Token: 0x040002DF RID: 735
	public SFX_Instance gruntSFX;

	// Token: 0x040002E0 RID: 736
	public SFX_Instance knockoutSFX;

	// Token: 0x040002E1 RID: 737
	public SFX_Instance[] biteSFX;

	// Token: 0x040002E2 RID: 738
	public AudioSource gruntSource;

	// Token: 0x040002E3 RID: 739
	private Character spawnedFromCharacter;

	// Token: 0x040002E4 RID: 740
	public float lifetime = 120f;

	// Token: 0x040002E5 RID: 741
	private bool mushroomsGrowing = true;

	// Token: 0x040002E6 RID: 742
	public Vector2 minMaxMushroomGrowTime;

	// Token: 0x040002E7 RID: 743
	public GameObject biteColliderObject;

	// Token: 0x040002E8 RID: 744
	private bool wearingSkirt = true;

	// Token: 0x040002E9 RID: 745
	private float timeDiedAt;

	// Token: 0x040002EA RID: 746
	public MushroomZombieSpawner spawner;

	// Token: 0x040002EB RID: 747
	public MushroomZombie.State _currentState;

	// Token: 0x040002EC RID: 748
	public Character discovered;

	// Token: 0x040002ED RID: 749
	internal Character character;

	// Token: 0x040002EE RID: 750
	private PhotonView view;

	// Token: 0x040002EF RID: 751
	private float timeAwoke;

	// Token: 0x040002F0 RID: 752
	public Vector2 zombieGruntWaitTimeMinMax = new Vector2(10f, 20f);

	// Token: 0x040002F1 RID: 753
	private List<float> mushroomGrowTimes = new List<float>();

	// Token: 0x040002F2 RID: 754
	public float sinceLookForTarget = 99f;

	// Token: 0x040002F3 RID: 755
	public float distanceToTarget;

	// Token: 0x040002F4 RID: 756
	private float sinceSeenTarget;

	// Token: 0x040002F5 RID: 757
	private float achievementTestTick;

	// Token: 0x040002F6 RID: 758
	public bool visible;

	// Token: 0x040002F7 RID: 759
	private float idledFor;

	// Token: 0x040002F8 RID: 760
	private float timeSpentLunging;

	// Token: 0x040002F9 RID: 761
	private float timeSpentRecoveringFromLunge;

	// Token: 0x040002FA RID: 762
	private float timeSpentWakingUp;

	// Token: 0x040002FB RID: 763
	private float timeSpentChasing;

	// Token: 0x040002FC RID: 764
	private float attackHeightDelta = 100f;

	// Token: 0x040002FD RID: 765
	private Vector3 lungeTargetForward;

	// Token: 0x02000407 RID: 1031
	public enum State
	{
		// Token: 0x04001773 RID: 6003
		Sleeping,
		// Token: 0x04001774 RID: 6004
		WakingUp,
		// Token: 0x04001775 RID: 6005
		Idle,
		// Token: 0x04001776 RID: 6006
		Chasing,
		// Token: 0x04001777 RID: 6007
		Lunging,
		// Token: 0x04001778 RID: 6008
		LungeRecovery,
		// Token: 0x04001779 RID: 6009
		Dead
	}
}
