using System;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class InjurySphere : MonoBehaviour
{
	// Token: 0x0600119D RID: 4509 RVA: 0x00058B97 File Offset: 0x00056D97
	private void Start()
	{
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x00058B9C File Offset: 0x00056D9C
	private void Update()
	{
		if (Vector3.Distance(Character.localCharacter.data.groundPos, base.transform.position) < base.transform.localScale.x / 2f)
		{
			if (this.isHealing)
			{
				Character.localCharacter.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Injury, Time.deltaTime * 0.2f, false, false);
				return;
			}
			Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, Time.deltaTime * 0.2f, false, true, true);
		}
	}

	// Token: 0x0400101D RID: 4125
	public bool isHealing;
}
