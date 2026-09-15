using System;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class TriggerOnInteract : MonoBehaviour, IInteractible
{
	// Token: 0x0600064F RID: 1615 RVA: 0x0002426A File Offset: 0x0002246A
	private void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x00024277 File Offset: 0x00022477
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0002427A File Offset: 0x0002247A
	public void Interact(Character interactor)
	{
		this.triggerEvent.TriggerEntered();
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00024287 File Offset: 0x00022487
	public void HoverEnter()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			base.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x000242B7 File Offset: 0x000224B7
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			base.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x000242E7 File Offset: 0x000224E7
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x000242F4 File Offset: 0x000224F4
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x000242FC File Offset: 0x000224FC
	public string GetInteractionText()
	{
		return LocalizedText.GetText("PICKUP", true);
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00024309 File Offset: 0x00022509
	public string GetName()
	{
		return this.interactableName;
	}

	// Token: 0x0400065C RID: 1628
	private MaterialPropertyBlock mpb;

	// Token: 0x0400065D RID: 1629
	public string interactText;

	// Token: 0x0400065E RID: 1630
	public TriggerEvent triggerEvent;

	// Token: 0x0400065F RID: 1631
	public string interactableName;
}
