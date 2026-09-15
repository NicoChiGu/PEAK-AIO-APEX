using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000331 RID: 817
public class ShittyPiton : MonoBehaviourPunCallbacks
{
	// Token: 0x06001512 RID: 5394 RVA: 0x0006B8AC File Offset: 0x00069AAC
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.handle = base.GetComponent<ClimbHandle>();
		ClimbHandle climbHandle = this.handle;
		climbHandle.onHangStart = (Action<Character>)Delegate.Combine(climbHandle.onHangStart, new Action<Character>(this.OnHang));
		ClimbHandle climbHandle2 = this.handle;
		climbHandle2.onHangStop = (Action)Delegate.Combine(climbHandle2.onHangStop, new Action(this.OnHangStop));
		this.totalSecondsOfHang = Random.Range(1f, 5f);
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x0006B934 File Offset: 0x00069B34
	private void OnHangStop()
	{
		this.isHung = false;
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x0006B93D File Offset: 0x00069B3D
	private void OnHang(Character character)
	{
		this.isHung = true;
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x0006B948 File Offset: 0x00069B48
	private void Update()
	{
		if (this.isBreaking)
		{
			if (this.isHung)
			{
				this.sinceCrack += Time.deltaTime;
			}
			if (this.sinceCrack > 1.5f)
			{
				this.Crack();
				this.sinceCrack = 0f;
			}
			this.crack.transform.localScale = Vector3.Lerp(this.crack.transform.localScale, Vector3.one * this.crackScale, Time.deltaTime * 15f);
			return;
		}
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.isHung)
		{
			this.totalSecondsOfHang -= Time.deltaTime;
			if (this.totalSecondsOfHang < 0f)
			{
				this.view.RPC("RPCA_StartBreaking", 0, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x0006BA24 File Offset: 0x00069C24
	private void Crack()
	{
		this.crackScale += 0.75f;
		this.cracksToBreak--;
		GamefeelHandler.instance.AddPerlinShakeProximity(base.transform.position, 2f + this.crackScale, 0.2f, 15f, 10f);
		for (int i = 0; i < this.cracKSFX.Length; i++)
		{
			this.cracKSFX[i].Play(base.transform.position);
		}
		if (this.cracksToBreak <= 0 && this.view.IsMine)
		{
			this.view.RPC("RPCA_Break", 0, Array.Empty<object>());
		}
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x0006BAD8 File Offset: 0x00069CD8
	[PunRPC]
	private void RPCA_Break()
	{
		this.vfx.transform.SetParent(null);
		this.vfx.SetActive(true);
		for (int i = 0; i < this.breakSFX.Length; i++)
		{
			this.breakSFX[i].Play(base.transform.position);
		}
		this.disabled = true;
		this.crack.transform.SetParent(null);
		this.handle.Break();
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x0006BB50 File Offset: 0x00069D50
	[PunRPC]
	public void RPCA_StartBreaking()
	{
		this.isBreaking = true;
		this.crack.SetActive(true);
		this.crack.transform.localScale *= 0f;
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x0006BB85 File Offset: 0x00069D85
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (this.disabled && !newPlayer.IsLocal && PhotonNetwork.IsMasterClient)
		{
			this.view.RPC("RPCA_Break", newPlayer, Array.Empty<object>());
		}
	}

	// Token: 0x0400139A RID: 5018
	private ClimbHandle handle;

	// Token: 0x0400139B RID: 5019
	private PhotonView view;

	// Token: 0x0400139C RID: 5020
	private float totalSecondsOfHang;

	// Token: 0x0400139D RID: 5021
	public GameObject crack;

	// Token: 0x0400139E RID: 5022
	public GameObject vfx;

	// Token: 0x0400139F RID: 5023
	private float crackScale;

	// Token: 0x040013A0 RID: 5024
	private int cracksToBreak = 4;

	// Token: 0x040013A1 RID: 5025
	private float sinceCrack = 10f;

	// Token: 0x040013A2 RID: 5026
	private bool disabled;

	// Token: 0x040013A3 RID: 5027
	public SFX_Instance[] cracKSFX;

	// Token: 0x040013A4 RID: 5028
	public SFX_Instance[] breakSFX;

	// Token: 0x040013A5 RID: 5029
	private bool isHung;

	// Token: 0x040013A6 RID: 5030
	private bool isBreaking;
}
