using System;
using UnityEngine.Serialization;

// Token: 0x0200029C RID: 668
[Serializable]
public class ItemRarityOverride
{
	// Token: 0x040010F7 RID: 4343
	public Rarity Rarity;

	// Token: 0x040010F8 RID: 4344
	[FormerlySerializedAs("spawnType")]
	public SpawnPool spawnPool;
}
