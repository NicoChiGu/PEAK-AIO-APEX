using System;
using UnityEngine;

// Token: 0x0200034A RID: 842
public class TerrainBrush : MonoBehaviour
{
	// Token: 0x06001597 RID: 5527 RVA: 0x0006F3CE File Offset: 0x0006D5CE
	private void Start()
	{
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x0006F3D0 File Offset: 0x0006D5D0
	public void Generate()
	{
		Object.FindAnyObjectByType<TerrainSplat>().Generate(this.brushType);
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x0006F3E4 File Offset: 0x0006D5E4
	private Bounds GetBounds()
	{
		Bounds bounds;
		bounds..ctor(base.transform.position, Vector3.zero);
		bounds.Encapsulate(base.transform.position + base.transform.right * 0.5f * base.transform.localScale.x * 1.4f);
		bounds.Encapsulate(base.transform.position + base.transform.right * -0.5f * base.transform.localScale.x * 1.4f);
		bounds.Encapsulate(base.transform.position + base.transform.forward * 0.5f * base.transform.localScale.z * 1.4f);
		bounds.Encapsulate(base.transform.position + base.transform.forward * -0.5f * base.transform.localScale.z * 1.4f);
		return bounds;
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x0006F538 File Offset: 0x0006D738
	private Vector3 GetPos(float pX, float pY)
	{
		Vector3 vector = base.transform.right * base.transform.localScale.x * Mathf.Lerp(-0.5f, 0.5f, pX);
		Vector3 vector2 = base.transform.forward * base.transform.localScale.z * Mathf.Lerp(-0.5f, 0.5f, pY);
		return base.transform.position + vector + vector2;
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x0006F5C8 File Offset: 0x0006D7C8
	internal void ApplySplatData(Color[,] colors, Bounds totalBounds)
	{
		foreach (Vector2Int vector2Int in HelperFunctions.GetIndexesInBounds(colors.GetLength(0), colors.GetLength(1), this.GetBounds(), totalBounds))
		{
			Vector3 vector = HelperFunctions.IDToWorldPos(vector2Int.x, vector2Int.y, colors.GetLength(0), colors.GetLength(1), totalBounds);
			if (this.brushType == TerrainBrush.BrushType.Splat)
			{
				colors[vector2Int.x, vector2Int.y] = this.SampleSplatColor(vector, colors[vector2Int.x, vector2Int.y]);
			}
			else
			{
				colors[vector2Int.x, vector2Int.y] = this.SampleDetailColor(vector, colors[vector2Int.x, vector2Int.y]);
			}
		}
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x0006F6BC File Offset: 0x0006D8BC
	private Color SampleSplatColor(Vector3 pos, Color beforeColor)
	{
		float num = this.SampleMask(pos);
		Color color = Color.Lerp(beforeColor * 2f, TerrainSplat.GetColor(this.color) * 2f, num * this.strength);
		float magnitude = new Vector4(color.r, color.g, color.b, color.a).magnitude;
		return color / magnitude;
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x0006F72C File Offset: 0x0006D92C
	private Color SampleDetailColor(Vector3 pos, Color beforeColor)
	{
		float num = this.SampleMask(pos);
		Color color = this.detailColor;
		color.a *= num;
		Color color2;
		if (beforeColor.a <= 0.01f)
		{
			color2 = color;
		}
		else
		{
			float num2 = color.a / beforeColor.a;
			Color color3 = Color.Lerp(beforeColor, color, num2);
			color3.a = Mathf.Lerp(beforeColor.a, color.a, num2);
			color2 = color3;
		}
		return color2;
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x0006F79C File Offset: 0x0006D99C
	private float SampleMask(Vector3 pos)
	{
		Vector3 vector = base.transform.InverseTransformPoint(pos);
		float num = -0.5f;
		float num2 = 0.5f;
		float num3 = Mathf.InverseLerp(num, num2, vector.x);
		float num4 = -0.5f;
		float num5 = 0.5f;
		float num6 = Mathf.InverseLerp(num4, num5, vector.z);
		float num7 = this.texture.GetPixel(Mathf.RoundToInt(num3 * (float)this.texture.width), Mathf.RoundToInt(num6 * (float)this.texture.height)).r;
		num7 = Mathf.InverseLerp(this.minMaxSlider.x, this.minMaxSlider.y, num7);
		return Mathf.Clamp01(num7);
	}

	// Token: 0x0400146C RID: 5228
	public TerrainBrush.BrushType brushType;

	// Token: 0x0400146D RID: 5229
	public Texture2D texture;

	// Token: 0x0400146E RID: 5230
	public TerrainSplat.SplatColor color;

	// Token: 0x0400146F RID: 5231
	[Range(0f, 1f)]
	public float strength = 1f;

	// Token: 0x04001470 RID: 5232
	public Color detailColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04001471 RID: 5233
	public Vector2 minMaxSlider = new Vector2(0f, 1f);

	// Token: 0x04001472 RID: 5234
	private TerrainSplat splat;

	// Token: 0x02000514 RID: 1300
	public enum BrushType
	{
		// Token: 0x04001B7C RID: 7036
		Splat,
		// Token: 0x04001B7D RID: 7037
		Detail,
		// Token: 0x04001B7E RID: 7038
		All
	}
}
