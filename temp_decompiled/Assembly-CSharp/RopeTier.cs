using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000179 RID: 377
public class RopeTier : ItemComponent
{
	// Token: 0x06000BEC RID: 3052 RVA: 0x0003FBAF File Offset: 0x0003DDAF
	private new void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.item = base.GetComponent<Item>();
		this.spool = base.GetComponent<RopeSpool>();
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x0003FBD5 File Offset: 0x0003DDD5
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000BEE RID: 3054 RVA: 0x0003FBD7 File Offset: 0x0003DDD7
	public bool LookingToPlaceAnchor
	{
		get
		{
			return this.ropeAnchor != null;
		}
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x0003FBE5 File Offset: 0x0003DDE5
	private void OnDestroy()
	{
		if (this.ropeAnchor)
		{
			Object.DestroyImmediate(this.ropeAnchor.gameObject);
		}
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x0003FC04 File Offset: 0x0003DE04
	public void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.item.itemState != ItemState.Held)
		{
			return;
		}
		if (this.releaseCheck)
		{
			if (Character.localCharacter.input.usePrimaryWasReleased)
			{
				this.releaseCheck = false;
			}
			return;
		}
		if (!Character.localCharacter.input.usePrimaryIsPressed)
		{
			this.item.overrideProgress = 0f;
			this.item.overrideForceProgress = false;
			if (this.ropeAnchor != null)
			{
				Object.DestroyImmediate(this.ropeAnchor.gameObject);
			}
			return;
		}
		if (this.ropeAnchor != null && this.goodAnchorPlace != null && Vector3.Distance(this.goodAnchorPlace.Value.point, base.transform.position) > this.maxAnchorGhostDistance)
		{
			this.item.overrideProgress = 0f;
			this.item.overrideForceProgress = false;
			Object.DestroyImmediate(this.ropeAnchor.gameObject);
			return;
		}
		if (this.ropeAnchor == null)
		{
			this.ropeAnchor = Object.Instantiate<GameObject>(this.anchorPreview).GetComponent<RopeAnchor>();
			this.ropeAnchor.anchorPoint.gameObject.SetActive(false);
			this.goodAnchorPlace = default(RaycastHit?);
			this.timeWithGoodAnchor = 0f;
		}
		if (this.goodAnchorPlace == null)
		{
			Transform transform = MainCamera.instance.transform;
			Vector3 position = transform.position;
			RaycastHit raycastHit = HelperFunctions.LineCheck(position, position + transform.forward * this.maxAnchorGhostDistance, HelperFunctions.LayerType.TerrainMap, 0f, 1);
			Debug.DrawLine(position, raycastHit.point, Color.red);
			if (raycastHit.collider == null)
			{
				return;
			}
			if (this.item == null)
			{
				Debug.Log("Item is null");
			}
			if (this.item.holderCharacter == null)
			{
				Debug.Log("Item holder is null");
			}
			float num = Vector3.Distance(raycastHit.point, this.item.holderCharacter.Center);
			this.ropeAnchor.Ghost = true;
			this.ropeAnchor.transform.position = raycastHit.point;
			this.ropeAnchor.transform.forward = Vector3.Cross(transform.right, raycastHit.normal);
			this.ropeAnchor.transform.up = raycastHit.normal;
			if (num < this.maxAnchorDistance)
			{
				this.goodAnchorPlace = new RaycastHit?(raycastHit);
				this.ropeAnchor.Ghost = false;
			}
			return;
		}
		else
		{
			this.item.overrideForceProgress = false;
			if (this.goodAnchorPlace == null)
			{
				return;
			}
			this.timeWithGoodAnchor += Time.deltaTime;
			this.item.overrideForceProgress = true;
			this.item.overrideProgress = this.timeWithGoodAnchor / this.castTime;
			if (this.timeWithGoodAnchor < this.castTime)
			{
				return;
			}
			Debug.Log("Cast anchor");
			this.item.overrideForceProgress = false;
			this.item.overrideProgress = 0f;
			GameObject gameObject = PhotonNetwork.Instantiate(this.anchorPrefab.name, this.ropeAnchor.transform.position, this.ropeAnchor.transform.rotation, 0, null);
			if (this.item.photonView.IsMine)
			{
				Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.RopePlaced, this.spool.rope.GetLengthInMeters());
				GameUtils.instance.IncrementPermanentItemsPlaced();
			}
			this.spool.rope.photonView.RPC("AttachToAnchor_Rpc", 3, new object[]
			{
				gameObject.GetComponent<PhotonView>(),
				this.spool.rope.Segments
			});
			Object.DestroyImmediate(this.ropeAnchor.gameObject);
			this.releaseCheck = true;
			this.ropeAnchor = null;
			return;
		}
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x0003FFE3 File Offset: 0x0003E1E3
	public override void OnDisable()
	{
		this.item.overrideForceProgress = false;
		this.item.overrideProgress = 0f;
		base.OnDisable();
	}

	// Token: 0x04000B0C RID: 2828
	public GameObject anchorPreview;

	// Token: 0x04000B0D RID: 2829
	public GameObject anchorPrefab;

	// Token: 0x04000B0E RID: 2830
	public float maxAnchorGhostDistance = 10f;

	// Token: 0x04000B0F RID: 2831
	public float maxAnchorDistance = 5f;

	// Token: 0x04000B10 RID: 2832
	public float castTime;

	// Token: 0x04000B11 RID: 2833
	private RaycastHit? goodAnchorPlace;

	// Token: 0x04000B12 RID: 2834
	public float timeWithGoodAnchor;

	// Token: 0x04000B13 RID: 2835
	private new Item item;

	// Token: 0x04000B14 RID: 2836
	private RopeSpool spool;

	// Token: 0x04000B15 RID: 2837
	public RopeAnchor ropeAnchor;

	// Token: 0x04000B16 RID: 2838
	private PhotonView view;

	// Token: 0x04000B17 RID: 2839
	private bool releaseCheck;
}
