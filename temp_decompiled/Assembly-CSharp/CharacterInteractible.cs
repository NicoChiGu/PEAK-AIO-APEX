using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000228 RID: 552
public class CharacterInteractible : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x06001058 RID: 4184 RVA: 0x0005240D File Offset: 0x0005060D
	private void Start()
	{
		this.character = base.GetComponent<Character>();
		this.localCannibalismSetting = GameHandler.Instance.SettingsHandler.GetSetting<CannibalismSetting>();
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x00052430 File Offset: 0x00050630
	public Vector3 Center()
	{
		return this.character.Center;
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x00052440 File Offset: 0x00050640
	public string GetInteractionText()
	{
		if (this.CarriedByLocalCharacter())
		{
			return LocalizedText.GetText("DROP", true).Replace("#", this.GetName());
		}
		if (this.IsCannibal())
		{
			return LocalizedText.GetText("EAT", true);
		}
		if (this.CanBeCarried())
		{
			return LocalizedText.GetText("CARRY", true).Replace("#", this.GetName());
		}
		return "";
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x000524AE File Offset: 0x000506AE
	private bool IsCannibal()
	{
		return !this.character.isBot && this.character.refs.customization.isCannibalizable;
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x000524D4 File Offset: 0x000506D4
	public string GetSecondaryInteractionText()
	{
		if (this.HasItemCanUseOnFriend())
		{
			return this.GetItemPrompt(Character.localCharacter.data.currentItem);
		}
		return "";
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x000524F9 File Offset: 0x000506F9
	public string GetItemPrompt(Item item)
	{
		return LocalizedText.GetText(item.UIData.secondaryInteractPrompt, true).Replace("#targetchar", this.GetName());
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x0005251C File Offset: 0x0005071C
	public string GetName()
	{
		return this.character.characterName;
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x00052529 File Offset: 0x00050729
	private bool CarriedByLocalCharacter()
	{
		return this.character.data.carrier && this.character.data.carrier == Character.localCharacter;
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00052560 File Offset: 0x00050760
	private bool CanBeCarried()
	{
		return !this.character.isBot && (this.character.data.fullyPassedOut && !this.character.data.dead) && !this.character.data.carrier;
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x000525BC File Offset: 0x000507BC
	private bool HasItemCanUseOnFriend()
	{
		return !this.character.data.dead && this.character != Character.localCharacter && Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.canUseOnFriend;
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x00052619 File Offset: 0x00050819
	public Transform GetTransform()
	{
		return this.character.GetBodypart(BodypartType.Torso).transform;
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x0005262C File Offset: 0x0005082C
	public void HoverEnter()
	{
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x0005262E File Offset: 0x0005082E
	public void HoverExit()
	{
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x00052630 File Offset: 0x00050830
	public void Interact(Character interactor)
	{
		if (this.CarriedByLocalCharacter())
		{
			interactor.refs.carriying.Drop(this.character);
			return;
		}
		if (this.IsCannibal())
		{
			return;
		}
		if (this.CanBeCarried())
		{
			interactor.refs.carriying.StartCarry(this.character);
			return;
		}
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x00052684 File Offset: 0x00050884
	public bool IsInteractible(Character interactor)
	{
		return !this.character.isBot && (this.IsPrimaryInteractible(interactor) || this.IsSecondaryInteractible(interactor));
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x000526A7 File Offset: 0x000508A7
	public bool IsPrimaryInteractible(Character interactor)
	{
		return this.character.refs.customization.isCannibalizable || this.CarriedByLocalCharacter() || this.CanBeCarried();
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x000526D8 File Offset: 0x000508D8
	public bool IsSecondaryInteractible(Character interactor)
	{
		if (!this.HasItemCanUseOnFriend())
		{
			return false;
		}
		if (this.character.data.fullyPassedOut)
		{
			return true;
		}
		Vector3 vector = HelperFunctions.ZeroY(this.character.data.lookDirection);
		Vector3 vector2 = HelperFunctions.ZeroY(interactor.data.lookDirection);
		return Vector3.Angle(vector, -vector2) <= Interaction.instance.maxCharacterInteractAngle;
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x00052744 File Offset: 0x00050944
	private void GetEaten(Character eater)
	{
		if (eater.IsLocal)
		{
			this.character.DieInstantly();
			eater.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false, false);
			eater.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.1f, false, true, true);
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.ResourcefulnessBadge);
		}
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x000527A2 File Offset: 0x000509A2
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.character.refs.customization.isCannibalizable;
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x000527B9 File Offset: 0x000509B9
	public float GetInteractTime(Character interactor)
	{
		return 3f;
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x000527C0 File Offset: 0x000509C0
	public void Interact_CastFinished(Character interactor)
	{
		if (interactor.IsLocal && this.character.refs.customization.isCannibalizable)
		{
			this.GetEaten(interactor);
		}
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x000527E8 File Offset: 0x000509E8
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x000527EA File Offset: 0x000509EA
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x0600106F RID: 4207 RVA: 0x000527EC File Offset: 0x000509EC
	public bool holdOnFinish { get; }

	// Token: 0x04000E9E RID: 3742
	public Character character;

	// Token: 0x04000E9F RID: 3743
	private CannibalismSetting localCannibalismSetting;
}
