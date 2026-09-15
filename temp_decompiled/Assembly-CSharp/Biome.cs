using System;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class Biome : MonoBehaviour
{
	// Token: 0x04000533 RID: 1331
	public MapHandler mapHandler;

	// Token: 0x04000534 RID: 1332
	public Biome.BiomeType biomeType;

	// Token: 0x02000414 RID: 1044
	public enum BiomeType
	{
		// Token: 0x0400179D RID: 6045
		Shore,
		// Token: 0x0400179E RID: 6046
		Tropics,
		// Token: 0x0400179F RID: 6047
		Alpine,
		// Token: 0x040017A0 RID: 6048
		Volcano,
		// Token: 0x040017A1 RID: 6049
		Peak = 5,
		// Token: 0x040017A2 RID: 6050
		Mesa,
		// Token: 0x040017A3 RID: 6051
		Roots
	}
}
