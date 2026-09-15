using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
[Serializable]
public class PerlinSampler
{
	// Token: 0x060007BD RID: 1981 RVA: 0x0002B034 File Offset: 0x00029234
	public bool Sample(Vector2 pos, int seed = 0)
	{
		float num = this.SampleValue(pos, seed);
		return num > this.minMax.x && num < this.minMax.y;
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0002B068 File Offset: 0x00029268
	public float SampleValue(Vector2 pos, int seed = 0)
	{
		float num = 0f;
		for (int i = 0; i < this.iterations; i++)
		{
			float num2 = this.scale;
			num2 *= Mathf.Pow(this.roughness, (float)i);
			float num3 = Mathf.PerlinNoise((float)(12345 + seed) + pos.x * num2 * 0.1f, (float)(12345 + seed) + pos.y * num2 * 0.1f);
			if (i == 0)
			{
				num = num3;
			}
			else
			{
				float num4 = Mathf.Pow(this.roughness, (float)i);
				num = Mathf.Lerp(num, num3, num4);
			}
		}
		if (!Mathf.Approximately(this.pow, 1f))
		{
			num = Mathf.Pow(num, this.pow);
		}
		return num;
	}

	// Token: 0x04000790 RID: 1936
	public float scale = 1f;

	// Token: 0x04000791 RID: 1937
	public int iterations = 2;

	// Token: 0x04000792 RID: 1938
	public float scaleIncrease = 3f;

	// Token: 0x04000793 RID: 1939
	public float roughness = 0.3f;

	// Token: 0x04000794 RID: 1940
	public float pow = 1f;

	// Token: 0x04000795 RID: 1941
	public Vector2 minMax = new Vector2(0f, 1f);
}
