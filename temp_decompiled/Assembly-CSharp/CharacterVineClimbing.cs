using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class CharacterVineClimbing : MonoBehaviour
{
	// Token: 0x0600107D RID: 4221 RVA: 0x00052D94 File Offset: 0x00050F94
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x00052DA2 File Offset: 0x00050FA2
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x00052DB0 File Offset: 0x00050FB0
	private void Update()
	{
		if (!this.character.IsLocal || !this.character.data.isVineClimbing || Time.timeScale == 0f)
		{
			return;
		}
		float sign = this.character.data.heldVine.GetSign(this.character.data.lookDirection_Flat, this.character.data.vinePercent);
		this._currentClimbInput = sign * this.character.input.movementInput.y;
		float num = ((this.Sliding() || Mathf.Abs(this.character.input.movementInput.y) < 0.01f) ? 0.005f : this.staminaUsage);
		if (this.character.input.jumpWasPressed || !this.character.UseStamina(num * Time.deltaTime, true) || this.character.data.currentRagdollControll < 0.5f)
		{
			this.view.RPC("StopVineClimbingRpc", 0, Array.Empty<object>());
			return;
		}
		this.syncC += Time.deltaTime;
		if (this.syncC > 0.25f)
		{
			this.syncC = 0f;
			this.view.RPC("RPCA_SyncVineClimb", 1, new object[]
			{
				this.character.data.vinePercent,
				this.attachVel
			});
		}
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00052F34 File Offset: 0x00051134
	[PunRPC]
	private void RPCA_SyncVineClimb(float p, float vel)
	{
		this.character.data.vinePercent = p;
		this.attachVel = vel;
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x00052F50 File Offset: 0x00051150
	private void FixedUpdate()
	{
		if (!this.character.data.isVineClimbing)
		{
			return;
		}
		float num = this.character.data.heldVine.LengthFactor();
		if (this.Sliding())
		{
			float vinePercent = this.character.data.vinePercent;
			if (vinePercent > 0.99f || vinePercent < 0.01f)
			{
				this.attachVel = 0f;
			}
			else
			{
				this.attachVel *= this.slideDeceleration;
			}
			this.character.data.vinePercent += this.slideFactor * num * Time.deltaTime * this.attachVel;
		}
		else
		{
			this.attachVel = 0f;
			float num2 = num * this.climbSpeed * this.climbSpeedMod * Time.deltaTime * this._currentClimbInput;
			this.character.data.vinePercent = Mathf.Clamp(this.character.data.vinePercent + num2, 0.01f, 0.99f);
		}
		this.Climbing();
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x0005305C File Offset: 0x0005125C
	public bool Sliding()
	{
		return Mathf.Abs(this.attachVel) > 3f;
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x00053070 File Offset: 0x00051270
	public void Stop()
	{
		this.view.RPC("StopVineClimbingRpc", 0, Array.Empty<object>());
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x00053088 File Offset: 0x00051288
	[PunRPC]
	private void StopVineClimbingRpc()
	{
		this.character.data.isVineClimbing = false;
		this.character.data.isJumping = false;
		this.character.data.sinceGrounded = 0f;
		this.syncC = 0f;
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x000530D8 File Offset: 0x000512D8
	private void Climbing()
	{
		Vector3 vector = (this.character.data.heldVine.GetPosition(this.character.data.vinePercent, 1, 1) + Vector3.down - this.character.TorsoPos()) * this.climbForce;
		this.character.AddForce(vector, 1f, 1f);
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x00053148 File Offset: 0x00051348
	[PunRPC]
	public void GrabVineRpc(PhotonView ropeView, int segmentIndex)
	{
		JungleVine component = ropeView.GetComponent<JungleVine>();
		if (component == null)
		{
			Debug.LogError("Failed to get rope from network object");
			return;
		}
		Debug.Log("Start Rope Climbing!");
		this.character.data.isRopeClimbing = false;
		this.character.data.isClimbing = false;
		this.character.data.isVineClimbing = true;
		this.character.data.heldVine = component;
		this.character.data.vinePercent = Mathf.Clamp(component.GetPercentFromSegmentIndex(segmentIndex), 0.01f, 0.99f);
		this.attachVel = component.GetVineVel(this.character.data.avarageVelocity, this.character.data.vinePercent);
	}

	// Token: 0x04000EAA RID: 3754
	private Character character;

	// Token: 0x04000EAB RID: 3755
	private float _currentClimbInput;

	// Token: 0x04000EAC RID: 3756
	public float climbForce;

	// Token: 0x04000EAD RID: 3757
	public float climbSpeed;

	// Token: 0x04000EAE RID: 3758
	public float climbSpeedMod = 1f;

	// Token: 0x04000EAF RID: 3759
	[Range(1f, 2f)]
	public float slideFactor = 1.3f;

	// Token: 0x04000EB0 RID: 3760
	[Range(0.9f, 0.9999f)]
	public float slideDeceleration = 0.985f;

	// Token: 0x04000EB1 RID: 3761
	public float staminaUsage;

	// Token: 0x04000EB2 RID: 3762
	private const float minVinePercent = 0.01f;

	// Token: 0x04000EB3 RID: 3763
	private const float maxVinePercent = 0.99f;

	// Token: 0x04000EB4 RID: 3764
	private PhotonView view;

	// Token: 0x04000EB5 RID: 3765
	private float attachVel;

	// Token: 0x04000EB6 RID: 3766
	private const float syncPeriod = 0.25f;

	// Token: 0x04000EB7 RID: 3767
	private const float idleStaminaCost = 0.005f;

	// Token: 0x04000EB8 RID: 3768
	private float syncC;
}
