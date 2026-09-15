using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x02000341 RID: 833
public class StatusTrigger : MonoBehaviour
{
	// Token: 0x0600156E RID: 5486 RVA: 0x0006DBF3 File Offset: 0x0006BDF3
	private void Update()
	{
		this.counter += Time.deltaTime;
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x0006DC08 File Offset: 0x0006BE08
	private void OnTriggerEnter(Collider other)
	{
		Character componentInParent = other.GetComponentInParent<Character>();
		if (componentInParent == null)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		if (this.counter < this.cooldown)
		{
			return;
		}
		this.counter = 0f;
		if (this.addStatus)
		{
			componentInParent.refs.afflictions.AddStatus(this.statusType, this.statusAmount, false, true, true);
		}
		if (this.poisonOverTime)
		{
			componentInParent.refs.afflictions.AddAffliction(new Affliction_PoisonOverTime(this.poisonOverTimeDuration, this.poisonOverTimeDelay, this.poisonOverTimeAmountPerSecond), false);
		}
	}

	// Token: 0x04001426 RID: 5158
	public float cooldown = 1f;

	// Token: 0x04001427 RID: 5159
	public bool addStatus;

	// Token: 0x04001428 RID: 5160
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04001429 RID: 5161
	public float statusAmount = 0.05f;

	// Token: 0x0400142A RID: 5162
	public bool poisonOverTime;

	// Token: 0x0400142B RID: 5163
	public float poisonOverTimeDuration = 5f;

	// Token: 0x0400142C RID: 5164
	public float poisonOverTimeDelay = 1f;

	// Token: 0x0400142D RID: 5165
	public float poisonOverTimeAmountPerSecond = 0.01f;

	// Token: 0x0400142E RID: 5166
	private float counter;
}
