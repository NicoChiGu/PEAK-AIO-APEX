using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class ItemScaleSyncer : ItemComponent
{
	// Token: 0x06000AFC RID: 2812 RVA: 0x0003A968 File Offset: 0x00038B68
	public override void Awake()
	{
		base.Awake();
		this.item.forceScale = false;
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0003A97C File Offset: 0x00038B7C
	public void Start()
	{
		this.InitScale();
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0003A984 File Offset: 0x00038B84
	public void InitScale()
	{
		if (this._isInitialized)
		{
			return;
		}
		this._isInitialized = true;
		FloatItemData floatItemData;
		if (this.item.HasData(DataEntryKey.Scale) && this.item.data.TryGetDataEntry<FloatItemData>(DataEntryKey.Scale, out floatItemData))
		{
			this.currentScale = floatItemData.Value;
			this.ApplyScale(this.currentScale);
		}
		else
		{
			this.currentScale = base.transform.localScale.x;
		}
		this.previousScale = this.currentScale;
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0003AA04 File Offset: 0x00038C04
	private void ApplyScale(float scale)
	{
		Vector3 vector = scale * Vector3.one;
		if (this._applyDirectlyToMeshAndCollider)
		{
			this.item.mainRenderer.transform.localScale = vector;
			this.item.colliders[0].transform.localScale = vector;
			return;
		}
		this.item.transform.localScale = vector;
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0003AA65 File Offset: 0x00038C65
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x0003AA68 File Offset: 0x00038C68
	public void Update()
	{
		if (this.photonView.IsMine && Mathf.Abs(this.currentScale - this.previousScale) > 0.01f)
		{
			this.OnScaleChanged();
		}
		float num = ((this.item.itemState == ItemState.InBackpack) ? (this.currentScale * 0.5f) : this.currentScale);
		this.ApplyScale(num);
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x0003AACC File Offset: 0x00038CCC
	private void OnScaleChanged()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		this.photonView.RPC("RPC_SyncScale", 0, new object[] { this.currentScale });
		this.previousScale = this.currentScale;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0003AB18 File Offset: 0x00038D18
	[PunRPC]
	private void RPC_SyncScale(float scale)
	{
		FloatItemData floatItemData;
		if (this.item.HasData(DataEntryKey.Scale) && this.item.data.TryGetDataEntry<FloatItemData>(DataEntryKey.Scale, out floatItemData))
		{
			floatItemData.Value = this.currentScale;
		}
		else
		{
			this.item.data.RegisterEntry<FloatItemData>(DataEntryKey.Scale, new FloatItemData
			{
				Value = this.currentScale
			});
		}
		this.currentScale = scale;
	}

	// Token: 0x04000A3A RID: 2618
	private bool _isInitialized;

	// Token: 0x04000A3B RID: 2619
	[SerializeField]
	private bool _applyDirectlyToMeshAndCollider;

	// Token: 0x04000A3C RID: 2620
	public float currentScale = 1f;

	// Token: 0x04000A3D RID: 2621
	private float previousScale;
}
