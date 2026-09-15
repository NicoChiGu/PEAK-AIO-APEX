using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001EF RID: 495
public static class Util
{
	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000F06 RID: 3846 RVA: 0x000498EA File Offset: 0x00047AEA
	public static Random random
	{
		get
		{
			if (Util.r == null)
			{
				Util.r = new Random();
			}
			return Util.r;
		}
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x00049904 File Offset: 0x00047B04
	public static float RangeLerp(float min, float max, float minParam, float maxParam, float param, bool clamp = true, AnimationCurve curve = null)
	{
		if (maxParam - minParam == 0f)
		{
			return min;
		}
		float num = Mathf.Clamp((param - minParam) / (maxParam - minParam), 0f, 1f);
		if (curve != null && curve.keys.Length != 0)
		{
			num = curve.Evaluate(num);
		}
		float num2 = max - min;
		return min + num2 * num;
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x00049958 File Offset: 0x00047B58
	public static T RandomSelection<T>(this IEnumerable<T> enumerable, Func<T, int> weightFunc)
	{
		int num = 0;
		T t = default(T);
		foreach (T t2 in enumerable)
		{
			int num2 = weightFunc.Invoke(t2);
			if (Util.random.Next(num + num2) >= num)
			{
				t = t2;
			}
			num += num2;
		}
		T t3 = t;
		return t;
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x000499CC File Offset: 0x00047BCC
	public static Vector2 FlattenVector3(Vector3 original)
	{
		return new Vector2(original.x, original.z);
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x000499E0 File Offset: 0x00047BE0
	public static float GenerateNormalDistribution(float mean, float stdDev)
	{
		double num = 1.0 - (double)Random.value;
		double num2 = 1.0 - (double)Random.value;
		double num3 = Math.Sqrt(-2.0 * Math.Log(num)) * Math.Cos(6.283185307179586 * num2);
		Debug.Log(string.Concat(new string[]
		{
			"Created random distribution result:",
			num3.ToString(),
			" mean: ",
			mean.ToString(),
			" stdDev: ",
			stdDev.ToString()
		}));
		float num4 = (float)num3;
		return mean + num4 * stdDev;
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x00049A84 File Offset: 0x00047C84
	public static bool Coinflip()
	{
		return (double)Random.value > 0.5;
	}

	// Token: 0x04000D12 RID: 3346
	private static Random r;
}
