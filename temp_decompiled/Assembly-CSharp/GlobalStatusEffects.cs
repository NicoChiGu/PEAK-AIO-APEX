using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000268 RID: 616
public class GlobalStatusEffects : MonoBehaviour
{
	// Token: 0x0600117E RID: 4478 RVA: 0x00057F8F File Offset: 0x0005618F
	private void Start()
	{
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x00057F94 File Offset: 0x00056194
	private void Update()
	{
		foreach (GlobalStatusEffects.Effect effect in this.effects)
		{
			foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
			{
				character.refs.afflictions.AddStatus(effect.type, effect.amount / effect.inTime * Time.deltaTime, false, true, true);
			}
		}
	}

	// Token: 0x04000FFC RID: 4092
	public List<GlobalStatusEffects.Effect> effects = new List<GlobalStatusEffects.Effect>();

	// Token: 0x020004DA RID: 1242
	[Serializable]
	public class Effect
	{
		// Token: 0x04001ABA RID: 6842
		public CharacterAfflictions.STATUSTYPE type;

		// Token: 0x04001ABB RID: 6843
		public float amount;

		// Token: 0x04001ABC RID: 6844
		public float inTime = 60f;
	}
}
