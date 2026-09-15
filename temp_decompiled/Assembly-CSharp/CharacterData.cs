using System;
using System.Collections.Generic;
using Peak.Afflictions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000010 RID: 16
public class CharacterData : MonoBehaviourPunCallbacks
{
	// Token: 0x0600015A RID: 346 RVA: 0x0000A560 File Offset: 0x00008760
	public float GetTargetRagdollControll()
	{
		if (this.carrier)
		{
			return 1f;
		}
		if (this.fallSeconds > 0f)
		{
			return 0f;
		}
		if (this.passedOut)
		{
			return 0f;
		}
		if (this.fullyPassedOut)
		{
			return 0f;
		}
		if (this.dead)
		{
			return 0f;
		}
		float num = 1f;
		float num2 = 1f - this.passOutValue;
		return Mathf.Clamp(Mathf.Min(num, num2), 0f, this.ragdollControlClamp);
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x0600015B RID: 347 RVA: 0x0000A5E5 File Offset: 0x000087E5
	// (set) Token: 0x0600015C RID: 348 RVA: 0x0000A5ED File Offset: 0x000087ED
	public float currentRagdollControll { get; set; }

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x0600015D RID: 349 RVA: 0x0000A5F6 File Offset: 0x000087F6
	// (set) Token: 0x0600015E RID: 350 RVA: 0x0000A5FE File Offset: 0x000087FE
	public bool isSkeleton
	{
		get
		{
			return this._isSkeleton;
		}
		private set
		{
			if (value != this._isSkeleton)
			{
				this._isSkeleton = value;
				this.character.refs.customization.refs.SetSkeleton(this._isSkeleton, this.character.IsLocal);
			}
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600015F RID: 351 RVA: 0x0000A63B File Offset: 0x0000883B
	public bool canBeSpectated
	{
		get
		{
			return !this.dead || this.sinceDied < 5f;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000160 RID: 352 RVA: 0x0000A654 File Offset: 0x00008854
	// (set) Token: 0x06000161 RID: 353 RVA: 0x0000A65C File Offset: 0x0000885C
	public bool dead
	{
		get
		{
			return this._dead;
		}
		set
		{
			if (!value)
			{
				this.zombified = false;
			}
			this._dead = value;
			Action characterStateUpdated = this.CharacterStateUpdated;
			if (characterStateUpdated == null)
			{
				return;
			}
			characterStateUpdated.Invoke();
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000162 RID: 354 RVA: 0x0000A67F File Offset: 0x0000887F
	// (set) Token: 0x06000163 RID: 355 RVA: 0x0000A687 File Offset: 0x00008887
	public float lastZombified { get; private set; }

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x06000165 RID: 357 RVA: 0x0000A6B1 File Offset: 0x000088B1
	// (set) Token: 0x06000164 RID: 356 RVA: 0x0000A690 File Offset: 0x00008890
	public bool zombified
	{
		get
		{
			return this._zombified;
		}
		set
		{
			if (!this._zombified && value)
			{
				this.lastZombified = Time.time;
			}
			this._zombified = value;
		}
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000A6BC File Offset: 0x000088BC
	public void RecalculateInvincibility()
	{
		this.isInvincible = false;
		this.isInvincibleMilk = false;
		Affliction affliction;
		if (this.character.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.BingBongShield, out affliction))
		{
			this.isInvincible = true;
			return;
		}
		Affliction affliction2;
		if (this.character.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.Invincibility, out affliction2))
		{
			this.isInvincible = true;
			this.isInvincibleMilk = (affliction2 as Affliction_Invincibility).isFromMilk;
		}
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000A730 File Offset: 0x00008930
	public void RecalculateLowGrav()
	{
		this.lowGravAmount = 0;
		Affliction affliction;
		if (this.character.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.LowGravity, out affliction))
		{
			Affliction_LowGravity affliction_LowGravity = affliction as Affliction_LowGravity;
			if (affliction_LowGravity != null)
			{
				this.lowGravAmount = affliction_LowGravity.lowGravAmount;
			}
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000168 RID: 360 RVA: 0x0000A775 File Offset: 0x00008975
	// (set) Token: 0x06000169 RID: 361 RVA: 0x0000A78A File Offset: 0x0000898A
	public bool cannibalismPermitted
	{
		get
		{
			return this._cannibalismPermitted && !this.isSkeleton;
		}
		private set
		{
			if (this._cannibalismPermitted != value)
			{
				this._cannibalismPermitted = value;
				Debug.Log(string.Format("Cannibalism permitted for {0} : {1}", this.character.characterName, value));
			}
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x0600016A RID: 362 RVA: 0x0000A7BC File Offset: 0x000089BC
	public bool fullyConscious
	{
		get
		{
			return !this.passedOut && !this.fullyPassedOut && !this.dead;
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x0600016B RID: 363 RVA: 0x0000A7D9 File Offset: 0x000089D9
	public bool isClimbingAnything
	{
		get
		{
			return this.isClimbing || this.isRopeClimbing || this.isVineClimbing;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x0600016C RID: 364 RVA: 0x0000A7F3 File Offset: 0x000089F3
	// (set) Token: 0x0600016D RID: 365 RVA: 0x0000A7FC File Offset: 0x000089FC
	public Item currentItem
	{
		get
		{
			return this._currentitem;
		}
		set
		{
			if (this._currentitem != value)
			{
				this._currentitem = value;
				StickyItemComponent stickyItemComponent;
				if (value != null && value.TryGetComponent<StickyItemComponent>(ref stickyItemComponent))
				{
					this.currentStickyItem = stickyItemComponent;
					return;
				}
				this.currentStickyItem = null;
			}
		}
	}

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x0600016E RID: 366 RVA: 0x0000A840 File Offset: 0x00008A40
	// (remove) Token: 0x0600016F RID: 367 RVA: 0x0000A878 File Offset: 0x00008A78
	public event Action CharacterStateUpdated;

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000170 RID: 368 RVA: 0x0000A8AD File Offset: 0x00008AAD
	// (set) Token: 0x06000171 RID: 369 RVA: 0x0000A8B5 File Offset: 0x00008AB5
	public float currentStamina
	{
		get
		{
			return this._stam;
		}
		set
		{
			if (this.character.infiniteStam)
			{
				return;
			}
			this._stam = value;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000172 RID: 370 RVA: 0x0000A8CC File Offset: 0x00008ACC
	// (set) Token: 0x06000173 RID: 371 RVA: 0x0000A8D4 File Offset: 0x00008AD4
	public float vinePercent
	{
		get
		{
			return this._vinePercent;
		}
		set
		{
			this._vinePercent = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000174 RID: 372 RVA: 0x0000A8E2 File Offset: 0x00008AE2
	public float TotalStamina
	{
		get
		{
			return this.currentStamina + this.extraStamina;
		}
	}

	// Token: 0x06000175 RID: 373 RVA: 0x0000A8F1 File Offset: 0x00008AF1
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.SetBadgeStatus();
	}

	// Token: 0x06000176 RID: 374 RVA: 0x0000A908 File Offset: 0x00008B08
	private void Start()
	{
		if (base.photonView.IsMine)
		{
			CannibalismSetting setting = GameHandler.Instance.SettingsHandler.GetSetting<CannibalismSetting>();
			this.cannibalismPermitted = setting.Value == 1;
			if (PhotonNetwork.InRoom)
			{
				base.photonView.RPC("RPCA_SyncCanBeCannibalized", 1, new object[] { this.cannibalismPermitted });
			}
		}
	}

	// Token: 0x06000177 RID: 375 RVA: 0x0000A970 File Offset: 0x00008B70
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		PhotonView photonView = this.character.photonView;
		string text = "RPC_SyncOnJoin";
		object[] array = new object[19];
		array[0] = this.passedOut;
		array[1] = this.fullyPassedOut;
		array[2] = this.dead;
		array[3] = this.isSprinting;
		int num = 4;
		Item currentItem = this.currentItem;
		array[num] = ((currentItem != null) ? currentItem.photonView : null);
		array[5] = this.isJumping;
		array[6] = this.isClimbing;
		array[7] = this.isRopeClimbing;
		array[8] = this.isVineClimbing;
		array[9] = this.vinePercent;
		array[10] = this.ropePercent;
		array[11] = this.isCrouching;
		array[12] = this.isReaching;
		int num2 = 13;
		JungleVine jungleVine = this.heldVine;
		array[num2] = ((jungleVine != null) ? jungleVine.photonView : null);
		int num3 = 14;
		Rope rope = this.heldRope;
		array[num3] = ((rope != null) ? rope.photonView : null);
		array[15] = this.sprintJump;
		array[16] = this.badgeStatus;
		array[17] = this.cannibalismPermitted;
		array[18] = this.isSkeleton;
		photonView.RPC(text, newPlayer, array);
	}

	// Token: 0x06000178 RID: 376 RVA: 0x0000AAD0 File Offset: 0x00008CD0
	[PunRPC]
	public void RPC_SyncOnJoin(bool passedOut, bool fullyPassedOut, bool dead, bool isSprinting, PhotonView currentItem, bool isJumping, bool isClimbing, bool isRopeClimbing, bool isVineClimbing, float vinePercent, float ropePercent, bool isCrouching, bool isReaching, PhotonView heldVine, PhotonView heldRope, bool sprintJump, bool[] badgeStatus, bool cannibalismPermitted, bool isSkeleton)
	{
		Debug.Log(string.Format("RPC_SyncOnJoin: {0}, {1}", passedOut, fullyPassedOut));
		this.passedOut = passedOut;
		this.fullyPassedOut = fullyPassedOut;
		this.dead = dead;
		this.isSprinting = isSprinting;
		this.currentItem = ((currentItem != null) ? currentItem.GetComponent<Item>() : null);
		this.isJumping = isJumping;
		this.isClimbing = isClimbing;
		this.isRopeClimbing = isRopeClimbing;
		this.isVineClimbing = isVineClimbing;
		this.vinePercent = vinePercent;
		this.ropePercent = ropePercent;
		this.isCrouching = isCrouching;
		this.isReaching = isReaching;
		this.heldVine = ((heldVine != null) ? heldVine.GetComponent<JungleVine>() : null);
		this.heldRope = ((heldRope != null) ? heldRope.GetComponent<Rope>() : null);
		this.sprintJump = sprintJump;
		this.badgeStatus = badgeStatus;
		this.cannibalismPermitted = cannibalismPermitted;
		this.isSkeleton = isSkeleton;
		if (this.character.refs.badgeUnlocker == null)
		{
			Debug.LogError("Badge unlocker not found...");
			return;
		}
		this.character.refs.badgeUnlocker.BadgeUnlockVisual();
	}

	// Token: 0x06000179 RID: 377 RVA: 0x0000ABEC File Offset: 0x00008DEC
	internal void SetBadgeStatus()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		this.badgeStatus = new bool[GUIManager.instance.mainBadgeManager.badgeData.Length];
		for (int i = 0; i < this.badgeStatus.Length; i++)
		{
			if (GUIManager.instance.mainBadgeManager.badgeData[i] != null)
			{
				this.badgeStatus[i] = !GUIManager.instance.mainBadgeManager.badgeData[i].IsLocked;
			}
			else
			{
				this.badgeStatus[i] = false;
			}
		}
		base.photonView.RPC("SyncBadgeStatus", 0, new object[] { this.badgeStatus });
	}

	// Token: 0x0600017A RID: 378 RVA: 0x0000AC9B File Offset: 0x00008E9B
	[PunRPC]
	public void RPCA_SyncCanBeCannibalized(bool canBeCannibalized)
	{
		this.cannibalismPermitted = canBeCannibalized;
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000ACA4 File Offset: 0x00008EA4
	[PunRPC]
	private void SyncBadgeStatus(bool[] statusArray)
	{
		this.badgeStatus = statusArray;
		this.character.refs.badgeUnlocker.BadgeUnlockVisual();
	}

	// Token: 0x0600017C RID: 380 RVA: 0x0000ACC2 File Offset: 0x00008EC2
	public void SetSkeleton(bool active)
	{
		base.photonView.RPC("RPC_SyncSkeleton", 0, new object[] { active });
	}

	// Token: 0x0600017D RID: 381 RVA: 0x0000ACE4 File Offset: 0x00008EE4
	[PunRPC]
	public void RPC_SyncSkeleton(bool active)
	{
		this.isSkeleton = active;
		if (this.isSkeleton)
		{
			this.character.refs.customization.BecomeHuman();
		}
	}

	// Token: 0x0600017E RID: 382 RVA: 0x0000AD0A File Offset: 0x00008F0A
	internal bool GetBadgeStatus(int index)
	{
		return index >= 0 && index < this.badgeStatus.Length && this.badgeStatus[index];
	}

	// Token: 0x0600017F RID: 383 RVA: 0x0000AD28 File Offset: 0x00008F28
	public void DestroyCheckpointFlags()
	{
		for (int i = this.checkpointFlags.Count - 1; i >= 0; i--)
		{
			CheckpointFlag checkpointFlag = this.checkpointFlags[i];
			if (checkpointFlag != null)
			{
				checkpointFlag.DestroySelf();
			}
		}
		this.checkpointFlags.Clear();
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x06000180 RID: 384 RVA: 0x0000AD74 File Offset: 0x00008F74
	public bool usingWheel
	{
		get
		{
			return this.usingEmoteWheel || this.usingBackpackWheel;
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000181 RID: 385 RVA: 0x0000AD86 File Offset: 0x00008F86
	public bool IsCarryingCharacter
	{
		get
		{
			return this.carriedPlayer != null;
		}
	}

	// Token: 0x040000D4 RID: 212
	public const float deathSpectateLingerTime = 5f;

	// Token: 0x040000D5 RID: 213
	[Range(0f, 1f)]
	public float passOutValue;

	// Token: 0x040000D6 RID: 214
	public bool passedOut;

	// Token: 0x040000D7 RID: 215
	public bool fullyPassedOut;

	// Token: 0x040000D8 RID: 216
	public float lastPassedOut;

	// Token: 0x040000D9 RID: 217
	public float deathTimer;

	// Token: 0x040000DA RID: 218
	public Segment lastRevivedSegment;

	// Token: 0x040000DB RID: 219
	private bool _isSkeleton;

	// Token: 0x040000DC RID: 220
	private bool _dead;

	// Token: 0x040000DD RID: 221
	private bool _zombified;

	// Token: 0x040000DF RID: 223
	[SerializeField]
	private bool _cannibalismPermitted = true;

	// Token: 0x040000E0 RID: 224
	[SerializeField]
	internal bool isInvincible;

	// Token: 0x040000E1 RID: 225
	internal int lowGravAmount;

	// Token: 0x040000E2 RID: 226
	[SerializeField]
	internal bool isInvincibleMilk;

	// Token: 0x040000E3 RID: 227
	public bool isGrounded;

	// Token: 0x040000E4 RID: 228
	public float sinceGrounded;

	// Token: 0x040000E5 RID: 229
	public Vector3 groundPos;

	// Token: 0x040000E6 RID: 230
	public Vector3 groundNormal;

	// Token: 0x040000E7 RID: 231
	public float targetHeadHeight;

	// Token: 0x040000E8 RID: 232
	public float targetHipHeight;

	// Token: 0x040000E9 RID: 233
	public Vector3 worldMovementInput;

	// Token: 0x040000EA RID: 234
	public Vector3 worldMovementInput_Grounded;

	// Token: 0x040000EB RID: 235
	public Vector3 worldMovementInput_Lerp;

	// Token: 0x040000EC RID: 236
	public Vector2 lookValues;

	// Token: 0x040000ED RID: 237
	public Vector3 lookDirection;

	// Token: 0x040000EE RID: 238
	public Vector3 lookDirection_Flat;

	// Token: 0x040000EF RID: 239
	public Vector3 lookDirection_Right;

	// Token: 0x040000F0 RID: 240
	public Vector3 lookDirection_Up;

	// Token: 0x040000F1 RID: 241
	public bool isSprinting;

	// Token: 0x040000F2 RID: 242
	public float lastBouncedTime;

	// Token: 0x040000F3 RID: 243
	public List<CheckpointFlag> checkpointFlags = new List<CheckpointFlag>();

	// Token: 0x040000F4 RID: 244
	private Item _currentitem;

	// Token: 0x040000F5 RID: 245
	[HideInInspector]
	public StickyItemComponent currentStickyItem;

	// Token: 0x040000F6 RID: 246
	public Vector3 avarageVelocity;

	// Token: 0x040000F7 RID: 247
	public Vector3 avarageLastFrameVelocity;

	// Token: 0x040000F8 RID: 248
	public float sinceJump;

	// Token: 0x040000F9 RID: 249
	public float sinceClimb;

	// Token: 0x040000FA RID: 250
	public float currentHeadHeight;

	// Token: 0x040000FB RID: 251
	public bool isJumping;

	// Token: 0x040000FC RID: 252
	public float groundedFor;

	// Token: 0x040000FD RID: 253
	public float lastGroundedHeight;

	// Token: 0x040000FE RID: 254
	public bool chargingJump;

	// Token: 0x040000FF RID: 255
	public bool isClimbing;

	// Token: 0x04000100 RID: 256
	public bool isRopeClimbing;

	// Token: 0x04000101 RID: 257
	public bool isVineClimbing;

	// Token: 0x04000102 RID: 258
	public Vector3 climbPos;

	// Token: 0x04000103 RID: 259
	public Vector3 climbNormal;

	// Token: 0x04000104 RID: 260
	public float spectateZoom;

	// Token: 0x04000105 RID: 261
	public bool isBlind;

	// Token: 0x04000106 RID: 262
	public bool wearingSunscreen;

	// Token: 0x04000108 RID: 264
	private float _stam;

	// Token: 0x04000109 RID: 265
	[FormerlySerializedAs("lastFrameStamina")]
	public float lastFrameTotalStamina;

	// Token: 0x0400010A RID: 266
	public float staminaDelta;

	// Token: 0x0400010B RID: 267
	public Rope heldRope;

	// Token: 0x0400010C RID: 268
	public JungleVine heldVine;

	// Token: 0x0400010D RID: 269
	private float _vinePercent;

	// Token: 0x0400010E RID: 270
	public float ropePercent;

	// Token: 0x0400010F RID: 271
	public Vector3 ropeClimbNormal;

	// Token: 0x04000110 RID: 272
	public Vector3 ropeClimbWorldNormal;

	// Token: 0x04000111 RID: 273
	public Vector3 ropeClimbWorldUp;

	// Token: 0x04000112 RID: 274
	public float sinceUseStamina;

	// Token: 0x04000113 RID: 275
	public bool isCrouching;

	// Token: 0x04000114 RID: 276
	public bool isReaching;

	// Token: 0x04000115 RID: 277
	public FixedJoint grabJoint;

	// Token: 0x04000116 RID: 278
	public float sincePressClimb = 10f;

	// Token: 0x04000117 RID: 279
	public float sincePressReach = 10f;

	// Token: 0x04000118 RID: 280
	public float lastConsumedItem;

	// Token: 0x04000119 RID: 281
	public float sinceHeldItem;

	// Token: 0x0400011A RID: 282
	public float lastAddedStatusAmount;

	// Token: 0x0400011B RID: 283
	public bool isInFog;

	// Token: 0x0400011C RID: 284
	public bool[] badgeStatus;

	// Token: 0x0400011D RID: 285
	public float overrideIKForSeconds;

	// Token: 0x0400011E RID: 286
	public float extraStamina;

	// Token: 0x0400011F RID: 287
	public float outOfStaminaFor;

	// Token: 0x04000120 RID: 288
	public float staminaMod;

	// Token: 0x04000121 RID: 289
	public float sinceClimbJump;

	// Token: 0x04000122 RID: 290
	public int climbingSpikeCount;

	// Token: 0x04000123 RID: 291
	public float grabFriendDistance;

	// Token: 0x04000124 RID: 292
	public float sinceFallSlide;

	// Token: 0x04000125 RID: 293
	public ClimbHandle currentClimbHandle;

	// Token: 0x04000126 RID: 294
	public float sinceClimbHandle;

	// Token: 0x04000127 RID: 295
	public float sinceGrabFriend;

	// Token: 0x04000128 RID: 296
	public bool usingEmoteWheel;

	// Token: 0x04000129 RID: 297
	public bool usingBackpackWheel;

	// Token: 0x0400012A RID: 298
	public float fallSeconds;

	// Token: 0x0400012B RID: 299
	[HideInInspector]
	public bool launchedByCannon;

	// Token: 0x0400012C RID: 300
	public float sinceAddedCold = 10f;

	// Token: 0x0400012D RID: 301
	public float sinceStartClimb;

	// Token: 0x0400012E RID: 302
	public Character carriedPlayer;

	// Token: 0x0400012F RID: 303
	public Character carrier;

	// Token: 0x04000130 RID: 304
	public bool sprintJump;

	// Token: 0x04000131 RID: 305
	public int jumpsRemaining = 1;

	// Token: 0x04000132 RID: 306
	public ClimbModifierSurface climbMod;

	// Token: 0x04000133 RID: 307
	public float slippy;

	// Token: 0x04000134 RID: 308
	public RaycastHit climbHit;

	// Token: 0x04000135 RID: 309
	internal Character grabbedPlayer;

	// Token: 0x04000136 RID: 310
	internal Character grabbingPlayer;

	// Token: 0x04000137 RID: 311
	public Transform spawnPoint;

	// Token: 0x04000138 RID: 312
	private Character character;

	// Token: 0x04000139 RID: 313
	public bool isKinecmatic;

	// Token: 0x0400013A RID: 314
	public bool isCarried;

	// Token: 0x0400013B RID: 315
	public float sinceLetGoOfFriend;

	// Token: 0x0400013C RID: 316
	public float sinceStandOnPlayer;

	// Token: 0x0400013D RID: 317
	public float sincePalJump = 10f;

	// Token: 0x0400013E RID: 318
	public Character lastStoodOnPlayer;

	// Token: 0x0400013F RID: 319
	public float myersDistance;

	// Token: 0x04000140 RID: 320
	public float sinceItemAttach = 10f;

	// Token: 0x04000141 RID: 321
	public float sinceCanClimb = 10f;

	// Token: 0x04000142 RID: 322
	public bool hasClimbedSinceGrounded;

	// Token: 0x04000143 RID: 323
	public float passedOutOnTheBeach;

	// Token: 0x04000144 RID: 324
	public float sinceDied;

	// Token: 0x04000145 RID: 325
	public float sinceDead;

	// Token: 0x04000146 RID: 326
	public Vector3 groundedForward;

	// Token: 0x04000147 RID: 327
	public Vector3 groundedRight;

	// Token: 0x04000148 RID: 328
	public float ragdollControlClamp;

	// Token: 0x04000149 RID: 329
	public bool staticClimbCost;

	// Token: 0x0400014A RID: 330
	public float sinceUnstuck = 10f;

	// Token: 0x0400014B RID: 331
	public bool isScoutmaster;

	// Token: 0x0400014C RID: 332
	public float inWater;
}
