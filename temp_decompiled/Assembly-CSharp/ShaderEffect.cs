using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class ShaderEffect : MonoBehaviour
{
	// Token: 0x060014CF RID: 5327 RVA: 0x00069BE3 File Offset: 0x00067DE3
	private void Start()
	{
		this.prop = new MaterialPropertyBlock();
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x00069BF0 File Offset: 0x00067DF0
	private void Update()
	{
		foreach (Renderer renderer in this.renderers)
		{
			this.PerRendere(renderer);
		}
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x00069C1D File Offset: 0x00067E1D
	private void PerRendere(Renderer item)
	{
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x00069C20 File Offset: 0x00067E20
	internal void SetEffect(Material mat, string key, float value)
	{
		if (!this.currentEffects.Contains(mat))
		{
			this.AddEffect(mat);
		}
		foreach (Renderer renderer in this.renderers)
		{
			this.prop.SetFloat(key, value);
			renderer.SetPropertyBlock(this.prop);
		}
	}

	// Token: 0x060014D3 RID: 5331 RVA: 0x00069C74 File Offset: 0x00067E74
	private void AddEffect(Material mat)
	{
		foreach (Renderer renderer in this.renderers)
		{
			List<Material> list = new List<Material>();
			list.AddRange(renderer.sharedMaterials);
			list.Add(mat);
			renderer.sharedMaterials = list.ToArray();
		}
		this.currentEffects.Add(mat);
	}

	// Token: 0x060014D4 RID: 5332 RVA: 0x00069CCB File Offset: 0x00067ECB
	internal void ClearEffect(Material mat)
	{
		if (this.currentEffects.Count == 0)
		{
			return;
		}
		if (this.currentEffects.Contains(mat))
		{
			this.RemoveEffect(mat);
		}
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x00069CF0 File Offset: 0x00067EF0
	private void RemoveEffect(Material mat)
	{
		foreach (Renderer renderer in this.renderers)
		{
			List<Material> list = new List<Material>();
			list.AddRange(renderer.sharedMaterials);
			list.Remove(mat);
			renderer.sharedMaterials = list.ToArray();
		}
		this.currentEffects.Remove(mat);
	}

	// Token: 0x04001345 RID: 4933
	public Renderer[] renderers;

	// Token: 0x04001346 RID: 4934
	private List<Material> currentEffects = new List<Material>();

	// Token: 0x04001347 RID: 4935
	private MaterialPropertyBlock prop;
}
