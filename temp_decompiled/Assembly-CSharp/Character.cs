using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x0200000A RID: 10
[DefaultExecutionOrder(-100)]
public class Character : MonoBehaviourPun
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000041 RID: 65 RVA: 0x00002F3C File Offset: 0x0000113C
	public bool IsPlayerControlled
	{
		get
		{
			return !this.isBot && !this.isZombie && !this.isScoutmaster;
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000042 RID: 66 RVA: 0x00002F59 File Offset: 0x00001159
	public Player player
	{
		get
		{
			return PlayerHandler.GetPlayer(this.view.Owner);
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000043 RID: 67 RVA: 0x00002F6C File Offset: 0x0000116C
	public static Character observedCharacter
	{
		get
		{
			Character specCharacter = MainCameraMovement.specCharacter;
			if (specCharacter)
			{
				return specCharacter;
			}
			return Character.localCharacter;
		}
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000044 RID: 68 RVA: 0x00002F8E File Offset: 0x0000118E
	public bool inAirport
	{
		get
		{
			return this.refs.afflictions.m_inAirport;
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000045 RID: 69 RVA: 0x00002FA0 File Offset: 0x000011A0
	// (set) Token: 0x06000046 RID: 70 RVA: 0x00002FA8 File Offset: 0x000011A8
	public PlayerGhost Ghost { get; set; }

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000047 RID: 71 RVA: 0x00002FB1 File Offset: 0x000011B1
	public bool IsGhost
	{
		get
		{
			return this.Ghost != null;
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000048 RID: 72 RVA: 0x00002FC0 File Offset: 0x000011C0
	public bool IsRegisteredToPlayer
	{
		get
		{
			Character character;
			return this.IsPlayerControlled && PlayerHandler.TryGetCharacter(base.photonView.OwnerActorNr, out character) && character == this;
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000049 RID: 73 RVA: 0x00002FF2 File Offset: 0x000011F2
	public bool IsLocal
	{
		get
		{
			return this == Character.localCharacter;
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600004A RID: 74 RVA: 0x00002FFF File Offset: 0x000011FF
	public bool IsObserved
	{
		get
		{
			return this == Character.observedCharacter;
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x0600004B RID: 75 RVA: 0x0000300C File Offset: 0x0000120C
	public Vector3 VirtualCenter
	{
		get
		{
			if (this.data.dead)
			{
				return this.LastLivingPosition;
			}
			if (!this.warping)
			{
				return this.Center;
			}
			return this._lastWarpTarget;
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600004C RID: 76 RVA: 0x00003037 File Offset: 0x00001237
	public Vector3 Center
	{
		get
		{
			return this.GetBodypart(BodypartType.Torso).transform.position;
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600004D RID: 77 RVA: 0x0000304A File Offset: 0x0000124A
	public Vector3 Head
	{
		get
		{
			return this.GetBodypart(BodypartType.Head).transform.position;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x0600004E RID: 78 RVA: 0x0000305D File Offset: 0x0000125D
	public string characterName
	{
		get
		{
			if (!this.isBot)
			{
				return this.view.Owner.NickName;
			}
			return "Bot";
		}
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003080 File Offset: 0x00001280
	public static bool GetCharacterWithPhotonID(int photonID, out Character characterResult)
	{
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i] != null && Character.AllCharacters[i].photonView.ViewID == photonID)
			{
				characterResult = Character.AllCharacters[i];
				return true;
			}
		}
		characterResult = null;
		return false;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000030E0 File Offset: 0x000012E0
	private void OnDestroy()
	{
		Character.AllCharacters.Remove(this);
		Character.AllBotCharacters.Remove(this);
	}

	// Token: 0x06000051 RID: 81 RVA: 0x000030FC File Offset: 0x000012FC
	private void Awake()
	{
		if (!this.isBot)
		{
			Character.AllCharacters.Add(this);
		}
		else
		{
			Character.AllBotCharacters.Add(this);
		}
		this.view = base.GetComponent<PhotonView>();
		if (this.view != null)
		{
			if (!this.isBot)
			{
				PlayerHandler.RegisterCharacter(this);
				if (this.view.IsMine)
				{
					Character.localCharacter = this;
					VoiceClientHandler.LocalPlayerAssigned(base.GetComponentInChildren<Recorder>());
				}
				base.gameObject.name = string.Format("Character [{0} : {1}]", this.view.Owner.NickName, this.view.Owner.ActorNumber);
			}
			else
			{
				base.gameObject.name = "Bot";
			}
		}
		this.refs.animatedVariables = base.GetComponentInChildren<AnimatedVariables>();
		this.refs.movement = base.GetComponent<CharacterMovement>();
		this.refs.carriying = base.GetComponent<CharacterCarrying>();
		this.refs.ragdoll = base.GetComponent<CharacterRagdoll>();
		this.refs.balloons = base.GetComponent<CharacterBalloons>();
		this.refs.ropeHandling = base.GetComponent<CharacterRopeHandling>();
		this.refs.rigCreator = base.GetComponentInChildren<RigCreator>();
		this.refs.animations = base.GetComponentInChildren<CharacterAnimations>();
		this.refs.animator = this.refs.rigCreator.GetComponent<Animator>();
		this.refs.items = base.GetComponent<CharacterItems>();
		this.refs.climbing = base.GetComponent<CharacterClimbing>();
		this.refs.afflictions = base.GetComponent<CharacterAfflictions>();
		this.refs.view = base.GetComponent<PhotonView>();
		this.refs.heatEmission = base.GetComponentInChildren<CharacterHeatEmission>();
		this.refs.vineClimbing = base.GetComponentInChildren<CharacterVineClimbing>();
		this.refs.interactible = base.GetComponent<CharacterInteractible>();
		this.refs.customization = base.GetComponentInChildren<CharacterCustomization>();
		this.refs.stats = base.GetComponentInChildren<CharacterStats>();
		this.refs.grabbing = base.GetComponent<CharacterGrabbing>();
		this.refs.hideTheBody = base.GetComponentInChildren<HideTheBody>();
		this.refs.badgeUnlocker = base.GetComponent<BadgeUnlocker>();
		this.jumpAction = (Action)Delegate.Combine(this.jumpAction, new Action(this.JumpStickEffect));
		this.refs.ikRigBuilder = this.refs.rigCreator.GetComponent<RigBuilder>();
		if (this.refs.ikRigBuilder)
		{
			this.refs.ikRig = this.refs.rigCreator.GetComponentInChildren<Rig>();
			this.refs.IKHandTargetLeft = this.refs.ikRig.transform.Find("IK_Arm_Left/Target");
			this.refs.IKHandTargetRight = this.refs.ikRig.transform.Find("IK_Arm_Right/Target");
			if (this.refs.IKHandTargetLeft)
			{
				this.refs.ikLeft = this.refs.IKHandTargetLeft.transform.parent.GetComponent<TwoBoneIKConstraint>();
				this.refs.ikRight = this.refs.IKHandTargetRight.transform.parent.GetComponent<TwoBoneIKConstraint>();
			}
		}
		this.CreateHelperObjects();
		this.input.Init();
	}

	// Token: 0x06000052 RID: 82 RVA: 0x0000344E File Offset: 0x0000164E
	public void BreakCharacterCarrying(bool sendRPC = false)
	{
		if (this.data.isCarried)
		{
			this.data.carrier.DropCarriedCharacter(sendRPC);
			return;
		}
		if (this.data.IsCarryingCharacter)
		{
			this.DropCarriedCharacter(sendRPC);
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00003484 File Offset: 0x00001684
	private void DropCarriedCharacter(bool sendRPC)
	{
		if (!this.data.IsCarryingCharacter)
		{
			Debug.LogWarning("Called DropCarriedCharacter but we're not carrying anyone. Doing nothing.");
			return;
		}
		if (sendRPC)
		{
			this.refs.carriying.Drop(this.data.carriedPlayer);
			return;
		}
		this.refs.carriying.RPCA_Drop(this.data.carriedPlayer.photonView);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x000034E8 File Offset: 0x000016E8
	internal void SetDeadAfterReconnect(Vector3 lastLivingPosition)
	{
		this.LastLivingPosition = lastLivingPosition;
		base.photonView.RPC("RPCA_SetDead", 0, Array.Empty<object>());
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000055 RID: 85 RVA: 0x00003507 File Offset: 0x00001707
	// (set) Token: 0x06000056 RID: 86 RVA: 0x0000350F File Offset: 0x0000170F
	public Vector3 LastLivingPosition { get; private set; }

	// Token: 0x06000057 RID: 87 RVA: 0x00003518 File Offset: 0x00001718
	public Vector3 GetSpectatePosition()
	{
		if (this.data.dead)
		{
			return this.LastLivingPosition;
		}
		return this.Center;
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00003534 File Offset: 0x00001734
	internal void AddForceAtPosition(Vector3 force, Vector3 point, float radius)
	{
		this.view.RPC("RPCA_AddForceAtPosition", 0, new object[] { force, point, radius });
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003568 File Offset: 0x00001768
	[PunRPC]
	public void RPCA_AddForceAtPosition(Vector3 force, Vector3 point, float radius)
	{
		foreach (Bodypart bodypart in this.refs.ragdoll.partList)
		{
			float num = Vector3.Distance(bodypart.Rig.worldCenterOfMass, point);
			float num2 = Mathf.InverseLerp(radius, radius * 0.1f, num);
			Rigidbody rig = bodypart.Rig;
			Vector3 vector = Vector3.Lerp(point, rig.worldCenterOfMass, 0.5f);
			rig.AddForceAtPosition(force * num2, vector, 1);
		}
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00003608 File Offset: 0x00001808
	[ConsoleCommand]
	public static void GainFullStamina()
	{
		Character.localCharacter.AddStamina(1f);
	}

	// Token: 0x0600005B RID: 91 RVA: 0x0000361C File Offset: 0x0000181C
	private void CreateHelperObjects()
	{
		this.refs.helperObjects = new GameObject("helperObjects").transform;
		this.refs.helperObjects.transform.SetParent(base.transform);
		this.refs.helperObjects.transform.localPosition = Vector3.zero;
		this.refs.helperObjects.transform.localRotation = Quaternion.identity;
		this.refs.animationHeadTransform = Object.Instantiate<GameObject>(this.refs.helperObjects.gameObject, this.refs.helperObjects).transform;
		this.refs.animationHeadTransform.gameObject.name = "animationHead";
		this.refs.animationHipTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationHipTransform.gameObject.name = "animationHip";
		this.refs.animationItemTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationItemTransform.gameObject.name = "animationItem";
		this.refs.animationLookTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationLookTransform.gameObject.name = "animationLook";
		this.refs.animationPositionTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationPositionTransform.gameObject.name = "animationPosition";
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00003804 File Offset: 0x00001A04
	public void Start()
	{
		if (this.started)
		{
			return;
		}
		this.started = true;
		this.refs.hip = this.GetBodypart(BodypartType.Hip);
		this.refs.head = this.GetBodypart(BodypartType.Head);
		base.gameObject.name = string.Format("Character [{0} : {1}]", this.view.Owner.NickName, this.view.Owner.ActorNumber);
		CharacterAfflictions afflictions = this.refs.afflictions;
		afflictions.OnAddedIncrementalStatus = (Action<CharacterAfflictions.STATUSTYPE, float>)Delegate.Combine(afflictions.OnAddedIncrementalStatus, new Action<CharacterAfflictions.STATUSTYPE, float>(this.OnAddedStatus));
		this.smoothedCamPos = this.GetBodypart(BodypartType.Head).transform.TransformPoint(Vector3.up * 1f);
	}

	// Token: 0x0600005D RID: 93 RVA: 0x000038D1 File Offset: 0x00001AD1
	private void OnAddedStatus(CharacterAfflictions.STATUSTYPE sTATUSTYPE, float amount)
	{
		if (sTATUSTYPE == CharacterAfflictions.STATUSTYPE.Cold && amount > 0f)
		{
			this.data.sinceAddedCold = 0f;
		}
	}

	// Token: 0x0600005E RID: 94 RVA: 0x000038F0 File Offset: 0x00001AF0
	private void Update()
	{
		this.HandleStickUpdate();
		this.UpdateVariables();
		if (this.data.dead)
		{
			this.data.sinceDied += Time.deltaTime;
		}
		else
		{
			this.data.sinceDied = 0f;
		}
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (this.data.dead)
		{
			this.HandleDeath();
			return;
		}
		if (this.data.passedOut || this.data.fullyPassedOut)
		{
			this.HandlePassedOut();
			return;
		}
		this.HandleLife();
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00003988 File Offset: 0x00001B88
	private void UpdateVariables()
	{
		this.data.ragdollControlClamp = Mathf.MoveTowards(this.data.ragdollControlClamp, 1f, Time.deltaTime * 5f);
		this.data.sinceUnstuck += Time.deltaTime;
	}

	// Token: 0x06000060 RID: 96 RVA: 0x000039D7 File Offset: 0x00001BD7
	public static Vector3 DeathPos()
	{
		return new Vector3(0f, 5000f, -5000f);
	}

	// Token: 0x06000061 RID: 97 RVA: 0x000039ED File Offset: 0x00001BED
	private void HandleDeath()
	{
		this.data.sinceDied += Time.deltaTime;
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00003A08 File Offset: 0x00001C08
	private void HandlePassedOut()
	{
		if (this.refs.afflictions.statusSum < 1f && Time.time - this.data.lastPassedOut > 3f)
		{
			if (!this.UnPassOutCalled)
			{
				this.view.RPC("RPCA_UnPassOut", 0, Array.Empty<object>());
				this.passOutFailsafeTick = 0f;
			}
			else
			{
				this.passOutFailsafeTick += Time.deltaTime;
				if (this.passOutFailsafeTick > 3f)
				{
					Debug.Log("Passed out failsafe triggered.");
					this.UnPassOutCalled = false;
				}
			}
		}
		this.ZombieFailsafe();
		if (this.data.deathTimer > 1f)
		{
			this.refs.items.EquipSlot(Optionable<byte>.None);
			if (!this.TryCheckpoint())
			{
				if (this.refs.afflictions.willZombify && !this.data.zombified)
				{
					if (!PhotonNetwork.IsMasterClient)
					{
						this.data.zombified = true;
					}
					this.view.RPC("RPCA_Zombify", 2, new object[] { this.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f });
					return;
				}
				this.view.RPC("RPCA_Die", 0, new object[] { this.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f });
			}
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00003BA7 File Offset: 0x00001DA7
	private void ZombieFailsafe()
	{
		if (this.data.zombified && !this.data.dead && Time.time > this.lastZombified + 5f)
		{
			this.data.zombified = false;
		}
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003BE4 File Offset: 0x00001DE4
	private bool TryCheckpoint()
	{
		if (this.data.checkpointFlags.Count == 0)
		{
			return false;
		}
		for (int i = this.data.checkpointFlags.Count - 1; i >= 0; i--)
		{
			if (this.data.checkpointFlags[i])
			{
				CheckpointFlag checkpointFlag = this.data.checkpointFlags[i];
				this.data.checkpointFlags.Remove(checkpointFlag);
				this.data.deathTimer = 0f;
				this.refs.afflictions.ClearAllStatus(false);
				this.refs.afflictions.ApplyStatusesFromFloatArray(checkpointFlag.currentStatuses);
				this.refs.afflictions.RemoveAllThorns();
				this.data.fallSeconds = 0f;
				this.data.currentRagdollControll = 0f;
				Character.localCharacter.view.RPC("RegainItems", 2, new object[] { Character.localCharacter.view });
				this.WarpPlayer(checkpointFlag.transform.position + Vector3.up, true);
				checkpointFlag.DestroySelf();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00003D1C File Offset: 0x00001F1C
	[PunRPC]
	private void RegainItems(PhotonView regainingCharacterView)
	{
		Character component = regainingCharacterView.GetComponent<Character>();
		if (!component)
		{
			return;
		}
		for (int i = 0; i < component.refs.items.droppedItems.Count; i++)
		{
			if (component.refs.items.droppedItems[i])
			{
				Item component2 = component.refs.items.droppedItems[i].GetComponent<Item>();
				if (component2)
				{
					component.refs.items.lastEquippedSlotTime = 0f;
					component2.Interact(component);
				}
			}
		}
		component.refs.items.droppedItems.Clear();
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003DCC File Offset: 0x00001FCC
	[ConsoleCommand]
	public static void Die()
	{
		Character.localCharacter.refs.items.EquipSlot(Optionable<byte>.None);
		Debug.Log("DYING");
		Character.localCharacter.view.RPC("RPCA_Die", 0, new object[] { Character.localCharacter.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f });
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00003E51 File Offset: 0x00002051
	internal void DieInstantly()
	{
		if (!this.TryCheckpoint())
		{
			this.view.RPC("RPCA_Die", 0, new object[] { this.Center });
		}
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00003E80 File Offset: 0x00002080
	[PunRPC]
	public void RPCA_SetDead()
	{
		this.data.dead = true;
		this.data.fullyPassedOut = true;
		this.data.deathTimer = 1f;
		this.data.passedOut = true;
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00003EB8 File Offset: 0x000020B8
	[PunRPC]
	public void RPCA_Die(Vector3 itemSpawnPoint)
	{
		this.refs.items.EquipSlot(Optionable<byte>.None);
		this.RPCA_SetDead();
		this.refs.stats.justDied = true;
		this.refs.stats.Record(false, 0f);
		ItemSlot[] itemSlots = this.player.itemSlots;
		this.refs.items.DropAllItems(true);
		if (this.IsLocal)
		{
			this.SetExtraStamina(0f);
		}
		Debug.Log(base.gameObject.name + " died");
		if (this.data.isSkeleton)
		{
			((GameObject)Object.Instantiate(Resources.Load("SkeletonExplosion"))).GetComponent<SkeletonExplosion>().Boom(this);
			if (this.IsLocal)
			{
				this.data.SetSkeleton(false);
			}
		}
		else
		{
			((GameObject)Object.Instantiate(Resources.Load("Skeleton"))).GetComponent<Skelleton>().SpawnSkelly(this);
		}
		this.WarpPlayer(Character.DeathPos(), false);
		this.CheckEndGame();
		Debug.Log("DIE");
		GlobalEvents.TriggerCharacterDied(this);
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00003FD4 File Offset: 0x000021D4
	[ConsoleCommand]
	public static void Zombify()
	{
		Character.localCharacter.view.RPC("RPCA_Zombify", 0, new object[] { Character.localCharacter.Center });
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00004004 File Offset: 0x00002204
	[ConsoleCommand]
	public static void TestWarp()
	{
		foreach (Bodypart bodypart in Character.localCharacter.transform.GetComponentsInChildren<Bodypart>())
		{
			Debug.Log("Warping body part " + bodypart.partType.ToString());
			bodypart.transform.position = bodypart.transform.position + Vector3.up * 50f;
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00004080 File Offset: 0x00002280
	[PunRPC]
	public void RPCA_Zombify(Vector3 itemSpawnPoint)
	{
		Debug.Log(base.gameObject.name + " became a zombie");
		if (!this.data.zombified)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Instantiate("MushroomZombie_Player", this.Center, base.transform.rotation, 0, null).GetComponent<PhotonView>().RPC("RPC_Arise", 0, new object[] { base.photonView.ViewID });
			}
			this.data.zombified = true;
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00004110 File Offset: 0x00002310
	public void FinishZombifying()
	{
		this.refs.items.EquipSlot(Optionable<byte>.None);
		this.data.dead = true;
		this.data.zombified = true;
		this.data.fullyPassedOut = true;
		this.data.deathTimer = 1f;
		this.data.passedOut = true;
		this.refs.stats.justDied = true;
		this.refs.stats.Record(false, 0f);
		if (this.IsLocal)
		{
			this.SetExtraStamina(0f);
		}
		ItemSlot[] itemSlots = this.player.itemSlots;
		this.refs.items.DropAllItems(true);
		this.WarpPlayer(Character.DeathPos(), false);
		GlobalEvents.TriggerCharacterDied(this);
		this.CheckEndGame();
	}

	// Token: 0x0600006E RID: 110 RVA: 0x000041E4 File Offset: 0x000023E4
	public void CheckEndGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			bool flag = true;
			for (int i = 0; i < Character.AllCharacters.Count; i++)
			{
				if (!Character.AllCharacters[i].data.dead)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.EndGame();
			}
		}
	}

	// Token: 0x0600006F RID: 111 RVA: 0x00004231 File Offset: 0x00002431
	[ConsoleCommand]
	internal static void TestWin()
	{
		Character.localCharacter.photonView.RPC("RPCEndGame_ForceWin", 0, Array.Empty<object>());
	}

	// Token: 0x06000070 RID: 112 RVA: 0x0000424D File Offset: 0x0000244D
	internal void EndGame()
	{
		base.photonView.RPC("RPCEndGame", 0, Array.Empty<object>());
		RunManager.Instance.EndGame();
	}

	// Token: 0x06000071 RID: 113 RVA: 0x0000426F File Offset: 0x0000246F
	[PunRPC]
	private void RPCEndGame_ForceWin()
	{
		Character.forceWin = true;
		this.RPCEndGame();
		Character.forceWin = false;
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00004284 File Offset: 0x00002484
	[PunRPC]
	private void RPCEndGame()
	{
		bool flag = false;
		foreach (Character character in Character.AllCharacters)
		{
			if (Character.CheckWinCondition(character))
			{
				character.refs.stats.Win();
				flag = true;
			}
		}
		foreach (Character character2 in Character.AllCharacters)
		{
			if (!Character.CheckWinCondition(character2))
			{
				character2.refs.stats.Lose(flag);
			}
		}
		MenuWindow.CloseAllWindows();
		if (flag)
		{
			GlobalEvents.TriggerSomeoneWonRun();
			Singleton<PeakHandler>.Instance.EndCutscene();
		}
		else
		{
			GUIManager.instance.endScreen.Open();
		}
		GlobalEvents.TriggerRunEnded();
	}

	// Token: 0x06000073 RID: 115 RVA: 0x0000436C File Offset: 0x0000256C
	public static bool CheckWinCondition(Character c)
	{
		return Character.forceWin || (((c.data.isRopeClimbing && c.data.heldRope.isHelicopterRope) || Singleton<MountainProgressHandler>.Instance.IsAtPeak(c.Center)) && !c.data.dead);
	}

	// Token: 0x06000074 RID: 116 RVA: 0x000043C4 File Offset: 0x000025C4
	[PunRPC]
	private void RPCA_UnPassOut()
	{
		this.UnPassOutCalled = true;
		this.data.deathTimer = 0f;
		if (this.IsLocal)
		{
			Transitions.instance.PlayTransition(TransitionType.FadeToBlack, new Action(this.UnPassOutDone), 1f, 1f);
			return;
		}
		this.UnPassOutDone();
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00004418 File Offset: 0x00002618
	private void UnPassOutDone()
	{
		Debug.Log("UhPassOut");
		Action unPassOutAction = this.UnPassOutAction;
		if (unPassOutAction != null)
		{
			unPassOutAction.Invoke();
		}
		this.data.fullyPassedOut = false;
		this.data.passedOut = false;
	}

	// Token: 0x06000076 RID: 118 RVA: 0x0000444D File Offset: 0x0000264D
	[ConsoleCommand]
	public static void PassOut()
	{
		CharacterAfflictions.Starve();
		Character.localCharacter.view.RPC("RPCA_PassOut", 0, Array.Empty<object>());
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00004470 File Offset: 0x00002670
	[PunRPC]
	public void RPCA_PassOut()
	{
		this.UnPassOutCalled = false;
		this.data.passedOut = true;
		this.refs.stats.justPassedOut = true;
		this.data.lastPassedOut = Time.time;
		this.refs.stats.Record(false, 0f);
		if (this.IsLocal)
		{
			GUIManager.instance.strugglePrompt.gameObject.SetActive(false);
		}
		GlobalEvents.TriggerCharacterPassedOut(this);
		if (PhotonNetwork.IsMasterClient)
		{
			this.refs.items.droppedItems.Clear();
		}
		if (this.IsLocal)
		{
			Transitions.instance.PlayTransition(TransitionType.FadeToBlack, new Action(this.<RPCA_PassOut>g__PassOutDone|90_0), 1f, 1f);
		}
		else
		{
			this.<RPCA_PassOut>g__PassOutDone|90_0();
		}
		Debug.Log("PASS OUT");
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00004544 File Offset: 0x00002744
	private void HandleLife()
	{
		if (this.refs.afflictions.shouldPassOut)
		{
			if (this.data.isSkeleton)
			{
				if (!this.TryCheckpoint())
				{
					this.view.RPC("RPCA_Die", 0, new object[] { Character.localCharacter.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f });
				}
				return;
			}
			this.data.passOutValue = Mathf.MoveTowards(this.data.passOutValue, 1f, Time.deltaTime / 5f);
			if (this.data.passOutValue > 0.999f)
			{
				this.view.RPC("RPCA_PassOut", 0, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.data.passOutValue = Mathf.MoveTowards(this.data.passOutValue, 0f, Time.deltaTime / 5f);
		}
	}

	// Token: 0x06000079 RID: 121 RVA: 0x0000464C File Offset: 0x0000284C
	public void PassOutInstantly()
	{
		this.data.passOutValue = 1f;
		this.view.RPC("RPCA_PassOut", 0, Array.Empty<object>());
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00004674 File Offset: 0x00002874
	private void FixedUpdate()
	{
		this.UpdateVariablesFixed();
		if (this.data.dead)
		{
			this.refs.ragdoll.MoveAllRigsInDirection(Character.DeathPos() - this.Center);
			this.refs.ragdoll.HaltBodyVelocity();
			return;
		}
		this.LastLivingPosition = this.Center;
	}

	// Token: 0x0600007B RID: 123 RVA: 0x000046D4 File Offset: 0x000028D4
	private void UpdateVariablesFixed()
	{
		float targetRagdollControll = this.data.GetTargetRagdollControll();
		if (targetRagdollControll < this.data.currentRagdollControll)
		{
			this.data.currentRagdollControll = targetRagdollControll;
		}
		else if (this.data.currentRagdollControll > 0.5f)
		{
			this.data.currentRagdollControll = Mathf.MoveTowards(this.data.currentRagdollControll, targetRagdollControll, Time.fixedDeltaTime * 1f);
		}
		else
		{
			this.data.currentRagdollControll = Mathf.MoveTowards(this.data.currentRagdollControll, targetRagdollControll, Time.fixedDeltaTime * 0.5f);
		}
		this.data.staminaDelta = this.data.currentStamina + this.data.extraStamina - this.data.lastFrameTotalStamina;
		this.data.lastFrameTotalStamina = this.data.currentStamina + this.data.extraStamina;
		if (this.data.isGrounded)
		{
			this.data.groundedFor += Time.fixedDeltaTime;
			this.data.sinceGrounded = 0f;
			this.data.lastGroundedHeight = this.Center.y;
		}
		else
		{
			this.data.groundedFor = 0f;
			if (this.data.sinceGrounded < 1f || this.data.avarageVelocity.y < -1f)
			{
				this.data.sinceGrounded += Time.fixedDeltaTime;
			}
		}
		if (this.data.isClimbing || this.data.isRopeClimbing || this.data.isVineClimbing)
		{
			this.data.sinceClimb = 0f;
		}
		if (this.data.dead)
		{
			this.data.sinceDead = 0f;
		}
		if (this.OutOfStamina())
		{
			this.data.outOfStaminaFor += Time.fixedDeltaTime;
		}
		else
		{
			this.data.outOfStaminaFor = 0f;
		}
		this.data.staminaMod = Mathf.Max(Mathf.Clamp01(this.GetTotalStamina() * 5f), 0.2f);
		this.data.sinceClimbJump += Time.fixedDeltaTime;
		if (this.data.fallSeconds > 0f)
		{
			if (this.data.isGrounded)
			{
				this.data.fallSeconds -= Time.fixedDeltaTime;
			}
			else
			{
				this.data.fallSeconds -= Time.fixedDeltaTime * 0.2f;
			}
			if (this.data.fallSeconds <= 0f)
			{
				this.StoppedForcedRagdolling();
			}
		}
		if (this.data.fullyPassedOut)
		{
			if (this.input.interactIsPressed)
			{
				this.data.deathTimer += Time.fixedDeltaTime * 0.33f;
			}
			else if (!this.data.carrier || this.refs.afflictions.willZombify)
			{
				if (!this.HasMeaningfulTempStatuses() && this.NobodyIsAlive())
				{
					this.data.deathTimer += Time.fixedDeltaTime / 10f;
				}
				else
				{
					this.data.deathTimer += Time.fixedDeltaTime / 60f;
				}
			}
		}
		else
		{
			this.data.sinceDied = 0f;
		}
		if (this.input.usePrimaryIsPressed && this.data.currentItem == null)
		{
			this.data.sincePressClimb = 0f;
		}
		if (this.input.useSecondaryIsPressed && this.data.currentItem == null)
		{
			this.data.sincePressReach = 0f;
		}
		this.data.sincePressClimb += Time.fixedDeltaTime;
		this.data.sincePressReach += Time.fixedDeltaTime;
		this.data.sinceAddedCold += Time.fixedDeltaTime;
		this.data.sinceStartClimb += Time.fixedDeltaTime;
		this.data.sinceGrabFriend += Time.fixedDeltaTime;
		this.data.sinceClimbHandle += Time.fixedDeltaTime;
		this.data.sinceFallSlide += Time.fixedDeltaTime;
		this.data.sinceUseStamina += Time.fixedDeltaTime;
		this.data.sinceClimb += Time.fixedDeltaTime;
		this.data.sinceJump += Time.fixedDeltaTime;
		this.data.sinceDead += Time.fixedDeltaTime;
		this.data.overrideIKForSeconds -= Time.fixedDeltaTime;
		this.data.slippy -= Time.deltaTime;
		this.data.sinceLetGoOfFriend += Time.fixedDeltaTime;
		this.data.sinceStandOnPlayer += Time.fixedDeltaTime;
		this.data.sincePalJump += Time.fixedDeltaTime;
		this.data.sinceItemAttach += Time.fixedDeltaTime;
		this.data.sinceCanClimb += Time.fixedDeltaTime;
		this.data.passedOutOnTheBeach -= Time.fixedDeltaTime;
		if (this.CanRegenStamina())
		{
			this.AddStamina(Time.fixedDeltaTime * 0.2f);
		}
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00004C78 File Offset: 0x00002E78
	private bool NobodyIsAlive()
	{
		List<Character> allCharacters = Character.AllCharacters;
		for (int i = 0; i < allCharacters.Count; i++)
		{
			if (allCharacters[i].data.fullyConscious)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00004CB4 File Offset: 0x00002EB4
	private bool HasMeaningfulTempStatuses()
	{
		float num = this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Thorns);
		if (!this.data.isInFog)
		{
			num += this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold);
		}
		return this.refs.afflictions.statusSum - num < 1f;
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00004D48 File Offset: 0x00002F48
	private bool CanRegenStamina()
	{
		if (this.data.currentClimbHandle)
		{
			return true;
		}
		if (this.IsStuck())
		{
			return true;
		}
		float num = ((this.data.currentStamina > 0f) ? 1f : 2f);
		return (this.data.sinceGrounded <= 0.2f || this.refs.afflictions.isWebbed) && this.data.sinceUseStamina >= num;
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00004DC7 File Offset: 0x00002FC7
	public float GetTotalStamina()
	{
		return this.data.currentStamina + this.data.extraStamina;
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00004DE0 File Offset: 0x00002FE0
	internal Bodypart GetBodypart(BodypartType head)
	{
		return this.refs.ragdoll.partDict[head];
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00004DF8 File Offset: 0x00002FF8
	internal Rigidbody GetBodypartRig(BodypartType head)
	{
		return this.refs.ragdoll.partDict[head].Rig;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00004E18 File Offset: 0x00003018
	internal void CalculateWorldMovementDir()
	{
		Vector3 vector = default(Vector3) + this.data.lookDirection * this.input.movementInput.y;
		vector.y = 0f;
		vector = vector.normalized;
		vector += this.data.lookDirection_Right * this.input.movementInput.x;
		this.data.worldMovementInput = vector.normalized;
		Vector3 lookDirection = this.data.lookDirection;
		Vector3 lookDirection_Right = this.data.lookDirection_Right;
		lookDirection.y = 0f;
		lookDirection.Normalize();
		Vector3 vector2 = this.data.groundNormal;
		if (this.data.sinceGrounded > 0.2f)
		{
			vector2 = Vector3.up;
		}
		Vector3 vector3 = HelperFunctions.GroundDirection(vector2, -lookDirection_Right);
		Vector3 vector4 = HelperFunctions.GroundDirection(vector2, lookDirection);
		if (this.data.sinceGrounded < 0.2f)
		{
			this.data.groundedForward = vector3;
			this.data.groundedRight = vector4;
		}
		Vector3 vector5 = vector3 * this.input.movementInput.y + vector4 * this.input.movementInput.x;
		vector5 = Vector3.ClampMagnitude(vector5, 1f);
		this.data.worldMovementInput_Grounded = vector5;
		Vector3 vector6 = this.data.worldMovementInput_Grounded;
		float num = Mathf.Lerp(this.refs.movement.movementTurnSpeed, this.refs.movement.airMovementTurnSpeed, this.data.sinceGrounded * 4f);
		if (!this.data.isGrounded)
		{
			vector6 = this.data.worldMovementInput;
		}
		this.data.worldMovementInput_Lerp = Vector3.MoveTowards(this.data.worldMovementInput_Lerp, vector6, Time.deltaTime * num);
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00005008 File Offset: 0x00003208
	internal void RecalculateLookDirections()
	{
		Vector3 normalized = HelperFunctions.LookToDirection(this.data.lookValues, Vector3.forward).normalized;
		this.data.lookDirection = normalized;
		normalized.y = 0f;
		normalized.Normalize();
		this.data.lookDirection_Flat = normalized;
		this.data.lookDirection_Right = Vector3.Cross(Vector3.up, this.data.lookDirection).normalized;
		this.data.lookDirection_Up = Vector3.Cross(this.data.lookDirection, this.data.lookDirection_Right).normalized;
	}

	// Token: 0x06000084 RID: 132 RVA: 0x000050B9 File Offset: 0x000032B9
	internal Vector3 GetCameraPos(float forwardOffset)
	{
		return this.GetBodypart(BodypartType.Head).transform.TransformPoint(Vector3.up * 1f + Vector3.forward * forwardOffset);
	}

	// Token: 0x06000085 RID: 133 RVA: 0x000050EC File Offset: 0x000032EC
	internal Vector3 GetAnimationRelativePosition(Vector3 position)
	{
		Vector3 vector = position - this.refs.animationHipTransform.position;
		return this.refs.hip.Rig.position + vector;
	}

	// Token: 0x06000086 RID: 134 RVA: 0x0000512B File Offset: 0x0000332B
	internal void OnLand(float sinceGrounded)
	{
		Action<float> action = this.landAction;
		if (action == null)
		{
			return;
		}
		action.Invoke(sinceGrounded);
	}

	// Token: 0x06000087 RID: 135 RVA: 0x0000513E File Offset: 0x0000333E
	internal void OnStartJump()
	{
		Action action = this.startJumpAction;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x06000088 RID: 136 RVA: 0x00005150 File Offset: 0x00003350
	internal void OnJump()
	{
		Action action = this.jumpAction;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00005162 File Offset: 0x00003362
	internal void OnStartClimb()
	{
		Action action = this.startClimbAction;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00005174 File Offset: 0x00003374
	internal Vector3 HipPos()
	{
		return this.GetBodypart(BodypartType.Hip).Rig.position;
	}

	// Token: 0x0600008B RID: 139 RVA: 0x00005187 File Offset: 0x00003387
	internal Vector3 TorsoPos()
	{
		return this.GetBodypart(BodypartType.Torso).Rig.position;
	}

	// Token: 0x0600008C RID: 140 RVA: 0x0000519C File Offset: 0x0000339C
	internal void AddForce(Vector3 move, float minRandomMultiplier = 1f, float maxRandomMultiplier = 1f)
	{
		foreach (Bodypart bodypart in this.refs.ragdoll.partList)
		{
			Vector3 vector = move;
			if (minRandomMultiplier != maxRandomMultiplier)
			{
				vector *= Random.Range(minRandomMultiplier, maxRandomMultiplier);
			}
			bodypart.AddForce(vector, 5);
		}
	}

	// Token: 0x0600008D RID: 141 RVA: 0x0000520C File Offset: 0x0000340C
	internal bool CheckStand()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing;
	}

	// Token: 0x0600008E RID: 142 RVA: 0x0000523C File Offset: 0x0000343C
	internal bool CheckGravity()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null);
	}

	// Token: 0x0600008F RID: 143 RVA: 0x0000528C File Offset: 0x0000348C
	internal bool CheckMovement()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null);
	}

	// Token: 0x06000090 RID: 144 RVA: 0x000052DC File Offset: 0x000034DC
	internal bool CheckJump()
	{
		return !this.data.fullyPassedOut && !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null);
	}

	// Token: 0x06000091 RID: 145 RVA: 0x0000533C File Offset: 0x0000353C
	internal bool CheckSprint()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null) && this.data.fullyConscious && (!this.data.currentItem || (!this.data.currentItem.isUsingPrimary && !this.data.currentItem.isUsingSecondary));
	}

	// Token: 0x06000092 RID: 146 RVA: 0x000053D4 File Offset: 0x000035D4
	internal void SetRotation()
	{
		if (this.data.carrier)
		{
			this.refs.rigCreator.transform.rotation = this.data.carrier.refs.carryPosRef.rotation;
			return;
		}
		if (this.data.isRopeClimbing)
		{
			this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(-this.data.ropeClimbWorldNormal, this.data.ropeClimbWorldUp);
			return;
		}
		if (this.data.isClimbing)
		{
			this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(-this.data.climbNormal);
			return;
		}
		if (this.data.lookDirection_Flat != Vector3.zero)
		{
			this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(this.data.lookDirection_Flat);
		}
	}

	// Token: 0x06000093 RID: 147 RVA: 0x000054DC File Offset: 0x000036DC
	internal bool UseStamina(float usage, bool useBonusStamina = true)
	{
		if (usage == 0f)
		{
			return false;
		}
		usage *= Ascents.climbStaminaMultiplier;
		if (!this.view.IsMine)
		{
			return this.data.currentStamina + this.data.extraStamina > usage;
		}
		if (this.data.currentStamina == 0f)
		{
			if (this.data.extraStamina > 0f && useBonusStamina)
			{
				this.data.extraStamina -= usage;
				this.data.extraStamina = Mathf.Clamp(this.data.extraStamina, 0f, 1f);
				this.data.sinceUseStamina = 0f;
				GUIManager.instance.bar.ChangeBar();
				return true;
			}
			return false;
		}
		else
		{
			this.data.currentStamina -= usage;
			this.data.sinceUseStamina = 0f;
			GUIManager.instance.bar.ChangeBar();
			if (this.data.currentStamina <= 0f)
			{
				this.ClampStamina();
				return this.data.extraStamina > 0f;
			}
			return true;
		}
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00005608 File Offset: 0x00003808
	[PunRPC]
	public void MoraleBoost(float staminaAdd, int scoutCount)
	{
		GUIManager.instance.bar.PlayMoraleBoost(scoutCount);
		this.AddExtraStamina(staminaAdd);
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00005621 File Offset: 0x00003821
	public void AddStamina(float add)
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.data.currentStamina += add;
		this.ClampStamina();
		GUIManager.instance.bar.ChangeBar();
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00005659 File Offset: 0x00003859
	public void ClampStamina()
	{
		this.data.currentStamina = Mathf.Clamp(this.data.currentStamina, 0f, this.GetMaxStamina());
	}

	// Token: 0x06000097 RID: 151 RVA: 0x00005681 File Offset: 0x00003881
	public float GetMaxStamina()
	{
		return Mathf.Max(1f - this.refs.afflictions.statusSum, 0f);
	}

	// Token: 0x06000098 RID: 152 RVA: 0x000056A3 File Offset: 0x000038A3
	public void SetExtraStamina(float amt)
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.data.extraStamina = Mathf.Clamp(amt, 0f, 1f);
		GUIManager.instance.bar.ChangeBar();
	}

	// Token: 0x06000099 RID: 153 RVA: 0x000056E0 File Offset: 0x000038E0
	public void AddExtraStamina(float add)
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.data.extraStamina += add;
		this.data.extraStamina = Mathf.Clamp(this.data.extraStamina, 0f, 1f);
		GUIManager.instance.bar.ChangeBar();
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00005742 File Offset: 0x00003942
	public void FeedItem(Item item)
	{
		base.photonView.RPC("GetFedItemRPC", base.photonView.Owner, new object[] { item.photonView.ViewID });
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00005778 File Offset: 0x00003978
	[PunRPC]
	public void GetFedItemRPC(int itemPhotonID)
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(itemPhotonID);
		if (photonView == null)
		{
			return;
		}
		Item item = ((photonView != null) ? photonView.GetComponent<Item>() : null);
		if (item == null)
		{
			return;
		}
		Debug.Log("I just got fed a: " + item.UIData.itemName);
		item.overrideHolderCharacter = this;
		if (item.OnPrimaryFinishedCast != null)
		{
			item.OnPrimaryFinishedCast.Invoke();
		}
		if (!item.consuming)
		{
			item.overrideHolderCharacter = null;
		}
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00005800 File Offset: 0x00003A00
	internal void DragTowards(Vector3 target, float force)
	{
		Action<Vector3, float> action = this.dragTowardsAction;
		if (action != null)
		{
			action.Invoke(target, force);
		}
		Vector3 vector = Vector3.ClampMagnitude(target - this.Center, 1f);
		this.AddForce(vector * force, 1f, 1f);
	}

	// Token: 0x0600009D RID: 157 RVA: 0x0000584E File Offset: 0x00003A4E
	internal bool OutOfStamina()
	{
		return this.data.currentStamina < 0.005f && this.data.extraStamina < 0.001f;
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00005876 File Offset: 0x00003A76
	internal bool OutOfRegularStamina()
	{
		return this.data.currentStamina < 0.005f;
	}

	// Token: 0x0600009F RID: 159 RVA: 0x0000588A File Offset: 0x00003A8A
	internal bool IsSliding()
	{
		return this.data.isClimbing && this.OutOfStamina();
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x000058A1 File Offset: 0x00003AA1
	internal bool CanDoInput()
	{
		return !GUIManager.instance.windowBlockingInput && !GUIManager.instance.wheelActive;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000058C0 File Offset: 0x00003AC0
	internal int GetPlayerListID(List<Character> playerList)
	{
		for (int i = 0; i < playerList.Count; i++)
		{
			if (playerList[i] == this)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000058F0 File Offset: 0x00003AF0
	internal void Fall(float seconds, float screenShake = 0f)
	{
		if (screenShake <= 1E-05f || !this.refs.view.IsMine)
		{
			this.refs.view.RPC("RPCA_Fall", 0, new object[] { seconds });
		}
		else
		{
			this.refs.view.RPC("RPCA_FallWithScreenShake", 0, new object[] { seconds, screenShake });
		}
		GlobalEvents.TriggerCharacterFell(this, seconds);
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00005973 File Offset: 0x00003B73
	[PunRPC]
	public void RPCA_UnFall()
	{
		this.data.fallSeconds = 0f;
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00005985 File Offset: 0x00003B85
	[PunRPC]
	public void RPCA_Fall(float seconds)
	{
		if (base.photonView.IsMine)
		{
			Debug.Log(string.Format("I fell for {0} seconds", seconds));
		}
		if (seconds > this.data.fallSeconds)
		{
			this.data.fallSeconds = seconds;
		}
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x000059C4 File Offset: 0x00003BC4
	[PunRPC]
	public void RPCA_FallWithScreenShake(float seconds, float shake)
	{
		if (base.photonView.IsMine)
		{
			Debug.Log(string.Format("I fell for {0} seconds", seconds));
		}
		GamefeelHandler.instance.AddPerlinShake(shake, 0.4f, 15f);
		if (seconds > this.data.fallSeconds)
		{
			this.data.fallSeconds = seconds;
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00005A24 File Offset: 0x00003C24
	[ConsoleCommand]
	public static void Revive()
	{
		Debug.Log(string.Format("Reviving, status: {0}, fullyPassedOut: {1}", Character.localCharacter.data.dead, Character.localCharacter.data.fullyPassedOut));
		if (Character.localCharacter.data.dead || Character.localCharacter.data.fullyPassedOut)
		{
			Character.localCharacter.view.RPC("RPCA_Revive", 0, new object[] { true });
		}
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00005AB0 File Offset: 0x00003CB0
	[PunRPC]
	internal void RPCA_Revive(bool applyStatus)
	{
		Action action = this.reviveAction;
		if (action != null)
		{
			action.Invoke();
		}
		this.data.dead = false;
		this.data.deathTimer = 0f;
		this.data.passedOut = false;
		this.data.fullyPassedOut = false;
		this.data.sinceGrounded = 0f;
		this.refs.afflictions.ClearAllStatus(true);
		this.refs.afflictions.RemoveAllThorns();
		this.refs.afflictions.ClearAllAfflictions();
		this.data.fallSeconds = 0f;
		if (applyStatus)
		{
			Character.ApplyPostReviveStatus(this.refs.afflictions);
		}
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00005B66 File Offset: 0x00003D66
	public static void ApplyPostReviveStatus(CharacterAfflictions afflictions)
	{
		afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.05f, true, true, true);
		afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.3f, true, true, true);
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00005B88 File Offset: 0x00003D88
	[PunRPC]
	internal void RPCA_ReviveAtPosition(Vector3 position, bool applyStatus, int statueSegment)
	{
		this.refs.items.DropAllItems(true);
		this.RPCA_Revive(applyStatus);
		this.WarpPlayer(position, true);
		if (statueSegment > -1)
		{
			this.data.lastRevivedSegment = (Segment)statueSegment;
		}
		this.refs.stats.justDied = false;
		this.refs.stats.justRevived = true;
		this.refs.stats.Record(true, position.y);
		Singleton<ReconnectHandler>.Instance.UpdateFromRevive(this, position);
	}

	// Token: 0x060000AA RID: 170 RVA: 0x00005C0B File Offset: 0x00003E0B
	[PunRPC]
	public void WarpPlayerRPC(Vector3 position, bool poof)
	{
		this.WarpPlayer(position, poof);
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00005C18 File Offset: 0x00003E18
	public void PlayPoofVFX(Vector3 pos)
	{
		this.refs.poof.transform.position = pos;
		this.refs.poof.main.startColor = this.refs.customization.PlayerColor;
		this.refs.poof.Play();
		for (int i = 0; i < this.poofSFX.Length; i++)
		{
			this.poofSFX[i].Play(pos);
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060000AC RID: 172 RVA: 0x00005C99 File Offset: 0x00003E99
	// (set) Token: 0x060000AD RID: 173 RVA: 0x00005CA1 File Offset: 0x00003EA1
	public float timeLastWarped { get; private set; }

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060000AE RID: 174 RVA: 0x00005CAA File Offset: 0x00003EAA
	// (set) Token: 0x060000AF RID: 175 RVA: 0x00005CB2 File Offset: 0x00003EB2
	public bool warping { get; private set; }

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060000B0 RID: 176 RVA: 0x00005CBC File Offset: 0x00003EBC
	// (remove) Token: 0x060000B1 RID: 177 RVA: 0x00005CF4 File Offset: 0x00003EF4
	public event Action<Character> WarpCompleted;

	// Token: 0x060000B2 RID: 178 RVA: 0x00005D29 File Offset: 0x00003F29
	private void TryWarpAgain()
	{
		if (this.warping)
		{
			return;
		}
		this.WarpPlayer(this._lastWarpTarget, false);
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x00005D44 File Offset: 0x00003F44
	internal void WarpPlayer(Vector3 position, bool poof)
	{
		Character.<>c__DisplayClass163_0 CS$<>8__locals1 = new Character.<>c__DisplayClass163_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.poof = poof;
		CS$<>8__locals1.position = position;
		base.StartCoroutine(CS$<>8__locals1.<WarpPlayer>g__IMove|0());
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00005D7C File Offset: 0x00003F7C
	internal void MoveBodypartTowardsPoint(BodypartType bodypart, Vector3 pos, float force, float clampDistance = 1f)
	{
		Bodypart bodypart2 = this.GetBodypart(bodypart);
		bodypart2.AddForce(Vector3.ClampMagnitude(pos - bodypart2.Rig.position, clampDistance) * force, 5);
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00005DB8 File Offset: 0x00003FB8
	public static bool PlayerIsDeadOrDown()
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.dead || character.data.fullyPassedOut)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00005E24 File Offset: 0x00004024
	internal BodypartType GetPartType(Rigidbody rigidbody)
	{
		foreach (Bodypart bodypart in this.refs.ragdoll.partList)
		{
			if (bodypart.Rig == rigidbody)
			{
				return bodypart.partType;
			}
		}
		return (BodypartType)(-1);
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00005E94 File Offset: 0x00004094
	internal void LimitFalling()
	{
		this.data.sinceGrounded = Mathf.Min(this.data.sinceGrounded, 0.5f);
		this.data.sinceJump = Mathf.Min(this.data.sinceJump, 0.5f);
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x00005EE1 File Offset: 0x000040E1
	internal void AddIllegalStatus(string illegalStatus, float amount)
	{
		Action<string, float> action = this.illegalStatusAction;
		if (action == null)
		{
			return;
		}
		action.Invoke(illegalStatus, amount);
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060000B9 RID: 185 RVA: 0x00005EF5 File Offset: 0x000040F5
	// (set) Token: 0x060000BA RID: 186 RVA: 0x00005EFD File Offset: 0x000040FD
	public bool infiniteStam { get; private set; }

	// Token: 0x060000BB RID: 187 RVA: 0x00005F08 File Offset: 0x00004108
	[ConsoleCommand]
	public static void InfiniteStamina()
	{
		if (!Character.localCharacter.infiniteStam)
		{
			Character.localCharacter.data.currentStamina = 1f;
		}
		Character.localCharacter.infiniteStam = !Character.localCharacter.infiniteStam;
		Debug.LogError(string.Format("Infinite Stamina: {0}", Character.localCharacter.infiniteStam));
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060000BC RID: 188 RVA: 0x00005F6A File Offset: 0x0000416A
	// (set) Token: 0x060000BD RID: 189 RVA: 0x00005F72 File Offset: 0x00004172
	public bool statusesLocked { get; private set; }

	// Token: 0x060000BE RID: 190 RVA: 0x00005F7B File Offset: 0x0000417B
	[ConsoleCommand]
	public static void LockStatuses()
	{
		Character.localCharacter.statusesLocked = !Character.localCharacter.statusesLocked;
		Debug.LogError(string.Format("Statuses Locked: {0}", Character.localCharacter.statusesLocked));
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00005FB2 File Offset: 0x000041B2
	private void OnGetMic(float db)
	{
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00005FB4 File Offset: 0x000041B4
	internal void StartPassedOutOnTheBeach()
	{
		Debug.Log("Starting passed out!");
		this.data.passedOutOnTheBeach = 3f;
		this.Fall(7f, 0f);
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00005FE0 File Offset: 0x000041E0
	public void AddForceToBodyPart(Rigidbody rig, Vector3 partForce, Vector3 wholeBodyForce)
	{
		Bodypart component = rig.GetComponent<Bodypart>();
		if (component == null)
		{
			return;
		}
		this.view.RPC("RPCA_AddForceToBodyPart", 0, new object[] { component.partType, partForce, wholeBodyForce });
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00006035 File Offset: 0x00004235
	[PunRPC]
	public void RPCA_AddForceToBodyPart(BodypartType bodypartType, Vector3 force, Vector3 wholeBodyForce)
	{
		this.GetBodypart(bodypartType).AddForce(force, 5);
		this.AddForce(wholeBodyForce, 1f, 1f);
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00006056 File Offset: 0x00004256
	internal void ClampSinceGrounded(float maxSinceGrounded)
	{
		this.data.sinceGrounded = Mathf.Clamp(this.data.sinceGrounded, 0f, maxSinceGrounded);
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x00006079 File Offset: 0x00004279
	internal void ClampRagdollControl(float maxRagdollControlClamp)
	{
		this.data.ragdollControlClamp = maxRagdollControlClamp;
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00006088 File Offset: 0x00004288
	private void HandleStickUpdate()
	{
		this.data.sinceUnstuck += Time.deltaTime;
		if (this.stickParts.Count == 0)
		{
			return;
		}
		bool flag = true;
		foreach (StickPart stickPart in this.stickParts)
		{
			if (stickPart.joint)
			{
				flag = false;
			}
			stickPart.sinceStick += Time.deltaTime;
			if (this.view.IsMine && stickPart.sinceStick > 4f && stickPart.joint)
			{
				this.view.RPC("RPCA_ClearJoint", 0, new object[] { stickPart.bodypart.partType });
			}
		}
		if (flag && this.view.IsMine)
		{
			this.view.RPC("RPCA_ClearStickData", 0, Array.Empty<object>());
			return;
		}
		this.ClampSinceGrounded(0.5f);
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x000061A4 File Offset: 0x000043A4
	[PunRPC]
	public void RPCA_ClearStickData()
	{
		this.stickParts.Clear();
		this.data.sinceUnstuck = 0f;
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x000061C4 File Offset: 0x000043C4
	[PunRPC]
	public void RPCA_ClearJoint(BodypartType bodypartType)
	{
		foreach (StickPart stickPart in this.stickParts)
		{
			if (bodypartType == stickPart.bodypart.partType)
			{
				Object.Destroy(stickPart.joint);
			}
		}
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x0000622C File Offset: 0x0000442C
	private void JumpStickEffect()
	{
		foreach (StickPart stickPart in this.stickParts)
		{
			stickPart.sinceStick += Random.Range(0.5f, 1.5f);
		}
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00006294 File Offset: 0x00004494
	public bool IsStuck()
	{
		return this.stickParts.Count > 0;
	}

	// Token: 0x060000CA RID: 202 RVA: 0x000062A8 File Offset: 0x000044A8
	internal bool TryStickBodypart(Bodypart bodypart, Vector3 stickAnchor, CharacterAfflictions.STATUSTYPE statusType, float statusAmount)
	{
		if (this.data.sinceUnstuck < 3f)
		{
			return false;
		}
		if (this.StickPartExists(bodypart))
		{
			return false;
		}
		this.view.RPC("RPCA_Stick", 0, new object[]
		{
			bodypart.partType,
			bodypart.transform.position,
			stickAnchor,
			statusType,
			statusAmount
		});
		return true;
	}

	// Token: 0x060000CB RID: 203 RVA: 0x0000632C File Offset: 0x0000452C
	[PunRPC]
	private void RPCA_Stick(BodypartType bodypartType, Vector3 pos, Vector3 stickAnchor, CharacterAfflictions.STATUSTYPE statusType, float statusAmount)
	{
		Bodypart bodypart = this.GetBodypart(bodypartType);
		bodypart.Rig.transform.position = pos;
		StickPart stickPart = new StickPart();
		stickPart.bodypart = bodypart;
		stickPart.sinceStick = 0f;
		ConfigurableJoint configurableJoint = bodypart.Rig.gameObject.AddComponent<ConfigurableJoint>();
		stickPart.joint = configurableJoint;
		this.stickParts.Add(stickPart);
		configurableJoint.xMotion = 0;
		configurableJoint.yMotion = 0;
		configurableJoint.zMotion = 0;
		configurableJoint.angularXMotion = 2;
		configurableJoint.angularYMotion = 2;
		configurableJoint.angularZMotion = 2;
		configurableJoint.anchor = bodypart.transform.InverseTransformPoint(stickAnchor);
		if (statusAmount > 0f)
		{
			this.refs.afflictions.AddStatus(statusType, statusAmount, true, true, true);
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x000063EB File Offset: 0x000045EB
	internal void UnStick()
	{
		this.refs.view.RPC("RPCA_Unstick", 0, Array.Empty<object>());
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00006408 File Offset: 0x00004608
	[PunRPC]
	public void RPCA_Unstick()
	{
		for (int i = this.stickParts.Count - 1; i >= 0; i--)
		{
			Object.Destroy(this.stickParts[i].joint);
		}
		this.RPCA_ClearStickData();
	}

	// Token: 0x060000CE RID: 206 RVA: 0x0000644C File Offset: 0x0000464C
	private bool StickPartExists(Bodypart bodypart)
	{
		foreach (StickPart stickPart in this.stickParts)
		{
			if (bodypart == stickPart.bodypart)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060000CF RID: 207 RVA: 0x000064B0 File Offset: 0x000046B0
	internal void AddForce(object value)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x000064B7 File Offset: 0x000046B7
	private void StoppedForcedRagdolling()
	{
		this.data.launchedByCannon = false;
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x000064C8 File Offset: 0x000046C8
	[ConsoleCommand]
	public static void WarpToSpawn()
	{
		Character.localCharacter.photonView.RPC("WarpPlayerRPC", 0, new object[]
		{
			SpawnPoint.allSpawnPoints[0].transform.position,
			false
		});
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00006549 File Offset: 0x00004749
	[CompilerGenerated]
	private void <RPCA_PassOut>g__PassOutDone|90_0()
	{
		this.data.fullyPassedOut = true;
		this.refs.items.DropAllItems(false);
	}

	// Token: 0x0400002E RID: 46
	public bool isBot;

	// Token: 0x0400002F RID: 47
	public bool isZombie;

	// Token: 0x04000030 RID: 48
	public bool isScoutmaster;

	// Token: 0x04000031 RID: 49
	public static Character localCharacter;

	// Token: 0x04000032 RID: 50
	public CharacterInput input;

	// Token: 0x04000033 RID: 51
	public CharacterData data;

	// Token: 0x04000034 RID: 52
	public Character.CharacterRefs refs;

	// Token: 0x04000036 RID: 54
	private PhotonView view;

	// Token: 0x04000037 RID: 55
	public static List<Character> AllCharacters = new List<Character>();

	// Token: 0x04000038 RID: 56
	public static List<Character> AllBotCharacters = new List<Character>();

	// Token: 0x04000039 RID: 57
	private Vector3 smoothedCamPos;

	// Token: 0x0400003B RID: 59
	private bool started;

	// Token: 0x0400003C RID: 60
	private float passOutFailsafeTick;

	// Token: 0x0400003D RID: 61
	private float lastZombified;

	// Token: 0x0400003E RID: 62
	public SFX_Instance[] poofSFX;

	// Token: 0x0400003F RID: 63
	private static bool forceWin;

	// Token: 0x04000040 RID: 64
	private bool UnPassOutCalled;

	// Token: 0x04000041 RID: 65
	public Action UnPassOutAction;

	// Token: 0x04000042 RID: 66
	private bool unPassOutCalled;

	// Token: 0x04000043 RID: 67
	public Action<float> landAction;

	// Token: 0x04000044 RID: 68
	public Action startJumpAction;

	// Token: 0x04000045 RID: 69
	public Action jumpAction;

	// Token: 0x04000046 RID: 70
	internal Action startClimbAction;

	// Token: 0x04000047 RID: 71
	public Action<Vector3, float> dragTowardsAction;

	// Token: 0x04000048 RID: 72
	public Action reviveAction;

	// Token: 0x0400004A RID: 74
	private Vector3 _lastWarpTarget;

	// Token: 0x0400004D RID: 77
	private static WaitForFixedUpdate warpWait = new WaitForFixedUpdate();

	// Token: 0x0400004E RID: 78
	public Action<string, float> illegalStatusAction;

	// Token: 0x04000051 RID: 81
	private List<StickPart> stickParts = new List<StickPart>();

	// Token: 0x020003F5 RID: 1013
	[Serializable]
	public class CharacterRefs
	{
		// Token: 0x040016F4 RID: 5876
		public Transform carryPosRef;

		// Token: 0x040016F5 RID: 5877
		public CharacterRopeHandling ropeHandling;

		// Token: 0x040016F6 RID: 5878
		public CharacterClimbing climbing;

		// Token: 0x040016F7 RID: 5879
		public CharacterMovement movement;

		// Token: 0x040016F8 RID: 5880
		public CharacterRagdoll ragdoll;

		// Token: 0x040016F9 RID: 5881
		public CharacterBalloons balloons;

		// Token: 0x040016FA RID: 5882
		public CharacterInteractible interactible;

		// Token: 0x040016FB RID: 5883
		public RigCreator rigCreator;

		// Token: 0x040016FC RID: 5884
		public Bodypart head;

		// Token: 0x040016FD RID: 5885
		public Bodypart hip;

		// Token: 0x040016FE RID: 5886
		public CharacterAnimations animations;

		// Token: 0x040016FF RID: 5887
		public Animator animator;

		// Token: 0x04001700 RID: 5888
		public RigBuilder ikRigBuilder;

		// Token: 0x04001701 RID: 5889
		public Rig ikRig;

		// Token: 0x04001702 RID: 5890
		public TwoBoneIKConstraint ikLeft;

		// Token: 0x04001703 RID: 5891
		public TwoBoneIKConstraint ikRight;

		// Token: 0x04001704 RID: 5892
		public CharacterItems items;

		// Token: 0x04001705 RID: 5893
		public AnimatedVariables animatedVariables;

		// Token: 0x04001706 RID: 5894
		public CharacterAfflictions afflictions;

		// Token: 0x04001707 RID: 5895
		public BadgeUnlocker badgeUnlocker;

		// Token: 0x04001708 RID: 5896
		public PhotonView view;

		// Token: 0x04001709 RID: 5897
		public CharacterHeatEmission heatEmission;

		// Token: 0x0400170A RID: 5898
		public CharacterVineClimbing vineClimbing;

		// Token: 0x0400170B RID: 5899
		public SkinnedMeshRenderer mainRenderer;

		// Token: 0x0400170C RID: 5900
		public CharacterCarrying carriying;

		// Token: 0x0400170D RID: 5901
		public CharacterCustomization customization;

		// Token: 0x0400170E RID: 5902
		public CharacterStats stats;

		// Token: 0x0400170F RID: 5903
		public CharacterGrabbing grabbing;

		// Token: 0x04001710 RID: 5904
		public HideTheBody hideTheBody;

		// Token: 0x04001711 RID: 5905
		public ParticleSystem poof;

		// Token: 0x04001712 RID: 5906
		public Transform IKHandTargetLeft;

		// Token: 0x04001713 RID: 5907
		public Transform IKHandTargetRight;

		// Token: 0x04001714 RID: 5908
		public Transform helperObjects;

		// Token: 0x04001715 RID: 5909
		public Transform animationHeadTransform;

		// Token: 0x04001716 RID: 5910
		public Transform animationHipTransform;

		// Token: 0x04001717 RID: 5911
		public Transform animationItemTransform;

		// Token: 0x04001718 RID: 5912
		public Transform animationLookTransform;

		// Token: 0x04001719 RID: 5913
		public Transform animationPositionTransform;

		// Token: 0x0400171A RID: 5914
		public Transform backpackTransform;
	}
}
