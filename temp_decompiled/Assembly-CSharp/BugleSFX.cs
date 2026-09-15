using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class BugleSFX : MonoBehaviourPun
{
	// Token: 0x06001005 RID: 4101 RVA: 0x0004F884 File Offset: 0x0004DA84
	private void Start()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x0004F894 File Offset: 0x0004DA94
	private void UpdateTooting()
	{
		if (base.photonView.IsMine)
		{
			bool flag = this.item.isUsingPrimary;
			if (this.magicBugle && this.magicBugle.currentFuel <= 0.02f)
			{
				flag = false;
			}
			if (flag != this.hold)
			{
				if (flag)
				{
					int num = Random.Range(0, this.bugle.Length);
					float num2 = Vector3.Dot(this.item.holderCharacter.data.lookDirection, Vector3.up);
					num2 = (num2 + 1f) / 2f;
					base.photonView.RPC("RPC_StartToot", 0, new object[] { num, num2 });
				}
				else
				{
					base.photonView.RPC("RPC_EndToot", 0, Array.Empty<object>());
				}
				this.hold = flag;
			}
		}
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x0004F974 File Offset: 0x0004DB74
	[PunRPC]
	private void RPC_StartToot(int clip, float pitch)
	{
		this.currentPitch = pitch;
		this.currentClip = clip;
		this.hold = true;
		if (this.particle1 && this.particle2)
		{
			if (!this.particle1.isPlaying)
			{
				this.particle1.Play();
			}
			if (!this.particle2.isPlaying)
			{
				this.particle2.Play();
			}
			ParticleSystem.EmissionModule emission = this.particle1.emission;
			ParticleSystem.EmissionModule emission2 = this.particle2.emission;
			emission.enabled = true;
			emission2.enabled = true;
		}
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x0004FA08 File Offset: 0x0004DC08
	[PunRPC]
	private void RPC_EndToot()
	{
		this.hold = false;
		if (this.particle1 && this.particle2)
		{
			ParticleSystem.EmissionModule emission = this.particle1.emission;
			ParticleSystem.EmissionModule emission2 = this.particle2.emission;
			emission.enabled = false;
			emission2.enabled = false;
		}
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x0004FA60 File Offset: 0x0004DC60
	private void Update()
	{
		this.UpdateTooting();
		if (this.hold && !this.t && !this.isProp)
		{
			this.buglePlayer.clip = this.bugle[this.currentClip];
			this.buglePlayer.Play();
			this.buglePlayer.volume = 0f;
			this.t = true;
		}
		this.item.defaultPos = new Vector3(this.item.defaultPos.x, this.hold ? 0.5f : 0f, this.item.defaultPos.z);
		if (this.hold)
		{
			this.buglePlayer.volume = Mathf.Lerp(this.buglePlayer.volume, this.volume, 10f * Time.deltaTime);
			float num = this.currentPitch * (1f + Mathf.Sin(Time.time * (1f + this.pitchWobble)) * this.pitchWobble);
			num = Mathf.Clamp(num, 0.01f, 0.99f);
			this.buglePlayer.pitch = Mathf.Lerp(this.pitchMin, this.pitchMax, num);
		}
		if (!this.hold)
		{
			this.buglePlayer.volume = Mathf.Lerp(this.buglePlayer.volume, 0f, 10f * Time.deltaTime);
		}
		if (!this.hold && this.t)
		{
			this.t = false;
		}
	}

	// Token: 0x04000E69 RID: 3689
	private Item item;

	// Token: 0x04000E6A RID: 3690
	public bool hold;

	// Token: 0x04000E6B RID: 3691
	private bool t;

	// Token: 0x04000E6C RID: 3692
	private int currentClip;

	// Token: 0x04000E6D RID: 3693
	public AudioClip[] bugle;

	// Token: 0x04000E6E RID: 3694
	public AudioSource buglePlayer;

	// Token: 0x04000E6F RID: 3695
	public AudioSource bugleEnd;

	// Token: 0x04000E70 RID: 3696
	public MagicBugle magicBugle;

	// Token: 0x04000E71 RID: 3697
	public ParticleSystem particle1;

	// Token: 0x04000E72 RID: 3698
	public ParticleSystem particle2;

	// Token: 0x04000E73 RID: 3699
	public float currentPitch;

	// Token: 0x04000E74 RID: 3700
	public float pitchMin = 0.7f;

	// Token: 0x04000E75 RID: 3701
	public float pitchMax = 1.3f;

	// Token: 0x04000E76 RID: 3702
	public float volume = 0.35f;

	// Token: 0x04000E77 RID: 3703
	public float pitchWobble;

	// Token: 0x04000E78 RID: 3704
	public bool isProp;
}
