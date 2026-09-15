using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000334 RID: 820
public class Skelleton : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x0600151F RID: 5407 RVA: 0x0006BDB0 File Offset: 0x00069FB0
	public void SpawnSkelly(Character target)
	{
		this.spawnedFromCharacter = target;
		foreach (Bodypart bodypart in base.transform.GetComponentsInChildren<Bodypart>())
		{
			Bodypart bodypart2 = target.GetBodypart(bodypart.partType);
			if (!(bodypart2 == null))
			{
				bodypart.transform.position = bodypart2.transform.position;
				bodypart.transform.rotation = bodypart2.transform.rotation;
			}
		}
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x0006BE24 File Offset: 0x0006A024
	private void Update()
	{
		if (this.spawnedFromCharacter && !this.spawnedFromCharacter.data.dead && this.spawnedFromCharacter.data.isSkeleton)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x0006BE62 File Offset: 0x0006A062
	public void SetCharacter(Character character)
	{
		this.spawnedFromCharacter = character;
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x0006BE6C File Offset: 0x0006A06C
	private bool CharacterHasBook(Character character)
	{
		Item currentItem = character.data.currentItem;
		return !(currentItem == null) && (currentItem.gameObject.CompareTag("BookOfBones") && currentItem.GetData<OptionableIntItemData>(DataEntryKey.ItemUses).Value > 0);
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x0006BEB4 File Offset: 0x0006A0B4
	public bool IsInteractible(Character interactor)
	{
		return this.CharacterHasBook(interactor);
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x0006BEC0 File Offset: 0x0006A0C0
	public void Interact(Character interactor)
	{
		Item currentItem = interactor.data.currentItem;
		if (currentItem == null)
		{
			return;
		}
		if (currentItem.gameObject.CompareTag("BookOfBones"))
		{
			Action_BookOfBonesAnim component = currentItem.GetComponent<Action_BookOfBonesAnim>();
			if (component == null)
			{
				return;
			}
			component.Open(true);
		}
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x0006BF06 File Offset: 0x0006A106
	public void HoverEnter()
	{
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x0006BF08 File Offset: 0x0006A108
	public void HoverExit()
	{
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x0006BF0A File Offset: 0x0006A10A
	public Vector3 Center()
	{
		return this.hip.transform.position;
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x0006BF1C File Offset: 0x0006A11C
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x0006BF24 File Offset: 0x0006A124
	public string GetInteractionText()
	{
		return LocalizedText.GetText("REVIVE_BOOKOFBONES", true);
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x0006BF34 File Offset: 0x0006A134
	public string GetName()
	{
		if (this.spawnedFromCharacter != null)
		{
			return LocalizedText.GetText("NAME_SKELETON_PLAYER", true).Replace("#", this.spawnedFromCharacter.characterName.ToUpper());
		}
		return LocalizedText.GetText("NAME_SKELETON", true);
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x0006BF80 File Offset: 0x0006A180
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.CharacterHasBook(interactor);
	}

	// Token: 0x0600152C RID: 5420 RVA: 0x0006BF89 File Offset: 0x0006A189
	public float GetInteractTime(Character interactor)
	{
		return this.revivalTime;
	}

	// Token: 0x0600152D RID: 5421 RVA: 0x0006BF94 File Offset: 0x0006A194
	public void Interact_CastFinished(Character interactor)
	{
		if (this.spawnedFromCharacter)
		{
			this.spawnedFromCharacter.data.SetSkeleton(true);
			if (interactor.IsLocal)
			{
				Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.ScoutsResurrected, 1);
			}
			this.spawnedFromCharacter.photonView.RPC("RPCA_ReviveAtPosition", 0, new object[]
			{
				this.hip.transform.position + base.transform.up * 1f,
				true,
				-1
			});
			this.ReleaseInteract(interactor);
			Action_ReduceUses action_ReduceUses;
			if (interactor.data.currentItem && interactor.data.currentItem.TryGetComponent<Action_ReduceUses>(ref action_ReduceUses))
			{
				action_ReduceUses.RunAction();
			}
			if (interactor.IsLocal)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AppliedEsotericaBadge);
			}
		}
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x0006C07D File Offset: 0x0006A27D
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x0006C080 File Offset: 0x0006A280
	public void ReleaseInteract(Character interactor)
	{
		Item currentItem = interactor.data.currentItem;
		if (currentItem == null)
		{
			return;
		}
		if (currentItem.gameObject.CompareTag("BookOfBones"))
		{
			Action_BookOfBonesAnim component = currentItem.GetComponent<Action_BookOfBonesAnim>();
			if (component == null)
			{
				return;
			}
			component.Close();
		}
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06001530 RID: 5424 RVA: 0x0006C0C5 File Offset: 0x0006A2C5
	public bool holdOnFinish { get; }

	// Token: 0x040013B4 RID: 5044
	[SerializeField]
	private Character spawnedFromCharacter;

	// Token: 0x040013B5 RID: 5045
	public Transform hip;

	// Token: 0x040013B6 RID: 5046
	public float revivalTime;
}
