using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200034B RID: 843
[ExecuteInEditMode]
public class TerrainSplat : MonoBehaviour
{
	// Token: 0x060015A0 RID: 5536 RVA: 0x0006F89C File Offset: 0x0006DA9C
	private void SetTerrainVariables()
	{
		Shader.SetGlobalFloat("TerrainTriplanarScale", this.TerrainTriplanarScale);
		Shader.SetGlobalTexture("TerrainTextureR", this.TerrainTextureR);
		Shader.SetGlobalColor("TerrainColorR", this.TerrainColorR.linear);
		Shader.SetGlobalVector("TerrainSmoothR", this.TerrainSmoothR);
		Shader.SetGlobalTexture("TerrainTextureG", this.TerrainTextureG);
		Shader.SetGlobalColor("TerrainColorG", this.TerrainColorG.linear);
		Shader.SetGlobalVector("TerrainSmoothG", this.TerrainSmoothG);
		Shader.SetGlobalTexture("TerrainTextureB", this.TerrainTextureB);
		Shader.SetGlobalColor("TerrainColorB", this.TerrainColorB.linear);
		Shader.SetGlobalVector("TerrainSmoothB", this.TerrainSmoothB);
		Shader.SetGlobalTexture("TerrainTextureA", this.TerrainTextureA);
		Shader.SetGlobalColor("TerrainColorA", this.TerrainColorA.linear);
		Shader.SetGlobalVector("TerrainSmoothA", this.TerrainSmoothA);
	}

	// Token: 0x060015A1 RID: 5537 RVA: 0x0006F9A1 File Offset: 0x0006DBA1
	private void Start()
	{
		this.Generate(TerrainBrush.BrushType.All);
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x0006F9AA File Offset: 0x0006DBAA
	private void GenerateAll()
	{
		this.Generate(TerrainBrush.BrushType.All);
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x0006F9B4 File Offset: 0x0006DBB4
	public void Generate(TerrainBrush.BrushType brushType)
	{
		this.SetTerrainVariables();
		this.GetBounds();
		if (brushType == TerrainBrush.BrushType.All)
		{
			this.SampleHeightMap();
			this.CreateHeighMap();
		}
		this.CreateColorData(brushType);
		this.ApplyBrushes(brushType);
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
		{
			this.splatMap = this.CreateTexture(this.splatMap, this.splatColors);
		}
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
		{
			this.detailMap = this.CreateTexture(this.detailMap, this.detailColors);
		}
		this.SetShaderData(brushType);
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x0006FA34 File Offset: 0x0006DC34
	private void SampleHeightMap()
	{
		this.heights = new Color[this.splatRess, this.splatRess];
		for (int i = 0; i < this.splatRess; i++)
		{
			for (int j = 0; j < this.splatRess; j++)
			{
				this.heights[i, j] = this.SampleHeight(i, j);
			}
		}
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x0006FA90 File Offset: 0x0006DC90
	private Color SampleHeight(int x, int y)
	{
		return new Color(HelperFunctions.GetGroundPos(this.GetPosFromIndex(x, y) + Vector3.up * 1000f, HelperFunctions.LayerType.Terrain, 0f).y / 10f, 0f, 0f, 0f);
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x0006FAE4 File Offset: 0x0006DCE4
	private void CreateHeighMap()
	{
		if (this.heightMap)
		{
			Object.DestroyImmediate(this.heightMap);
		}
		this.heightMap = new Texture2D(this.splatRess, this.splatRess, 18, 0, true);
		this.heightMap.filterMode = 1;
		this.heightMap.wrapMode = 1;
		this.heightMap.SetPixels(HelperFunctions.GridToFlatArray<Color>(this.heights));
		this.heightMap.Apply();
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x0006FB60 File Offset: 0x0006DD60
	private void SetShaderData(TerrainBrush.BrushType brushType)
	{
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
		{
			Shader.SetGlobalTexture("TerrainDetail", this.detailMap);
		}
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
		{
			Shader.SetGlobalTexture("TerrainSplat", this.splatMap);
		}
		if (brushType == TerrainBrush.BrushType.All)
		{
			Shader.SetGlobalTexture("TerrainHeight", this.heightMap);
		}
		Shader.SetGlobalVector("TerrainCenter", this.bounds.center);
		Shader.SetGlobalVector("TerrainSize", this.bounds.size);
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x0006FBE4 File Offset: 0x0006DDE4
	private void OnDestroy()
	{
		if (this.splatMap)
		{
			Object.DestroyImmediate(this.splatMap);
		}
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x0006FC00 File Offset: 0x0006DE00
	private void CreateColorData(TerrainBrush.BrushType brushType)
	{
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
		{
			this.splatColors = new Color[this.splatRess, this.splatRess];
			for (int i = 0; i < this.splatRess; i++)
			{
				for (int j = 0; j < this.splatRess; j++)
				{
					this.splatColors[i, j] = TerrainSplat.GetColor(this.baseColor);
				}
			}
		}
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
		{
			this.detailColors = new Color[this.splatRess, this.splatRess];
			for (int k = 0; k < this.splatRess; k++)
			{
				for (int l = 0; l < this.splatRess; l++)
				{
					this.detailColors[k, l] = new Color(0.5f, 0.5f, 0.5f, 0f);
				}
			}
		}
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x0006FCCC File Offset: 0x0006DECC
	private void ApplyBrushes(TerrainBrush.BrushType brushType)
	{
		foreach (TerrainBrush terrainBrush in Enumerable.ToArray<TerrainBrush>(HelperFunctions.SortBySiblingIndex<TerrainBrush>(Object.FindObjectsByType<TerrainBrush>(1))))
		{
			if (brushType == TerrainBrush.BrushType.All || brushType == terrainBrush.brushType)
			{
				this.ApplySplatBrush(terrainBrush);
			}
		}
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x0006FD10 File Offset: 0x0006DF10
	private void ApplySplatBrush(TerrainBrush item)
	{
		if (item.brushType == TerrainBrush.BrushType.Splat)
		{
			item.ApplySplatData(this.splatColors, this.bounds);
			return;
		}
		item.ApplySplatData(this.detailColors, this.bounds);
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x0006FD40 File Offset: 0x0006DF40
	private void GetBounds()
	{
		Renderer[] array = HelperFunctions.GetComponentListFromComponentArray<TerrainSplatMesh, Renderer>(Object.FindObjectsByType<TerrainSplatMesh>(0)).ToArray();
		this.bounds = HelperFunctions.GetTotalBounds(array);
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x0006FD6C File Offset: 0x0006DF6C
	private Texture2D CreateTexture(Texture2D texture, Color[,] colors)
	{
		if (texture)
		{
			Object.DestroyImmediate(texture);
		}
		texture = new Texture2D(this.splatRess, this.splatRess, 0, 0);
		texture.filterMode = 1;
		texture.wrapMode = 1;
		texture.SetPixels(HelperFunctions.GridToFlatArray<Color>(colors));
		texture.Apply();
		return texture;
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x0006FDBD File Offset: 0x0006DFBD
	private Vector3 GetPosFromIndex(int x, int y)
	{
		return this.GetPos((float)x / ((float)this.splatRess - 1f), (float)y / ((float)this.splatRess - 1f));
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x0006FDE8 File Offset: 0x0006DFE8
	private Vector3 GetPos(float pX, float pY)
	{
		Vector3 vector = Vector3.right * this.bounds.size.x * Mathf.Lerp(-0.5f, 0.5f, pX);
		Vector3 vector2 = Vector3.forward * this.bounds.size.z * Mathf.Lerp(-0.5f, 0.5f, pY);
		return this.bounds.center + vector + vector2;
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x0006FE6C File Offset: 0x0006E06C
	internal static Color GetColor(TerrainSplat.SplatColor color)
	{
		if (color == TerrainSplat.SplatColor.Black)
		{
			return new Color(0f, 0f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Red)
		{
			return new Color(1f, 0f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Green)
		{
			return new Color(0f, 1f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Blue)
		{
			return new Color(0f, 0f, 1f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Alpha)
		{
			return new Color(0f, 0f, 0f, 1f);
		}
		if (color == TerrainSplat.SplatColor.HalfRed)
		{
			return new Color(0.5f, 0f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.HalfGreen)
		{
			return new Color(0f, 0.5f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.HalfBlue)
		{
			return new Color(0f, 0f, 0.5f, 0f);
		}
		return new Color(0f, 0f, 0f, 0.5f);
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x0006FF84 File Offset: 0x0006E184
	internal Color GetSplatPixelAtWorldPos(Vector3 point)
	{
		float num = Mathf.InverseLerp(this.bounds.min.x, this.bounds.max.x, point.x);
		float num2 = Mathf.InverseLerp(this.bounds.min.z, this.bounds.max.z, point.z);
		Vector2Int vector2Int;
		vector2Int..ctor(Mathf.RoundToInt(num * (float)this.splatMap.width), Mathf.RoundToInt(num2 * (float)this.splatMap.height));
		return this.splatMap.GetPixel(vector2Int.x, vector2Int.y);
	}

	// Token: 0x04001473 RID: 5235
	public float TerrainTriplanarScale = 0.2f;

	// Token: 0x04001474 RID: 5236
	public Texture2D TerrainTextureR;

	// Token: 0x04001475 RID: 5237
	public Color TerrainColorR;

	// Token: 0x04001476 RID: 5238
	public Vector2 TerrainSmoothR = new Vector2(0f, 1f);

	// Token: 0x04001477 RID: 5239
	public Texture2D TerrainTextureG;

	// Token: 0x04001478 RID: 5240
	public Color TerrainColorG;

	// Token: 0x04001479 RID: 5241
	public Vector2 TerrainSmoothG = new Vector2(0f, 1f);

	// Token: 0x0400147A RID: 5242
	public Texture2D TerrainTextureB;

	// Token: 0x0400147B RID: 5243
	public Color TerrainColorB;

	// Token: 0x0400147C RID: 5244
	public Vector2 TerrainSmoothB = new Vector2(0f, 1f);

	// Token: 0x0400147D RID: 5245
	public Texture2D TerrainTextureA;

	// Token: 0x0400147E RID: 5246
	public Color TerrainColorA;

	// Token: 0x0400147F RID: 5247
	public Vector2 TerrainSmoothA = new Vector2(0f, 1f);

	// Token: 0x04001480 RID: 5248
	public int splatRess;

	// Token: 0x04001481 RID: 5249
	public TerrainSplat.SplatColor baseColor;

	// Token: 0x04001482 RID: 5250
	public bool displayBrushes;

	// Token: 0x04001483 RID: 5251
	public Texture2D splatMap;

	// Token: 0x04001484 RID: 5252
	public Texture2D heightMap;

	// Token: 0x04001485 RID: 5253
	public Texture2D detailMap;

	// Token: 0x04001486 RID: 5254
	private Bounds bounds;

	// Token: 0x04001487 RID: 5255
	private Color[,] splatColors;

	// Token: 0x04001488 RID: 5256
	private Color[,] detailColors;

	// Token: 0x04001489 RID: 5257
	private Color[,] heights;

	// Token: 0x02000515 RID: 1301
	public enum SplatColor
	{
		// Token: 0x04001B80 RID: 7040
		Black,
		// Token: 0x04001B81 RID: 7041
		Red,
		// Token: 0x04001B82 RID: 7042
		Green,
		// Token: 0x04001B83 RID: 7043
		Blue,
		// Token: 0x04001B84 RID: 7044
		Alpha,
		// Token: 0x04001B85 RID: 7045
		HalfRed,
		// Token: 0x04001B86 RID: 7046
		HalfGreen,
		// Token: 0x04001B87 RID: 7047
		HalfBlue,
		// Token: 0x04001B88 RID: 7048
		HalfAlpha
	}
}
