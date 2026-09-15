using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class Balloon : ItemComponent
{
	// Token: 0x06000F7D RID: 3965 RVA: 0x0004CF04 File Offset: 0x0004B104
	public void Start()
	{
		base.StartCoroutine(this.InitColorYieldRoutine());
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x0004CF14 File Offset: 0x0004B114
	private void InitColor()
	{
		if (base.HasData(DataEntryKey.Color))
		{
			this.colorIndex = base.GetData<IntItemData>(DataEntryKey.Color).Value;
			this.SetColor(this.colorIndex);
			return;
		}
		if (this.photonView.IsMine)
		{
			this.RandomizeColor();
			this.photonView.RPC("RPC_SyncColor", 0, new object[] { this.colorIndex });
		}
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x0004CF84 File Offset: 0x0004B184
	private void SetColor(int index)
	{
		this.r.sharedMaterial = Character.localCharacter.refs.balloons.balloonColors[this.colorIndex];
		this.item.UIData.icon = this.icons[this.colorIndex];
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x0004CFD4 File Offset: 0x0004B1D4
	private void RandomizeColor()
	{
		Material[] balloonColors = Character.localCharacter.refs.balloons.balloonColors;
		this.colorIndex = Random.Range(0, balloonColors.Length);
		this.SetColor(this.colorIndex);
		base.GetData<IntItemData>(DataEntryKey.Color).Value = this.colorIndex;
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x0004D024 File Offset: 0x0004B224
	[PunRPC]
	public void RPC_SyncColor(int colorIndex)
	{
		this.colorIndex = colorIndex;
		this.SetColor(this.colorIndex);
		base.GetData<IntItemData>(DataEntryKey.Color).Value = colorIndex;
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x0004D047 File Offset: 0x0004B247
	public override void OnInstanceDataSet()
	{
		base.StartCoroutine(this.InitColorYieldRoutine());
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x0004D056 File Offset: 0x0004B256
	private IEnumerator InitColorYieldRoutine()
	{
		while (!Character.localCharacter)
		{
			yield return null;
		}
		if (!this.isBunch)
		{
			this.InitColor();
		}
		yield break;
	}

	// Token: 0x04000DD5 RID: 3541
	public new Item item;

	// Token: 0x04000DD6 RID: 3542
	public Renderer r;

	// Token: 0x04000DD7 RID: 3543
	public Texture2D[] icons;

	// Token: 0x04000DD8 RID: 3544
	public int colorIndex;

	// Token: 0x04000DD9 RID: 3545
	public bool isBunch;

	// Token: 0x04000DDA RID: 3546
	public GameObject popParticle;
}
