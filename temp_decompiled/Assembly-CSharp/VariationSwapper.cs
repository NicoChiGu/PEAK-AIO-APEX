using System;
using System.Linq;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class VariationSwapper : MonoBehaviour, IGenConfigStep
{
	// Token: 0x06000F0C RID: 3852 RVA: 0x00049A98 File Offset: 0x00047C98
	public void EnableRandom()
	{
		float num = Enumerable.Sum<VariationSwapper.Variation>(this.Variations, (VariationSwapper.Variation variation) => variation.chance);
		float num2 = Random.Range(0f, num);
		GameObject gameObject = Enumerable.First<VariationSwapper.Variation>(this.Variations).parent;
		float num3 = 0f;
		foreach (VariationSwapper.Variation variation2 in this.Variations)
		{
			num3 += variation2.chance;
			if (num2 < num3)
			{
				Debug.Log(string.Format("Found new: {0}", variation2.parent));
				gameObject = variation2.parent;
				break;
			}
		}
		if (gameObject != null)
		{
			VariationSwapper.Variation[] array = this.Variations;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].parent.SetActive(false);
			}
			gameObject.SetActive(true);
		}
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x00049B7E File Offset: 0x00047D7E
	public void RunStep()
	{
		this.EnableRandom();
	}

	// Token: 0x04000D13 RID: 3347
	public VariationSwapper.Variation[] Variations;

	// Token: 0x020004C1 RID: 1217
	[Serializable]
	public class Variation
	{
		// Token: 0x04001A60 RID: 6752
		public GameObject parent;

		// Token: 0x04001A61 RID: 6753
		public float chance = 1f;
	}
}
