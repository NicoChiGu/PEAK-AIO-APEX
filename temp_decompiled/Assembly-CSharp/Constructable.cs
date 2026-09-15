using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000109 RID: 265
public class Constructable : ItemComponent
{
	// Token: 0x060008A1 RID: 2209 RVA: 0x0002F0B5 File Offset: 0x0002D2B5
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0002F0B8 File Offset: 0x0002D2B8
	protected virtual void Update()
	{
		if (this.item.holderCharacter && this.item.holderCharacter.IsLocal)
		{
			if (!this.constructing)
			{
				this.TryUpdatePreview();
			}
			else if (this.constructing && Vector3.Distance(MainCamera.instance.transform.position, this.currentConstructHit.point) > this.maxConstructDistance)
			{
				this.DestroyPreview();
				this.item.CancelUsePrimary();
			}
		}
		else
		{
			this.DestroyPreview();
		}
		if (!this.valid)
		{
			this.item.overrideUsability = Optionable<bool>.Some(false);
			return;
		}
		this.item.overrideUsability = Optionable<bool>.None;
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0002F16B File Offset: 0x0002D36B
	private void OnDestroy()
	{
		this.DestroyPreview();
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0002F174 File Offset: 0x0002D374
	public virtual void TryUpdatePreview()
	{
		RaycastHit raycastHit = HelperFunctions.LineCheckIgnoreItem(MainCamera.instance.transform.position, MainCamera.instance.transform.position + MainCamera.instance.transform.forward.normalized * this.maxConstructDistance, HelperFunctions.LayerType.TerrainMap, this.item);
		this.currentConstructHit = raycastHit;
		this.valid = this.CurrentHitIsValid();
		if (raycastHit.collider == null)
		{
			this.DestroyPreview();
			return;
		}
		this.CreateOrMovePreview();
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0002F202 File Offset: 0x0002D402
	private void OnDrawGizmosSelected()
	{
		if (this.currentConstructHit.collider != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(this.currentConstructHit.point, 0.5f);
		}
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0002F238 File Offset: 0x0002D438
	private void CreateOrMovePreview()
	{
		if (this.currentPreview == null)
		{
			this.currentPreview = Object.Instantiate<ConstructablePreview>(this.previewPrefab);
			if (this.isAngleable)
			{
				this.UpdateAngle();
			}
		}
		this.currentPreview.transform.position = this.currentConstructHit.point;
		if (this.angleToNormal)
		{
			Vector3 normalized = Vector3.ProjectOnPlane(MainCamera.instance.transform.forward, this.currentConstructHit.normal).normalized;
			this.currentPreview.transform.rotation = Quaternion.LookRotation(normalized, this.currentConstructHit.normal);
		}
		else
		{
			Vector3 normalized2 = Vector3.ProjectOnPlane(MainCamera.instance.transform.forward, Vector3.up).normalized;
			this.currentPreview.transform.rotation = Quaternion.LookRotation(normalized2, Vector3.up);
		}
		if (!this.currentPreview.CollisionValid())
		{
			this.valid = false;
		}
		this.currentPreview.SetValid(this.valid);
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0002F342 File Offset: 0x0002D542
	internal void DestroyPreview()
	{
		this.constructing = false;
		if (this.currentPreview != null)
		{
			Object.Destroy(this.currentPreview.gameObject);
		}
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0002F36C File Offset: 0x0002D56C
	private bool CurrentHitIsValid()
	{
		return this.currentConstructHit.distance <= this.maxConstructDistance && (this.maxConstructVerticalAngle <= 0f || Vector3.Angle(Vector3.up, this.currentConstructHit.normal) <= this.maxConstructVerticalAngle);
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0002F3BB File Offset: 0x0002D5BB
	public virtual void StartConstruction()
	{
		if (this.valid)
		{
			this.constructing = true;
		}
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x0002F3CC File Offset: 0x0002D5CC
	public virtual GameObject FinishConstruction()
	{
		if (!this.constructing)
		{
			return null;
		}
		if (this.currentPreview == null)
		{
			return null;
		}
		GameObject gameObject = null;
		if (this.constructedPrefab.GetComponent<PhotonView>() == null)
		{
			this.photonView.RPC("CreatePrefabRPC", 3, new object[]
			{
				this.currentPreview.transform.position,
				this.currentPreview.transform.rotation
			});
		}
		else
		{
			gameObject = PhotonNetwork.Instantiate(this.constructedPrefab.name, this.currentPreview.transform.position, this.currentPreview.transform.rotation, 0, null);
			if (this.isAngleable)
			{
				this.photonView.RPC("AngleIt", 3, new object[]
				{
					gameObject.GetComponent<PhotonView>(),
					this.angleOffset
				});
			}
		}
		if (this.item.holderCharacter.IsLocal)
		{
			GameUtils.instance.IncrementPermanentItemsPlaced();
		}
		if (Character.localCharacter.data.currentItem == this.item)
		{
			Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
			Character.localCharacter.refs.afflictions.UpdateWeight();
		}
		PhotonNetwork.Destroy(base.gameObject);
		return gameObject;
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0002F530 File Offset: 0x0002D730
	public void UpdateAngle()
	{
		if (this.currentPreview != null)
		{
			this.currentPreview.transform.GetChild(0).GetChild(0).localEulerAngles = new Vector3(-45f + this.angleOffset, 0f, 0f);
		}
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0002F582 File Offset: 0x0002D782
	[PunRPC]
	protected void CreatePrefabRPC(Vector3 position, Quaternion rotation)
	{
		Object.Instantiate<GameObject>(this.constructedPrefab, position, rotation);
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0002F592 File Offset: 0x0002D792
	[PunRPC]
	protected void AngleIt(PhotonView view, float angle)
	{
		view.transform.GetChild(0).transform.localEulerAngles = new Vector3(-45f + angle, 0f, 0f);
	}

	// Token: 0x0400084A RID: 2122
	public ConstructablePreview previewPrefab;

	// Token: 0x0400084B RID: 2123
	public GameObject constructedPrefab;

	// Token: 0x0400084C RID: 2124
	public float maxPreviewDistance;

	// Token: 0x0400084D RID: 2125
	public float maxConstructDistance;

	// Token: 0x0400084E RID: 2126
	public float maxConstructVerticalAngle;

	// Token: 0x0400084F RID: 2127
	public bool angleToNormal;

	// Token: 0x04000850 RID: 2128
	[SerializeField]
	public ConstructablePreview currentPreview;

	// Token: 0x04000851 RID: 2129
	public float angleOffset;

	// Token: 0x04000852 RID: 2130
	public bool isAngleable;

	// Token: 0x04000853 RID: 2131
	protected RaycastHit currentConstructHit;

	// Token: 0x04000854 RID: 2132
	protected bool constructing;

	// Token: 0x04000855 RID: 2133
	private bool valid;
}
