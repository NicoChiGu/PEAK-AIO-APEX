using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200000D RID: 13
public class CharacterAnimations : MonoBehaviour
{
	// Token: 0x0600013A RID: 314 RVA: 0x00008F46 File Offset: 0x00007146
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x0600013B RID: 315 RVA: 0x00008F54 File Offset: 0x00007154
	private void Start()
	{
		Character character = this.character;
		character.landAction = (Action<float>)Delegate.Combine(character.landAction, new Action<float>(this.Land));
		Character character2 = this.character;
		character2.startJumpAction = (Action)Delegate.Combine(character2.startJumpAction, new Action(this.StartJump));
		Character character3 = this.character;
		character3.jumpAction = (Action)Delegate.Combine(character3.jumpAction, new Action(this.Jump));
		Character character4 = this.character;
		character4.startClimbAction = (Action)Delegate.Combine(character4.startClimbAction, new Action(this.StartClimb));
		this.headBobSetting = GameHandler.Instance.SettingsHandler.GetSetting<HeadBobSetting>();
		this.UpdateHeadBob();
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00009018 File Offset: 0x00007218
	public void SetAnimatorController(RuntimeAnimatorController controller)
	{
		this.character.refs.animator.runtimeAnimatorController = controller;
		this.CacheAnimatorHashes();
	}

	// Token: 0x0600013D RID: 317 RVA: 0x00009038 File Offset: 0x00007238
	private void CacheAnimatorHashes()
	{
		this._animatorHashes = new HashSet<int>(Enumerable.Select<AnimatorControllerParameter, int>(this.character.refs.animator.parameters, (AnimatorControllerParameter p) => p.nameHash));
		this._lastHashCachedController = this.character.refs.animator.runtimeAnimatorController;
	}

	// Token: 0x0600013E RID: 318 RVA: 0x000090A4 File Offset: 0x000072A4
	private void UpdateHeadBob()
	{
		if (this.character == Character.localCharacter)
		{
			this.SetAnimatorController((this.headBobSetting.Value == 1) ? this.noHeadBobController : this.defaultController);
		}
	}

	// Token: 0x0600013F RID: 319 RVA: 0x000090DC File Offset: 0x000072DC
	private void Update()
	{
		Animator animator = this.character.refs.animator;
		bool flag = this.headBobSetting.Value == 1;
		if (flag != this._headBobRemoved)
		{
			this._headBobRemoved = flag;
			this.UpdateHeadBob();
		}
		if (this._lastHashCachedController != animator.runtimeAnimatorController)
		{
			Debug.LogWarning("Somebody changed animator controllers without using SetAnimatorController! Re-caching parameter hashes.");
			this.CacheAnimatorHashes();
		}
		if (this.point)
		{
			animator.SetBool(this.AN_POINT, true);
			animator.SetFloat(this.AN_POINTX, this.character.GetBodypart(BodypartType.Hip).transform.InverseTransformPoint(this.point.transform.position).x / (Vector3.Distance(this.character.GetBodypart(BodypartType.Hip).transform.position, this.point.transform.position) / 5f));
			animator.SetFloat(this.AN_POINTY, this.character.GetBodypart(BodypartType.Hip).transform.InverseTransformPoint(this.point.transform.position).y / (Vector3.Distance(this.character.GetBodypart(BodypartType.Hip).transform.position, this.point.transform.position) / 5f));
		}
		else
		{
			animator.SetBool(this.AN_POINT, false);
		}
		animator.SetBool(this.AN_CLIMB_SURFACE, this.character.data.isClimbing);
		animator.SetBool(this.AN_CLIMB_ROPE, this.character.data.isRopeClimbing);
		animator.SetFloat(this.AN_INPUTX, this.character.input.movementInput.x, 0.125f, Time.deltaTime);
		animator.SetFloat(this.AN_INPUTY, this.character.input.movementInput.y, 0.125f, Time.deltaTime);
		animator.SetFloat(this.AN_THROW_CHARGE, this.character.refs.items.throwChargeLevel);
		animator.SetFloat(this.AN_THROW, this.throwTime);
		if (Mathf.Abs(animator.GetFloat(this.AN_INPUTX)) < 0.125f && Mathf.Abs(this.character.input.movementInput.x) < 0.125f)
		{
			animator.SetFloat(this.AN_INPUTX, 0f);
		}
		if (Mathf.Abs(animator.GetFloat(this.AN_INPUTY)) < 0.125f && Mathf.Abs(this.character.input.movementInput.y) < 0.125f)
		{
			animator.SetFloat(this.AN_INPUTY, 0f);
		}
		animator.SetBool(this.AN_IS_GROUNDED, true);
		animator.SetFloat(this.AN_VELOCITY_Y, this.character.data.avarageVelocity.y);
		animator.SetFloat(this.AN_VELOCITY_Z, this.character.data.avarageVelocity.z);
		if (this.lookRef)
		{
			Vector3 cameraPos = this.character.GetCameraPos(0f);
			Vector3 lookDirection = this.character.data.lookDirection;
			Matrix4x4 matrix4x = Matrix4x4.TRS(cameraPos, Quaternion.LookRotation(lookDirection, Vector3.up), Vector3.one);
			this.lookRef.rotation = Quaternion.Euler(0f, matrix4x.rotation.eulerAngles.y, 0f);
			animator.SetFloat(this.AN_LOOK_Y, Matrix4x4Extensions.TransformDirection(matrix4x.inverse, this.lookRef.forward).y);
			animator.SetFloat(this.AN_LOOK_X, this.character.input.lookInput.x, 0.25f, Time.deltaTime);
		}
		if (this.character.data.sinceGrounded > 0.3f || this.character.data.avarageVelocity.y > 5f || this.character.data.isJumping || this.character.data.sinceClimb < 0.25f)
		{
			animator.SetBool(this.AN_IS_GROUNDED, false);
		}
		if (this.character.data.isSprinting)
		{
			animator.SetFloat(this.AN_SPRINT, 1f, 0.125f, Time.deltaTime);
		}
		if (!this.character.data.isSprinting)
		{
			animator.SetFloat(this.AN_SPRINT, 0f, 0.125f, Time.deltaTime);
		}
		animator.SetBool(this.AN_CROUCH, this.character.data.isCrouching);
		animator.SetBool(this.AN_REACH, this.character.data.isReaching);
		animator.SetBool(this.AN_GRAB, this.character.data.grabJoint);
		animator.SetBool(this.AN_VINE_HANG, this.character.data.isVineClimbing);
		if (this.character.data.isVineClimbing && this.character.data.heldVine)
		{
			animator.SetBool(this.AN_VINE_SLIDE, this.character.refs.vineClimbing.Sliding());
			animator.SetInteger(this.AN_VINE_TYPE, this.character.data.heldVine.vineType);
		}
		else if (this._animatorHashes.Contains(this.AN_VINE_SLIDE))
		{
			animator.SetBool(this.AN_VINE_SLIDE, false);
		}
		animator.SetBool(this.AN_IS_SLIDING, this.character.IsSliding());
		animator.SetBool(this.AN_CLIMB_JUMP, this.character.data.sinceClimbJump < 0.3f);
		if (!this.character.data.isSprinting && animator.GetFloat(this.AN_SPRINT) < 0.75f)
		{
			animator.SetFloat(this.AN_SPRINT, 0f);
		}
		animator.SetBool(this.AN_CHARGE_JUMP, this.character.data.chargingJump);
		animator.SetBool(this.AN_JUMP, this.character.data.isJumping);
		animator.SetFloat(this.AN_SINCE_GROUNDED, this.character.data.sinceGrounded, 0.25f, Time.deltaTime);
		animator.SetInteger(this.AN_REACH_TYPE, 0);
		animator.SetFloat(this.AN_MYERS_DISTANCE, this.character.data.myersDistance);
		this.character.data.myersDistance = 1000f;
		animator.SetBool(this.AN_HANG, this.character.data.currentClimbHandle != null);
		animator.SetBool(this.AN_HELP, false);
		animator.SetBool(this.AN_WEBBED, this.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Web) > 0f);
		animator.SetFloat(this.AN_FLOOR_LEAN_FORWARD, this.character.data.groundedForward.normalized.y, 0.2f, Time.deltaTime);
		animator.SetFloat(this.AN_FLOOR_LEAN_RIGHT, this.character.data.groundedRight.normalized.y, 0.2f, Time.deltaTime);
		if (this.character.data.grabFriendDistance <= 3.5f && !this.character.data.isClimbing)
		{
			animator.SetBool(this.AN_HELP, true);
		}
		if (!animator.GetBool(this.AN_IS_GROUNDED))
		{
			animator.SetInteger(this.AN_REACH_TYPE, 1);
		}
		if (this.character.data.isCrouching)
		{
			animator.SetInteger(this.AN_REACH_TYPE, 2);
		}
		this.HandleIK();
		this.SetAnimSpeed();
		Bodypart bodypart = this.character.GetBodypart(BodypartType.Hip);
		this.character.refs.animationPositionTransform.position = bodypart.transform.position;
		this.throwTime -= Time.deltaTime;
		if (this.throwTime <= 0f)
		{
			this.throwTime = 0f;
		}
		this.sinceEmoteStart += Time.deltaTime;
		if (this.emoting && (this.sinceEmoteStart > 2f || (this.sinceEmoteStart > 0.7f && (this.character.input.movementInput.magnitude > 0.1f || this.character.input.jumpWasPressed || this.character.data.sinceGrounded > 0.2f))))
		{
			this.character.refs.animator.SetBool(this.AN_EMOTE, false);
			this.emoting = false;
		}
		this.character.refs.animator.SetFloat(this.AN_INWATER, this.character.data.inWater);
		this.character.data.inWater = 1f;
	}

	// Token: 0x06000140 RID: 320 RVA: 0x000099EF File Offset: 0x00007BEF
	public void ResetForceSpeed()
	{
		this.forceSpeed = -1f;
	}

	// Token: 0x06000141 RID: 321 RVA: 0x000099FC File Offset: 0x00007BFC
	public void ForceSpeed(float value)
	{
		this.forceSpeed = value;
	}

	// Token: 0x06000142 RID: 322 RVA: 0x00009A08 File Offset: 0x00007C08
	private void SetAnimSpeed()
	{
		if (this.forceSpeed != -1f)
		{
			this.character.refs.animator.speed = this.forceSpeed;
			return;
		}
		if (this.character.data.carrier)
		{
			this.character.refs.animator.speed = 1f;
			return;
		}
		if (this.character.data.dead || this.character.data.fullyPassedOut)
		{
			this.character.refs.animator.speed = 0f;
			return;
		}
		if (this.character.data.isClimbing && this.character.data.sinceClimbJump > 0.5f)
		{
			this.character.refs.animator.speed = this.character.data.staminaMod;
			return;
		}
		this.character.refs.animator.speed = 1f;
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00009B18 File Offset: 0x00007D18
	private bool ReachIK()
	{
		return !this.character.data.isCrouching && this.character.data.isReaching && this.character.data.sinceGrabFriend > 0.5f;
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00009B58 File Offset: 0x00007D58
	private void HandleIK()
	{
		if (!this.character.refs.ikRight)
		{
			return;
		}
		if (this.ReachIK())
		{
			this.character.refs.ikRig.weight = 1f;
			this.character.refs.ikRight.weight = 1f;
			this.character.refs.ikLeft.weight = 0f;
			return;
		}
		if (this.character.data.currentItem && this.character.data.overrideIKForSeconds <= 0f)
		{
			this.character.refs.ikRig.weight = 1f;
			this.character.refs.ikRight.weight = 1f;
			this.character.refs.ikLeft.weight = 1f;
			return;
		}
		this.character.refs.ikRig.weight = 0f;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00009C6B File Offset: 0x00007E6B
	private void Land(float sinceGrounded)
	{
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00009C6D File Offset: 0x00007E6D
	private void Jump()
	{
	}

	// Token: 0x06000147 RID: 327 RVA: 0x00009C6F File Offset: 0x00007E6F
	private void StartJump()
	{
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00009C71 File Offset: 0x00007E71
	private void StartClimb()
	{
	}

	// Token: 0x06000149 RID: 329 RVA: 0x00009C73 File Offset: 0x00007E73
	public void PlaySpecificAnimation(string animationName)
	{
		if (this.character.refs.animator == null)
		{
			return;
		}
		this.character.refs.animator.Play(animationName, 0, 0f);
	}

	// Token: 0x0600014A RID: 330 RVA: 0x00009CAA File Offset: 0x00007EAA
	public void PrepIK()
	{
	}

	// Token: 0x0600014B RID: 331 RVA: 0x00009CAC File Offset: 0x00007EAC
	public void ConfigureIK()
	{
		if (this.character.refs.IKHandTargetLeft == null)
		{
			return;
		}
		if (this.character.data.currentItem)
		{
			this.character.refs.IKHandTargetLeft.position = this.character.refs.items.GetItemPosLeft(this.character.data.currentItem);
			this.character.refs.IKHandTargetRight.position = this.character.refs.items.GetItemPosRight(this.character.data.currentItem);
			this.character.refs.IKHandTargetRight.rotation = this.character.refs.items.GetItemRotRight(this.character.data.currentItem);
			this.character.refs.IKHandTargetLeft.rotation = this.character.refs.items.GetItemRotLeft(this.character.data.currentItem);
			return;
		}
		if (this.ReachIK())
		{
			this.character.refs.IKHandTargetRight.position = this.character.refs.animationHeadTransform.position + this.character.refs.animationLookTransform.TransformDirection(new Vector3(0.15f, -0.1f, 1.5f));
			this.character.refs.IKHandTargetRight.localEulerAngles = new Vector3(this.ReachHandPos.x, this.ReachHandPos.y, this.ReachHandPos.z + this.character.data.lookValues.y);
		}
	}

	// Token: 0x0600014C RID: 332 RVA: 0x00009E8C File Offset: 0x0000808C
	internal void PlayEmote(string emoteName)
	{
		bool flag = Random.value > 0.5f;
		this.character.refs.view.RPC("RPCA_PlayRemove", 0, new object[] { emoteName, flag });
	}

	// Token: 0x0600014D RID: 333 RVA: 0x00009ED4 File Offset: 0x000080D4
	[PunRPC]
	private void RPCA_PlayRemove(string emoteName, bool succeeded)
	{
		if (emoteName == "A_Scout_Emote_PlayDead")
		{
			this.character.Fall(3f, 0f);
			return;
		}
		if (emoteName == "A_Scout_Emote_BackFlip" && !this.emoting && this.character.data.fallSeconds <= 0f && this.character.data.fullyConscious)
		{
			base.StartCoroutine(this.BackflipRoutine(succeeded));
		}
		this.character.refs.animator.SetBool(this.AN_EMOTE, true);
		this.character.refs.animator.Play(emoteName, 0, 0f);
		this.sinceEmoteStart = 0f;
		this.emoting = true;
	}

	// Token: 0x0600014E RID: 334 RVA: 0x00009F9A File Offset: 0x0000819A
	private IEnumerator BackflipRoutine(bool succeeded)
	{
		yield return new WaitForSeconds(0.35f);
		if (this.character.IsLocal)
		{
			Vector3 vector = -this.character.GetBodypart(BodypartType.Hip).transform.forward;
			vector.y = 0f;
			this.character.AddForceAtPosition((vector.normalized + Vector3.up) * 200f, this.character.GetBodypart(BodypartType.Hip).transform.position, 5f);
		}
		yield return new WaitForSeconds(0.15f);
		if (succeeded)
		{
			this.character.Fall(1f, 0f);
		}
		yield break;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x00009FB0 File Offset: 0x000081B0
	internal void SetBool(string boolKey, bool boolValue)
	{
		this.character.refs.animator.SetBool(boolKey, boolValue);
	}

	// Token: 0x04000091 RID: 145
	private RuntimeAnimatorController _lastHashCachedController;

	// Token: 0x04000092 RID: 146
	private HashSet<int> _animatorHashes;

	// Token: 0x04000093 RID: 147
	public GameObject point;

	// Token: 0x04000094 RID: 148
	private Character character;

	// Token: 0x04000095 RID: 149
	public Transform lookRef;

	// Token: 0x04000096 RID: 150
	[HideInInspector]
	public float throwTime;

	// Token: 0x04000097 RID: 151
	private bool _headBobRemoved;

	// Token: 0x04000098 RID: 152
	public RuntimeAnimatorController defaultController;

	// Token: 0x04000099 RID: 153
	public RuntimeAnimatorController noHeadBobController;

	// Token: 0x0400009A RID: 154
	public HeadBobSetting headBobSetting;

	// Token: 0x0400009B RID: 155
	public StormAudio stormAudio;

	// Token: 0x0400009C RID: 156
	public AmbienceAudio ambienceAudio;

	// Token: 0x0400009D RID: 157
	private int AN_POINT = Animator.StringToHash("Point");

	// Token: 0x0400009E RID: 158
	private int AN_POINTX = Animator.StringToHash("Point X");

	// Token: 0x0400009F RID: 159
	private int AN_POINTY = Animator.StringToHash("Point Y");

	// Token: 0x040000A0 RID: 160
	private int AN_CLIMB_SURFACE = Animator.StringToHash("Climb Surface");

	// Token: 0x040000A1 RID: 161
	private int AN_CLIMB_ROPE = Animator.StringToHash("Climb Rope");

	// Token: 0x040000A2 RID: 162
	private int AN_CLIMB_JUMP = Animator.StringToHash("Climb Jump");

	// Token: 0x040000A3 RID: 163
	private int AN_INPUTX = Animator.StringToHash("Input X");

	// Token: 0x040000A4 RID: 164
	private int AN_INPUTY = Animator.StringToHash("Input Y");

	// Token: 0x040000A5 RID: 165
	private int AN_THROW_CHARGE = Animator.StringToHash("Throw Charge");

	// Token: 0x040000A6 RID: 166
	private int AN_THROW = Animator.StringToHash("Throw");

	// Token: 0x040000A7 RID: 167
	private int AN_IS_GROUNDED = Animator.StringToHash("Is Grounded");

	// Token: 0x040000A8 RID: 168
	private int AN_VELOCITY_Y = Animator.StringToHash("Velocity Y");

	// Token: 0x040000A9 RID: 169
	private int AN_VELOCITY_Z = Animator.StringToHash("Velocity Z");

	// Token: 0x040000AA RID: 170
	private int AN_LOOK_Y = Animator.StringToHash("Look Y");

	// Token: 0x040000AB RID: 171
	private int AN_LOOK_X = Animator.StringToHash("Look X");

	// Token: 0x040000AC RID: 172
	private int AN_SPRINT = Animator.StringToHash("Sprint");

	// Token: 0x040000AD RID: 173
	private int AN_CROUCH = Animator.StringToHash("Crouch");

	// Token: 0x040000AE RID: 174
	private int AN_REACH = Animator.StringToHash("Reach");

	// Token: 0x040000AF RID: 175
	private int AN_GRAB = Animator.StringToHash("Grab");

	// Token: 0x040000B0 RID: 176
	private int AN_VINE_HANG = Animator.StringToHash("Vine Hang");

	// Token: 0x040000B1 RID: 177
	private int AN_VINE_SLIDE = Animator.StringToHash("Vine Slide");

	// Token: 0x040000B2 RID: 178
	private int AN_VINE_TYPE = Animator.StringToHash("Vine Type");

	// Token: 0x040000B3 RID: 179
	private int AN_IS_SLIDING = Animator.StringToHash("Is Sliding");

	// Token: 0x040000B4 RID: 180
	private int AN_CHARGE_JUMP = Animator.StringToHash("Charge Jump");

	// Token: 0x040000B5 RID: 181
	private int AN_JUMP = Animator.StringToHash("Jump");

	// Token: 0x040000B6 RID: 182
	private int AN_SINCE_GROUNDED = Animator.StringToHash("Since Grounded");

	// Token: 0x040000B7 RID: 183
	private int AN_REACH_TYPE = Animator.StringToHash("Reach Type");

	// Token: 0x040000B8 RID: 184
	private int AN_MYERS_DISTANCE = Animator.StringToHash("Myers Distance");

	// Token: 0x040000B9 RID: 185
	private int AN_HANG = Animator.StringToHash("Hang");

	// Token: 0x040000BA RID: 186
	private int AN_HELP = Animator.StringToHash("Help");

	// Token: 0x040000BB RID: 187
	private int AN_WEBBED = Animator.StringToHash("Webbed");

	// Token: 0x040000BC RID: 188
	private int AN_FLOOR_LEAN_FORWARD = Animator.StringToHash("Floor Lean Forward");

	// Token: 0x040000BD RID: 189
	private int AN_FLOOR_LEAN_RIGHT = Animator.StringToHash("Floor Lean Right");

	// Token: 0x040000BE RID: 190
	private int AN_EMOTE = Animator.StringToHash("Emote");

	// Token: 0x040000BF RID: 191
	private int AN_INWATER = Animator.StringToHash("InWater");

	// Token: 0x040000C0 RID: 192
	private float forceSpeed = -1f;

	// Token: 0x040000C1 RID: 193
	private Vector3 ReachHandPos = new Vector3(-30f, -90f, -70f);

	// Token: 0x040000C2 RID: 194
	private bool emoting;

	// Token: 0x040000C3 RID: 195
	private float sinceEmoteStart = 10f;
}
