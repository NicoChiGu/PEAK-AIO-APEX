using System;
using UnityEngine;

// Token: 0x02000237 RID: 567
public class ColorblindVariant : MonoBehaviour
{
	// Token: 0x060010C7 RID: 4295 RVA: 0x00054938 File Offset: 0x00052B38
	private void Awake()
	{
		if (GUIManager.instance && GUIManager.instance.colorblindness)
		{
			if (this.matIndex == 0)
			{
				this.rend.material = this.colorblindMaterial;
			}
			else
			{
				Material[] materials = this.rend.materials;
				materials[this.matIndex] = this.colorblindMaterial;
				this.rend.materials = materials;
			}
			Item item;
			if (base.TryGetComponent<Item>(ref item))
			{
				item.UIData.icon = item.UIData.altIcon;
			}
		}
	}

	// Token: 0x04000F11 RID: 3857
	public Renderer rend;

	// Token: 0x04000F12 RID: 3858
	public Material colorblindMaterial;

	// Token: 0x04000F13 RID: 3859
	public int matIndex;
}
