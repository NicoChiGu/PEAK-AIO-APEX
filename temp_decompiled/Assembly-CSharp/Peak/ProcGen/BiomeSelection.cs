using System;
using System.Collections.Generic;
using Zorro.Core;

namespace Peak.ProcGen
{
	// Token: 0x020003D1 RID: 977
	[Serializable]
	public class BiomeSelection
	{
		// Token: 0x06001928 RID: 6440 RVA: 0x0007D3D1 File Offset: 0x0007B5D1
		public int GetSelectionResult(out BiomeSelectionOption biomeResult)
		{
			biomeResult = ListExtensions.SelectRandomWeighted<BiomeSelectionOption>(this.biomeOptions, (BiomeSelectionOption biome) => biome.weight);
			return this.biomeOptions.IndexOf(biomeResult);
		}

		// Token: 0x040016BC RID: 5820
		public List<BiomeSelectionOption> biomeOptions = new List<BiomeSelectionOption>();
	}
}
