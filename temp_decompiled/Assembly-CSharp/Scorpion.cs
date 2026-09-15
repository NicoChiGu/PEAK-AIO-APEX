using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class Scorpion : Mob
{
	// Token: 0x06000A62 RID: 2658 RVA: 0x00037164 File Offset: 0x00035364
	protected override void InflictAttack(Character character)
	{
		if (character.IsLocal)
		{
			float num = 0.5f;
			float num2 = 1f - character.refs.afflictions.statusSum;
			float num3 = Mathf.Max(num, num2 + 0.05f);
			character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.025f, false, true, true);
			character.refs.afflictions.AddAffliction(new Affliction_PoisonOverTime(this.totalPoisonTime, 0f, num3 / this.totalPoisonTime), false);
			character.AddForceAtPosition(500f * this.mesh.forward, base.transform.position, 2f);
		}
	}

	// Token: 0x040009CF RID: 2511
	public float totalPoisonTime = 10f;
}
