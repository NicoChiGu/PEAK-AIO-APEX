using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200033F RID: 831
public class StatusEmitter : MonoBehaviour
{
	// Token: 0x06001564 RID: 5476 RVA: 0x0006D63F File Offset: 0x0006B83F
	private void Start()
	{
		this.timeSinceLastTick = Random.value * this.randomStart;
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x0006D653 File Offset: 0x0006B853
	protected virtual bool InRange()
	{
		return !(Character.localCharacter == null) && Vector3.Distance(Character.localCharacter.Center, base.transform.position) < this.radius + this.outerFade;
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x0006D690 File Offset: 0x0006B890
	public void Update()
	{
		bool flag = this.InRange();
		if (flag && this.preventOverlap && this.preventOverlap)
		{
			this.isOverlapPriority = this.TryMakePriorityOverlappedEmitter();
		}
		if (this.inZoneWarning && this.inZone && (!flag || this.emitterDisabledByWind))
		{
			this.inZone = false;
			if (!this.preventOverlap || this.isOverlapPriority)
			{
				GUIManager.instance.sporesWarning.EndFX();
			}
		}
		if (this.inZoneWarning && flag && !this.inZone && !this.emitterDisabledByWind)
		{
			if (!this.preventOverlap || this.isOverlapPriority)
			{
				GUIManager.instance.sporesWarning.StartFX(0.5f);
			}
			this.inZone = true;
			this.timeSinceLastTick = -this.extraWarningTime;
			return;
		}
		if (this.emitterDisabledByWind)
		{
			return;
		}
		if (flag)
		{
			if (this.preventOverlap && !this.isOverlapPriority)
			{
				this.timeSinceLastTick = 0f;
				return;
			}
			this.timeSinceLastTick += Time.deltaTime;
			if (this.timeSinceLastTick < this.tickTime)
			{
				return;
			}
			float num = this.amount;
			float num2 = Vector3.Distance(Character.localCharacter.Center, base.transform.position);
			if (this.outerFade > 0.01f)
			{
				num *= Mathf.InverseLerp(this.radius + this.outerFade, num2, num2);
			}
			float num3 = 0f;
			if (this.innerFade > 0.01f)
			{
				num3 = (num2 - (this.radius - this.innerFade)) / this.innerFade;
			}
			num3 = Mathf.Clamp(1f - num3, this.minAmount, 1f);
			this.debug = num3;
			if (num > 0f)
			{
				Character.localCharacter.refs.afflictions.AddStatus(this.statusType, this.amount * this.tickTime * num3, false, true, true);
			}
			if (num < 0f)
			{
				Character.localCharacter.refs.afflictions.SubtractStatus(this.statusType, Mathf.Abs(this.amount * this.tickTime), false, false);
			}
		}
		this.timeSinceLastTick = 0f;
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x0006D8A8 File Offset: 0x0006BAA8
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0f, 1f, 0.4f);
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
		Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
		Gizmos.DrawWireSphere(base.transform.position, this.radius + this.outerFade);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radius - this.innerFade);
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x0006D94C File Offset: 0x0006BB4C
	private bool TryMakePriorityOverlappedEmitter()
	{
		if (!StatusEmitter.overlapPreventedStatusEmitters.ContainsKey(this.overlapIdentifier))
		{
			StatusEmitter.overlapPreventedStatusEmitters.Add(this.overlapIdentifier, this);
			return true;
		}
		if (StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier] == this)
		{
			return true;
		}
		if (StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier] == null)
		{
			StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier] = this;
			return true;
		}
		float num = Vector3.Distance(StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier].transform.position, Character.localCharacter.Center);
		if (Vector3.Distance(base.transform.position, Character.localCharacter.Center) < num)
		{
			StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier] = this;
			return true;
		}
		return false;
	}

	// Token: 0x0400140B RID: 5131
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x0400140C RID: 5132
	public float amount;

	// Token: 0x0400140D RID: 5133
	public float radius = 1f;

	// Token: 0x0400140E RID: 5134
	public float outerFade;

	// Token: 0x0400140F RID: 5135
	public float innerFade;

	// Token: 0x04001410 RID: 5136
	public float minAmount = 0.2f;

	// Token: 0x04001411 RID: 5137
	private float timeSinceLastTick;

	// Token: 0x04001412 RID: 5138
	private float tickTime = 0.5f;

	// Token: 0x04001413 RID: 5139
	public bool inZoneWarning;

	// Token: 0x04001414 RID: 5140
	public float extraWarningTime = 1f;

	// Token: 0x04001415 RID: 5141
	private bool inZone;

	// Token: 0x04001416 RID: 5142
	public float randomStart;

	// Token: 0x04001417 RID: 5143
	public float debug;

	// Token: 0x04001418 RID: 5144
	public bool preventOverlap;

	// Token: 0x04001419 RID: 5145
	public static Dictionary<string, StatusEmitter> overlapPreventedStatusEmitters = new Dictionary<string, StatusEmitter>();

	// Token: 0x0400141A RID: 5146
	public string overlapIdentifier;

	// Token: 0x0400141B RID: 5147
	public bool isOverlapPriority;

	// Token: 0x0400141C RID: 5148
	public bool emitterDisabledByWind;
}
