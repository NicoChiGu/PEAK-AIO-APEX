using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200000E RID: 14
public class CharacterBalloons : MonoBehaviourPunCallbacks
{
	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000151 RID: 337 RVA: 0x0000A23F File Offset: 0x0000843F
	public int currentBalloonCount
	{
		get
		{
			return this.heldBalloonCount + this.tiedBalloons.Count;
		}
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000A253 File Offset: 0x00008453
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06000153 RID: 339 RVA: 0x0000A264 File Offset: 0x00008464
	private void FixedUpdate()
	{
		int num = this.currentBalloonCount;
		if ((float)this.character.data.lowGravAmount > 0.01f)
		{
			num -= 2;
			if (num < 0)
			{
				num = 0;
			}
		}
		this.character.refs.movement.balloonFloatMultiplier = 1f - (this.balloonFloatAmount * (float)num - this.lowGravFloatAmount * (float)this.character.data.lowGravAmount);
		this.character.refs.movement.balloonJumpMultiplier = 1f + (this.balloonJumpAmount * (float)num + this.lowGravJumpAmount * (float)this.character.data.lowGravAmount);
		if (this.character.refs.movement.balloonFloatMultiplier < 0f)
		{
			this.character.refs.movement.balloonFloatMultiplier -= this.extraFloatUpwardMultiplier;
		}
		for (int i = 0; i < this.tiedBalloons.Count; i++)
		{
			this.tiedBalloons[i].anchor.position = this.character.Head + Vector3.up * this.headOffset;
		}
		if (this.currentBalloonCount > 0)
		{
			float num2 = Mathf.Clamp(2f - this.balloonSinceGroundedCapAmount * (float)this.currentBalloonCount, 0.5f, 2f);
			if (this.character.data.sinceGrounded > num2)
			{
				this.character.data.sinceGrounded = num2;
			}
			if (this.currentBalloonCount >= 6 && !this.character.data.isGrounded)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AeronauticsBadge);
			}
		}
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000A418 File Offset: 0x00008618
	public void TieNewBalloon(int colorIndex)
	{
		PhotonNetwork.Instantiate("TiedBalloon", this.character.Center, Quaternion.identity, 0, null).GetComponent<TiedBalloon>().Init(this, this.character.Center.y, colorIndex);
		for (int i = 0; i < this.balloonTie.Length; i++)
		{
			this.balloonTie[i].Play(this.character.Center);
		}
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000A488 File Offset: 0x00008688
	public void RemoveBalloon(TiedBalloon balloon)
	{
		if (this.tiedBalloons.Remove(balloon))
		{
			Object.Instantiate<GameObject>(this.popParticle, balloon.rb.transform.position, Quaternion.identity);
			this.character.data.sinceGrounded = 0f;
		}
	}

	// Token: 0x040000C4 RID: 196
	internal Character character;

	// Token: 0x040000C5 RID: 197
	internal List<TiedBalloon> tiedBalloons = new List<TiedBalloon>();

	// Token: 0x040000C6 RID: 198
	public GameObject popParticle;

	// Token: 0x040000C7 RID: 199
	public Material[] balloonColors;

	// Token: 0x040000C8 RID: 200
	public SFX_Instance[] balloonTie;

	// Token: 0x040000C9 RID: 201
	public int heldBalloonCount;

	// Token: 0x040000CA RID: 202
	public float balloonSinceGroundedCapAmount = 0.2f;

	// Token: 0x040000CB RID: 203
	public float balloonFloatAmount = 0.1f;

	// Token: 0x040000CC RID: 204
	public float balloonJumpAmount = 0.25f;

	// Token: 0x040000CD RID: 205
	public float lowGravFloatAmount = 0.1f;

	// Token: 0x040000CE RID: 206
	public float lowGravJumpAmount = 0.4f;

	// Token: 0x040000CF RID: 207
	public float headOffset = 0.5f;

	// Token: 0x040000D0 RID: 208
	public float extraFloatUpwardMultiplier = 1f;

	// Token: 0x040000D1 RID: 209
	private bool lastBalloonCount;
}
