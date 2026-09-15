using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000340 RID: 832
public class StatusField : MonoBehaviour
{
	// Token: 0x0600156B RID: 5483 RVA: 0x0006DA5E File Offset: 0x0006BC5E
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x0006DA80 File Offset: 0x0006BC80
	public void Update()
	{
		if (!Character.localCharacter || Vector3.Distance(Character.localCharacter.Center, base.transform.position) > this.radius)
		{
			this.inflicting = false;
			return;
		}
		if (this.doNotApplyIfStatusesMaxed && Character.localCharacter.refs.afflictions.statusSum >= 1f)
		{
			this.inflicting = false;
			return;
		}
		Character.localCharacter.refs.afflictions.AdjustStatus(this.statusType, this.statusAmountPerSecond * Time.deltaTime, false);
		foreach (StatusField.StatusFieldStatus statusFieldStatus in this.additionalStatuses)
		{
			Character.localCharacter.refs.afflictions.AdjustStatus(statusFieldStatus.statusType, statusFieldStatus.statusAmountPerSecond * Time.deltaTime, false);
		}
		if (!this.inflicting && this.statusAmountOnEntry != 0f && Time.time - this.lastEnteredTime > this.entryCooldown)
		{
			Character.localCharacter.refs.afflictions.AdjustStatus(this.statusType, this.statusAmountOnEntry, false);
			this.lastEnteredTime = Time.time;
		}
		this.inflicting = true;
	}

	// Token: 0x0400141D RID: 5149
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x0400141E RID: 5150
	public float statusAmountPerSecond;

	// Token: 0x0400141F RID: 5151
	public float statusAmountOnEntry;

	// Token: 0x04001420 RID: 5152
	public float radius;

	// Token: 0x04001421 RID: 5153
	private float lastEnteredTime;

	// Token: 0x04001422 RID: 5154
	public float entryCooldown = 1f;

	// Token: 0x04001423 RID: 5155
	public bool doNotApplyIfStatusesMaxed;

	// Token: 0x04001424 RID: 5156
	public List<StatusField.StatusFieldStatus> additionalStatuses;

	// Token: 0x04001425 RID: 5157
	private bool inflicting;

	// Token: 0x02000511 RID: 1297
	[Serializable]
	public class StatusFieldStatus
	{
		// Token: 0x04001B73 RID: 7027
		public CharacterAfflictions.STATUSTYPE statusType;

		// Token: 0x04001B74 RID: 7028
		public float statusAmountPerSecond;
	}
}
