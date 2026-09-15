using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000236 RID: 566
public class CollisionModifier : MonoBehaviour
{
	// Token: 0x060010C1 RID: 4289 RVA: 0x00054431 File Offset: 0x00052631
	private void Awake()
	{
		this.collisionEvents = base.GetComponents<ICollisionModifierEvent>();
		if (this.bounceAnimator != null)
		{
			this.bounceAnimator.enabled = false;
		}
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x0005445C File Offset: 0x0005265C
	public void Collide(Character character, ContactPoint contactPoint, Collision collision, Bodypart bodypart)
	{
		CollisionModifier.<>c__DisplayClass20_0 CS$<>8__locals1 = new CollisionModifier.<>c__DisplayClass20_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.character = character;
		Action<Character, CollisionModifier, Collision, Bodypart> action = this.onCollide;
		if (action != null)
		{
			action.Invoke(CS$<>8__locals1.character, this, collision, bodypart);
		}
		if (!this.applyEffects)
		{
			return;
		}
		if (this.characterList.Contains(CS$<>8__locals1.character))
		{
			return;
		}
		foreach (CollisionMod collisionMod in this.additionalMods)
		{
			CS$<>8__locals1.character.refs.afflictions.AddStatus(collisionMod.statusType, collisionMod.amount, false, true, true);
			CS$<>8__locals1.character.AddForce((CS$<>8__locals1.character.Center - contactPoint.point).normalized * collisionMod.knockback, 1f, 1f);
		}
		CS$<>8__locals1.character.refs.afflictions.AddStatus(this.statusType, this.damage, false, true, true);
		if (this.knockback > 0f && Vector3.Dot(contactPoint.normal, base.transform.forward) > this.bounceDotMinimum && CS$<>8__locals1.character.data.lastBouncedTime + 0.2f < Time.time)
		{
			float num = 1f;
			num *= Mathf.Clamp(Mathf.Pow(Mathf.Clamp(CS$<>8__locals1.character.data.sinceGrounded + this.sinceGroundedOffset, 1f, 5f), this.superBouncePow), 1f, 5f);
			Parasol parasol;
			if (CS$<>8__locals1.character.data.currentItem && CS$<>8__locals1.character.data.currentItem.TryGetComponent<Parasol>(ref parasol) && parasol.isOpen)
			{
				num = 1f;
			}
			if (CS$<>8__locals1.character.data.fallSeconds > 0f || CS$<>8__locals1.character.refs.afflictions.shouldPassOut)
			{
				num = 1f;
			}
			CS$<>8__locals1.character.AddStamina(this.staminaBackOnBounce);
			CS$<>8__locals1.character.data.lastBouncedTime = Time.time;
			CS$<>8__locals1.character.data.sinceJump = 0f;
			if (this.useBounceCoroutine)
			{
				base.StartCoroutine(CS$<>8__locals1.<Collide>g__BounceRoutine|1(Vector3.Lerp((CS$<>8__locals1.character.Center - contactPoint.point).normalized, base.transform.forward, this.knockbackTowardsFwdVector) * num));
			}
			else
			{
				float num2 = this.knockback;
				if (CS$<>8__locals1.character.data.fallSeconds > 0f || CS$<>8__locals1.character.refs.afflictions.shouldPassOut)
				{
					num2 = this.ragdolledKnockback;
				}
				CS$<>8__locals1.character.AddForce(Vector3.Lerp((CS$<>8__locals1.character.Center - contactPoint.point).normalized, base.transform.forward, this.knockbackTowardsFwdVector) * num * num2, 1f, 1f);
			}
			if (this.bounceAnimator)
			{
				if (this.bounceAnimatorRoutine != null)
				{
					base.StopCoroutine(this.bounceAnimatorRoutine);
				}
				this.bounceAnimator.enabled = true;
				this.bounceAnimator.SetTrigger("Bounce");
				this.bounceAnimatorRoutine = base.StartCoroutine(this.DisableBounceAnimator());
			}
			SFX_Player.instance.PlaySFX(this.bounceSFX, contactPoint.point, null, null, 1f, false);
			this.TriggerCharacterBouncedEvents(CS$<>8__locals1.character);
		}
		base.StartCoroutine(CS$<>8__locals1.<Collide>g__IHoldPlayer|0());
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x00054840 File Offset: 0x00052A40
	private IEnumerator DisableBounceAnimator()
	{
		yield return new WaitForSeconds(1f);
		this.bounceAnimator.enabled = false;
		yield break;
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x00054850 File Offset: 0x00052A50
	private void TriggerCharacterBouncedEvents(Character bouncedCharacter)
	{
		ICollisionModifierEvent[] array = this.collisionEvents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnBouncedCharacter(bouncedCharacter);
		}
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x0005487B File Offset: 0x00052A7B
	internal bool CanStand(Character character)
	{
		return !this.hasStandableRange || Vector3.Distance(base.transform.position, character.Center) >= this.standableRange;
	}

	// Token: 0x04000EFB RID: 3835
	private List<Character> characterList = new List<Character>();

	// Token: 0x04000EFC RID: 3836
	private ICollisionModifierEvent[] collisionEvents;

	// Token: 0x04000EFD RID: 3837
	public bool applyEffects = true;

	// Token: 0x04000EFE RID: 3838
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000EFF RID: 3839
	public float damage = 0.15f;

	// Token: 0x04000F00 RID: 3840
	public float cooldown = 1f;

	// Token: 0x04000F01 RID: 3841
	public float knockback = 20f;

	// Token: 0x04000F02 RID: 3842
	public float ragdolledKnockback = 100f;

	// Token: 0x04000F03 RID: 3843
	public float knockbackTowardsFwdVector;

	// Token: 0x04000F04 RID: 3844
	public List<CollisionMod> additionalMods = new List<CollisionMod>();

	// Token: 0x04000F05 RID: 3845
	public Action<Character, CollisionModifier, Collision, Bodypart> onCollide;

	// Token: 0x04000F06 RID: 3846
	public bool useBounceCoroutine;

	// Token: 0x04000F07 RID: 3847
	public bool standable = true;

	// Token: 0x04000F08 RID: 3848
	public Animator bounceAnimator;

	// Token: 0x04000F09 RID: 3849
	public float superBouncePow = 2f;

	// Token: 0x04000F0A RID: 3850
	public float sinceGroundedOffset = -0.5f;

	// Token: 0x04000F0B RID: 3851
	public float bounceDotMinimum = -1f;

	// Token: 0x04000F0C RID: 3852
	public float staminaBackOnBounce = 0.1f;

	// Token: 0x04000F0D RID: 3853
	public SFX_Instance bounceSFX;

	// Token: 0x04000F0E RID: 3854
	private Coroutine bounceAnimatorRoutine;

	// Token: 0x04000F0F RID: 3855
	internal bool hasStandableRange;

	// Token: 0x04000F10 RID: 3856
	internal float standableRange;
}
