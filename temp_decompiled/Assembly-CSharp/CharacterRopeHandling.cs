using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000229 RID: 553
public class CharacterRopeHandling : MonoBehaviour
{
	// Token: 0x06001071 RID: 4209 RVA: 0x000527FC File Offset: 0x000509FC
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x0005280A File Offset: 0x00050A0A
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00052818 File Offset: 0x00050A18
	private void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.character.data.isRopeClimbing)
		{
			if (!ClassExtensionsMethods.UnityObjectExists<Rope>(this.character.data.heldRope))
			{
				this.view.RPC("StopRopeClimbingRpc", 0, Array.Empty<object>());
				return;
			}
			if (this.character.data.heldRope != null)
			{
				float angleAtPercent = this.character.data.heldRope.climbingAPI.GetAngleAtPercent(this.character.data.ropePercent);
				if (!this.character.data.heldRope.IsActive() || (angleAtPercent > this.maxRopeAngle && 180f - angleAtPercent > this.maxRopeAngle))
				{
					Debug.Log(string.Format("Rope climbing failed. Angle up: {0} Angle down: {1}", angleAtPercent, 180f - angleAtPercent));
					this.view.RPC("StopRopeClimbingRpc", 0, Array.Empty<object>());
					return;
				}
			}
			float num = ((this.character.input.movementInput.y < 0f) ? 3f : 1f);
			this.character.data.ropePercent += this.character.data.heldRope.climbingAPI.GetMove() * this.character.input.movementInput.y * num * this.climbSpeed * this.climbSpeedMod * Time.deltaTime * this.character.data.heldRope.climbingAPI.UpMult(this.character.data.ropePercent);
			this.character.data.ropePercent = Mathf.Clamp01(this.character.data.ropePercent);
			float num2 = this.staminaUsage;
			if (this.character.input.movementInput.y > 0.01f)
			{
				num2 = this.staminaUsageUp;
			}
			if (this.character.IsLocal && (this.character.input.jumpWasPressed || !this.character.UseStamina(num2 * Time.deltaTime, true) || this.character.data.currentRagdollControll < 0.3f))
			{
				this.view.RPC("StopRopeClimbingRpc", 0, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x00052A84 File Offset: 0x00050C84
	public void Stop()
	{
		this.view.RPC("StopRopeClimbingRpc", 0, Array.Empty<object>());
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x00052A9C File Offset: 0x00050C9C
	[PunRPC]
	private void StopRopeClimbingRpc()
	{
		if (this.character.data.heldRope != null)
		{
			this.character.data.heldRope.RemoveCharacterClimbing(this.character);
		}
		this.character.data.isRopeClimbing = false;
		this.character.data.isJumping = false;
		this.character.data.sinceGrounded = 0f;
		this.character.data.heldRope = null;
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x00052B24 File Offset: 0x00050D24
	private void FixedUpdate()
	{
		if (this.character.data.isRopeClimbing)
		{
			this.Climbing();
			return;
		}
		this.TryToStartWallClimb();
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x00052B48 File Offset: 0x00050D48
	private void Climbing()
	{
		if (this.character.data.heldRope == null || this.character.data.heldRope.photonView == null)
		{
			if (this.character.photonView.IsMine)
			{
				this.character.refs.climbing.StopAnyClimbing();
				return;
			}
		}
		else
		{
			this.character.data.ropeClimbWorldNormal = this.character.data.ropeClimbNormal;
			this.character.data.ropeClimbWorldUp = this.character.data.heldRope.climbingAPI.GetUp(this.character.data.ropePercent);
			this.character.AddForce(this.ClimbForce(), 1f, 1f);
		}
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x00052C2A File Offset: 0x00050E2A
	private Vector3 ClimbForce()
	{
		return (this.GetPosition() - this.character.TorsoPos()) * this.climbForce;
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x00052C50 File Offset: 0x00050E50
	private Vector3 GetPosition()
	{
		return this.character.data.heldRope.climbingAPI.GetPosition(this.character.data.ropePercent) + this.character.data.ropeClimbWorldNormal * 0.5f;
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x00052CA6 File Offset: 0x00050EA6
	private void TryToStartWallClimb()
	{
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x00052CA8 File Offset: 0x00050EA8
	[PunRPC]
	public void GrabRopeRpc(PhotonView ropeView, int segmentIndex)
	{
		Rope componentInChildren = ropeView.GetComponentInChildren<Rope>();
		if (componentInChildren == null)
		{
			Debug.LogError("Failed to get rope from network object");
			return;
		}
		Debug.Log("Start Rope Climbing!");
		componentInChildren.AddCharacterClimbing(this.character);
		this.character.data.isRopeClimbing = true;
		this.character.data.heldRope = componentInChildren;
		this.character.data.ropePercent = componentInChildren.climbingAPI.GetPercentFromSegmentIndex(segmentIndex);
		this.character.data.ropeClimbNormal = -this.character.data.lookDirection_Flat;
		this.character.data.isClimbing = false;
		this.character.data.isVineClimbing = false;
	}

	// Token: 0x04000EA1 RID: 3745
	private Character character;

	// Token: 0x04000EA2 RID: 3746
	public float climbForce;

	// Token: 0x04000EA3 RID: 3747
	public float climbSpeed;

	// Token: 0x04000EA4 RID: 3748
	public float climbSpeedMod = 1f;

	// Token: 0x04000EA5 RID: 3749
	public float climbDrag = 0.85f;

	// Token: 0x04000EA6 RID: 3750
	public float staminaUsage;

	// Token: 0x04000EA7 RID: 3751
	public float staminaUsageUp;

	// Token: 0x04000EA8 RID: 3752
	private PhotonView view;

	// Token: 0x04000EA9 RID: 3753
	public float maxRopeAngle = 90f;
}
