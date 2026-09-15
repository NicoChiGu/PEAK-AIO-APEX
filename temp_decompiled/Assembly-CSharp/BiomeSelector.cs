using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000066 RID: 102
public class BiomeSelector : MonoBehaviour
{
	// Token: 0x060004C3 RID: 1219 RVA: 0x0001C55C File Offset: 0x0001A75C
	public void Select(Biome.BiomeType biome)
	{
		bool flag = false;
		foreach (BiomeSelector.BiomeOption biomeOption in this.Biomes)
		{
			bool flag2 = biomeOption.biomeParent.biomeType == biome;
			biomeOption.biomeParent.gameObject.SetActive(flag2);
			if (flag2)
			{
				Debug.Log("Successfully found and enabled biome: " + biome.ToString());
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.Log(string.Format("Couldn't find biome {0}, selecting at random...", biome));
			this.Select();
		}
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x0001C608 File Offset: 0x0001A808
	public void Select()
	{
		BiomeSelector.BiomeOption biomeOption = ListExtensions.SelectRandomWeighted<BiomeSelector.BiomeOption>(this.Biomes, (BiomeSelector.BiomeOption biome) => biome.Weight);
		foreach (BiomeSelector.BiomeOption biomeOption2 in this.Biomes)
		{
			biomeOption2.biomeParent.gameObject.SetActive(false);
		}
		biomeOption.biomeParent.gameObject.SetActive(true);
	}

	// Token: 0x04000535 RID: 1333
	public List<BiomeSelector.BiomeOption> Biomes;

	// Token: 0x02000415 RID: 1045
	[Serializable]
	public class BiomeOption
	{
		// Token: 0x040017A4 RID: 6052
		public Biome biomeParent;

		// Token: 0x040017A5 RID: 6053
		public float Weight;
	}
}
