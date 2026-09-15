using System;

namespace Peak.ProcGen
{
	// Token: 0x020003D2 RID: 978
	[Serializable]
	public class BiomeSelectionOption
	{
		// Token: 0x040016BD RID: 5821
		public Biome.BiomeType biome;

		// Token: 0x040016BE RID: 5822
		public float weight = 1f;

		// Token: 0x040016BF RID: 5823
		public int preventMoreThanXInARow = 2;
	}
}
