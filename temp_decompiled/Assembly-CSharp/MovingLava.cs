using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002A8 RID: 680
public class MovingLava : MonoBehaviour
{
	// Token: 0x0600129F RID: 4767 RVA: 0x0005ED9D File Offset: 0x0005CF9D
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x0005EDAC File Offset: 0x0005CFAC
	private void Update()
	{
		if (base.transform.position.y > 1150f)
		{
			return;
		}
		if (!this.timeToMove)
		{
			if (this.PlayersHaveMovedOn())
			{
				this.view.RPC("RPCA_StartLavaRise", 0, Array.Empty<object>());
			}
			return;
		}
		base.transform.position += Vector3.up * this.speed * Time.deltaTime;
		this.sinceSync += Time.deltaTime;
		if (this.sinceSync > 1f)
		{
			this.sinceSync = 0f;
			this.view.RPC("RPCA_SyncLavaHeight", 0, new object[] { base.transform.position.y });
		}
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x0005EE81 File Offset: 0x0005D081
	[PunRPC]
	public void RPCA_SyncLavaHeight(float height)
	{
		base.transform.position = new Vector3(base.transform.position.x, height, base.transform.position.z);
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x0005EEB4 File Offset: 0x0005D0B4
	[PunRPC]
	public void RPCA_StartLavaRise()
	{
		this.rockAnim.Play("RockDoor", 0, 0f);
		this.timeToMove = true;
		GamefeelHandler.instance.AddPerlinShake(3f, 2f, 10f);
		GamefeelHandler.instance.AddPerlinShake(15f, 0.3f, 15f);
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x0005EF10 File Offset: 0x0005D110
	private bool PlayersHaveMovedOn()
	{
		if (Character.AllCharacters.Count == 0)
		{
			return false;
		}
		float num = 879f;
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i].Center.y > num)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04001159 RID: 4441
	public float speed = 0.25f;

	// Token: 0x0400115A RID: 4442
	public Animator rockAnim;

	// Token: 0x0400115B RID: 4443
	private PhotonView view;

	// Token: 0x0400115C RID: 4444
	private bool timeToMove;

	// Token: 0x0400115D RID: 4445
	private float sinceSync;
}
