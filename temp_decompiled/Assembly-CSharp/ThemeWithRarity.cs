using System;
using UnityEngine;

// Token: 0x0200022E RID: 558
[Serializable]
public class ThemeWithRarity
{
	// Token: 0x04000ECA RID: 3786
	[Tooltip("Parallel slot array. Index i should correspond to the same logical slot across all themes.")]
	public Material[] mats;

	// Token: 0x04000ECB RID: 3787
	[Tooltip("Weighted chance to pick this theme.")]
	public float rarity = 1f;

	// Token: 0x04000ECC RID: 3788
	public string name;
}
