using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000022 RID: 34
[DefaultExecutionOrder(600)]
public class Interaction : MonoBehaviour
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000261 RID: 609 RVA: 0x000117E2 File Offset: 0x0000F9E2
	// (set) Token: 0x06000262 RID: 610 RVA: 0x000117EA File Offset: 0x0000F9EA
	public float currentInteractableHeldTime
	{
		get
		{
			return this._cihf;
		}
		set
		{
			this._cihf = value;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000263 RID: 611 RVA: 0x000117F3 File Offset: 0x0000F9F3
	public float constantInteractableProgress
	{
		get
		{
			return this.currentInteractableHeldTime / this.currentConstantInteractableTime;
		}
	}

	// Token: 0x06000264 RID: 612 RVA: 0x00011802 File Offset: 0x0000FA02
	private void Awake()
	{
		Interaction.instance = this;
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000265 RID: 613 RVA: 0x0001180C File Offset: 0x0000FA0C
	private bool canInteract
	{
		get
		{
			return !Character.localCharacter.data.passedOut && !Character.localCharacter.data.fullyPassedOut && Character.localCharacter.CanDoInput() && !Character.localCharacter.data.currentStickyItem;
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x00011860 File Offset: 0x0000FA60
	private void LateUpdate()
	{
		this.currentHovered = null;
		if (!Character.localCharacter)
		{
			return;
		}
		if (!this.canInteract)
		{
			this.bestInteractable = null;
			this.bestCharacter = null;
		}
		else
		{
			this.DoInteractableRaycasts(out this.bestInteractable);
			this.bestCharacter = this.bestInteractable as CharacterInteractible;
			this.DoInteraction(this.bestInteractable);
		}
		this.bestInteractableName = ((this.bestInteractable == null) ? "null" : this.bestInteractable.GetTransform().gameObject.name);
		this.currentHovered = this.bestInteractable;
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000267 RID: 615 RVA: 0x000118F8 File Offset: 0x0000FAF8
	public bool hasValidTargetCharacter
	{
		get
		{
			return this.bestCharacter != null;
		}
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00011908 File Offset: 0x0000FB08
	private void DoInteraction(IInteractible interactable)
	{
		if (Character.localCharacter.input.interactWasReleased && interactable != null && this.currentHeldInteractible == interactable && this.readyToReleaseInteract)
		{
			IInteractibleConstant interactibleConstant = interactable as IInteractibleConstant;
			if (interactibleConstant != null)
			{
				interactibleConstant.ReleaseInteract(Character.localCharacter);
			}
			this.readyToReleaseInteract = false;
		}
		if (!Character.localCharacter.input.interactIsPressed)
		{
			this.readyToInteract = true;
			this.CancelHeldInteract();
		}
		else
		{
			if (this.readyToInteract && interactable != null)
			{
				this.readyToReleaseInteract = true;
				IInteractibleConstant interactibleConstant2 = interactable as IInteractibleConstant;
				if (interactibleConstant2 != null && interactibleConstant2.IsConstantlyInteractable(Character.localCharacter))
				{
					this.currentHeldInteractible = interactibleConstant2;
					this.currentConstantInteractableTime = interactibleConstant2.GetInteractTime(Character.localCharacter);
				}
				interactable.Interact(Character.localCharacter);
				this.readyToInteract = false;
				return;
			}
			if (Character.localCharacter.input.interactIsPressed && this.currentHeldInteractible != null)
			{
				if (interactable != this.currentHeldInteractible)
				{
					this.currentHeldInteractible = null;
				}
				else
				{
					this.currentInteractableHeldTime += Time.deltaTime;
					if (this.currentInteractableHeldTime >= this.currentConstantInteractableTime)
					{
						this.currentHeldInteractible.Interact_CastFinished(Character.localCharacter);
						this.readyToReleaseInteract = false;
						if (!this.currentHeldInteractible.holdOnFinish)
						{
							this.CancelHeldInteract();
						}
					}
				}
			}
		}
		if (this.currentHeldInteractible == null)
		{
			this.CancelHeldInteract();
		}
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00011A50 File Offset: 0x0000FC50
	private void DoInteractableRaycasts(out IInteractible interactableResult)
	{
		if (Character.localCharacter.data.carriedPlayer != null && Character.localCharacter.refs.items.currentSelectedSlot.IsSome && Character.localCharacter.refs.items.currentSelectedSlot.Value == 3)
		{
			Debug.Log("HEUH");
			interactableResult = Character.localCharacter.data.carriedPlayer.refs.interactible;
			return;
		}
		float num = Vector3.Angle(Vector3.down, MainCamera.instance.transform.forward);
		if (num <= 10f)
		{
			using (List<StickyItemComponent>.Enumerator enumerator = StickyItemComponent.ALL_STUCK_ITEMS.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					StickyItemComponent stickyItemComponent = enumerator.Current;
					if (stickyItemComponent.stuckToCharacter == Character.localCharacter && stickyItemComponent.item.Center().y <= Character.localCharacter.Center.y)
					{
						interactableResult = stickyItemComponent.item;
						return;
					}
				}
				goto IL_0187;
			}
		}
		if (num >= 170f)
		{
			foreach (StickyItemComponent stickyItemComponent2 in StickyItemComponent.ALL_STUCK_ITEMS)
			{
				if (stickyItemComponent2.stuckToCharacter == Character.localCharacter && stickyItemComponent2.item.Center().y >= Character.localCharacter.Center.y)
				{
					interactableResult = stickyItemComponent2.item;
					return;
				}
			}
		}
		IL_0187:
		Ray ray;
		ray..ctor(MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
		RaycastHit[] array = HelperFunctions.LineCheckAll(ray.origin, ray.origin + ray.direction * this.distance, HelperFunctions.LayerType.AllPhysical, 0f, 2);
		IInteractible interactible = null;
		RaycastHit raycastHit = default(RaycastHit);
		raycastHit.distance = float.MaxValue;
		foreach (RaycastHit raycastHit2 in array)
		{
			if (raycastHit2.distance < raycastHit.distance && !Character.localCharacter.refs.ragdoll.colliderList.Contains(raycastHit2.collider))
			{
				Item componentInParent = raycastHit2.transform.GetComponentInParent<Item>();
				if (!componentInParent || !(componentInParent == Character.localCharacter.data.currentItem))
				{
					raycastHit = raycastHit2;
				}
			}
		}
		if (raycastHit.collider != null)
		{
			IInteractible componentInParent2 = raycastHit.collider.GetComponentInParent<IInteractible>();
			if (componentInParent2 != null && componentInParent2.IsInteractible(Character.localCharacter))
			{
				interactible = componentInParent2;
			}
		}
		bool flag = interactible == null;
		if (flag)
		{
			float num2 = float.MaxValue;
			this.sphereCastResults = new RaycastHit[100];
			int num3 = Physics.SphereCastNonAlloc(MainCamera.instance.transform.position + MainCamera.instance.transform.forward * (this.area / 2f), this.area, MainCamera.instance.transform.forward, this.sphereCastResults, Mathf.Min(raycastHit.distance, this.distance), HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysical), 2);
			int num4 = 0;
			while (num4 < num3 && num4 < this.sphereCastResults.Length)
			{
				RaycastHit raycastHit3 = this.sphereCastResults[num4];
				Item componentInParent3 = raycastHit3.transform.GetComponentInParent<Item>();
				if (!componentInParent3 || !(componentInParent3 == Character.localCharacter.data.currentItem))
				{
					float num5 = Vector3.Angle(raycastHit3.point - MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
					if (flag && num5 < num2)
					{
						IInteractible componentInParent4 = raycastHit3.collider.GetComponentInParent<IInteractible>();
						if (componentInParent4 != null && componentInParent4.IsInteractible(Character.localCharacter))
						{
							Item componentInParent5 = raycastHit3.transform.GetComponentInParent<Item>();
							if (!componentInParent5 || !(componentInParent5 == Character.localCharacter.data.currentItem))
							{
								RaycastHit raycastHit4 = HelperFunctions.LineCheck(ray.origin, raycastHit3.point, HelperFunctions.LayerType.TerrainMap, 0f, 2);
								if (raycastHit4.collider != null && raycastHit4.collider.GetComponentInParent<IInteractible>() != componentInParent4)
								{
									Debug.DrawLine(ray.origin, raycastHit3.point, Color.red);
								}
								else
								{
									Debug.DrawLine(ray.origin, raycastHit3.point, Color.green);
									num2 = num5;
									interactible = componentInParent4;
								}
							}
						}
					}
				}
				num4++;
			}
		}
		interactableResult = interactible;
	}

	// Token: 0x0600026A RID: 618 RVA: 0x00011F28 File Offset: 0x00010128
	private void CancelHeldInteract()
	{
		if (this.currentHeldInteractible != null)
		{
			this.currentHeldInteractible.CancelCast(Character.localCharacter);
		}
		this.currentInteractableHeldTime = 0f;
		this.currentHeldInteractible = null;
	}

	// Token: 0x0400023A RID: 570
	public float distance = 2f;

	// Token: 0x0400023B RID: 571
	public float area = 0.5f;

	// Token: 0x0400023C RID: 572
	public float maxCharacterInteractAngle = 90f;

	// Token: 0x0400023D RID: 573
	public static Interaction instance;

	// Token: 0x0400023E RID: 574
	public IInteractible currentHovered;

	// Token: 0x0400023F RID: 575
	public IInteractibleConstant currentHeldInteractible;

	// Token: 0x04000240 RID: 576
	public float currentConstantInteractableTime;

	// Token: 0x04000241 RID: 577
	private float _cihf;

	// Token: 0x04000242 RID: 578
	public RaycastHit[] sphereCastResults = new RaycastHit[100];

	// Token: 0x04000243 RID: 579
	internal IInteractible bestInteractable;

	// Token: 0x04000244 RID: 580
	[SerializeField]
	internal CharacterInteractible bestCharacter;

	// Token: 0x04000245 RID: 581
	[HideInInspector]
	public bool readyToInteract = true;

	// Token: 0x04000246 RID: 582
	[HideInInspector]
	public bool readyToReleaseInteract = true;

	// Token: 0x04000247 RID: 583
	[SerializeField]
	private string bestInteractableName;
}
