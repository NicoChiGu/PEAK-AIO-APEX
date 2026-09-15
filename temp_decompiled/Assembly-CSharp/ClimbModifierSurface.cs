using System;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class ClimbModifierSurface : MonoBehaviour
{
	// Token: 0x060010B4 RID: 4276 RVA: 0x000540C8 File Offset: 0x000522C8
	public void OnClimb(Character character)
	{
		Action<Character> action = this.onClimbAction;
		if (action != null)
		{
			action.Invoke(character);
		}
		if (!this.applyStatus)
		{
			return;
		}
		if (!character.IsLocal)
		{
			return;
		}
		if (Time.time < this.lastTriggerTime + this.statusCooldown)
		{
			return;
		}
		character.refs.afflictions.AddStatus(this.statusType, this.statusAmount, false, true, true);
		this.lastTriggerTime = Time.time;
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x00054139 File Offset: 0x00052339
	public void OnClimbEnter()
	{
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x0005413B File Offset: 0x0005233B
	public void OnClimbExit()
	{
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x0005413D File Offset: 0x0005233D
	internal float OverrideClimbAngle(Character character, float climbAngle)
	{
		if (this.hasAlwaysClimbableRange && Vector3.Distance(base.transform.position, character.Center) < this.alwaysClimbableRange)
		{
			return 90f;
		}
		return climbAngle;
	}

	// Token: 0x04000EDE RID: 3806
	public bool onlySlideDown;

	// Token: 0x04000EDF RID: 3807
	public float speedMultiplier = 1f;

	// Token: 0x04000EE0 RID: 3808
	public float staminaUsageMultiplier = 1f;

	// Token: 0x04000EE1 RID: 3809
	public bool applyStatus;

	// Token: 0x04000EE2 RID: 3810
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000EE3 RID: 3811
	public float statusAmount = 0.5f;

	// Token: 0x04000EE4 RID: 3812
	public float statusCooldown = 0.5f;

	// Token: 0x04000EE5 RID: 3813
	private float lastTriggerTime;

	// Token: 0x04000EE6 RID: 3814
	public bool staticClimbCost;

	// Token: 0x04000EE7 RID: 3815
	public Action<Character> onClimbAction;

	// Token: 0x04000EE8 RID: 3816
	internal bool hasAlwaysClimbableRange;

	// Token: 0x04000EE9 RID: 3817
	internal float alwaysClimbableRange;
}
