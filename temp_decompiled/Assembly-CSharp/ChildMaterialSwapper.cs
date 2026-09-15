using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class ChildMaterialSwapper : MonoBehaviour, IGenConfigStep
{
	// Token: 0x06001093 RID: 4243 RVA: 0x00053593 File Offset: 0x00051793
	public void RunStep()
	{
		this.chosenTheme = this.GetRandomTheme();
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x000535A1 File Offset: 0x000517A1
	private void Start()
	{
		if (this.forceDisable)
		{
			return;
		}
		if (this.chosenTheme != null)
		{
			this.ApplyRandomTheme();
		}
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x000535BC File Offset: 0x000517BC
	public void ApplyRandomTheme()
	{
		if (this.themes == null || this.themes.Count == 0)
		{
			Debug.LogError("No themes configured.");
			return;
		}
		if (this.chosenTheme == null)
		{
			Debug.LogError("Failed to choose a theme.");
			return;
		}
		this.BuildMaterialSlotLookup();
		foreach (MeshRenderer meshRenderer in base.GetComponentsInChildren<MeshRenderer>(true))
		{
			this.ReplaceMaterialsForRenderer(meshRenderer, this.chosenTheme);
		}
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x0005362C File Offset: 0x0005182C
	private void ReplaceMaterialsForRenderer(MeshRenderer rend, ThemeWithRarity chosenTheme)
	{
		if (rend == null || chosenTheme == null || chosenTheme.mats == null)
		{
			return;
		}
		Material[] sharedMaterials = rend.sharedMaterials;
		bool flag = false;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			Material material = sharedMaterials[i];
			int num;
			if (!(material == null) && this._materialToSlot != null && this._materialToSlot.TryGetValue(material, ref num))
			{
				if (num >= 0 && num < chosenTheme.mats.Length && chosenTheme.mats[num] != null)
				{
					if (sharedMaterials[i] != chosenTheme.mats[num])
					{
						sharedMaterials[i] = chosenTheme.mats[num];
						flag = true;
					}
				}
				else
				{
					Debug.LogWarning(string.Format("Chosen theme \"{0}\" doesn't have a material at slot {1}.", chosenTheme.name, num));
				}
			}
		}
		if (flag)
		{
			rend.sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x000536FC File Offset: 0x000518FC
	private void BuildMaterialSlotLookup()
	{
		this._materialToSlot = new Dictionary<Material, int>();
		for (int i = 0; i < this.themes.Count; i++)
		{
			ThemeWithRarity themeWithRarity = this.themes[i];
			if (themeWithRarity != null && themeWithRarity.mats != null)
			{
				for (int j = 0; j < themeWithRarity.mats.Length; j++)
				{
					Material material = themeWithRarity.mats[j];
					if (!(material == null) && !this._materialToSlot.ContainsKey(material))
					{
						this._materialToSlot.Add(material, j);
					}
				}
			}
		}
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x00053784 File Offset: 0x00051984
	private ThemeWithRarity GetRandomTheme()
	{
		float num = 0f;
		foreach (ThemeWithRarity themeWithRarity in this.themes)
		{
			if (themeWithRarity != null)
			{
				num += Mathf.Max(0f, themeWithRarity.rarity);
			}
		}
		if (num <= 0f)
		{
			return null;
		}
		float num2 = Random.Range(0f, num);
		float num3 = 0f;
		foreach (ThemeWithRarity themeWithRarity2 in this.themes)
		{
			if (themeWithRarity2 != null)
			{
				float num4 = Mathf.Max(0f, themeWithRarity2.rarity);
				num3 += num4;
				if (num2 <= num3)
				{
					return themeWithRarity2;
				}
			}
		}
		return null;
	}

	// Token: 0x04000EC6 RID: 3782
	public List<ThemeWithRarity> themes = new List<ThemeWithRarity>();

	// Token: 0x04000EC7 RID: 3783
	private Dictionary<Material, int> _materialToSlot;

	// Token: 0x04000EC8 RID: 3784
	public bool forceDisable;

	// Token: 0x04000EC9 RID: 3785
	public ThemeWithRarity chosenTheme;
}
