using System;
using UnityEngine;

// Token: 0x020002A4 RID: 676
public class MaterialLayerSwapper : MonoBehaviour
{
	// Token: 0x06001287 RID: 4743 RVA: 0x0005E398 File Offset: 0x0005C598
	private void Swap()
	{
		string text = "_Color" + this.layer.x.ToString("F0");
		string text2 = "_Smooth" + this.layer.x.ToString("F0");
		string text3 = "_Height" + this.layer.x.ToString("F0");
		string text4 = "_Texture" + this.layer.x.ToString("F0");
		string text5 = "_Triplanar" + this.layer.x.ToString("F0");
		string text6 = "_UV" + this.layer.x.ToString("F0");
		string text7 = "_Flip" + this.layer.x.ToString("F0");
		string text8 = "_Remap" + this.layer.x.ToString("F0");
		Material material = base.GetComponentInChildren<Renderer>().sharedMaterials[this.targetMaterial];
		this.color = material.GetColor(text);
		this.smooth = material.GetFloat(text2);
		this.height = material.GetFloat(text3);
		this.texture = material.GetTexture(text4);
		this.triplanar = material.GetFloat(text5);
		this.uv = material.GetFloat(text6);
		this.flip = material.GetFloat(text7);
		this.remap = material.GetVector(text8);
		string text9 = "_Color" + this.layer.y.ToString("F0");
		string text10 = "_Smooth" + this.layer.y.ToString("F0");
		string text11 = "_Height" + this.layer.y.ToString("F0");
		string text12 = "_Texture" + this.layer.y.ToString("F0");
		string text13 = "_Triplanar" + this.layer.y.ToString("F0");
		string text14 = "_UV" + this.layer.y.ToString("F0");
		string text15 = "_Flip" + this.layer.y.ToString("F0");
		string text16 = "_Remap" + this.layer.y.ToString("F0");
		this.color2 = material.GetColor(text9);
		this.smooth2 = material.GetFloat(text10);
		this.height2 = material.GetFloat(text11);
		this.texture2 = material.GetTexture(text12);
		this.triplanar2 = material.GetFloat(text13);
		this.uv2 = material.GetFloat(text14);
		this.flip2 = material.GetFloat(text15);
		this.remap2 = material.GetVector(text16);
		material.SetColor(text9, this.color);
		material.SetFloat(text10, this.smooth);
		material.SetFloat(text11, this.height);
		material.SetTexture(text12, this.texture);
		material.SetFloat(text13, this.triplanar);
		material.SetFloat(text14, this.uv);
		material.SetFloat(text15, this.flip);
		material.SetVector(text16, this.remap);
		material.SetColor(text, this.color2);
		material.SetFloat(text2, this.smooth2);
		material.SetFloat(text3, this.height2);
		material.SetTexture(text4, this.texture2);
		material.SetFloat(text5, this.triplanar2);
		material.SetFloat(text6, this.uv2);
		material.SetFloat(text7, this.flip2);
		material.SetVector(text8, this.remap2);
	}

	// Token: 0x0400113F RID: 4415
	public int targetMaterial;

	// Token: 0x04001140 RID: 4416
	public Vector2Int layer;

	// Token: 0x04001141 RID: 4417
	[ColorUsage(true, true)]
	public Color color;

	// Token: 0x04001142 RID: 4418
	public float smooth;

	// Token: 0x04001143 RID: 4419
	public float height;

	// Token: 0x04001144 RID: 4420
	public Texture texture;

	// Token: 0x04001145 RID: 4421
	public float triplanar;

	// Token: 0x04001146 RID: 4422
	public float uv;

	// Token: 0x04001147 RID: 4423
	public float flip;

	// Token: 0x04001148 RID: 4424
	public Vector2 remap;

	// Token: 0x04001149 RID: 4425
	[ColorUsage(true, true)]
	public Color color2;

	// Token: 0x0400114A RID: 4426
	public float smooth2;

	// Token: 0x0400114B RID: 4427
	public float height2;

	// Token: 0x0400114C RID: 4428
	public Texture texture2;

	// Token: 0x0400114D RID: 4429
	public float triplanar2;

	// Token: 0x0400114E RID: 4430
	public float uv2;

	// Token: 0x0400114F RID: 4431
	public float flip2;

	// Token: 0x04001150 RID: 4432
	public Vector2 remap2;
}
