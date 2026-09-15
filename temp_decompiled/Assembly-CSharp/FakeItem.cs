using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI.Extensions;

// Token: 0x0200001A RID: 26
[ExecuteAlways]
public class FakeItem : MonoBehaviour, IInteractible
{
	// Token: 0x06000226 RID: 550 RVA: 0x000106D6 File Offset: 0x0000E8D6
	private void Awake()
	{
		if (Application.isPlaying)
		{
			this.AddPropertyBlock();
		}
	}

	// Token: 0x06000227 RID: 551 RVA: 0x000106E8 File Offset: 0x0000E8E8
	private void AddPropertyBlock()
	{
		this.mpb = new MaterialPropertyBlock();
		this.mainRenderer = base.GetComponentInChildren<MeshRenderer>();
		if (!this.mainRenderer)
		{
			this.mainRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
		}
		this.mainRenderer.GetPropertyBlock(this.mpb);
	}

	// Token: 0x06000228 RID: 552 RVA: 0x00010738 File Offset: 0x0000E938
	public void HoverEnter()
	{
		if (this.mpb != null && this.mainRenderer != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			this.mainRenderer.SetPropertyBlock(this.mpb);
			return;
		}
		if (this.mpb == null)
		{
			Debug.LogError("Fake item is missing it's material property block");
			return;
		}
		Debug.LogError("Fake item is missing it's renderer");
	}

	// Token: 0x06000229 RID: 553 RVA: 0x000107A0 File Offset: 0x0000E9A0
	public void HoverExit()
	{
		if (this.mpb != null && this.mainRenderer != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			this.mainRenderer.SetPropertyBlock(this.mpb);
			return;
		}
		if (this.mpb == null)
		{
			Debug.LogError("Fake item is missing it's material property block");
			return;
		}
		Debug.LogError("Fake item is missing it's renderer");
	}

	// Token: 0x0600022A RID: 554 RVA: 0x00010807 File Offset: 0x0000EA07
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0001080C File Offset: 0x0000EA0C
	public void Interact(Character interactor)
	{
		if (!interactor.player.HasEmptySlot(this.realItemPrefab.itemID))
		{
			return;
		}
		base.gameObject.SetActive(false);
		FakeItemManager.Instance.photonView.RPC("RPC_RequestFakeItemPickup", 2, new object[]
		{
			interactor.GetComponent<PhotonView>(),
			this.index
		});
		Debug.Log("Picking up " + base.gameObject.name);
	}

	// Token: 0x0600022C RID: 556 RVA: 0x0001088A File Offset: 0x0000EA8A
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00010897 File Offset: 0x0000EA97
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0001089F File Offset: 0x0000EA9F
	public string GetInteractionText()
	{
		return LocalizedText.GetText("PICKUP", true);
	}

	// Token: 0x0600022F RID: 559 RVA: 0x000108AC File Offset: 0x0000EAAC
	public string GetName()
	{
		return LocalizedText.GetText(LocalizedText.GetNameIndex(this.itemName), true);
	}

	// Token: 0x06000230 RID: 560 RVA: 0x000108BF File Offset: 0x0000EABF
	public virtual void PickUpVisibly()
	{
		base.gameObject.SetActive(false);
		FakeItemManager.Instance.fakeItemData.hiddenItems.Add(this.index);
		this.pickedUp = true;
	}

	// Token: 0x06000231 RID: 561 RVA: 0x000108EE File Offset: 0x0000EAEE
	public virtual void UnPickUpVisibly()
	{
		base.gameObject.SetActive(true);
		FakeItemManager.Instance.fakeItemData.hiddenItems.Remove(this.index);
		this.pickedUp = false;
	}

	// Token: 0x04000207 RID: 519
	public string itemName;

	// Token: 0x04000208 RID: 520
	public Item realItemPrefab;

	// Token: 0x04000209 RID: 521
	[ReadOnly]
	public bool pickedUp;

	// Token: 0x0400020A RID: 522
	[ReadOnly]
	public int index;

	// Token: 0x0400020B RID: 523
	private MaterialPropertyBlock mpb;

	// Token: 0x0400020C RID: 524
	public Renderer mainRenderer;

	// Token: 0x0400020D RID: 525
	private double lastCulledItems;
}
